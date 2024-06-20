FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-jammy-amd64 AS base
EXPOSE 8080
EXPOSE 80

# Building the core project
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy-amd64 AS build
WORKDIR /app

# Copy project files and restore deps
COPY ["./EscapeRoomAPI.csproj", "./EscapeRoomAPI/"]
RUN dotnet restore "./EscapeRoomAPI/EscapeRoomAPI.csproj" 

WORKDIR "/app/EscapeRoomAPI"
# Copy remaining project files
COPY . . 

RUN dotnet build "./EscapeRoomAPI.csproj" --configuration Release --runtime linux-x64 -o ./build

FROM build as publish
RUN dotnet publish "./EscapeRoomAPI.csproj" --configuration Release --runtime linux-x64 -o /app/out 

# Setting up the final image
FROM base AS final
COPY --from=publish /app/out .

# Copy the firebase configuration file
COPY ./firebase.json /

# ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENTRYPOINT [ "dotnet", "EscapeRoomAPI.dll"]