pipeline {
    agent any
    environment {
        GIT_SSH_COMMAND = 'ssh -o StrictHostKeyChecking=no'  // Temporary fix
    }
    stages {
        stage('Clone') {
            steps {
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: '*/feature/Configure-pipeline-master']],
                    extensions: [[
                        $class: 'GitSSH',
                        sshPrivateKey: credentials('github-ssh-key')  // Your credential ID
                    ]],
                    userRemoteConfigs: [[
                        url: 'git@github.com:frederico101/DeveloperStore.git'
                    ]]
                ])
            }
        }
    }
}