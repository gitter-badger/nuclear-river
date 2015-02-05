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