CREATE VIEW [CustomerIntelligence].[Territory]
AS
SELECT  Id, Name, OrganizationUnitId
FROM    BusinessDirectory.Territories
WHERE   (IsActive = 1)
