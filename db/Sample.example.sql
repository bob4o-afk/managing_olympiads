USE OlympiadDB;

INSERT INTO Users (Name, DateOfBirth, AcademicYearId, Username, Email, Password, Gender, EmailVerified, PersonalSettings, Notifications)
VALUES 
('Admin name', 'some-date', 1, 'Admin username', 'admin@mail.com', 'hashedPassword', 'Other', TRUE, '{}', '{}'); 

SET @AdminUserId = LAST_INSERT_ID();

SET @AdminRoleId = (SELECT RoleId FROM Roles WHERE RoleName = 'Admin');

INSERT INTO UserRoleAssignments (UserId, RoleId)
VALUES (@AdminUserId, @AdminRoleId);