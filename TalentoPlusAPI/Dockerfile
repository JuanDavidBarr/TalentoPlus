# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source code and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install necessary libraries for PDFs (libgdiplus)
RUN apt-get update && apt-get install -y libgdiplus libc6-dev && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Expose port
EXPOSE 80

# Default environment variables (overridden in docker-compose)
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TalentoPlusAPI.dll"]