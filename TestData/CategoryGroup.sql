CREATE VIEW [CustomerIntelligence].[CategoryGroup]
AS
SELECT  Id, CategoryGroupName Name, GroupRate Rate
FROM    BusinessDirectory.CategoryGroups
WHERE   IsActive = 1 AND IsDeleted = 0
