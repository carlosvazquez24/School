# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo de solución y todos los proyectos relacionados
COPY *.sln ./
COPY AppServices/AppServices.csproj ./AppServices/
COPY DataAccess/DataAccess.csproj ./DataAccess/
COPY DTO's/DTO's.csproj ./DTO's/
COPY Entities/Entities.csproj ./Entities/

# Restaurar las dependencias
RUN dotnet restore

# Copiar todo el código fuente
COPY . ./

# Publicar la aplicación
RUN dotnet publish -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "School.dll"]
