# Build project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

RUN curl -Lo /tmp/datadog-dotnet-apm_2.28.0_amd64.deb https://github.com/DataDog/dd-trace-dotnet/releases/download/v2.28.0/datadog-dotnet-apm_2.28.0_amd64.deb

WORKDIR /home/app

ADD SuperbackCiamOTP/SuperbackCiamOTP.csproj .
ADD stylecop.json .
RUN dotnet restore

ADD . .

RUN dotnet build -c Release -o build

# Expose build		

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final

COPY --from=build /tmp/datadog-dotnet-apm_2.28.0_amd64.deb /tmp/datadog-dotnet-apm_2.28.0_amd64.deb

RUN dpkg -i /tmp/datadog-dotnet-apm_2.28.0_amd64.deb \
     && /opt/datadog/createLogPath.sh

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog

WORKDIR /home/app/

# ENV AUTH0_DOMAIN=superback4u.us.auth0.com
# ENV CLIENT_ID=""
# ENV CLIENT_SECRET=""

EXPOSE 80

COPY --from=build /home/app/build .

ENTRYPOINT [ "dotnet", "SuperbackCiamOTP.dll" ]

