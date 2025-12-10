# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código y compilar
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Instalar librerías necesarias para PDFs (libgdiplus)
RUN apt-get update && apt-get install -y libgdiplus libc6-dev && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Exponer puerto
EXPOSE 80

# Variables de entorno por defecto (se sobrescriben en docker-compose)
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TalentoPlusAPI.dll"]