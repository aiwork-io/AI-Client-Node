# AI-Client-Node
* This repo contains the codes and documentations of the AI Client Node which is to be installed on computer of users who wish to contribute their CPU/GPU resources to the network

## How to run AI-Client-Node

### Prerequisites

- Have inference-engine loaded to your docker engine

### How to run
To run the project, simply execute the following command in the root folder
* docker-compose up --build
* To stop the project and clean up the resources, execute `docker-compose down`

### How to test
1. Navigate to `localhost:8083` and login with test user
2. Submit a new job to rabbit mq
