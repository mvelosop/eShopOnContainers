
| cold build | time mm:ss |
| all.ps1 - cold | 22:01 |
| all.ps1 - warm - no change | 

pre-installed images:

- microsoft/dotnet:2.2.0-aspnetcore-runtime
- microsoft/dotnet:2.2.100-sdk
- node:8.11

## ordering.api

```Dockerfile
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2.100-sdk AS build
WORKDIR /src
COPY . .

#3 Restore for all projects
RUN dotnet restore /ignoreprojectextensions:.dcproj
WORKDIR /src/src/Services/Ordering/Ordering.API
#3 RUN dotnet restore -nowarn:msb3202,nu1503

#2 RUN dotnet build --no-restore -c Release -o /app

# FROM build as functionaltest
# WORKDIR /src/src/Services/Ordering/Ordering.FunctionalTests

# FROM build as unittest
# WORKDIR /src/src/Services/Ordering/Ordering.UnitTests

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
```

| # | Action | Time | Notes |
|---|--------|------|-------|
| 0 | Clean build | 01:10 | 01:09, 01:10, 01:10, 01:09 |
| 1 | Build after ordering.background w/o cleaning | 01:08 | No significant difference |
| 2 | Remove `RUN dotnet build...` | 00:57 | -13s, `dotnet publish...` also builds |
| 3 | | 02:16 | |


## ordering.backgroundtasks

```Dockerfile
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2.100-sdk AS build
WORKDIR /src
COPY . .

#3 Restore for all projects
RUN dotnet restore /ignoreprojectextensions:.dcproj
WORKDIR /src/src/Services/Ordering/Ordering.BackgroundTasks
#3 RUN dotnet restore -nowarn:msb3202,nu1503

#2 RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore  -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ordering.BackgroundTasks.dll"]
```

| # | Action | Time | Notes |
|---|--------|------|-------|
| 0 | Clean build | 01:03 | 01:04, 00:53, 01:07, 00:55 |
| 1 | Build after ordering.api w/o cleaning | 01:00 | No significant difference |
| 2 | Remove `RUN dotnet build...` | 00:43 | -20s, `dotnet publish...` also builds |

