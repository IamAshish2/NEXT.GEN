FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
# "Take the .csproj file from the MinimalAPIConcepts folder on my local
#  machine and copy it into a subdirectory called MinimalAPIConcepts 
#  inside the Docker image’s /source directory."
COPY MinimalAPIConcepts/*.csproj ./MinimalAPIConcepts/

# Restore dependencies
RUN dotnet restore 

# copy everything else and build the app
COPY MinimalAPIConcepts/. ./MinimalAPIConcepts/ 

# Publish the application
WORKDIR /source/MinimalAPIConcepts
RUN dotnet publish -c Release -o /app --no-restore 

#  # Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./ 

# the entrypoint command
ENTRYPOINT [ "dotnet","NEXT.GEN.dll" ]