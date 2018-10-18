pipeline {
    agent any
    stages {
        environment {
            dockerimage = 'lykkedev/lykke-job-blobtoblobconverter-orderbook'
            dockerimagetag = 'dev'
            dockercredentials = 'lykkedev'
        }
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
              sh 'dotnet publish --configuration Release --no-restore --output app'
            }
        }
        stage('Build docker image') {
            steps{
                script {
                    dockerImage = docker.build dockerimage + ':' + dockerimagetag
                }
            }
        }
        stage('Deploy docker image') {
            steps{
                script {
                  docker.withRegistry( '', dockercredentials ) {
                      dockerImage.push()
                  }
                }
            }
        }
    }
}