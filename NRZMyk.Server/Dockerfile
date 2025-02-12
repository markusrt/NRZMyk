#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra AS base
#RUN apt-get -yqq update && apt-get -yqq install libgdiplus

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS build
WORKDIR /src
COPY . .
WORKDIR "/src/NRZMyk.Server"
RUN dotnet build "NRZMyk.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NRZMyk.Server.csproj" -c Release -o /app/publish
WORKDIR "/app/publish/wwwroot"
RUN version="$(date '+%Y-%m-%d-%H-%M-%S')" && sed -i "s/\${version}/${version}/" index.html

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NRZMyk.Server.dll"]