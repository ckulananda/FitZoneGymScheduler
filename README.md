# 🏋️ FitZone - Gym Management & Workout Planning System

## Overview

FitZone is a modern desktop application developed using **C#**, **.NET 8**, **WPF (MVVM)**, and **SQL Server** to simplify gym management and workout planning.

The application provides an efficient solution for managing gym members, creating personalized workout plans, maintaining an exercise library, and generating professional reports. It was designed with scalability, maintainability, and user experience in mind while following modern software engineering practices.

---

## Features

* 🔐 Secure User Authentication
* 👤 Account Creation
* 🛡️ Password Hashing
* 👥 Member Management
* 💪 Exercise Library
* 📅 Workout Plan Creation
* 📝 Workout Schedule Management
* 📊 Dashboard
* 📄 PDF Report Generation
* 📱 WhatsApp Workout Sharing
* 🔍 Search & Filtering
* ✏️ Full CRUD Operations
* 🗄️ SQL Server Database Integration
* ✅ Data Validation
* 🏗️ MVVM Architecture

---

## Technology Stack

### Programming Language

* C#

### Framework

* .NET 8
* WPF

### Architecture

* MVVM (Model-View-ViewModel)

### Database

* Microsoft SQL Server
* Entity Framework Core

### Development Tools

* Visual Studio 2022
* Git
* GitHub

### Additional Libraries

* QuestPDF
* Microsoft.Extensions.DependencyInjection
* Microsoft.Extensions.Hosting
* Entity Framework Core

---

## Application Modules

* User Authentication
* User Management
* Member Management
* Exercise Library
* Workout Planning
* Workout Schedule Management
* Reports
* Settings

---

## Screenshots

> Screenshots will be added soon.

---

## Project Structure

```text
FitZone/
│
├── Models/
├── ViewModels/
├── Views/
├── Services/
├── Data/
├── Helpers/
├── Commands/
├── Resources/
├── Assets/
└── App.xaml
```

---

## Installation

### Prerequisites

* Windows 10 / 11
* .NET 8 Runtime
* SQL Server Express or SQL Server
* Visual Studio 2022 (for development)

---

### Clone Repository

```bash
git clone https://github.com/yourusername/FitZone.git
```

---

### Configure Database

1. Create or restore the SQL Server database.
2. Update the connection string in:

```text
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=FitZoneDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

### Apply Migrations

```bash
Update-Database
```

or

```bash
dotnet ef database update
```

---

### Run the Application

Open the solution in **Visual Studio 2022** and press **F5**.

---

## Project Goals

The primary objective of FitZone is to replace manual gym management processes with a centralized desktop solution that enables trainers and administrators to efficiently manage members, workout plans, and daily operations.

---

## Challenges

* Designing a scalable desktop architecture
* Implementing secure authentication
* Managing relational database operations
* Maintaining clean separation of concerns using MVVM
* Generating printable reports
* Providing an intuitive user interface

---

## Solution

FitZone addresses these challenges by combining WPF, MVVM, Entity Framework Core, and SQL Server into a structured desktop application that delivers secure authentication, organized data management, and streamlined gym operations.

---

## Future Improvements

* Nutrition Plan Management
* Attendance Tracking
* Body Measurement History
* Progress Charts
* Email Notifications
* Cloud Synchronization
* Multi-Gym Support
* Barcode/QR Code Member Check-In
* Mobile Companion Application

---

## Learning Outcomes

This project strengthened practical knowledge in:

* Desktop Application Development
* WPF
* MVVM Design Pattern
* Entity Framework Core
* SQL Server
* Authentication & Security
* Database Design
* Object-Oriented Programming
* Software Architecture
* Git Version Control

---

## Author

**Chamodha Kulananda**

Software Engineer | Desktop Application Developer | .NET Developer

---

## License

This project is intended for educational and portfolio purposes.
