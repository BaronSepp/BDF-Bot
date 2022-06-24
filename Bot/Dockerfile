FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine-amd64 AS build-env
WORKDIR /src

COPY ./Bot/Bot.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./Bot/Bot.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine-amd64
WORKDIR /app
COPY --from=build-env /src/out ./

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs

ENTRYPOINT ["dotnet", "Bot.dll"]