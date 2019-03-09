FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS publish
WORKDIR /src
COPY . .
WORKDIR /src/src/Services/Location/Locations.API
RUN dotnet restore --packages /src/packages
RUN dotnet publish --no-restore -c Release -o /app

FROM publish as functionaltest
WORKDIR /src/src/Services/Location/Locations.FunctionalTests

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Locations.API.dll"]
