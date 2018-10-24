pipeline {
    environment {
            scmUrl = "https://github.com/LykkeCity/Lykke.Job.BlobToBlobConverter.Orderbook"
            branch = "dev"
            dockerimagename = "lykkedev/lykke-job-blobtoblobconverter-orderbook"
            dockerimagetag = "dev"
            dockercredentials = "lykkedev"
            publishproject = "Lykke.Job.BlobToBlobConverter.Orderbook"
        }
    agent {label 'agent1'}
    stages {
        stage('checkout git') {
            steps {
                git branch: branch, url: scmUrl
            }
        }
        stage("build") {
            steps {
                sh "dotnet build --configuration Release"
            }
        }
        stage("test") {
            steps {
                sh "dotnet test tests/**/*.csproj --configuration Release --no-build"
            }
        }
        stage("publish") {
            steps {
              sh "dotnet publish src/${env.publishproject}/${env.publishproject}.csproj --configuration Release --no-restore --output ${env.WORKSPACE}/app/${env.publishproject}"
            }
        }
        stage("build docker image") {
            steps{
                script {
                    dockerImage = docker.build("${env.dockerimagename}:${env.dockerimagetag}", "app/${env.publishproject}")
                }
            }
        }
        stage("deploy docker image") {
            steps{
                script {
                  docker.withRegistry( "", dockercredentials ) {
                      dockerImage.push()
                  }
                }
            }
        }
        stage("docker image cleanup") {
            steps{
                sh "docker rmi ${env.dockerimagename}:${env.dockerimagetag}"
            }
        }
    }
}