# Identity Verification Using Face Recognition for Enhanced Security and Fraud Prevention

This project implements a secure, offline identity verification system using facial recognition. 
It enables user registration through face images and verifies identity during authentication or exams using webcam capture. 
The system uses **C#.NET Web API**, **Emgu.CV (OpenCV wrapper)**, and **SQL Server**. 
It is suitable for real-world fraud prevention scenarios without relying on paid APIs like Azure Face API.

---

## Features

- User Registration with Face Image Upload
- Face Detection using HaarCascade
- Face Comparison using LBPH (Local Binary Patterns Histogram)
- Real-time Webcam Capture for Verification
- Identity Fraud Detection
- Bootstrap-based Frontend Interface
- SQL Server for Metadata Storage
- Works Completely Offline (No Cloud API Required)

---

## Technologies Used

| Layer       | Technology                                |
|-------------|-------------------------------------------|
| Frontend    | HTML5, Bootstrap 5, JavaScript, Webcam.js |
| Backend     | ASP.NET Core Web API (C#)                 |
| Face Logic  | Emgu.CV (OpenCV via .NET wrapper)         |
| Database    | SQL Server (LocalDB or Express)           |
| IDE         | Visual Studio 2022                        |

---

## Project Structure

/FaceAuthApp/
├── Controllers/
│ ├── UserController.cs
│ ├── VerifyController.cs
├── Services/
│ ├── FaceService.cs			# Capture, encode, compare faces
│ ├── ImageCompareService.cs	# Local face matching logic
├── Models/
│ ├── User.cs
| ├── UserRegisterDto.cs
├── Data/
│ ├── AppDbcontext.cs			# entity framework for database
├── wwwroot/
| ├── index.html				# Image upload + form
| ├── uploads/					# for storing images
| ├── js/ 
|   ├── Webcam.js
├── appsettings.json
├── Program.cs
├── Create_FaceAuthDb.sql (Database Script)
└── README.md


---

## Setup Instructions

1. **Clone the Repository**
   
   git clone https://github.com/pallavismulik/FaceAuthApp.git
   
2. Restore NuGet Packages

	- Microsoft.EntityFrameworkCore
	- Microsoft.EntityFrameworkCore.SqlServer
	- Emgu.CV
	- Emgu.CV.Bitmap
	- Emgu.CV.runtime.windows

3.	Update appsettings.json	

	{
	  "ConnectionStrings": {
		"DefaultConnection": "Server=.;Database=FaceAuthDb;Trusted_Connection=True;TrustServerCertificate=True;"
	  }
	}

4.	Run SQL Script

	- Open Create_FaceAuthDb.sql in SSMS
	- Execute to create tables and seed data

5.	Build and Run the Project

	- Press F5 or use dotnet run
	- Navigate to http://localhost:7027 in your browser
	
6.	Try the Frontend

	- Use the UI for Registration and Verification
	- Upload face images or use webcam

## Usage flow

1. Register a User

	- Provide name, email, and face image

	- Cropped face is stored with metadata

2.	Verify Identity

	- Capture face via webcam

	- System compares with registered image

	- Displays message

3.	Prevent Fraud

	- Rejects mismatched or spoofed images

	- Can be extended for exams, attendance, secure logins

## Future enhancements

	- Export verification logs

	- Add password authentication 

	- Extend for exam attendance with liveness detection

## Deliverables

	- Source Code (C#.NET Web API + Frontend)

	- SQL Server Script

	- README.md

	
