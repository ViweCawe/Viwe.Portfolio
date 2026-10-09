CREATE PROCEDURE dbo.Portfolio_Update @Id int, @Section int, @Title nvarchar(120), @Summary nvarchar(500), @Body nvarchar(6000), @Category nvarchar(120), @Url nvarchar(500), @DisplayOrder int, @IsPublished bit AS
UPDATE dbo.PortfolioEntries SET Section=@Section,Title=@Title,Summary=@Summary,Body=@Body,Category=@Category,Url=@Url,DisplayOrder=@DisplayOrder,IsPublished=@IsPublished,UpdatedAtUtc=SYSUTCDATETIME() WHERE Id=@Id AND IsArchived=0;
SELECT @@ROWCOUNT;
