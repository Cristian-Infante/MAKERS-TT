# Makers Loans

Sistema de gestión de préstamos bancarios. API REST en .NET 8 + frontend Next.js.

## Requisitos

- .NET SDK 8
- Node.js 20+
- Docker Desktop

## Backend

### 1. Levantar SQL Server

```bash
cd Back
cp .env.example .env   # completar SA_PASSWORD y Jwt__Secret```bash
docker-compose up -d
```

```

```

### 2. Ejecutar API

```bash
dotnet run --project Back/Loans.Api
```

La API corre en `https://localhost:7253`. DbUp migra la BD y el seeder inserta usuarios de prueba automáticamente al arrancar en Development.

### Usuarios de prueba

| Email              | Contraseña | Rol   |
| ------------------ | ---------- | ----- |
| `usuario@test.com` | `123`      | User  |
| `admin@test.com`   | `123`      | Admin |

### Ejecutar tests

```bash
dotnet test Back/Loans.sln
```

### Documentación API

Scalar disponible en `https://localhost:7253/scalar/v1` (solo en Development).

## Frontend

```bash
cd Front
cp .env.example .env.local   # ajustar NEXT_PUBLIC_API_URL si es necesario
npm install
npm run dev
```

Corre en `http://localhost:3000`.

## Endpoints

| Método | Ruta                      | Auth  | Descripción         |
| ------- | ------------------------- | ----- | -------------------- |
| POST    | `/api/v1/Auth/login`    | —    | Login                |
| POST    | `/api/v1/Auth/register` | —    | Registro             |
| POST    | `/api/v1/Loans`         | User  | Crear préstamo      |
| GET     | `/api/v1/Loans/my`      | User  | Mis préstamos       |
| GET     | `/api/v1/Loans/admin`   | Admin | Todos los préstamos |
| PATCH   | `/api/v1/Loans/{id}`    | Admin | Aprobar o rechazar   |
| GET     | `/health`               | —    | Estado de la BD      |

## Postman

Importar `postman/makers-loans.postman_collection.yml` en Postman Desktop.

Variables de colección: `baseUrl` = `https://localhost:7253`. Ejecutar Login (User) y Login (Admin) primero — los scripts guardan `tokenUser` y `tokenAdmin` automáticamente.
