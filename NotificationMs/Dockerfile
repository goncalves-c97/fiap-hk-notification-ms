FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solution e projetos primeiro (melhor cache)
COPY *.sln ./
COPY Core/*.csproj ./Core/
COPY Infra.Messaging/*.csproj ./Infra.Messaging/
COPY Infra.Email/*.csproj ./Infra.Email/
COPY Test/*.csproj ./Test/
COPY NotificationSenderWorker/*.csproj ./NotificationSenderWorker/

RUN dotnet restore

COPY . .

WORKDIR /src/NotificationSenderWorker
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "NotificationSenderWorker.dll"]
