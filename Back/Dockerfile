FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Loans.Domain/Loans.Domain.csproj Loans.Domain/
COPY Loans.Application/Loans.Application.csproj Loans.Application/
COPY Loans.Infrastructure/Loans.Infrastructure.csproj Loans.Infrastructure/
COPY Loans.Api/Loans.Api.csproj Loans.Api/

RUN dotnet restore Loans.Api/Loans.Api.csproj

COPY . .
RUN dotnet publish Loans.Api/Loans.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5016
EXPOSE 5016
ENTRYPOINT ["dotnet", "Loans.Api.dll"]
