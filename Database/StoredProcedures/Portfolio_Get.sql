CREATE PROCEDURE dbo.Portfolio_Get @Id int, @IncludeDrafts bit = 0 AS
SELECT Id, Section, Title, Summary, Body, Category, Url, DisplayOrder, IsPublished, UpdatedAtUtc FROM dbo.PortfolioEntries WHERE Id=@Id AND IsArchived=0 AND (@IncludeDrafts=1 OR IsPublished=1);
