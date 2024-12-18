FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
# find . -type f -name *.csproj | sort | xargs -I{} sh -c 'file={} && i=${file#./} && echo "COPY ${i} ${i%/*}/"'
COPY Sources/Core/Core.csproj Sources/Core/
COPY Sources/ThematicMapCreator.Client/ThematicMapCreator.Client.csproj Sources/ThematicMapCreator.Client/
COPY Sources/ThematicMapCreator.Contracts/ThematicMapCreator.Contracts.csproj Sources/ThematicMapCreator.Contracts/
COPY Sources/ThematicMapCreator.Domain/ThematicMapCreator.Domain.csproj Sources/ThematicMapCreator.Domain/
COPY Sources/ThematicMapCreator.Host/ThematicMapCreator.Host.csproj Sources/ThematicMapCreator.Host/
COPY Sources/ThematicMapCreator.Tests/ThematicMapCreator.Tests.csproj Sources/ThematicMapCreator.Tests/
COPY ThematicMapCreator.sln Directory.Build.props stylecop.ruleset ./
RUN dotnet restore
COPY Sources/ Sources/
RUN dotnet build --no-restore --configuration 'Release'

FROM build AS test
CMD dotnet test --no-build --configuration 'Release' --verbosity 'normal'

FROM build AS publish-host
RUN dotnet publish Sources/ThematicMapCreator.Host/ThematicMapCreator.Host.csproj --no-build --configuration 'Release' --output '/publish'

FROM build AS publish-client
RUN dotnet publish Sources/ThematicMapCreator.Client/ThematicMapCreator.Client.csproj --no-build --configuration 'Release' --output '/publish'

FROM base AS host
WORKDIR /app
COPY --from=publish-host /publish .
ENTRYPOINT ["dotnet", "ThematicMapCreator.Host.dll"]

FROM base AS client
WORKDIR /app
COPY --from=publish-client /publish .
ENTRYPOINT ["dotnet", "ThematicMapCreator.Client.dll"]
