# SyntroVaccP – Vaccination Management System

## Project Overview
SyntroVaccP is a **web-based vaccination management system** built with ASP.NET Core MVC.  
It allows healthcare staff to:

- Manage patients, vaccines, and vaccine batches
- Record vaccination administrations
- Generate reports for administration, stock, and overdue vaccinations
- Track audit logs for compliance and accountability

The system uses **role-based access control** (Admin, Clinician, Clerk, Auditor) and includes a **seed database** for testing purposes.

---

## Technology Stack
- **Backend:** ASP.NET Core MVC (C#)  
- **Frontend:** Bootstrap 5, jQuery  
- **Database:** SQL Server (DB-First approach)  
- **Authentication & Authorization:** ASP.NET Identity  
- **API Documentation:** Swagger  

---

## Setup Instructions

### 1. Clone the Repository

git clone https://github.com/<your-github-username>/SyntroVaccP.git
cd SyntroVaccP

---

### 1.2. Database Setup
Option 1 – Restore from Backup (.bak)

Open SQL Server Management Studio (SSMS).

Right-click Databases → Restore Database…

Select Device → Browse → Choose the backup file:

Backup/SyntroVaccP.bak


Set the destination database name (e.g., SyntroVaccP).

Click OK to restore.

Option 2 – Execute SQL Script (.sql)

Open SSMS → Create a new database SyntroVaccP.

Open the SQL script:

Backup/SyntroVaccP.sql


Execute the script (F5) to create tables and insert seed data.

Note: The backup and script include seed data for testing purposes.

---
### 3. Configure Connection String

In appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SyntroVaccP;Trusted_Connection=True;"
}


Update Server if using a remote or named SQL Server instance.

Ensure the database name matches your restored database.

---
### 4. Run the Application
dotnet run


Open your browser and navigate to:

https://localhost:7116


The Swagger UI will open automatically in development mode.

---
### Seed Users & Test Logins
Role	Username	Password
Admin	admin	Admin@123
Clinician	clinician	Clin@123
Clerk	clerk	Clerk@123
Auditor	auditor	Audit@123

Use these credentials to test role-specific functionality.

---
### Swagger API

Access Swagger at:

https://localhost:7116/swagger


Allows testing all API endpoints interactively.

Swagger only opens automatically in development mode; in production, you can navigate manually.

---
### Features
Patient Management

Add, edit, search, and view patient records

Role-based access for sensitive data

Vaccines & Batches

Admin/Clerk can add, edit, and manage vaccine batches

View stock levels and batch history

Administrations

Record vaccine administrations

AJAX-powered modals for adding/editing

Pagination and live search for large datasets

Reports

Summary reports, overdue vaccinations, batch stock

Audit Logs

Tracks changes and accesses based on role

---
### Notes for Assessors

The project uses DB-First approach; migrations are not included.

Seed data exists to allow immediate testing.

All modals, tables, and pagination have been tested with sample data.

Swagger is included to demonstrate API endpoints.

All roles and their permissions are defined in AppRoles.cs.

---
### Git Setup for Submission

Ensure the Backup/ folder is included in Git.

Exclude bin/, obj/, .vs/, and appsettings.Development.json using .gitignore.

Commit all source code, database backup, and README:

git add .
git commit -m "Initial submission for assessment"
git push origin main

---
### Folder Structure (Key Files)
SyntroVaccP/
│
├─ Backup/
│   ├─ SyntroVaccP.bak      # SQL Server backup for assessment
│   └─ SyntroVaccP.sql      # Optional SQL script for DB creation
│
├─ Controllers/
├─ Views/
├─ Models/
├─ wwwroot/
├─ appsettings.json
├─ Program.cs
├─ README.md
└─ ... other project files

---
### Additional Notes

The system uses AJAX modals for creating/editing records to improve UX.

All role-based restrictions have been implemented and tested.

The assessor can restore the database, login with test users, and test all features without extra setup.

---	
### Author Contact:
Gasie Van Rooyen - Email: gasiev123@gmail.com
 