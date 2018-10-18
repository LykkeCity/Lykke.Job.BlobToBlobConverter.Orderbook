pipeline {
    environment {
            dockerimagename = 'lykkedev/lykke-job-blobtoblobconverter-orderbook'
            dockerimagetag = 'dev'
            dockercredentials = 'lykkedev'
            projectfilepath = 'src/Lykke.Job.BlobToBlobConverter.Orderbook/Lykke.Job.BlobToBlobConverter.Orderbook.csproj'
        }
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
              sh 'dotnet publish ' + projectfilepath + ' --configuration Release --no-restore --output app'
            }
        }
        stage('Build docker image') {
            steps{
                script {
                    dockerImage = docker.build dockerimagename + ':' + dockerimagetag
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