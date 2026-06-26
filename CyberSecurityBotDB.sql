USE CyberSecurityBotDB;
GO

CREATE TABLE Tasks
(
    TaskID INT IDENTITY(1,1) PRIMARY KEY,
    TaskName NVARCHAR(255),
    ReminderDate DATE
);