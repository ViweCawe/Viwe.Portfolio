CREATE PROCEDURE dbo.Portfolio_List @Section int, @IncludeDrafts bit = 0 AS
SELECT Id, Section, Title, Summary, Body, Category, Url, DisplayOrder, IsPublished, UpdatedAtUtc FROM dbo.PortfolioEntries WHERE Section=@Section AND IsArchived=0 AND (@IncludeDrafts=1 OR IsPublished=1) ORDER BY DisplayOrder, Id;
