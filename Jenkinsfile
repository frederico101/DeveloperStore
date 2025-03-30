pipeline {
    agent any
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'URL do repositório Git')
    }
    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = "C:\\data\\project"
        TESTS_PATH = "C:\\data\\tests-suite"
        // Use SSH URL or GitHub App token instead of password
        GIT_CREDENTIALS_ID = 'your-github-ssh-key-credential-id' // Replace with your SSH key credential ID in Jenkins
    }

    stages {
        stage('Clone Repository') {
            steps {
                script {
                    if (fileExists("${CANDIDATE_WORKSPACE}")) {
                        echo "Directory ${CANDIDATE_WORKSPACE} already exists"
                    } else {
                        if (params.GIT_REPO_URL == '') {
                            error "Git repository URL is required!"
                        }
                        // Use SSH URL or authenticated HTTPS with token
                        checkout([
                            $class: 'GitSCM',
                            branches: [[name: '*/main']],
                            extensions: [],
                            userRemoteConfigs: [[
                                credentialsId: GIT_CREDENTIALS_ID,
                                url: params.GIT_REPO_URL
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