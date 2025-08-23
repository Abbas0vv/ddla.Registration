Product Registration System – State Maritime and Port Agency

GitHub Repository: https://github.com/Abbas0vv/ddla.Registration

Purpose

Internal web application for the IT Department (10 staff members) of the State Maritime and Port Agency.
The system is designed for:

Tracking product handover and acceptance (delivery logs)

Managing warehouse inventory

Managing IT equipment and assets

Role-based access and auditing

Mobile support is not required. Access is limited to internal network or VPN.

Technologies Used

C# .NET / ASP.NET Core MVC / Razor Pages

Entity Framework Core

MS SQL Server

LDAP Integration (Active Directory Synchronization)

ClosedXML (Excel export)

iTextSharp (PDF export)

Architecture

3-Layered Architecture:

Database Layer

Service Layer

UI Layer

Integration: LDAP server & Active Directory sync

Security:

Password hashing

Password complexity checks

Anti-forgery tokens ([ValidateAntiForgeryToken])

Modules & Features
1. Authentication & Authorization

Login page with LDAP integration

Role & permission logic

Additional routing for Admin and SuperAdmin

2. Handover Journal

Table columns: Signed/Unsigned (Green/Red), Inventory ID, User, Product, Description, Department, Issue Date, Return Date, PDF file, Actions (Edit, Delete, Mark Signed)

Dynamic & global search

Create new handover & act document

Department auto-detection based on user

LDAP user selection & automatic data population

3. Warehouse

Grouped by product type

Columns: Product name, Description, Total count, In use, Details page

Add new product & inventory codes

Database tracks in-use, in-stock, total counts

Edit allowed for description & inventory code only

4. Equipment Management

Table columns: Status (In stock/In use), Name, Description, Inventory code, Registration date, File upload, Actions

Status auto-updated (Active/Inactive)

5. Users Page

Data from LDAP: Full name, Email, Phone, Department, Position

Column search & global search

Sorting by: Name, Department, Phone

Department & position translated into Azerbaijani

6. Settings

Profile editing: profile picture, username, password

Email, name, surname cannot be changed

Password strength indicator

7. Statistics

Handover/Acceptance: Today, This Month, This Year, Total

Products: Total, In stock, In use

Charts: Handover timeline chart, Product pie chart

8. Permission Management (Admin / SuperAdmin)

CRUD permissions:

Handover (CRUD)

Warehouse (CRUD)

Equipment (Read/Update)

Error pages:

403 AccessDenied

404 PageNotFound

503 ConnectionFailed

9. Logs (Admin / SuperAdmin)

All HTTP POST operations are logged

Features:

Filter by date

Filter by user

Pagination

Detailed log entry: previous value → new value, user, timestamp, action type

10. Security

Role-based login: users can only access pages according to roles/permissions

Permission-based access: [Permission] attributes enforce page-level operations

CSRF protection with [ValidateAntiForgeryToken]

Global error pages for LDAP connection issues, 403, 404, etc.

11. Reporting / Export

Journal: Export full journal to Excel, export specific handovers to PDF

LDAP-based reports: Generate acts per user

Equipment: Export all equipment to Excel

Logs: Export logs to Excel

12. General Features

Profile picture, username, logout option

Custom error pages (403, 404) with redirection

No access without authentication

✅ This document describes a production-ready internal system tailored for a government IT department, with strong role-based security, full auditing, and reporting capabilities.