FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Unzer.sln ./
COPY Unzer/*.csproj ./Unzer/
RUN dotnet restore

COPY . ./
WORKDIR /app/Unzer
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Unzer.dll"]
