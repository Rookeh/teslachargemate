FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["TeslaChargeMate.csproj", ""]
RUN dotnet restore "./TeslaChargeMate.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TeslaChargeMate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeslaChargeMate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeslaChargeMate.dll"]