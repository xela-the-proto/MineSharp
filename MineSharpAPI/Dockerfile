FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
RUN useradd -m condopal
USER condopal
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CondoPal/CondoPal.csproj", "CondoPal/"]
RUN dotnet restore "CondoPal/CondoPal.csproj"
COPY . .
WORKDIR "/src/CondoPal"

# Install EF CLI tools in the build phase
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Build the app so EF tools can reflect types
RUN dotnet build "CondoPal.csproj" -c $BUILD_CONFIGURATION

# Optional: copy appsettings if used in design-time factory
#COPY CondoPal/appsettings.json /src/CondoPal/data/appsettings.json
ENV DEPLOYING="true"
# Generate the migration (this is where it previously failed)
RUN dotnet ef migrations add InitialCreate --project CondoPal.csproj --startup-project CondoPal.csproj --output-dir Migrations

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CondoPal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/data && chown condopal:condopal /app/data
RUN chmod -R 775 /app/data
ENV DEPLOYING="false"
ENTRYPOINT ["dotnet", "CondoPal.dll"]
