pipeline {
    agent any
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'URL do repositório Git')
    }
    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = "C:\\data\\project"
        TESTS_PATH = "C:\\data\\tests-suite"
        // Using SSH for authentication (recommended for local development)
        GIT_BRANCH = "feature/Configure-pipeline-master"
    }

    stages {
        stage('Clone Repository') {
            steps {
                script {
                    if (fileExists("${CANDIDATE_WORKSPACE}")) {
                        echo "Directory ${CANDIDATE_WORKSPACE} already exists"
                        dir("${CANDIDATE_WORKSPACE}") {
                            // If directory exists, pull latest changes
                            bat "git pull origin ${GIT_BRANCH}"
                        }
                    } else {
                        if (params.GIT_REPO_URL == '') {
                            error "Git repository URL is required!"
                        }
                        // Using simple git clone command (no credentials needed for public repo)
                        bat """
                            git clone ${params.GIT_REPO_URL} ${CANDIDATE_WORKSPACE}
                            cd ${CANDIDATE_WORKSPACE}
                            git checkout ${GIT_BRANCH}
                        """
                    }
                }
            }
        }

        stage('Start Docker Environment') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    script {
                        bat """
                        docker network create ${DOCKER_NETWORK} || exit 0
                        docker-compose up -d --build
                        """
                    }
                }
            }
        }

        stage('Run Automated Tests in .NET Container') {
            steps {
                script {
                    bat """
                    echo "Waiting for Gateway to start..."
                    timeout /t 10
                    docker run --rm --network=${DOCKER_NETWORK} -v ${TESTS_PATH}:/tests mcr.microsoft.com/dotnet/sdk:8.0 cmd /c "
                    mkdir C:\\tests\\results &&
                    cd C:\\tests &&
                    dotnet restore &&
                    dotnet test --logger 'trx;LogFileName=results.trx' --results-directory C:\\tests\\results
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
                    docker network rm ${DOCKER_NETWORK} || exit 0
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