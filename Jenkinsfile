pipeline {
    agent any
    stages {
        stage('build') {
            steps {
                sh 'dotnet build --configuration Release'
            }
        }
        stage('test') {
            steps {
                sh 'dotnet test tests/**/*.csproj --configuration Release --no-build'
            }
        }
        stage('publish') {
            steps {
              sh 'dotnet publish --configuration Release --no-restore'
            }
        }
    }
}