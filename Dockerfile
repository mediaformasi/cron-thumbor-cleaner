FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder
WORKDIR /app
COPY . .

RUN set -ex; \
  dotnet publish -c Release; \
  mv /app/bin/Release/net8.0/linux-x64/publish/CronThumborCleaner /app/

FROM docker.io/library/alpine:latest as runner
WORKDIR /app
COPY --from=builder /app/CronThumborCleaner /app/
