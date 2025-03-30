pipeline {
    agent any
    
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'Git repository URL')
        string(name: 'BRANCH_NAME', defaultValue: 'feature/Configure-pipeline-master', description: 'Branch to build (master, dev, feature/Configure-pipeline-master, etc.)')
    }

    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = "${WORKSPACE}/project"
        TESTS_PATH = "${WORKSPACE}/tests-suite"
        // Use one of these credential options:
         GIT_CREDENTIALS_ID = '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' // For HTTPS auth
        // GIT_CREDENTIALS_ID = 'github-ssh'   // For SSH auth
    }

    stages {
        stage('Prepare Workspace') {
            steps {
                script {
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
                    // Option 1: HTTPS with credentials
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
                            url: params.GIT_REPO_URL,
                            credentialsId: env.GIT_CREDENTIALS_ID ?: null
                        ]]
                    ])
                    
                    // Option 2: SSH alternative (uncomment if using SSH)
                    /*
                    sshagent(credentials: [env.GIT_CREDENTIALS_ID]) {
                        sh """
                        git clone -b ${params.BRANCH_NAME} git@github.com:frederico101/DeveloperStore.git "${CANDIDATE_WORKSPACE}"
                        """
                    }
                    */
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

        stage('Run Automated Tests') {
            steps {
                script {
                    sh """
                    echo "Waiting for services to initialize..."
                    sleep 30
                    docker run --rm --network=${DOCKER_NETWORK} \
                    -v "${TESTS_PATH}:/tests" \
                    mcr.microsoft.com/dotnet/sdk:8.0 \
                    bash -c "mkdir -p /tests/results && cd /tests && dotnet restore && dotnet test --logger 'trx;LogFileName=results.trx' --results-directory /tests/results"
                    """
                }
            }
        }

        stage('Shut Down Environment') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    sh """
                    docker-compose down || true
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