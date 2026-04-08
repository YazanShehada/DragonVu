DragonVu

DragonVu is a web-based Quiz & Exam Management System, designed to simplify the creation, organization, and evaluation of quizzes for schools and educational institutions. Built with ASP.NET MVC and a responsive interface, DragonVu allows instructors to efficiently manage subjects, chapters, questions, and quizzes, while providing students with a seamless learning experience.

Features
Admin / Instructor
Subject & Chapter Management – Organize content by subjects and chapters.
Question Bank – Create, update, and categorize questions.
Quiz Creation – Generate quizzes by selecting specific chapters or entire subjects.
Student Management – Add students and track performance.
Results Overview – Monitor student scores and progress.
Student
Chapter Selection – Take quizzes for selected chapters or full subjects.
Quiz Participation – Interactive interface with immediate feedback.
Progress Tracking – View completed quizzes and scores.
Technologies Used
Backend: ASP.NET MVC, C#
Database: SQL Server with Entity Framework Core (Code-First)
Frontend: HTML5, CSS3, JavaScript, Bootstrap
Authentication: ASP.NET Identity
Version Control: Git & GitHub
Development Tools: Visual Studio
Project Structure
DragonVu/
│
├── DragonVu.Models/        # EF models like Subject, Chapter, Question
├── DragonVu.Data/          # Database context and migrations
├── DragonVu.Controllers/   # MVC controllers
├── DragonVu.Views/         # Razor views for the UI
└── wwwroot/                # Static files (CSS, JS, images)
Installation & Setup
Clone the repository:
git clone https://github.com/yourusername/DragonVu.git
Open in Visual Studio and restore NuGet packages.
Update the database connection string in appsettings.json.
Apply migrations to create the database:
Update-Database
Run the project locally using IIS Express or Kestrel.
How It Works
Admin creates subjects and chapters.
Questions are added to chapters.
Quizzes can be generated for specific chapters or full subjects.
Students select chapters/subjects and take quizzes.
Results are stored and displayed in the dashboard.
Why DragonVu

DragonVu demonstrates full-stack web development skills, including:

Backend design using ASP.NET MVC
Database design with EF Core
Responsive front-end development
Authentication and user management
Version control and project organization

It is a practical project that highlights your ability to build a complete educational system, perfect to showcase in your CV.

Future Enhancements
Add timed quizzes with automatic grading.
Implement leaderboards and student achievements.
Add real-time notifications for progress tracking.
