# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the .csproj file from root and restore
COPY ["WinterSheltonHouse.csproj", "./"]
RUN dotnet restore "WinterSheltonHouse.csproj"

# Copy everything else
COPY . .
RUN dotnet build "WinterSheltonHouse.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "WinterSheltonHouse.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: copy published files and set entrypoint
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WinterSheltonHouse.dll"]