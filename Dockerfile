FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . /src
RUN dotnet restore TextParsingLibPOC.sln
RUN dotnet build TextParsingLibPOC.sln -c Release -o /app


FROM base AS runtime
COPY --from=build /app .
ENTRYPOINT ["dotnet", "TextParsingLibPOC.dll"]
