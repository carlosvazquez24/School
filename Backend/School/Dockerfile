## syntax=docker/dockerfile:1

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution and project files
COPY ["School.sln", "./"]
COPY ["AppServices/AppServices.csproj", "AppServices/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Dtos/Dtos.csproj", "Dtos/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["School/School.csproj", "School/"]

# Restore dependencies
RUN dotnet restore "School/School.csproj"

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish "School/School.csproj" -c Release -o /app/publish


# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port (adjust if different)
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "School.dll"]
