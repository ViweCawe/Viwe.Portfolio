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
