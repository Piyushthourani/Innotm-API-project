# Innotm – Backend API (ASP.NET Core)

This is the backend Web API for the Innotm Digital Wallet Application. Built using ASP.NET Core, it handles user login, balance management, money transfers, and transaction tracking. It communicates with the Angular-based frontend via REST APIs.

🔗 **Frontend Repo:** [Innotm](https://github.com/Piyushthourani/Innotm)

---

## ⚙️ Tech Stack

- **Language**: C#
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **DTO Pattern**: Clean separation using DTOs
- **IDE**: Visual Studio

---

## 📁 Project Structure

```
Innotm-API-project/
├── Controllers/
│ ├── AuthController.cs
│ ├── ChatController.cs
│ ├── TransactionsController.cs
│ ├── UsersController.cs
│ ├── WalletController.cs
│ └── WeatherForecastController.cs
├── Data/
│ └── AppDbContext.cs
├── DTO/
│ ├── AddMoneyDto.cs
│ ├── AddMoneyResponse.cs
│ ├── ApiResponse.cs
│ ├── LoginDto.cs
│ ├── PayMoneyDto.cs
│ ├── SignupDto.cs
│ ├── TransactionResponse.cs
│ └── UserResponse.cs
├── Models/
│ ├── Transaction.cs
│ └── User.cs
├── Migrations/
├── appsettings.json
├── Program.cs
├── Startup.cs (if present)
└── WeatherForecast.cs
```

---

## 📚 Key API Endpoints

| Method | Endpoint                 | Description                     |
|--------|--------------------------|---------------------------------|
| POST   | /api/Auth/signup         | Register a new user             |
| POST   | /api/Auth/login          | User login                      |
| POST   | /api/Wallet/add          | Add money to wallet             |
| POST   | /api/Transactions/pay    | Send money to another user      |
| GET    | /api/Users/balance       | Get user balance (by ID)        |
| GET    | /api/Transactions/history| Get transaction history         |
| GET    | /api/Chat/ask            | Prompt based transfers          |

> 🧾 DTOs are used for request and response formatting in every major route.

---

## 🧪 How to Run

1. **Clone the repo**
   ```bash
   git clone https://github.com/Piyushthourani/Innotm-API-project
   ```

2. **Open in Visual Studio**

3. **Update connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=InnotmDB;Trusted_Connection=True;"
   }
   ```

4. **(Optional) Run EF migrations**
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```

5. **Run the project (F5 or Debug)**

6. API available at:
   ```
   https://localhost:44377/api
   ```

---

## 📜 License

MIT License  
© 2025 [Piyush Thourani](https://github.com/Piyushthourani)
