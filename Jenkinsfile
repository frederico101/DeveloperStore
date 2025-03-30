pipeline {
    agent any
    
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'Git repository URL')
        string(name: 'BRANCH_NAME', defaultValue: 'feature/Configure-pipeline-master', description: 'Branch to checkout')
    }
    
    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = "/data/project"
        TESTS_PATH = "/data/tests-suite"
        GIT_CREDENTIALS_ID = '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' // Ensure this credential is properly set up in Jenkins
    }

    stages {
        stage('Prepare Workspace') {
            steps {
                script {
                    // Create directories if they don't exist
                    sh """
                    mkdir -p "${CANDIDATE_WORKSPACE}"
                    mkdir -p "${TESTS_PATH}"
                    """
                }
            }
        }

        stage('Clone Repository') {
            steps {
                script {
                    dir("${CANDIDATE_WORKSPACE}") {
                        // Clean workspace if it exists
                        sh 'rm -rf *'
                        
                        // Clone using HTTPS with credentials
                        checkout([
                            $class: 'GitSCM',
                            branches: [[name: "*/${params.BRANCH_NAME}"]],
                            extensions: [[
                                $class: 'CloneOption',
                                timeout: 30,
                                depth: 1,
                                noTags: true,
                                honorRefspec: true
                            ]],
                            userRemoteConfigs: [[
                                url: params.GIT_REPO_URL,
                                credentialsId: GIT_CREDENTIALS_ID
                            ]]
                        ])
                    }
                }
            }
        }

        stage('Start Docker Environment') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    script {
                        sh """
                        docker network create ${DOCKER_NETWORK} || true
                        docker-compose up -d --build
                        """
                    }
                }
            }
        }

        stage('Wait for Services') {
            steps {
                script {
                    // Wait for SQL Server to be ready
                    sh """
                    echo "Waiting for services to start..."
                    sleep 30
                    """
                }
            }
        }

        stage('Run Automated Tests') {
            steps {
                script {
                    sh """
                    echo "Running tests..."
                    docker run --rm --network=${DOCKER_NETWORK} -v "${TESTS_PATH}:/tests" mcr.microsoft.com/dotnet/sdk:8.0 sh -c "
                    mkdir -p /tests/results &&
                    cd /tests &&
                    dotnet restore &&
                    dotnet test --logger 'trx;LogFileName=results.trx' --results-directory /tests/results
                    "
                    """
                }
            }
        }

        stage('Shut Down Environment') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    sh """
                    docker-compose down
                    docker network rm ${DOCKER_NETWORK} || true
                    """
                }
            }
        }
    }

    post {
        always {
            script {
                archiveArtifacts artifacts: "${TESTS_PATH}/results/*.trx", allowEmptyArchive: true
                junit "${TESTS_PATH}/results/*.trx"
                cleanWs()
            }
        }
    }
}