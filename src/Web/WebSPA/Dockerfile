ARG NODE_IMAGE=node:8.11
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM ${NODE_IMAGE} as node-build
WORKDIR /web
COPY src/Web/WebSPA .
RUN npm install
RUN npm run build:prod

FROM microsoft/dotnet:2.1-sdk as dotnet-build
WORKDIR /src
COPY . .
RUN dotnet restore /ignoreprojectextensions:.dcproj
WORKDIR /src/src/Web/WebSPA/wwwroot
COPY --from=node-build /web/wwwroot .
WORKDIR /src/src/Web/WebSPA
RUN dotnet restore 

FROM dotnet-build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebSPA.dll"]
