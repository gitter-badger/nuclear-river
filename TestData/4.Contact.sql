create view CustomerIntelligence.Contact
as
with

ContactActive as (select * from Billing.Contacts where IsActive = 1 and IsDeleted = 0)

select
Id,
AccountRole Role,
IsFired,
ClientId

from ContactActive C