pipeline {
    environment {
            dockerimagename = 'lykkedev/lykke-job-blobtoblobconverter-orderbook'
            dockerimagetag = 'dev'
            dockercredentials = 'lykkedev'
            publishproject = 'Lykke.Job.BlobToBlobConverter.Orderbook'
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
              sh 'dotnet publish src/' + projectfilepath + '/' + publishproject + '.csproj --configuration Release --no-restore --output app/' + publishproject
            }
        }
        stage('Build docker image') {
            steps{
                script {
                    dockerImage = docker.build(dockerimagename + ':' + dockerimagetag, 'app/' + publishproject)
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