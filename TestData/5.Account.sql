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