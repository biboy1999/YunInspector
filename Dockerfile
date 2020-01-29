FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dotnet-build-env
ARG project_name
COPY . /$project_name
WORKDIR /
RUN dotnet publish $project_name -o /publish --configuration Release

### Publish Stage
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ARG project_name
WORKDIR /app
COPY --from=dotnet-build-env /publish .
ENV project_dll="${project_name}.dll"
ENTRYPOINT dotnet $project_dll