# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar todos los archivos .csproj y carpetas necesarias para el restore
COPY ./src/*.csproj ./src/
RUN dotnet restore

# Copiar el resto del código fuente
COPY . ./

# Publicar el proyecto
RUN dotnet publish -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar el resultado de la etapa de build
COPY --from=build /app/out ./

# Establecer el punto de entrada para ejecutar la aplicación
ENTRYPOINT ["dotnet", "School.dll"]
