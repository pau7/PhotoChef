# PhotoChef API

PhotoChef API is an application for managing recipes and recipe books. It allows users to upload images, generate recipe books in PDF format, and manage ingredients and allergens in a structured way.

## Description

This project enables users to:
- Register and authenticate using JWT.
- Create, read, update, and delete recipes and recipe books.
- Upload images related to recipes and books.
- Generate recipe books in PDF format with detailed images, ingredients, and allergens.

The API is designed to be scalable and runs within Docker containers. It is currently deployed on AWS.

## Key Features

- **Authentication and Authorization:** Uses JWT to ensure secure access.
- **Recipe Management:** Full CRUD operations for recipes, supporting ingredients and allergens.
- **Recipe Book Management:** Full CRUD operations for recipe books, including cover pages.
- **PDF Generation:** Converts recipes into a ready-to-print book format.
- **Integration with Docker and AWS.**

## Prerequisites

Ensure the following components are installed before proceeding:
- [.NET SDK 8.0](https://dotnet.microsoft.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [AWS CLI](https://aws.amazon.com/cli/)
- SQLite (configured within the Docker container)

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/your-user/photo-chef.git
   cd photo-chef
   ```

2. Build and run the application using Docker:
   ```bash
   docker build -t photochef-api .
   docker run -p 8080:80 photochef-api
   ```

3. The API will be available at: `http://localhost:8080`.

## Usage

### Main Endpoints
- **Users:**
  - `POST /api/users/register` - Register a new user.
  - `POST /api/users/login` - Log in and obtain a JWT token.

- **Recipes:**
  - `POST /api/recipes` - Create a recipe.
  - `GET /api/recipes/{id}` - Retrieve a recipe by ID.
  - `DELETE /api/recipes/{id}` - Delete a recipe.

- **Recipe Books:**
  - `POST /api/recipebooks` - Create a recipe book.
  - `GET /api/recipebooks/{id}` - Retrieve a recipe book.

### JWT Token
To use protected endpoints, you need a JWT token. Register and log in to obtain it.

### Swagger
API documentation is available at: `http://localhost:8080/swagger`.

## Testing

The project includes a suite of unit tests to verify core functionality. Run tests using:
```bash
dotnet test
```

## Deployment

The application is configured for deployment on AWS using Fargate. Follow these steps to deploy:
1. Build the Docker image.
2. Push the image to Amazon ECR.
3. Set up an ECS cluster with Fargate.
4. Ensure port 80 is exposed and HTTP traffic is allowed.


## License

This project is licensed under the Unlicense license. See the `LICENSE` file for more details.

## Contact

Author: Pau Segarra Agut  
Email: psegarra7@gmail.com

