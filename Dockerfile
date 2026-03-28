FROM mcr.microsoft.com/dotnet/sdk:8.0-focal AS build
WORKDIR /src
COPY ["ConstructionManagement_Backend/ConstructionManagement_Backend.csproj", "ConstructionManagement_Backend/"]
RUN dotnet restore "ConstructionManagement_Backend/ConstructionManagement_Backend.csproj"
COPY . .
WORKDIR "/src/ConstructionManagement_Backend"
RUN dotnet build "ConstructionManagement_Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConstructionManagement_Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-focal AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080
ENTRYPOINT ["dotnet", "ConstructionManagement_Backend.dll"]
