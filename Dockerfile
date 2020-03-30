
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM base AS final
WORKDIR /app
COPY geeksync-server .
ENTRYPOINT ["dotnet", "GeekSyncServer.dll"]
