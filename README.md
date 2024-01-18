# MyCode - Full Stack Code Storage

MyCode is a full-stack ASP.NET application designed to store and share users' favorite codes in a database with a React Frontend Server.

## Project Status

**ONGOING Project!**

## Installation Instructions
  - Install the .NET SDK 8.0.1.
  - Set up the database connection in the `secrets.json` file.
  - Install dependencies.
     
## About the Application
  - Register and log in to add your favorite code snippets.
  - Edit and delete your codes as needed.
  - Explore codes by visibility or user.

The backend is built on the ASP.NET 8 framework, with the main goals of being:
  - Secure
  - Transparent
  - Easy to use
  - Object-Oriented

The frontend is powered by React using VITE to provide a fast and an interactive user interface:
  - also Transparent
  - Easy to use: for example -> Centralized data structure in the Services folder
  
## Security
The application implements secure practices such as JWT token usage and secure password storage.

## Configuration

On the frontend side, sensitive data is stored in the `.env` file. To set up the application, create a `.env` file in the root directory and populate it with the following keys and values:

```env
VITE_FRONTEND_URL=https://
VITE_FRONTEND_PORT=
VITE_BACKEND_URL=https://
VITE_BACKEND_PORT=
```

On the backend side, sensitive data is stored in the `secrets.json` file. To set up the application, create a `secrets.json` file in the root directory and populate it with the following keys and values:

```json
{
  // Database Connection
  "ConnectionString": "YourDatabaseConnectionString",
  
  // Frontend Connection
  "FEAddress": "YourFrontendAddress",
  
  // Token Claims and Signature
  "IssueAudience": "YourIssueAudience",
  "IssueSign": "YourIssueSignature",
  
  // First Admin User Details
  "AEmail": "admin@example.com",
  "APass": "AdminPassword",
  "ACall": "123-456-7890",
  "UName": "AdminUserName",
  "AName": "AdminDisplayName"
}
```

## Acknowledgments

A special thanks to all contributors and libraries used in this ASP.NET/React project.
