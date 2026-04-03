# E-Kanban Dockerfile
# Based on .NET 8 SDK for building and running

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY ["VOL.WebApi/VOL.WebApi.csproj", "VOL.WebApi/"]
COPY ["VOL.Entity/VOL.Entity.csproj", "VOL.Entity/"]
COPY ["VOL.Core/VOL.Core.csproj", "VOL.Core/"]
COPY ["VOL.Sys/VOL.Sys.csproj", "VOL.Sys/"]
COPY ["EKanban/EKanban.csproj", "EKanban/"]
COPY ["VOL.Builder/VOL.Builder.csproj", "VOL.Builder/"]
COPY ["VOL.MES/VOL.MES.csproj", "VOL.MES/"]

RUN dotnet restore "VOL.WebApi/VOL.WebApi.csproj"

# Copy all source and build
COPY . .
WORKDIR "/src/VOL.WebApi"
RUN dotnet build "VOL.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "VOL.WebApi.csproj" -c Release -o /app/publish --self-contained false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install dependencies for System.Drawing
RUN apt-get update && apt-get install -y libgdiplus libfontconfig1 && rm -rf /var/lib/apt/lists/*
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll

# Expose port
EXPOSE 9991

ENTRYPOINT ["dotnet", "VOL.WebApi.dll"]
