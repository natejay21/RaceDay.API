# RaceDay API - Part 2

This repository contains the RESTful API for the RaceDay event management system, built using ASP.NET Core Web API, Entity Framework Core, and SQL Server.

## System Roles

The system enforces strict role-based access control (RBAC) for two distinct user types:

1. **Organiser:** Can create, update, and delete race events and categories. They can also view all enrolments for their events and record race results.
2. **Participant:** Can browse events, enrol in specific categories, view their own race history ("my-races"), and view their personal results.

## Setup and Run Instructions

### Prerequisites
- .NET 8.0 SDK
- Microsoft SQL Server (LocalDB)
- Visual Studio 2022

### How to Run
1. Clone the repository: `git clone [https://github.com/natejay21/RaceDay.API]`
2. Open the solution in Visual Studio.
3. Ensure your connection string in `appsettings.json` points to your local SQL Server.
4. Open the **Package Manager Console** and run:
   ```bash
   Update-Database
