# MyCode - Full Stack Code Storage

MyCode is a full-stack ASP.NET application designed to store and share users favorite codes in a database with a React Frontend Server.

## Project Status

**ONGOING Project!**
*Docker Compose under construction*

## Backend Installation Instructions
  - Install the .NET SDK 8.0.300 (.NET 8.0.5)
  - Install dependencies.
  - Set up the database connections, and other needs through the `.env`, and `appsettings.json` files.
     
## About the Application
  - Register than log in to add your favorite code snippets.
  - Edit and delete your codes as You desire.
  - Explore codes by visibility or user to extend Your knowledge.
  - Now able to login via GitHub, Google and Facebook accounts
  - Local storage in use for Monaco Editor setups (for each logged in user)
  - Chatbot and Live chat service for customer assistance

The backend is built on the ASP.NET 8 framework, with the main goals of being/having:
  - Secure /for example: mfa (which obsolete 2fa)/
  - Transparent
  - Easy to use
  - Object-Oriented
  - MSSQL database /Testing uses separate MSSQL database/
  - SignalR websocket connection

The frontend is powered by React using VITE to provide a fast and an interactive user interface:
  - also Transparent
  - Secure (for example: need Verified account to be able to add/modify any code)
  - Styled Components
  - Easy to use: for example -> Centralized data structure in the Services folder
  - Monaco Editor
  - Interactive chatbot
  - Live chat with support (SignalR)
  - Easter eggs
  
## Security
The application implements secure practices such as JWT token, refresh token and hashed password storage, password confirmation, currently running two dockerized MSSQL databases (one for proper testing),
and sensitive data is stored in the `appsettings.json` and `.env` files.

## Configuration
On the frontend side, sensitive data is stored in the `.env` file. To set up the application, create a `.env` file in the root directory and populate it with the following keys and values:

```env
VITE_FRONTEND_URL=https://
VITE_FRONTEND_PORT=
VITE_BACKEND_URL=https://
VITE_BACKEND_PORT=
```

On the backend side, sensitive data is stored in the `secrets.json` file. To set up the application, create a `secrets.json` file in the user profile directory:
   - Windows: %APPDATA%\microsoft\UserSecrets\<userSecretsId>\secrets.json
   - Linux: ~/. microsoft/usersecrets/<userSecretsId>/secrets.json
   - Mac: ~/. microsoft/usersecrets/<userSecretsId>/secrets.json

  - Database can be initialized, the basic setup contains dummy data (1 admin, 30 users, and about 80 codes being generated)
  - Test database will be initialized when You run tests, the basic setup contains dummy data (1 admin, 10 users, and about 80 codes being generated)
   
In the outer backend server folder with examples:
```env
# Required for Compose
MSSQL1_PASSWORD=yourStrong(!)Password
MSSQL2_PASSWORD=anotherStrong(!)Password
FE_URL=https://localhost
FE_PORT=5173
BE_URL=https://localhost
BE_PORT=7001
# Required for Backend Server
UName=MainAdmin
ASPNETCORE_Kestrel_Certificates_Default_Password=guid
ASPNETCORE_Kestrel__Certificates__Default__Path=toYourCertfile.pfx
IssueSign=V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1ts
IssueAudience=api With Authentication comes and goes here
# If You need dummy data ->
InitDb=True
GoogleClientSecret=placeHere
GoogleClientId=placeHere
GitHubClientSecret=placeHere
GitHubClientId=placeHere
FacebookClientSecret=placeHere
FacebookClientId=placeHere
ConnectionString=Server=name,1433;Database=sql-server;User Id=sa;Password=somethingStrong;Encrypt=False;
APass=AdminWithReallyStrongPassword
AName=Super Admin
AEmail=admin@sadmin.com
ACall=12-34/567-8910
```

In the inner backend server folder with examples:
```env
UName=MainAdmin
Kestrel_Certificates_Default_Password=guid
IssueSign=V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1ts
IssueAudience=api With Authentication comes and goes here
# If You need dummy data ->
InitDb=True
GoogleClientSecret=placeHere
GoogleClientId=placeHere
GitHubClientSecret=placeHere
GitHubClientId=placeHere
FacebookClientSecret=placeHere
FacebookClientId=placeHere
ConnectionString=Server=name,1433;Database=sql-server;User Id=sa;Password=somethingStrong;Encrypt=False;
APass=AdminWithReallyStrongPassword
AName=Super Admin
AEmail=admin@sadmin.com
ACall=12-34/567-8910
```

The `appsettings.json` file should contain the address for the "live" test database, and the frontend address:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  // Frontend Connection
  "FEAddress": "Your Frontend Address",

  // Token Expiration (Access in minutes, Refresh in hours)
  "AccessTokenExp": 10,
  "RefreshTokenExp": 2,

    // "Live" database for test purposes
  "ConnectionStrings": {
    "TestConnectionString": "YourConnectionToYourLiveTestDatabase;"
  },
  "IssueSign": "V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng",
  "ValidAudience": "api With Test Authentication comes and goes here",
  "ValidIssuer": "api With Test Authentication comes and goes here"
}
```

## Acknowledgments

A special thanks to all contributors and libraries used in this ASP.NET/React project.
