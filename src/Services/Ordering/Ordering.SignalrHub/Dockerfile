FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS publish
WORKDIR /src
COPY . .
RUN dotnet restore /ignoreprojectextensions:.dcproj
WORKDIR /src/src/Services/Ordering/Ordering.SignalrHub
RUN dotnet publish  --no-restore  Ordering.SignalrHub.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ordering.SignalrHub.dll"]
