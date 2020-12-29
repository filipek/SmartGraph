#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["SmartGraph.TestApp/SmartGraph.TestApp.csproj", "SmartGraph.TestApp/"]
COPY ["SmartGraph/SmartGraph.csproj", "SmartGraph/"]
COPY ["SmartGraph.Nodes/SmartGraph.Nodes.csproj", "SmartGraph.Nodes/"]
RUN dotnet restore "SmartGraph.TestApp/SmartGraph.TestApp.csproj"
COPY . .
WORKDIR "/src/SmartGraph.TestApp"
RUN dotnet build "SmartGraph.TestApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartGraph.TestApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartGraph.TestApp.dll"]