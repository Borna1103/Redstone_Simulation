# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Render injects PORT — ASP.NET MUST listen on it
ENV ASPNETCORE_URLS=http://+:${PORT}

COPY --from=build /app .

ENTRYPOINT ["dotnet", "Redstone_Simulation.dll"]
