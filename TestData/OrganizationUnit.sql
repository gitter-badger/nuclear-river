CREATE VIEW [CustomerIntelligence].[OrganizationUnit]
AS
SELECT  Id, Name
FROM    Billing.OrganizationUnits
WHERE   (IsActive = 1) AND (IsDeleted = 0)

