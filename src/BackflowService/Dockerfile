#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy-amd64 AS base
RUN apt-get update && apt-get install -y --no-install-recommends git \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./BackflowService.csproj"
RUN dotnet build "BackflowService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackflowService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /src/source-mappings.json .
COPY --from=publish /app/publish .
RUN git config --global user.email "dotnet-maestro[bot]@users.noreply.github.com" \
    && git config --global user.name "dotnet-maestro[bot]" \
    && git config --global core.autocrlf input \
    && git config --global core.filemode false
ENTRYPOINT ["dotnet", "BackflowService.dll"]
