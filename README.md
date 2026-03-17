# FantasyAggregator

A full-stack fantasy sports aggregator — .NET 10 API backend + React frontend + MySQL database.

---

## Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Get the Code](#2-get-the-code)
3. [Database Setup](#3-database-setup)
4. [Backend — Build & Run](#4-backend--build--run)
5. [Frontend — Build & Run](#5-frontend--build--run)
6. [Verify the Install](#6-verify-the-install)
7. [Common Problems & Fixes](#7-common-problems--fixes)

---

## 1. Prerequisites

| Tool | Version | Install |
|------|---------|---------|
| Git | any | https://git-scm.com |
| .NET SDK | 10+ | https://aka.ms/dotnet/download |
| Node.js (LTS) | ^18 | https://nodejs.org |
| MySQL Server | 8.x | https://dev.mysql.com/downloads/mysql |
| MySQL Workbench | any | optional, recommended |
| VS Code | any | https://code.visualstudio.com |

**Quick check:**
```bash
git --version
dotnet --version   # expect 10.x
node -v && npm -v
```

---

## 2. Get the Code

```bash
git clone https://github.com/<yourusername>/FantasyAggregator.git
cd FantasyAggregator
```

Key folders:

```
FantasyAggregator/
├── sql/
│   ├── schema.sql
│   ├── seed.sql
│   └── reset_and_seed.sql      # combined script (recommended)
├── FantasyAggregator.Api/
│   ├── appsettings.json.example
│   └── ...
├── FantasyAggregator.WebClient/ # React client
└── FantasyAggregator.Client/   # console/data layer
```

---

## 3. Database Setup

### 3a. Run SQL scripts

**Recommended — combined script (drops, recreates, and seeds):**
```bash
mysql -u root -p < sql/reset_and_seed.sql
```

**Or run separately:**
```bash
mysql -u root -p < sql/schema.sql
mysql -u root -p < sql/seed.sql
```

### 3b. Create a dedicated app user

Run in Workbench or the MySQL shell as root:

```sql
CREATE USER IF NOT EXISTS 'fanagg_user'@'localhost' IDENTIFIED BY 'YourStrongPassword!';
GRANT SELECT, INSERT, UPDATE, DELETE ON FantasyAggregator.* TO 'fanagg_user'@'localhost';
FLUSH PRIVILEGES;
```

> If MySQL is on a different host, replace `'localhost'` with the appropriate hostname.

---

## 4. Backend — Build & Run

### 4a. Configure the connection string

```bash
cp FantasyAggregator.Api/appsettings.json.example FantasyAggregator.Api/appsettings.json
```

Edit `appsettings.json`:
```json
{
  "ConnectionString": "Server=localhost;Port=3306;Database=FantasyAggregator;Uid=fanagg_user;Pwd=YourStrongPassword!;",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

> **Do not commit `appsettings.json`.** Add it to `.gitignore`.  
> Alternatively, set env var `CONNECTIONSTRING` instead of using the file.

### 4b. Build & run

```bash
cd FantasyAggregator.Api
dotnet build
dotnet run
```

Expected output:
```
Now listening on: http://localhost:5196
Now listening on: https://localhost:7196
```

### 4c. Confirm the API is up

- **Swagger UI:** http://localhost:5196/swagger/index.html
- **curl:** `curl -i http://localhost:5196/api/players` → expect `HTTP/1.1 200 OK` + JSON

---

## 5. Frontend — Build & Run

```bash
cd FantasyAggregator.WebClient
npm install
```

Add a dev proxy to `package.json` (avoids CORS in development):
```json
"proxy": "http://localhost:5196"
```

Start the dev server:
```bash
npm start
```

Opens at **http://localhost:3000**.

---

## 6. Verify the Install

| Check | Expected |
|-------|----------|
| API terminal | `Now listening on: http://localhost:5196`, no fatal errors |
| `curl http://localhost:5196/api/players` | `HTTP 200` + JSON array |
| Swagger UI | Loads at `/swagger/index.html`; "Try it out" works |
| React UI | Dashboard loads; team click shows roster; player search returns results |
| MySQL | `SELECT COUNT(*) FROM Players;` returns seeded rows |

---

## 7. Common Problems & Fixes

**`react-scripts` not recognized**
```bash
cd FantasyAggregator.WebClient
rm -rf node_modules
npm install && npm start
```

**CORS error in browser**  
Ensure `"proxy": "http://localhost:5196"` is in `package.json` and restart `npm start`.  
Or add `http://localhost:3000` to the API's CORS policy in `Program.cs` and restart the API.

**`Access denied for user 'fanagg_user'@'localhost'`**  
Test the connection directly:
```bash
mysql -u fanagg_user -p -h 127.0.0.1 -P 3306
```
Then re-run the `CREATE USER` / `GRANT` block from [Section 3b](#3b-create-a-dedicated-app-user).

**API returns HTML instead of JSON to the client**  
The dev proxy isn't active. Confirm `"proxy"` is set in `package.json` and restart `npm start`.

**`No .NET SDKs were found`**  
Install .NET SDK 10+ from https://aka.ms/dotnet/download, then restart your terminal.

**JSON shape mismatch between server and client**  
Inspect the actual response in browser DevTools → Network tab and update the client code to match the real property names.

---

## Quick-Reference Commands

```bash
# Database
mysql -u root -p < sql/reset_and_seed.sql

# API
cd FantasyAggregator.Api && dotnet build && dotnet run

# React client
cd FantasyAggregator.WebClient && npm install && npm start

# Console client
cd FantasyAggregator.Client && dotnet run
```
