pipeline {
    agent any
    parameters {
        string(name: 'GIT_REPO_URL', defaultValue: 'https://github.com/frederico101/DeveloperStore.git', description: 'URL do repositório Git do candidato')
    }
    environment {
        DOCKER_NETWORK = "evaluation-network"
        CANDIDATE_WORKSPACE = '/data/project'
        TESTS_PATH = '/data/tests-suite'
        GIT_USERNAME = 'frederico101'
        GIT_PASSWORD = credentials('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') 
    }
    
    stages {
        stage('Clonar Repositório do Candidato') {
            steps {
                script {
                    if (fileExists("${CANDIDATE_WORKSPACE}")) {
                        echo "Diretório ${CANDIDATE_WORKSPACE} já existe"
                    }else{
                        if (params.GIT_REPO_URL == '') {
                            error "O URL do repositório não foi informado!"
                        }
                        sh "git config --global --add safe.directory ${CANDIDATE_WORKSPACE}"
                        sh "git clone https://${GIT_USERNAME}:${GIT_PASSWORD}@github.com/frederico101/DeveloperStore.git ${CANDIDATE_WORKSPACE}"
                        sh 'chown -R $(whoami):$(whoami) ${CANDIDATE_WORKSPACE}'
                        dir("${CANDIDATE_WORKSPACE}") {
                            sh 'git checkout main || git checkout master'
                        }
                    }
                }
            }
        }

        stage('Subir Ambiente com Docker Compose') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    script {
                        sh '''
                        docker network create ${DOCKER_NETWORK} || true
                        docker-compose up -d --build
                        '''
                    }
                }
            }
        }

        stage('Executar Testes Automatizados no Container .NET') {
            steps {
                script {
                    sh '''
                    echo "Aguardando o Gateway inicializar..."
                    sleep 10
                    docker run --rm --network=${DOCKER_NETWORK} -v ${TESTS_PATH}:/tests mcr.microsoft.com/dotnet/sdk:8.0 sh -c "
                    mkdir -p /tests/results &&
                    cd /tests &&
                    dotnet restore &&
                    dotnet test --logger 'trx;LogFileName=results.trx' --results-directory /tests/results
                    "
                    '''
                }
            }
        }

        stage('Finalizar Ambiente') {
            steps {
                dir("${CANDIDATE_WORKSPACE}") {
                    sh '''
                    docker-compose down
                    docker network rm ${DOCKER_NETWORK} || true
                    '''
                }
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: '${TESTS_PATH}/results/*.trx', allowEmptyArchive: true
            junit '${TESTS_PATH}/results/*.trx'
            cleanWs()
        }
    }
}
