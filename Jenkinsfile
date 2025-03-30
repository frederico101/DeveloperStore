pipeline {
    agent any
    
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'Git repository URL')
        string(name: 'BRANCH_NAME', defaultValue: 'feature/Configure-pipeline-master', description: 'Branch to checkout')
    }
    
    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = "C:\\data\\project"
        TESTS_PATH = "C:\\data\\tests-suite"
        // Make sure this credential is properly set up in Jenkins
        GIT_CREDENTIALS_ID = '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' 
    }

    stages {
        stage('Prepare Workspace') {
            steps {
                script {
                    // Create directories if they don't exist
                    bat """
                    if not exist "${CANDIDATE_WORKSPACE}" mkdir "${CANDIDATE_WORKSPACE}"
                    if not exist "${TESTS_PATH}" mkdir "${TESTS_PATH}"
                    """
                }
            }
        }

        stage('Clone Repository') {
            steps {
                script {
                    dir("${CANDIDATE_WORKSPACE}") {
                        // Clean workspace if it exists
                        bat 'if exist . ( rmdir /s /q . )'
                        
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
                        bat """
                        docker network create ${DOCKER_NETWORK} || echo "Network already exists"
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
                    bat """
                    echo "Waiting for services to start..."
                    timeout /t 30
                    """
                }
            }
        }

        stage('Run Automated Tests') {
            steps {
                script {
                    bat """
                    echo "Running tests..."
                    docker run --rm --network=${DOCKER_NETWORK} -v "${TESTS_PATH}:C:\\tests" mcr.microsoft.com/dotnet/sdk:8.0 cmd /c "
                    mkdir C:\\tests\\results || echo Results dir exists
                    cd C:\\tests
                    dotnet restore
                    dotnet test --logger \"trx;LogFileName=results.trx\" --results-directory C:\\tests\\results
                    "
                    """
                }
            }
        }

        stage('Shut Down Environment') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    bat """
                    docker-compose down
                    docker network rm ${DOCKER_NETWORK} || echo "Network removal failed"
                    """
                }
            }
        }
    }

    post {
        always {
            script {
                archiveArtifacts artifacts: "${TESTS_PATH}\\results\\*.trx", allowEmptyArchive: true
                junit "${TESTS_PATH}\\results\\*.trx"
                cleanWs()
            }
        }
    }
}