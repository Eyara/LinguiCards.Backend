FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LinguiCards/LinguiCards.csproj", "LinguiCards/"]
RUN dotnet restore "LinguiCards/LinguiCards.csproj"
COPY . .
WORKDIR "/src/LinguiCards"
RUN dotnet build "LinguiCards.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinguiCards.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinguiCards.dll"]
