:ON ERROR EXIT
-- Run in SSMS against your local SQL Server / LocalDB. This script creates only a NEW database.
-- Stop if the database already exists; use DACPAC publishing for later schema updates.
USE master;
GO
IF DB_ID(N'ViwePortfolio') IS NOT NULL
    THROW 50000, 'ViwePortfolio already exists. Use the database project Publish instead.', 1;
GO
CREATE DATABASE ViwePortfolio;
GO
USE ViwePortfolio;
GO
CREATE TABLE dbo.PortfolioEntries (
 Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PortfolioEntries PRIMARY KEY,
 Section int NOT NULL CONSTRAINT CK_Portfolio_Section CHECK (Section BETWEEN 0 AND 3),
 Title nvarchar(120) NOT NULL,
 Summary nvarchar(500) NOT NULL,
 Body nvarchar(6000) NOT NULL,
 Category nvarchar(120) NOT NULL,
 Url nvarchar(500) NULL,
 DisplayOrder int NOT NULL CONSTRAINT CK_Portfolio_Order CHECK (DisplayOrder BETWEEN 0 AND 9999),
 IsPublished bit NOT NULL,
 IsArchived bit NOT NULL CONSTRAINT DF_Portfolio_Archived DEFAULT 0,
 UpdatedAtUtc datetime2 NOT NULL CONSTRAINT DF_Portfolio_Updated DEFAULT SYSUTCDATETIME()
);
GO
CREATE INDEX IX_Portfolio_Section ON dbo.PortfolioEntries(Section, IsArchived, IsPublished, DisplayOrder);

GO
CREATE PROCEDURE dbo.Portfolio_Archive @Id int AS
UPDATE dbo.PortfolioEntries SET IsArchived=1,IsPublished=0,UpdatedAtUtc=SYSUTCDATETIME() WHERE Id=@Id AND IsArchived=0;
SELECT @@ROWCOUNT;

GO
CREATE PROCEDURE dbo.Portfolio_Create @Section int, @Title nvarchar(120), @Summary nvarchar(500), @Body nvarchar(6000), @Category nvarchar(120), @Url nvarchar(500), @DisplayOrder int, @IsPublished bit AS
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(@Section,@Title,@Summary,@Body,@Category,@Url,@DisplayOrder,@IsPublished);
SELECT CAST(SCOPE_IDENTITY() AS int);

GO
CREATE PROCEDURE dbo.Portfolio_Get @Id int, @IncludeDrafts bit = 0 AS
SELECT Id, Section, Title, Summary, Body, Category, Url, DisplayOrder, IsPublished, UpdatedAtUtc FROM dbo.PortfolioEntries WHERE Id=@Id AND IsArchived=0 AND (@IncludeDrafts=1 OR IsPublished=1);

GO
CREATE PROCEDURE dbo.Portfolio_List @Section int, @IncludeDrafts bit = 0 AS
SELECT Id, Section, Title, Summary, Body, Category, Url, DisplayOrder, IsPublished, UpdatedAtUtc FROM dbo.PortfolioEntries WHERE Section=@Section AND IsArchived=0 AND (@IncludeDrafts=1 OR IsPublished=1) ORDER BY DisplayOrder, Id;

GO
CREATE PROCEDURE dbo.Portfolio_Update @Id int, @Section int, @Title nvarchar(120), @Summary nvarchar(500), @Body nvarchar(6000), @Category nvarchar(120), @Url nvarchar(500), @DisplayOrder int, @IsPublished bit AS
UPDATE dbo.PortfolioEntries SET Section=@Section,Title=@Title,Summary=@Summary,Body=@Body,Category=@Category,Url=@Url,DisplayOrder=@DisplayOrder,IsPublished=@IsPublished,UpdatedAtUtc=SYSUTCDATETIME() WHERE Id=@Id AND IsArchived=0;
SELECT @@ROWCOUNT;

GO
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries)
BEGIN
-- Initial content only. Existing admin edits are never overwritten.
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries WHERE Section=0 AND Title=N'Viwe Cawe')
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(0,N'Viwe Cawe',N'.NET Software Developer in the Western Cape, South Africa.',N'I build business applications with C#, ASP.NET Core and SQL Server. Currently developing my skills in Blazor, REST APIs and deployment workflows.',N'About',N'https://github.com/ViweCawe',0,1);
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries WHERE Section=1 AND Title=N'C# and .NET')
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(1,N'C# and .NET',N'Business application development.',N'ASP.NET Core, REST APIs and reusable class libraries.',N'Development',NULL,1,1);
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries WHERE Section=1 AND Title=N'SQL Server and Dapper')
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(1,N'SQL Server and Dapper',N'Relational data and efficient queries.',N'Database design, stored procedures and data access.',N'Data',NULL,2,1);
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries WHERE Section=1 AND Title=N'Blazor, MAUI and CI/CD')
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(1,N'Blazor, MAUI and CI/CD',N'My current learning focus.',N'Building my first portfolio web application and exploring mobile development.',N'Learning',NULL,3,1);
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries WHERE Section=2 AND Title=N'LockedOut')
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(2,N'LockedOut',N'An accountability app for replacing bad habits with better routines.',N'Planned project. I am learning the web and mobile technologies needed to build it.',N'Planned',NULL,4,1);
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries WHERE Section=3 AND Title=N'Web application development')
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(3,N'Web application development',N'Practical applications for business workflows.',N'Planning, development and integration using .NET. Contact me to discuss scope and availability.',N'Development',NULL,5,1);
IF NOT EXISTS (SELECT 1 FROM dbo.PortfolioEntries WHERE Section=3 AND Title=N'Database integration')
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(3,N'Database integration',N'Connect applications to structured business data.',N'SQL Server schema design, stored procedures and Dapper integration.',N'Data',NULL,6,1);
END;

GO
