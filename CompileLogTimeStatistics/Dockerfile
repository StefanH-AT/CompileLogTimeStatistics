FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CompileLogTimeStatistics/CompileLogTimeStatistics.csproj", "CompileLogTimeStatistics/"]
RUN dotnet restore "CompileLogTimeStatistics/CompileLogTimeStatistics.csproj"
COPY . .
WORKDIR "/src/CompileLogTimeStatistics"
RUN dotnet build "CompileLogTimeStatistics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CompileLogTimeStatistics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompileLogTimeStatistics.dll"]
