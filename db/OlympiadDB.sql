-- Drop the OlympiadDB database if it exists
DROP DATABASE IF EXISTS OlympiadDB;

CREATE DATABASE OlympiadDB;

USE OlympiadDB;

CREATE TABLE AcademicYear (
    AcademicYearId INT AUTO_INCREMENT PRIMARY KEY,
    StartYear INT NOT NULL,
    EndYear INT NOT NULL
);

CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    DateOfBirth DATE NOT NULL,
    AcademicYearId INT,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Gender VARCHAR(50),
    EmailVerified BOOLEAN DEFAULT FALSE,
    PersonalSettings JSON,
    Notifications JSON,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (AcademicYearId) REFERENCES AcademicYear(AcademicYearId)
);

CREATE TABLE Roles (
    RoleId INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(255) NOT NULL UNIQUE,
    Permissions JSON
);

CREATE TABLE UserRoleAssignments (
    AssignmentId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    RoleId INT,
    AssignedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

CREATE TABLE Olympiads (
    OlympiadId INT AUTO_INCREMENT PRIMARY KEY,
    Subject VARCHAR(255) NOT NULL,
    Description TEXT,
    DateOfOlympiad DATE,
    Round VARCHAR(50),
    Location VARCHAR(255),
    StartTime TIMESTAMP,
    AcademicYearId INT,
    ClassNumber INT NOT NULL,
    FOREIGN KEY (AcademicYearId) REFERENCES AcademicYear(AcademicYearId)
);

CREATE TABLE StudentOlympiadEnrollment (
    EnrollmentId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    OlympiadId INT,
    AcademicYearId INT,
    EnrollmentStatus VARCHAR(50) NOT NULL DEFAULT 'pending',
    StatusHistory JSON,
    Score INT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (OlympiadId) REFERENCES Olympiads(OlympiadId),
    FOREIGN KEY (AcademicYearId) REFERENCES AcademicYear(AcademicYearId)
);

CREATE TABLE UserToken (
    UserTokenId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    Token VARCHAR(255) NOT NULL,
    Expiration DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);


-- Insert Sample Data
INSERT INTO AcademicYear (StartYear, EndYear) VALUES (2023, 2024), (2024, 2025);

INSERT INTO Roles (RoleName, Permissions) VALUES
('Admin', '{"create_users": true, "delete_users": true}'),
('Student', '{"enroll_olympiads": true}');

