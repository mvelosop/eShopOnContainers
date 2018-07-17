FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS publish
WORKDIR /src
COPY . .
RUN dotnet restore /ignoreprojectextensions:.dcproj
WORKDIR /src/src/ApiGateways/Web.Bff.Shopping/aggregator
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.Shopping.HttpAggregator.dll"]
