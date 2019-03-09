FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS publish
WORKDIR /src
COPY . .
WORKDIR /src/src/Services/Catalog/Catalog.API
RUN dotnet restore --packages /src/packages
RUN dotnet publish --no-restore -c Release -o /app

FROM publish as unittest
WORKDIR /src/src/Services/Catalog/Catalog.UnitTests

FROM publish as functionaltest
WORKDIR /src/src/Services/Catalog/Catalog.FunctionalTests

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
