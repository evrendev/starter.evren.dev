# --- Build aşaması ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore BackendBoilerplate.sln
RUN dotnet publish src/backend/PublicApi/PublicApi.csproj -c Release -o /out

# --- Development aşaması ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /app
COPY . .

# Install dotnet tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Restore packages
RUN dotnet restore BackendBoilerplate.sln

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5001

# Default command for development
CMD ["dotnet", "watch", "run", "--project", "src/backend/PublicApi/PublicApi.csproj", "--urls", "http://+:5001"]

# --- Runtime aşaması ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "EvrenDev.PublicApi.dll"]
