# Event-Mangaement-Framework 
Event Management System

Overview

The Event Management System is a web-based platform developed using ASP.NET Core MVC and Web API. It facilitates event creation, registration, and management while supporting different user roles such as Admin and Participant. The system leverages MS SQL Server for data storage and integrates with Azure Cloud Services for enhanced performance and scalability.

Features

1. User Management

Role-based authentication (Admin & Participant)

Secure login using JWT authentication

Registration approval system for participants

2. Event Management

Create, update, and delete events (Admin only)

Set event details (date, time, location, description, etc.)

Assign categories and tags for events

3. Participant Management

Users can register for events

View event details and schedules

Track event history and attendance

4. Real-time Notifications

Email notifications for event registrations and updates

Admin alerts for new participant registrations

5. Reports & Analytics

Generate reports on event participation

Track event success metrics

Technology Stack

Backend

ASP.NET Core Web API (for business logic)

ASP.NET Core MVC (for UI)

C# & Entity Framework Core (for database interactions)

MS SQL Server (for relational database management)

Frontend

Razor Views (MVC) for UI rendering

Bootstrap, HTML, CSS, JavaScript for styling and interactivity

Cloud & DevOps

Azure Service Bus (for notifications)

Azure Cosmos DB (optional for scalable data storage)

Azure App Service (for hosting)

Azure Function App & WebJobs (for background processing)

Azure Key Vault (for secure credential management)

Azure DevOps (CI/CD automation)

Application Insights (for monitoring & diagnostics)


