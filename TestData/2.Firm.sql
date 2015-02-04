create view CustomerIntelligence.Firm
as
with

FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, ClientsActive as (select * from Billing.Clients where IsActive = 1 and IsDeleted = 0)
, ContactsActive as (select * from Billing.Contacts where IsActive = 1 and IsDeleted = 0)
, FirmAddressesActive as (select * from BusinessDirectory.FirmAddresses where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, OrdersArchive as (select * from Billing.Orders where IsActive = 1 and IsDeleted = 0 and WorkflowStepId in (4, 6))
, OrganizationUnitsActive as (select * from Billing.OrganizationUnits where IsActive = 1 and IsDeleted = 0)
, FirmCategoryGroupIds (FirmId, CategoryGroup) as
(
	select FirmId, max(CategoryGroup) from CustomerIntelligence.FirmCategory group by FirmId
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
FirmCategoryGroupIds.CategoryGroup

from FirmsActive F
inner join LastDisqualifiedOns on F.Id = LastDisqualifiedOns.FirmId
inner join LastDistributedOns on F.Id = LastDistributedOns.FirmId
inner join FirmAddressCounts on F.Id = FirmAddressCounts.FirmId
inner join ContactsAggregate on F.Id = ContactsAggregate.FirmId
inner join FirmCategoryGroupIds ON F.Id = FirmCategoryGroupIds.FirmId