FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /repo

COPY FoodTracker.sln global.json ./
COPY src/FoodTracker.Domain/FoodTracker.Domain.csproj src/FoodTracker.Domain/
COPY src/FoodTracker.Application/FoodTracker.Application.csproj src/FoodTracker.Application/
COPY src/FoodTracker.Infrastructure/FoodTracker.Infrastructure.csproj src/FoodTracker.Infrastructure/
COPY src/FoodTracker.Api/FoodTracker.Api.csproj src/FoodTracker.Api/
RUN dotnet restore src/FoodTracker.Api/FoodTracker.Api.csproj

COPY src/ src/
RUN dotnet publish src/FoodTracker.Api/FoodTracker.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FoodTracker.Api.dll"]