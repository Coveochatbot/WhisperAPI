FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY WhisperAPI/*.csproj ./WhisperAPI/
RUN dotnet restore WhisperAPI/WhisperAPI.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out WhisperAPI/WhisperAPI.csproj

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/WhisperAPI/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "WhisperAPI.dll"]