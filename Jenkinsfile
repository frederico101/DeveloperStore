pipeline {
    agent any
    environment {
        GIT_SSH_COMMAND = 'ssh -o StrictHostKeyChecking=no'  // Disable strict host key checking
    }
    stages {
        stage('Clone') {
            steps {
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: '*/feature/Configure-pipeline-master']],
                    userRemoteConfigs: [[
                        url: 'git@github.com:frederico101/DeveloperStore.git',
                        credentialsId: 'github-ssh-key'  // Use the credential ID you added
                    ]]
                ])
            }
        }
    }
}