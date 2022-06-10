FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./Bot/Bot.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./Bot/Bot.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build-env /src/out ./

ENTRYPOINT ["dotnet", "Bot.dll"]