create view CustomerIntelligence.FirmCategory
as
with
FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoriesActive as (select * from BusinessDirectory.categories where IsActive = 1 and IsDeleted = 0)
, FirmAddressesActive as (select * from BusinessDirectory.FirmAddresses where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoryFirmAddressesActive as (select * from BusinessDirectory.CategoryFirmAddresses where IsActive = 1 and IsDeleted = 0)
, CategoryOrganizationUnitsActive as (select COU.* from (select * from BusinessDirectory.CategoryOrganizationUnits where IsActive = 1 and IsDeleted = 0) COU inner join BusinessDirectory.CategoryGroups CG ON COU.CategoryGroupId = CG.Id)
, CategoriesWithParent (CategoryId, CategoryParentId) as
(
	select Id, ParentId from CategoriesActive CA
	union all
	select CA.Id, CWP.CategoryParentId from CategoriesWithParent CWP inner join CategoriesActive CA on CA.ParentId = CWP.CategoryId
)
, CategoriesWithParentNotNull (CategoryId, CategoryOrCategoryParentId) as (select CategoryId, isnull(CategoryParentId, CategoryId) from CategoriesWithParent)
, CategoryFirmAddressesWithParent (Id, CategoryId, FirmAddressId) as
(
	select max(Id), CategoryId, FirmAddressId from
	(
		select ROW_NUMBER() OVER(ORDER BY CFA.Id) Id, CWP.CategoryOrCategoryParentId CategoryId, CFA.FirmAddressId from CategoryFirmAddressesActive CFA
		left join CategoriesWithParentNotNull CWP on CFA.CategoryId = CWP.CategoryId
	) T group by FirmAddressId, CategoryId
)
, FirmAddressCategoryGroupIds (Id, FirmId, FirmAddressId, CategoryId, CategoryGroupId) as
(
	select CFA.Id, F.Id, FA.Id, CFA.CategoryId, COU.CategoryGroupId from CategoryFirmAddressesWithParent CFA
	inner join FirmAddressesActive FA on FA.Id = CFA.FirmAddressId
	inner join FirmsActive F on F.Id = FA.FirmId
	left join CategoryOrganizationUnitsActive COU on COU.CategoryId = CFA.CategoryId and F.OrganizationUnitId = COU.OrganizationUnitId
)
, FirmCategoryGroupIds (Id, CategoryId, CategoryGroupId, FirmId) as
(
	select isnull(max(Id), 0), isnull(CategoryId, 0), max(CategoryGroupId), FirmId from FirmAddressCategoryGroupIds
	group by FirmId, CategoryId
)

select * from FirmCategoryGroupIds