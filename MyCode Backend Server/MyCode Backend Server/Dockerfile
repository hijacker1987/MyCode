# Stage 1: Base image with .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 443
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build image with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["MyCode Backend Server.csproj", "MyCode Backend Server/"]
RUN dotnet restore "MyCode Backend Server/MyCode Backend Server.csproj"

# Copy the appsettings.json file
COPY ["appsettings.json", "/app/appsettings.json"]

# Copy everything else and build
COPY . .
WORKDIR "/src/MyCode Backend Server/MyCode Backend Server"
RUN dotnet build "MyCode Backend Server/MyCode Backend Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ensure OpenSSL is available and the certificate file has the correct permissions
RUN apt-get update && apt-get install -y openssl

# Set environment variables from arguments
ARG IssueAudience
ARG ConnectionString
ARG IssueSign
ARG UName
ARG Kestrel_Certificates_Development_Password
ARG InitDb
ARG GoogleClientSecret
ARG GoogleClientId
ARG GitHubClientSecret
ARG GitHubClientId
ARG FacebookClientSecret
ARG FacebookClientId
ARG APass
ARG AName
ARG AEmail
ARG ACall

ENV DOCKER_CONTAINER=true \
    IssueAudience=$IssueAudience \
    ConnectionString=$ConnectionString \
    IssueSign=$IssueSign \
    UName=$UName \
    Kestrel_Certificates_Development_Password=$Kestrel_Certificates_Development_Password \
    InitDb=$InitDb \
    GoogleClientSecret=$GoogleClientSecret \
    GoogleClientId=$GoogleClientId \
    GitHubClientSecret=$GitHubClientSecret \
    GitHubClientId=$GitHubClientId \
    FacebookClientSecret=$FacebookClientSecret \
    FacebookClientId=$FacebookClientId \
    APass=$APass \
    AName=$AName \
    AEmail=$AEmail \
    ACall=$ACall

# Stage 3: Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyCode Backend Server/MyCode Backend Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final image with runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy the server certificate file from the build stage
COPY --from=build ["/src/MyCode Backend Server/MyCode Backend Server/Certs/mycode.pfx", "/https/mycode.pfx"]
RUN chmod 644 /https/mycode.pfx

ENTRYPOINT ["dotnet", "MyCode Backend Server.dll"]
