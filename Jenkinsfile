pipeline {
    agent any
    stages {
        stage('build') {
            steps {
                sh 'dotnet build --configuration Release'
            }
        }
        stage('publish') {
            steps {
              sh 'dotnet publish --configuration Release --no-restore'
            }
        }
    }
}