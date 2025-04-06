FROM docker.io/library/golang:alpine AS build
WORKDIR /build
COPY . .

RUN apk update; \
    apk add make gcc musl-dev;

RUN go get -C ./cmd/cron-thumbor-cleaner; \
    go build -C ./cmd/cron-thumbor-cleaner \
        -ldflags="-linkmode external -extldflags -static -w -s" \
        -o "../../cron-thumbor-cleaner"; \
    chmod +x ./cron-thumbor-cleaner;

FROM docker.io/library/alpine:latest
WORKDIR /
COPY --from=build /build/cron-thumbor-cleaner /usr/bin/cron-thumbor-cleaner
CMD ["cron-thumbor-cleaner"]
