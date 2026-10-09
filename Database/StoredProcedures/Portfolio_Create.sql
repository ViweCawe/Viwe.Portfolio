CREATE PROCEDURE dbo.Portfolio_Create @Section int, @Title nvarchar(120), @Summary nvarchar(500), @Body nvarchar(6000), @Category nvarchar(120), @Url nvarchar(500), @DisplayOrder int, @IsPublished bit AS
INSERT dbo.PortfolioEntries(Section,Title,Summary,Body,Category,Url,DisplayOrder,IsPublished) VALUES(@Section,@Title,@Summary,@Body,@Category,@Url,@DisplayOrder,@IsPublished);
SELECT CAST(SCOPE_IDENTITY() AS int);
