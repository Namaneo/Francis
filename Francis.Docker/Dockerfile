FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS sdk
WORKDIR src/

COPY . .

RUN apk add yarn

WORKDIR Francis/
RUN dotnet publish -c Release -o /publish --runtime linux-x64

FROM lsiobase/ubuntu:bionic AS runtime
WORKDIR app/

COPY --from=sdk /publish .
COPY --from=sdk /src/Francis.Docker/lsiobase /

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
ENV DOTNET_RUNNING_IN_CONTAINER=true

VOLUME /config
EXPOSE 4703
