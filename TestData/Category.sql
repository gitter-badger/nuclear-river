CREATE VIEW [CustomerIntelligence].[Category]
AS
SELECT  Id, Name, [Level]
FROM    BusinessDirectory.Categories
WHERE   IsActive = 1 AND IsDeleted = 0
