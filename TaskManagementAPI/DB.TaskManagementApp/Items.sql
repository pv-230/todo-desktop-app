CREATE TABLE [dbo].[Items]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
  [Name] NVARCHAR(MAX) NOT NULL, 
  [Description] NVARCHAR(MAX) NOT NULL, 
  [IsCompleted] BIT NOT NULL, 
  [Priority] INT NOT NULL, 
  [DeadlineDate] DATETIMEOFFSET NULL, 
  [DeadlineTime] TIME NULL, 
  [StartDate] DATETIMEOFFSET NULL, 
  [StartTime] TIME NULL, 
  [StopDate] DATETIMEOFFSET NULL, 
  [StopTime] TIME NULL, 
  [AttendeesStr] NVARCHAR(MAX) NULL, 
    [Discriminator] NVARCHAR(MAX) NOT NULL 
)