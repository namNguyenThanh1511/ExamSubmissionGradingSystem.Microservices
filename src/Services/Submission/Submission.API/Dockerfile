# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Submission/Submission.API/Submission.API.csproj", "Services/Submission/Submission.API/"]
COPY ["Services/Submission/Submission.Services/Submission.Services.csproj", "Services/Submission/Submission.Services/"]
COPY ["Services/Submission/Submission.Repositories/Submission.Repositories.csproj", "Services/Submission/Submission.Repositories/"]
RUN dotnet restore "./Services/Submission/Submission.API/Submission.API.csproj"
COPY . .
WORKDIR "/src/Services/Submission/Submission.API"
RUN dotnet build "./Submission.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Submission.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Submission.API.dll"]