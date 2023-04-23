FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GameSolverAPI/GameSolverAPI.csproj", "GameSolverAPI/"]
RUN dotnet restore "GameSolverAPI/GameSolverAPI.csproj"
COPY . .
WORKDIR "/src/GameSolverAPI"
RUN dotnet build "GameSolverAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameSolverAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameSolverAPI.dll"]
