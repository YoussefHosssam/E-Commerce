# 🛒 Modern E-Commerce Platform API

![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-brightgreen)
![CQRS](https://img.shields.io/badge/Pattern-CQRS-blue)
![Database](https://img.shields.io/badge/Database-SQL_Server-cc292b?logo=microsoft-sql-server)

A robust, highly scalable, and feature-rich E-Commerce RESTful API built with **.NET 9.0** following **Clean Architecture** and **CQRS** principles.

## 🌟 Features

### 🔐 Advanced Identity & Security
- **JWT Authentication & Authorization:** Secure endpoints with Role-based access control.
- **Two-Factor Authentication (2FA):** Multi-layered security using Authenticator Apps (OTP via QR Code), SMS/Email challenges, and Recovery Codes.
- **OAuth Integration:** Social logins supported (Google, Facebook, etc.).
- **Session Management:** Refresh tokens and active auth token tracking.

### 📦 Product & Catalog Management
- Hierarchical categories.
- Complex product definitions with multi-level **Variants** (e.g., Size, Color).
- Extensive image management for both products and individual variants.

### 🧮 Inventory & Warehouse
- Real-time **Inventory Tracking** per variant.
- **Stock Movements:** Automated logging of IN, OUT, and Adjustment movements.
- **Stock Alerts:** Automated notifications for low-stock items.

### 🛒 Cart & Checkout
- Persistent User Carts with active status tracking.
- Seamless conversion from Cart to Order.

### 💳 Orders & Payments
- Comprehensive order lifecycle management.
- Integration-ready Payment tracking and Refund processing.

### ⚙️ Background Processing & Notifications
- **Hangfire Integration:** Reliable background job processing (e.g., sending emails, cleaning expired tokens, stock alerts).
- **Email Service:** Email messaging via MailKit.
- **In-App Notifications:** Real-time and persisted user notifications.

### 🛡️ System Auditing
- Detailed **Audit Logs** for tracking user actions and system events.

## 🏗️ Architecture

The solution is divided into 5 core projects following Clean Architecture principles:

1. **E-Commerce.Domain:** Contains Enterprise-wide logic and Types (Entities, Value Objects, Enums, Exceptions). No external dependencies.
2. **E-Commerce.Application:** Contains Business logic and Use Cases. Implements CQRS using MediatR. Handles validation (FluentValidation) and mapping (AutoMapper).
3. **E-Commerce.Infrastructure:** Contains external service implementations (Email via MailKit, Background jobs via Hangfire, OAuth handling, OTP Generation).
4. **E-Commerce.Persistence:** Contains Database access logic, Entity Framework Core 9.0 configurations, and Migrations for SQL Server.
5. **E-Commerce.API:** The presentation layer. RESTful endpoints with API Versioning and OpenAPI/Swagger documentation.

## 🛠️ Technology Stack

- **Framework:** .NET 9.0
- **Database:** Microsoft SQL Server via Entity Framework Core 9
- **Architecture & Patterns:** Clean Architecture, CQRS, Repository Pattern
- **Mediator:** MediatR
- **Validation:** FluentValidation
- **Mapping:** AutoMapper
- **Authentication:** JWT, ASP.NET Core Identity, OTP.NET, QRCoder
- **Background Jobs:** Hangfire
- **Email:** MailKit
- **Logging:** Serilog
- **API Documentation:** OpenAPI (Swagger) & ASP.NET API Versioning

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB or Docker instance)

### Installation

1. Clone the repository:
   ```bash
   git clone <repo-url>
   ```

2. Navigate to the API project directory:
   ```bash
   cd E-Commerce/E-Commerce.API
   ```

3. Update the `appsettings.json` with your SQL Server Connection String.

4. Apply database migrations:
   ```bash
   dotnet ef database update --project ../E-Commerce.Persistence --startup-project .
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

6. Open your browser and navigate to `https://localhost:<port>/swagger` to view the API documentation.

## 🔮 Roadmap (Future Considerations)
- Distributed Caching using Redis.
- Microservices extraction for high-load domains (e.g., Inventory, Orders).
- Event-Driven Architecture with RabbitMQ/Kafka.
- ElasticSearch for advanced product searching and filtering.

---
*This project is continuously evolving. Contributions and feedback are welcome!*
