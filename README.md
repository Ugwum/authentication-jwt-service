# Authentication Service

The Authentication Service is a .NET Core application that provides token-based authentication using JSON Web Tokens (JWT). It includes features for token generation, refresh, revocation, and secure public key exchange for client validation.

## Table of Contents

- [Authentication Service](#authentication-service)
  - [Table of Contents](#table-of-contents)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
  - [Usage](#usage)
    - [Authentication](#authentication)
    - [Token Refresh](#token-refresh)
    - [Token Revocation](#token-revocation)
    - [Key Exchange](#key-exchange)
  - [Configuration](#configuration)
  - [Dependencies](#dependencies)
  - [Contributing](#contributing)
  - [License](#license)

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) or any preferred IDE
- Auth Service source code

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Ugwum/authentication-jwt-service.git
2. Navigate to the project folder:
    ```bash
    cd AuthService.API
3. Build and run the application 
     ```bash
    dotnet build
    dotnet run

## Usage
### Authentication
To authenticate a user and obtain an access token, make a POST request to the /api/v1/auth/token endpoint with the user's credentials.

Example Request:
   ``` bash 
        curl -X POST -H "Content-Type: application/json" -d '{"username": "user123", "password": "password123"}' http://localhost:5000/api/v1/auth/token
   ```
### Token Refresh
To refresh an expired access token, make a POST request to the /api/v1/auth/refresh endpoint with the expired token.

Example Request:
   ```bash
   curl -X POST -H "Content-Type: application/json" -d '{"expiredToken": "your_expired_token_here"}' http://localhost:5000/api/v1/auth/refresh
   ```
Token Revocation
To revoke an access token, make a POST request to the /api/v1/auth/revoketoken endpoint with the access token to be revoked.

Example Request:
  ```bash
  curl -X POST -H "Content-Type: application/json" -d '{"accesstoken": "your_access_token_here"}' http://localhost:5000/api/v1/auth/revoketoken
  ```

## Dependencies
  - Microsoft.AspNetCore
  - Newtonsoft.Json
  - Polly
  - System.IdentityModel.Tokens.Jwt
- Ensure these dependencies are included in your project.

## Contributing
Contributions are welcome! Please follow the Contributing Guidelines.

## License
This project is licensed under the MIT License.




