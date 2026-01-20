#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app // No need use
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient.csproj", "CaoGiaConstruction.WebClient/"]
COPY ["CaoGiaConstruction.Utilities/CaoGiaConstruction.Utilities.csproj", "CaoGiaConstruction.Utilities/"]
RUN dotnet restore "./CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient.csproj"
COPY . .
WORKDIR "/src/CaoGiaConstruction.WebClient"
RUN dotnet build "./CaoGiaConstruction.WebClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CaoGiaConstruction.WebClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt update
RUN apt-get install -y libfontconfig1 
RUN apt-get -qq -y install libgdiplus
ENTRYPOINT ["dotnet", "CaoGiaConstruction.WebClient.dll"]