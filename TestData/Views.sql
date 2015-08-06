-- drop views
if object_id('ERM.ViewClient', 'view') is not null drop view ERM.ViewClient;
if object_id('BIT.FirmCategory', 'view') is not null drop view BIT.FirmCategory;
if object_id('CustomerIntelligence.FirmCategory', 'view') is not null drop view CustomerIntelligence.FirmCategory;
go

-- ViewClient, indexed view for query optimization
create view ERM.ViewClient
with schemabinding
as
select 
	Firm.ClientId,
	CategoryFirmAddress.FirmAddressId,
	CategoryOrganizationUnit.CategoryId,
	CategoryOrganizationUnit.CategoryGroupId,
	CategoryGroup.Rate
from ERM.Firm
	inner join ERM.FirmAddress on Firm.Id = FirmAddress.FirmId
	inner join ERM.CategoryFirmAddress on FirmAddress.Id = CategoryFirmAddress.FirmAddressId
	inner join ERM.CategoryOrganizationUnit on CategoryFirmAddress.CategoryId = CategoryOrganizationUnit.CategoryId AND Firm.OrganizationUnitId = CategoryOrganizationUnit.OrganizationUnitId
	inner join ERM.CategoryGroup on CategoryOrganizationUnit.CategoryGroupId = CategoryGroup.Id
where Firm.ClientId is not null
go
create unique clustered index PK_ViewClient
    on ERM.ViewClient (FirmAddressId, CategoryId);
go
create nonclustered index IX_ViewClient_ClientId_CategoryGroupId_Rate
	on ERM.ViewClient (ClientId, Rate)
	include (CategoryGroupId)
go

-- FirmCategory
create view BIT.FirmCategory
as
select distinct ERM.Project.Id as ProjectId, ERM.Firm.Id as FirmId, ERM.CategoryFirmAddress.CategoryId
from ERM.Firm 
  inner join ERM.Project on ERM.Project.OrganizationUnitId = ERM.Firm.OrganizationUnitId 
  inner join ERM.FirmAddress on ERM.FirmAddress.FirmId = ERM.Firm.Id 
  inner join ERM.CategoryFirmAddress on ERM.CategoryFirmAddress.FirmAddressId = ERM.FirmAddress.Id
go