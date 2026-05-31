# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the .csproj file and restore dependencies
# NOTE: Agar aapke project ka naam 'AuraMist.csproj' hai tou niche wahi naam likhein
COPY ["AuraMist/AuraMist.csproj", "AuraMist/"]
RUN dotnet restore "AuraMist/AuraMist.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/AuraMist"
RUN dotnet build "AuraMist.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "AuraMist.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: copy published files and set entrypoint
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuraMist.dll"]