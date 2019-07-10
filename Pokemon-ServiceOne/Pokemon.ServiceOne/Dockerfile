FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Pokemon.ServiceOne/Pokemon.ServiceOne.csproj", "Pokemon.ServiceOne/"]
RUN dotnet restore "Pokemon.ServiceOne/Pokemon.ServiceOne.csproj"
COPY . .
WORKDIR "/src/Pokemon.ServiceOne"
RUN dotnet build "Pokemon.ServiceOne.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Pokemon.ServiceOne.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Pokemon.ServiceOne.dll"]