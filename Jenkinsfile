// Jenkinsfile — Pipeline CI/CD complet (Parties 3 & 4)
pipeline {
    agent { label 'agent-1' }

    environment {
        IMAGE_NAME = "httpserver"
        IMAGE_TAG  = "${env.BUILD_NUMBER}"
        DOCKER_HUB_USER = credentials('dockerhub-creds')   // credential Jenkins
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore & Build') {
            steps {
                sh 'dotnet restore src/Http-Server.csproj'
                sh 'dotnet build src/Http-Server.csproj -c Release --no-restore'
            }
        }

        stage('Publish') {
            steps {
                sh 'dotnet publish src/Http-Server.csproj -c Release -o ./publish --no-build'
            }
        }

        stage('Build Docker Image') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-creds',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh "docker build -t ${DOCKER_USER}/${IMAGE_NAME}:${IMAGE_TAG} ."
                    sh "docker tag ${DOCKER_USER}/${IMAGE_NAME}:${IMAGE_TAG} ${DOCKER_USER}/${IMAGE_NAME}:latest"
                }
            }
        }
        //
        stage('Push to Docker Hub') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-creds',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh '''
                        echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin
                        docker push $DOCKER_USER/''' + env.IMAGE_NAME + ''':''' + env.IMAGE_TAG + '''
                        docker push $DOCKER_USER/''' + env.IMAGE_NAME + ''':latest
                    '''
                }
            }
        } 
        stage('Deploy') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-creds',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh """
                        docker stop ${IMAGE_NAME} || true
                        docker rm   ${IMAGE_NAME} || true
                        docker pull $DOCKER_USER/${IMAGE_NAME}:latest
                        docker run -d --name ${IMAGE_NAME} -p 3000:3000 $DOCKER_USER/${IMAGE_NAME}:latest
                    """
                }
            }
        }
    }

    post {
        success { echo "Déployé avec succès — build #${env.BUILD_NUMBER}" }
        failure  { echo 'Pipeline échoué.' }
    }
}
