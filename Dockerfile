FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY MyWeather.sln .
COPY MyWeather/MyWeather.csproj MyWeather/
RUN dotnet restore
COPY MyWeather MyWeather
RUN dotnet publish MyWeather/MyWeather.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y --no-install-recommends libkrb5-3 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "MyWeather.dll"]

