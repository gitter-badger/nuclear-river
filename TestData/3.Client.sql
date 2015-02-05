create view CustomerIntelligence.Client
as
with

FirmsActive as (select * from BusinessDirectory.Firms where IsActive = 1 and IsDeleted = 0 and ClosedForAscertainment = 0)
, ClientsActive as (select * from Billing.Clients where IsActive = 1 and IsDeleted = 0)

, FirmCategoryGroupIds (FirmId, CategoryGroupId) as
(
	select FirmId, max(CategoryGroupId) from CustomerIntelligence.FirmCategory group by FirmId
)
, FirmCategoryGroupIdsExpanded as
(
	select C.Id ClientId, FCG.* from FirmCategoryGroupIds FCG
	inner join FirmsActive F on F.Id = FCG.FirmId
	inner join ClientsActive C on F.ClientId = C.Id
)
, ClientCategoryGroupIds (ClientId, CategoryGroupId) as
(
	select ClientId, max(CategoryGroupId) from FirmCategoryGroupIdsExpanded group by ClientId
)

select
Id,
Name,
ClientCategoryGroupIds.CategoryGroupId

from ClientsActive C
inner join ClientCategoryGroupIds ON C.Id = ClientCategoryGroupIds.ClientId