pipeline {
    agent any
    
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'Git repository URL')
    }
    
    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = "/data/project"
        TESTS_PATH = "/data/tests-suite"
        GIT_BRANCH = "feature/Configure-pipeline-master"
        // Use Jenkins credentials for GitHub authentication
        GIT_CREDENTIALS_ID = 'github-credentials' // Replace with your Jenkins credential ID
    }

    stages {
        stage('Prepare Workspace') {
            steps {
                script {
                    sh """
                    mkdir -p ${CANDIDATE_WORKSPACE}
                    mkdir -p ${TESTS_PATH}
                    chmod -R 777 ${CANDIDATE_WORKSPACE}
                    chmod -R 777 ${TESTS_PATH}
                    """
                }
            }
        }

        stage('Clone Repository') {
            steps {
                script {
                    dir("${CANDIDATE_WORKSPACE}") {
                        // Clean workspace if it exists
                        sh 'rm -rf * .git || true'
                        
                        // Clone repository with authentication
                        withCredentials([usernamePassword(
                            credentialsId: GIT_CREDENTIALS_ID,
                            usernameVariable: 'frederico101',
                            passwordVariable: '121245Fred*'
                        )]) {
                            sh """
                            git config --global --add safe.directory ${CANDIDATE_WORKSPACE}
                            git clone https://${GIT_USERNAME}:${GIT_PASSWORD}@${params.GIT_REPO_URL.replace('https://', '')} .
                            git checkout ${GIT_BRANCH}
                            """
                        }
                    }
                }
            }
        }

        stage('Start Docker Environment') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    sh """
                    docker network create ${DOCKER_NETWORK} || true
                    docker-compose up -d --build
                    """
                }
            }
        }

        stage('Wait for Services') {
            steps {
                script {
                    // Wait for services to be ready
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
                    docker run --rm --network=${DOCKER_NETWORK} -v ${TESTS_PATH}:/tests mcr.microsoft.com/dotnet/sdk:8.0 sh -c "
                    mkdir -p /tests/results
                    cd /tests
                    dotnet restore
                    dotnet test --logger \\\"trx;LogFileName=results.trx\\\" --results-directory /tests/results
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