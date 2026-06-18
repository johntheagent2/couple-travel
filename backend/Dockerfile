FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY CoupleTravel.Api/CoupleTravel.Api.csproj CoupleTravel.Api/
RUN dotnet restore CoupleTravel.Api/CoupleTravel.Api.csproj
COPY . .
RUN dotnet publish CoupleTravel.Api/CoupleTravel.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CoupleTravel.Api.dll"]
