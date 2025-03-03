# FoundationApi

A sample ASP.NET Core Web API that uses **MySQL** as its database and **ASP.NET Core Identity** for authentication and user management.

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
  - [1. Clone the Repository](#1-clone-the-repository)
  - [2. Install Dependencies](#2-install-dependencies)
  - [3. Configure the Database](#3-configure-the-database)
  - [4. Run Migrations](#4-run-migrations)
  - [5. Launch the App](#5-launch-the-app)
- [MySQL Workbench Installation](#mysql-workbench-installation)
  - [Mac](#mac)
  - [Windows](#windows)
  - [Linux](#linux)
- [Using Swagger](#using-swagger)
- [License](#license)  
  _(Or any other sections you’d like to include.)_

---

## Overview

**FoundationApi** is a .NET 7 Web API that demonstrates:

- JWT-based authentication
- ASP.NET Core Identity for user registration, login, and email confirmation
- MySQL database integration via [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
- Entity Framework Core migrations

---

## Prerequisites

1. **.NET 7 SDK**

   - Install from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/7.0).

2. **MySQL Server**

   - Ensure MySQL is installed and running. If you don’t have it, follow the [MySQL Workbench Installation](#mysql-workbench-installation) instructions below.

3. **Git** (optional but recommended)
   - To clone and manage this repository locally.

---

## Setup Instructions

### 1. Clone the Repository

Using a terminal or command prompt, run:

```bash
git clone https://github.com/YourUsername/FoundationApi.git
cd FoundationApi
```

_(Adjust the URL to your actual repo.)_

### 2. Install Dependencies

From inside the project folder (where the `.csproj` is located), run:

```bash
dotnet restore
```

This will pull down all required NuGet packages.

### 3. Configure the Database

1. Make sure **MySQL Server** is running on your machine (e.g., `localhost:3306`).
2. Create a new MySQL user with permissions or use an existing user.
3. Update **`appsettings.json`** or **`appsettings.Development.json`** with your connection string. For example:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=foundations;Uid=mydevuser;Pwd=mydevpassword;"
     },
     "JwtSettings": {
       "Key": "Your_JWT_Secret_Key"
     }
   }
   ```

### 4. Run Migrations

To create the required Identity tables in your MySQL database:

```bash
dotnet ef migrations add InitialIdentity
dotnet ef database update
```

This applies the migration to the `foundations` database, creating `AspNetUsers`, `AspNetRoles`, etc.

### 5. Launch the App

Start the application:

```bash
dotnet run
```

By default, the application will listen on ports like `http://localhost:5000` and `https://localhost:7000` (depending on your .NET config). Check the console output for the exact URLs.

---

## MySQL Workbench Installation

MySQL Workbench is a GUI tool for managing MySQL databases. Below are high-level instructions for different OSes:

### Mac

1. Go to [MySQL Downloads](https://dev.mysql.com/downloads/workbench/).
2. Select **macOS**.
3. Download the `.dmg` or `.pkg` installer for **MySQL Workbench**.
4. Install and open MySQL Workbench.
5. Create a new connection to `localhost:3306` using your MySQL root or dev credentials.

### Windows

1. Visit [MySQL Downloads](https://dev.mysql.com/downloads/workbench/) and select **Windows (x86, 64-bit)**.
2. Run the downloaded MSI.
3. Follow the setup wizard.
4. Open MySQL Workbench; connect to your local MySQL instance (often `localhost:3306`).

### Linux

On most Linux distros, you can install via package managers, for example (Ubuntu/Debian):

```bash
sudo apt-get update
sudo apt-get install mysql-workbench
```

Then launch Workbench from your apps menu or type `mysql-workbench` in a terminal.

---

## Using Swagger

This project includes **Swagger** (OpenAPI) for testing the endpoints:

1. With the app running, navigate to `http://localhost:5000/swagger` (or the HTTPS port if applicable, e.g. `https://localhost:7000/swagger`).
2. You can test `Register`, `Login`, and other endpoints directly from the UI.

_(Endpoints that require JWT will need the “Authorize” button with `Bearer <your token>` once you have a token.)_

---

## License

_(Add your license information here if needed, e.g., MIT, Apache 2.0, etc.)_

---

### That’s It!

You now have a fully working foundation to **register users**, **confirm emails**, **login with JWT**, and manage them via ASP.NET Core Identity, all connected to a **MySQL** backend. Enjoy building out your app!
