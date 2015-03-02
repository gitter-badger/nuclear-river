-- create schema
if not exists (select * from sys.schemas where name = 'CustomerIntelligence')
	exec('create schema CustomerIntelligence')
go

-- FirmCategory1
if object_id('CustomerIntelligence.FirmCategory1') is not null
	drop view CustomerIntelligence.FirmCategory1
go
create view CustomerIntelligence.FirmCategory1
as
with
FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoriesActive as (select * from BusinessDirectory.Categories where IsActive = 1 and IsDeleted = 0)
, FirmAddressesActive as (select * from BusinessDirectory.FirmAddresses where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoryFirmAddressesActive as (select * from BusinessDirectory.CategoryFirmAddresses where IsActive = 1 and IsDeleted = 0)
, CategoriesWithParent (CategoryId, CategoryParentId) as
(
	select Id, ParentId from CategoriesActive CA
	union all
	select CA.Id, CWP.CategoryParentId from CategoriesWithParent CWP inner join CategoriesActive CA on CA.ParentId = CWP.CategoryId
)
, CategoriesWithParentNotNull (CategoryId, CategoryOrCategoryParentId) as (select CategoryId, isnull(CategoryParentId, CategoryId) from CategoriesWithParent)
, CategoriesWithParentLevel as (select CWP.*, C.Level from CategoriesWithParentNotNull CWP inner join CategoriesActive C on CWP.CategoryOrCategoryParentId = C.Id)

, FirmCategories (FirmId, CategoryId) as 
(
	select distinct F.Id, CFA.CategoryId from CategoryFirmAddressesActive CFA
	inner join FirmAddressesActive FA on FA.Id = CFA.FirmAddressId
	inner join FirmsActive F on F.Id = FA.FirmId
)
, FirmCategories1 (Id, FirmId, CategoryId) as
(
	select distinct concat(FC.FirmId, CWP.CategoryOrCategoryParentId), FC.FirmId, isnull(CWP.CategoryOrCategoryParentId, 0) from FirmCategories FC
	inner join CategoriesWithParentLevel CWP on CWP.CategoryId = FC.CategoryId and CWP.Level = 1
)

select * from FirmCategories1
go

-- FirmCategory2
if object_id('CustomerIntelligence.FirmCategory2') is not null
	drop view CustomerIntelligence.FirmCategory2
go
create view CustomerIntelligence.FirmCategory2
as
with
FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoriesActive as (select * from BusinessDirectory.Categories where IsActive = 1 and IsDeleted = 0)
, FirmAddressesActive as (select * from BusinessDirectory.FirmAddresses where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoryFirmAddressesActive as (select * from BusinessDirectory.CategoryFirmAddresses where IsActive = 1 and IsDeleted = 0)
, CategoriesWithParent (CategoryId, CategoryParentId) as
(
	select Id, ParentId from CategoriesActive CA
	union all
	select CA.Id, CWP.CategoryParentId from CategoriesWithParent CWP inner join CategoriesActive CA on CA.ParentId = CWP.CategoryId
)
, CategoriesWithParentNotNull (CategoryId, CategoryOrCategoryParentId) as (select CategoryId, isnull(CategoryParentId, CategoryId) from CategoriesWithParent)
, CategoriesWithParentLevel as (select CWP.*, C.Level from CategoriesWithParentNotNull CWP inner join CategoriesActive C on CWP.CategoryOrCategoryParentId = C.Id)

, FirmCategories (FirmId, CategoryId) as 
(
	select distinct F.Id, CFA.CategoryId from CategoryFirmAddressesActive CFA
	inner join FirmAddressesActive FA on FA.Id = CFA.FirmAddressId
	inner join FirmsActive F on F.Id = FA.FirmId
)
, FirmCategories2 (Id, FirmId, CategoryId) as
(
	select distinct concat(FC.FirmId, CWP.CategoryOrCategoryParentId), FC.FirmId, isnull(CWP.CategoryOrCategoryParentId, 0) from FirmCategories FC
	inner join CategoriesWithParentLevel CWP on CWP.CategoryId = FC.CategoryId and CWP.Level = 2
)

select * from FirmCategories2
go

-- FirmCategory3
if object_id('CustomerIntelligence.FirmCategory3') is not null
	drop view CustomerIntelligence.FirmCategory3
go
create view CustomerIntelligence.FirmCategory3
as
with
FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoriesActive as (select * from BusinessDirectory.Categories where IsActive = 1 and IsDeleted = 0)
, FirmAddressesActive as (select * from BusinessDirectory.FirmAddresses where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, CategoryFirmAddressesActive as (select * from BusinessDirectory.CategoryFirmAddresses where IsActive = 1 and IsDeleted = 0)
, CategoryOrganizationUnitsActive as (select * from BusinessDirectory.CategoryOrganizationUnits where IsActive = 1 and IsDeleted = 0)
, CategoryOrganizationUnitsActive2 as
(
	select
	COU.CategoryId,
	COU.OrganizationUnitId,
	CategoryGroup =
	case
		when COU.CategoryGroupId = 1 then 5
		when COU.CategoryGroupId = 2 then 4
		when COU.CategoryGroupId = 3 then 2
		when COU.CategoryGroupId = 4 then 1
		when COU.CategoryGroupId is null and C.Level = 3 then 3
		else null
	end
	from CategoryOrganizationUnitsActive COU
	inner join CategoriesActive C on C.Id = COU.CategoryId
)
, FirmAddressCategoryGroups (Id, FirmId, FirmAddressId, CategoryId, CategoryGroup) as
(
	select CFA.Id, F.Id, FA.Id, CFA.CategoryId, COU.CategoryGroup from CategoryFirmAddressesActive CFA
	inner join FirmAddressesActive FA on FA.Id = CFA.FirmAddressId
	inner join FirmsActive F on F.Id = FA.FirmId
	left join CategoryOrganizationUnitsActive2 COU on COU.CategoryId = CFA.CategoryId and F.OrganizationUnitId = COU.OrganizationUnitId
)
, FirmCategoryGroups (Id, CategoryId, CategoryGroup, FirmId) as
(
	select isnull(max(Id), 0), isnull(CategoryId, 0), isnull(max(CategoryGroup), 3), FirmId from FirmAddressCategoryGroups
	group by FirmId, CategoryId
)

select * from FirmCategoryGroups
go

-- Firm
if object_id('CustomerIntelligence.Firm') is not null
	drop view CustomerIntelligence.Firm
go
create view CustomerIntelligence.Firm
as
with

FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, ClientsActive as (select * from Billing.Clients where IsActive = 1 and IsDeleted = 0)
, ContactsActive as (select * from Billing.Contacts where IsActive = 1 and IsDeleted = 0)
, FirmAddressesActive as (select * from BusinessDirectory.FirmAddresses where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, OrdersArchive as (select * from Billing.Orders where IsActive = 1 and IsDeleted = 0 and WorkflowStepId in (4, 6))
, OrganizationUnitsActive as (select * from Billing.OrganizationUnits where IsActive = 1 and IsDeleted = 0)
, FirmCategoryGroups (FirmId, CategoryGroup) as
(
	select FirmId, max(CategoryGroup) from CustomerIntelligence.FirmCategory3 group by FirmId
)
, Contacts1 as (
	select
		HasPhone = case when (ContactType = 1) then 1 else 0 end,
		HasWebsite = case when (ContactType = 4) then 1 else 0 end,
	* from BusinessDirectory.FirmContacts
)
, Contacts2 as (
	select
		HasPhone = case when (MainPhoneNumber is not null or AdditionalPhoneNumber1 is not null or AdditionalPhoneNumber2 is not null) then 1 else 0 end,
		HasWebsite = case when (Website is not null) then 1 else 0 end,
	* from ClientsActive
)
, Contacts3 as (
	select
		HasPhone = case when (MainPhoneNumber is not null or AdditionalPhoneNumber is not null or MobilePhoneNumber is not null or HomePhoneNumber is not null) then 1 else 0 end,
		HasWebsite = case when (Website is not null) then 1 else 0 end,
	* from ContactsActive
)
, LastDisqualifiedOns (FirmId, LastDisqualifiedOn) as
(
	select F.Id, cast(isnull(F.LastDisqualifyTime, C.LastDisqualifyTime) as datetimeoffset) from FirmsActive F
	left join ClientsActive C on F.ClientId = C.Id
)
, LastDistributedOns (FirmId, LastDistributedOn) as
(
	select F.Id, cast(LastDistributedOn as datetimeoffset) from FirmsActive F
	left join (select FirmId, max(EndDistributionDateFact) as LastDistributedOn from OrdersArchive group by FirmId) O on F.Id = O.FirmId
)
, FirmAddressCounts (FirmId, AddressCount) as 
(
	select F.Id, isnull(AddressCount, 0) from FirmsActive F
	left join (select FirmId, count(*) as AddressCount from FirmAddressesActive group by FirmId) FA ON F.Id = FA.FirmId
)
, Contacts_FirmAddress (Id, FirmId, HasPhone, HasWebsite) as 
(
	select FA.Id, FA.FirmId, isnull(FC.HasPhone, 0), isnull(FC.HasWebsite, 0) from FirmAddressesActive FA
	left join (select FirmAddressId, max(HasPhone) as HasPhone, max(HasWebsite) as HasWebsite from Contacts1 group by FirmAddressId) FC ON FC.FirmAddressId = FA.Id
)
, Contacts_Firm (FirmId, ClientId, HasPhone, HasWebsite) as
(
	select F.Id, F.ClientId, isnull(FA.HasPhone, 0), isnull(FA.HasWebsite, 0) from FirmsActive F
	left join (select FirmId, max(HasPhone) as HasPhone, max(HasWebsite) as HasWebsite from Contacts_FirmAddress group by FirmId) FA on FA.FirmId = F.Id
)
, Contacts_Client (ClientId, HasPhone, HasWebsite) as
(
	select C.Id, isnull(CC.HasPhone, 0), isnull(CC.HasWebsite, 0) from ClientsActive C
	left join (select ClientId, max(HasPhone) as HasPhone, max(HasWebsite) as HasWebsite from Contacts3 group by ClientId) CC ON CC.ClientId = C.Id
)
, ContactsAggregate (FirmId, HasPhone, HasWebsite) as
(
	select FirmId, 
		isnull(cast(FirmHasPhone | ClientHasPhone | ContactHasPhone as bit), 0),
		isnull(cast(FirmHasWebsite | ClientHasWebsite | ContactHasWebsite as bit), 0)
	from
	(
		select
			FC.FirmId,
			FC.HasPhone as FirmHasPhone,
			FC.HasWebsite as FirmHasWebsite,
			isnull(CC.HasPhone, 0) as ClientHasPhone,
			isnull(CC.HasWebsite, 0) as ClientHasWebsite,
			isnull(CCC.HasPhone, 0) as ContactHasPhone,
			isnull(CCC.HasWebsite, 0) as ContactHasWebsite
		from Contacts_Firm FC
		left join Contacts2 CC on CC.Id = FC.ClientId
		left join Contacts_Client CCC on CCC.ClientId = CC.Id
	) ContactsAggregate
)

select
Id,
Name,
OrganizationUnitId,
TerritoryId,
isnull(cast(CreatedOn as datetimeoffset), GETUTCDATE()) CreatedOn,
ClientId,

LastDisqualifiedOns.LastDisqualifiedOn,
LastDistributedOns.LastDistributedOn,
FirmAddressCounts.AddressCount,
ContactsAggregate.HasPhone,
ContactsAggregate.HasWebsite,
isnull(FirmCategoryGroups.CategoryGroup, 3) CategoryGroup

from FirmsActive F
inner join LastDisqualifiedOns on F.Id = LastDisqualifiedOns.FirmId
inner join LastDistributedOns on F.Id = LastDistributedOns.FirmId
inner join FirmAddressCounts on F.Id = FirmAddressCounts.FirmId
inner join ContactsAggregate on F.Id = ContactsAggregate.FirmId
inner join FirmCategoryGroups ON F.Id = FirmCategoryGroups.FirmId
go

-- Client
if object_id('CustomerIntelligence.Client') is not null
	drop view CustomerIntelligence.Client
go
create view CustomerIntelligence.Client
as
with

FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, ClientsActive as (select * from Billing.Clients where IsActive = 1 and IsDeleted = 0)

, FirmCategoryGroups (FirmId, CategoryGroup) as
(
	select FirmId, max(CategoryGroup) from CustomerIntelligence.FirmCategory3 group by FirmId
)
, FirmCategoryGroupsExpanded as
(
	select C.Id ClientId, FCG.* from FirmCategoryGroups FCG
	inner join FirmsActive F on F.Id = FCG.FirmId
	inner join ClientsActive C on F.ClientId = C.Id
)
, ClientCategoryGroups (ClientId, CategoryGroup) as
(
	select ClientId, max(CategoryGroup) from FirmCategoryGroupsExpanded group by ClientId
)

select
Id,
Name,
isnull(ClientCategoryGroups.CategoryGroup, 3) CategoryGroup

from ClientsActive C
inner join ClientCategoryGroups ON C.Id = ClientCategoryGroups.ClientId
go

-- Contact
if object_id('CustomerIntelligence.Contact') is not null
	drop view CustomerIntelligence.Contact
go
create view CustomerIntelligence.Contact
as
with

ContactActive as (select * from Billing.Contacts where IsActive = 1 and IsDeleted = 0)
, ClientsActive as (select * from Billing.Clients where IsActive = 1 and IsDeleted = 0)

select
CA.Id,
AccountRole [Role],
IsFired,
ClientId

from ContactActive CA
inner join ClientsActive C on CA.ClientId = C.Id
go

-- Account
if object_id('CustomerIntelligence.Account') is not null
	drop view CustomerIntelligence.Account
go
create view CustomerIntelligence.Account
as
with

AccountActive as (select * from Billing.Accounts where IsActive = 1 and IsDeleted = 0)
, ClientsActive as (select * from Billing.Clients where IsActive = 1 and IsDeleted = 0)
, LegalPersonActive as (select * from Billing.LegalPersons where IsActive = 1 and IsDeleted = 0)

, AccountAggregate (LegalPersonId, Id, Balance) as
(
	select LegalPersonId, max(Id), isnull(sum(Balance), 0) from AccountActive group by LegalPersonId
)

select
isnull(A.Id, 0) Id,
A.Balance,
C.Id ClientId

from AccountAggregate A
inner join LegalPersonActive LP on A.LegalPersonId = LP.Id
inner join ClientsActive C on LP.ClientId = C.Id
go

-- Category
if object_id('CustomerIntelligence.Category') is not null
	drop view CustomerIntelligence.Category
go
CREATE VIEW [CustomerIntelligence].[Category]
AS
SELECT  Id, Name, [Level]
FROM    BusinessDirectory.Categories
WHERE   IsActive = 1 AND IsDeleted = 0
go

-- OrganizationUnit
if object_id('CustomerIntelligence.OrganizationUnit') is not null
	drop view CustomerIntelligence.OrganizationUnit
go
CREATE VIEW [CustomerIntelligence].[OrganizationUnit]
AS
SELECT  Id, Name
FROM    Billing.OrganizationUnits
WHERE   (IsActive = 1) AND (IsDeleted = 0)
go

-- Territory
if object_id('CustomerIntelligence.Territory') is not null
	drop view CustomerIntelligence.Territory
go
CREATE VIEW [CustomerIntelligence].[Territory]
AS
SELECT  Id, Name, OrganizationUnitId
FROM    BusinessDirectory.Territories
WHERE   (IsActive = 1)
