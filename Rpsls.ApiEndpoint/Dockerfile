FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Rpsls.ApiEndpoint/Rpsls.ApiEndpoint.csproj", "Rpsls.ApiEndpoint/"]
RUN dotnet restore "Rpsls.ApiEndpoint/Rpsls.ApiEndpoint.csproj"
COPY . .
WORKDIR "/src/Rpsls.ApiEndpoint"
RUN dotnet build "Rpsls.ApiEndpoint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rpsls.ApiEndpoint.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rpsls.ApiEndpoint.dll"]
