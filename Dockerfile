FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

RUN echo "{ \
    \"OPENAPI_SECRET_KEY\": \"$OPENAPI_SECRET_KEY\", \
    \"OPENAPI_ENDPOINT\": \"$OPENAPI_ENDPOINT\", \
    \"GPT_35_TURBO_DN\": \"$GPT_35_TURBO_DN\", \
    \"SECRET_TOKEN\": \"$SECRET_TOKEN\" \
}" > secrets.json

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /App

COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "SK_API.dll"]