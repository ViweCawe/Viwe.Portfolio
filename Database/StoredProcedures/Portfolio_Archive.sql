CREATE PROCEDURE dbo.Portfolio_Archive @Id int AS
UPDATE dbo.PortfolioEntries SET IsArchived=1,IsPublished=0,UpdatedAtUtc=SYSUTCDATETIME() WHERE Id=@Id AND IsArchived=0;
SELECT @@ROWCOUNT;
