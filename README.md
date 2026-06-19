# Makers Loans

Sistema de gestión de préstamos bancarios. API REST en .NET 8 + frontend Next.js.

## Requisitos

- Docker Desktop

---

## Inicio rápido (Docker — stack completo)

```bash
cp .env.example .env        # completar SA_PASSWORD y JWT_SECRET
docker-compose up --build
```

| Servicio  | URL                          |
| --------- | ---------------------------- |
| Frontend  | http://localhost:3000        |
| API       | https://localhost:7253        |
| Scalar    | https://localhost:7253/scalar/v1 |

DbUp migra la BD y el seeder inserta usuarios de prueba automáticamente al arrancar.

### Variables requeridas (`.env`)

| Variable     | Descripción                              |
| ------------ | ---------------------------------------- |
| `SA_PASSWORD` | Contraseña SA de SQL Server (mín. 8 chars, mayúscula, número y símbolo) |
| `JWT_SECRET`  | Clave HMAC para JWT (mín. 32 chars)     |

---

## Usuarios de prueba

| Email              | Contraseña | Rol   |
| ------------------ | ---------- | ----- |
| `usuario@test.com` | `123`      | User  |
| `admin@test.com`   | `123`      | Admin |

---

## Endpoints

| Método  | Ruta                      | Auth  | Descripción          |
| ------- | ------------------------- | ----- | -------------------- |
| POST    | `/api/v1/Auth/login`      | —     | Login                |
| POST    | `/api/v1/Auth/register`   | —     | Registro             |
| POST    | `/api/v1/Loans`           | User  | Crear préstamo       |
| GET     | `/api/v1/Loans/my`        | User  | Mis préstamos        |
| GET     | `/api/v1/Loans/admin`     | Admin | Todos los préstamos  |
| PATCH   | `/api/v1/Loans/{id}`      | Admin | Aprobar o rechazar   |
| GET     | `/health`                 | —     | Estado de la BD      |

---

## Postman

Importar `postman/makers-loans.postman_collection.yml` en Postman Desktop.

Variable `baseUrl` = `https://localhost:7253`. Ejecutar Login (User) y Login (Admin) primero — los scripts guardan `tokenUser` y `tokenAdmin` automáticamente.

---

## Desarrollo local (sin Docker para API y front)

Requisitos adicionales: .NET SDK 8, Node.js 20+.

### 1. Solo la BD en Docker

```bash
cd Back
docker-compose up -d
```

### 2. API

```bash
cp Back/.env.example Back/.env   # completar SA_PASSWORD y Jwt__Secret
dotnet run --project Back/Loans.Api
```

API en `https://localhost:7253`. Scalar en `https://localhost:7253/scalar/v1`.

### 3. Frontend

```bash
cp Front/.env.example Front/.env.local
npm --prefix Front install
npm --prefix Front run dev
```

Frontend en `http://localhost:3000`.

### Ejecutar tests

```bash
dotnet test Back/Loans.sln
```
