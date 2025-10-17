FROM mcr.microsoft.com/dotnet/sdk:9.0.306-alpine3.22 AS build
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.22
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080

LABEL org.opencontainers.image.title="OneUrlApi"
LABEL org.opencontainers.image.licenses="GPL v3"
LABEL org.opencontainers.image.url="https://github.com/Hammered-Dev/OneUrl"

CMD [ "dotnet", "OneUrl.dll" ]