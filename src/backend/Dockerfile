# --- Build aşaması ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore BackendBoilerplate.sln
RUN dotnet publish src/backend/PublicApi/PublicApi.csproj -c Release -o /out

# --- Runtime aşaması ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out ./

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "EvrenDev.PublicApi.dll"]
