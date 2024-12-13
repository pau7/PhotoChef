# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos de solución y proyectos
COPY *.sln .
COPY PhotoChef.API/*.csproj ./PhotoChef.API/
COPY PhotoChef.Domain/*.csproj ./PhotoChef.Domain/
COPY PhotoChef.Infrastructure/*.csproj ./PhotoChef.Infrastructure/
COPY PhotoChef.Tests/*.csproj ./PhotoChef.Tests/

# Restaurar dependencias
RUN dotnet restore

# Copiar el resto de los archivos al contenedor
COPY . .

# Publicar la aplicación
WORKDIR /app/PhotoChef.API
RUN dotnet publish -c Release -o out

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar los archivos publicados desde la etapa de construcción
COPY --from=build /app/PhotoChef.API/out ./

# Copiar el archivo SQLite preconfigurado al contenedor desde el proyecto API
COPY PhotoChef.API/PhotoChef.db /app/PhotoChef.db

# Exponer puertos
EXPOSE 80
EXPOSE 443

# Configurar el punto de entrada
ENTRYPOINT ["dotnet", "PhotoChef.API.dll"]
