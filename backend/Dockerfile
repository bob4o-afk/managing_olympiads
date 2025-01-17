FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY OlympiadApi/*.csproj ./OlympiadApi/
RUN dotnet restore ./OlympiadApi

COPY OlympiadApi/ ./OlympiadApi/
WORKDIR /app/OlympiadApi

RUN dotnet build -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/build .

EXPOSE 5138

ENV DOTNET_ENVIRONMENT=Production

CMD ["dotnet", "OlympiadApi.dll"]
