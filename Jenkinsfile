pipeline {
    agent any
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'URL do repositório Git')
        string(name: 'BRANCH_NAME', defaultValue: 'feature/Configure-pipeline-master', description: 'Branch name to checkout')
    }
    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = "C:\\data\\project"
        TESTS_PATH = "C:\\data\\tests-suite"
        // Make sure this matches exactly the credential ID you created in Jenkins
        GIT_CREDENTIALS_ID = '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' 
    }

    stages {
        stage('Clone Repository') {
            steps {
                script {
                    if (fileExists("${CANDIDATE_WORKSPACE}")) {
                        echo "Directory ${CANDIDATE_WORKSPACE} already exists"
                        // If directory exists, still ensure we're on the right branch
                        dir("${CANDIDATE_WORKSPACE}") {
                            checkout([
                                $class: 'GitSCM',
                                branches: [[name: "*/${params.BRANCH_NAME}"]],
                                extensions: [],
                                userRemoteConfigs: [[
                                    credentialsId: GIT_CREDENTIALS_ID,
                                    url: params.GIT_REPO_URL
                                ]]
                            ])
                        }
                    } else {
                        if (params.GIT_REPO_URL == '') {
                            error "Git repository URL is required!"
                        }
                        checkout([
                            $class: 'GitSCM',
                            branches: [[name: "*/${params.BRANCH_NAME}"]],
                            extensions: [[
                                $class: 'CloneOption',
                                timeout: 30,
                                depth: 1,
                                noTags: true
                            ]],
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