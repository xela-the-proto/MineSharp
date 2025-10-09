# MineSharpAPI

MineSharpAPI is an experimental C# project inspired by **Pterodactyl** and similar server management panels.  
It is currently being developed as a learning and experimental platform for managing Minecraft servers using .NET

> ⚠️ **Important:** This project is still in early development.  
> It is **not ready for production use** and should not be used in any live or critical environment.

---

## 🚧 Project Overview

MineSharpAPI aims to provide an API-based backend for managing and monitoring Minecraft servers and related user operations.  
While currently limited in features, the goal is to create a fast and user friendly backend to be expanded upon (i hate doing frontend).

---

## 🧩 Technologies Used

- **.NET 9 / ASP.NET Core**
- **Entity Framework Core** (PostgreSQL)

---

## 🛠️ Setup (Development Only)

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/MineSharpAPI.git
   cd MineSharpAPI
   ```

2. **Configure the database connection**
   Edit the connection settings in `Program.cs`:

3. **Run the API**
   ```bash
   dotnet run
   ```

4. Access it locally:
   ```
   http://localhost:5000
   ```

---

## 🧭 Development Roadmap

### 🔹 Core API and Database
- [x] Basic ASP.NET Core setup  
- [x] Entity Framework integration with PostgreSQL  
- [ ] Validation and error handling middleware  
- [ ] Logging and monitoring improvements  

### 🔹 Authentication & Security
- [x] API key authentication system  
- [ ] User management and role-based permissions  
- [x] Token-based authorization for third-party clients  

### 🔹 Minecraft Server Management
- [ ] Server start/stop/restart API endpoints  
- [ ] Container integration (Docker)  
- [~] Resource monitoring (CPU, RAM, Disk, etc.)  

### 🔹 Developer & Integration Tools
- [ ] REST API documentation (Swagger / OpenAPI)  
- [x] WebSocket layer for live status updates  

### 🔹 Frontend (Future)
- [ ] Web dashboard (possibly React or Blazor)  
- [ ] User session management  
- [ ] Minimal control panel interface  

---

## ⚠️ Disclaimer

This software is provided **as-is**, without any guarantees or support.  
Use it **only for learning or testing purposes**.  
Any feature, structure, or API endpoint may change at any time.

---

## 📄 License

MIT License – feel free to read and learn from the code,  
but do **not** deploy it in production environments.
