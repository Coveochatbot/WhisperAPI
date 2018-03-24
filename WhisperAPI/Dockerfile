FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY WhisperAPI/*.csproj ./WhisperAPI/
RUN dotnet restore WhisperAPI/WhisperAPI.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out WhisperAPI/WhisperAPI.csproj

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/WhisperAPI/out .
ENTRYPOINT ["dotnet", "WhisperAPI.dll"]