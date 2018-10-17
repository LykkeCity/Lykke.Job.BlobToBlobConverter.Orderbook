pipeline {
    agent any
    stages {
        stage('checkout') {
          steps {
            checkout([
                $class: 'GitSCM',
                branches: scm.branches,
                gitTool: 'native git'
            ])
          }
        }
        stage('build') {
            steps {
                sh 'dotnet build --configuration Release --platform "Any CPU"'
            }
        }
        stage('publish') {
            steps {
              sh 'dotnet publish --configuration Release --platform "Any CPU" --no-restore'
            }
        }
    }
}