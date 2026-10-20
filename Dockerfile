# ==========================================
# Multi-stage build for TJK Delta (.NET 8)
# ==========================================

# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["tjk/tjk.csproj", "tjk/"]
RUN dotnet restore "tjk/tjk.csproj"

# Copy full source and build Release
COPY . .
WORKDIR "/src/tjk"
RUN dotnet publish "tjk.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Configure ASP.NET Core environment for production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Copy published binaries
COPY --from=build /app/publish .

# Set up writable data folder for SQLite and uploads
RUN mkdir -p /app/data/uploads

ENTRYPOINT ["dotnet", "tjk.dll"]
