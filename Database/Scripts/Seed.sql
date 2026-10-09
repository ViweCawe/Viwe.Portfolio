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
