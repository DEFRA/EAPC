#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Forestry.Eapc.External.Web/Forestry.Eapc.External.Web.csproj", "Forestry.Eapc.External.Web/"]
RUN dotnet restore "Forestry.Eapc.External.Web/Forestry.Eapc.External.Web.csproj"
COPY . .
WORKDIR "/src/Forestry.Eapc.External.Web"
RUN dotnet build "Forestry.Eapc.External.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Forestry.Eapc.External.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Forestry.Eapc.External.Web.dll"]
