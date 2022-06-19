FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine-amd64 AS build-env
WORKDIR /src

COPY ./Bot/Bot.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./Bot/Bot.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine-amd64
WORKDIR /app
COPY --from=build-env /src/out ./

ENTRYPOINT ["dotnet", "Bot.dll"]