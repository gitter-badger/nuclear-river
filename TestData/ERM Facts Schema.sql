-- create schema
if not exists (select * from sys.schemas where name = 'ERM')
	exec('create schema ERM')
go

-- drop tables
if object_id('ERM.Order') is not null drop table ERM.[Order];
if object_id('ERM.CategoryOrganizationUnit') is not null drop table ERM.CategoryOrganizationUnit;
if object_id('ERM.CategoryFirmAddress') is not null drop table ERM.CategoryFirmAddress;
if object_id('ERM.FirmContact') is not null drop table ERM.FirmContact;
if object_id('ERM.FirmAddress') is not null drop table ERM.FirmAddress;
if object_id('ERM.Firm') is not null drop table ERM.Firm;

if object_id('ERM.Account') is not null drop table ERM.Account;
if object_id('ERM.Contact') is not null drop table ERM.Contact;
if object_id('ERM.LegalPerson') is not null drop table ERM.LegalPerson;
if object_id('ERM.Client') is not null drop table ERM.Client;
go


-- Client
create table ERM.Client(
	Id bigint not null
    , Name nvarchar(250) not null
    , LastDisqualifiedOn datetimeoffset(2) null
    , HasPhone bit not null constraint DF_Clients_HasPhone default 0
    , HasWebsite bit not null constraint DF_Clients_HasWebsite default 0
    , constraint PK_Clients primary key (Id)
)
go

-- Contact
create table ERM.Contact(
	Id bigint not null
	, [Role] int not null
	, IsFired bit not null constraint DF_Contacts_IsFired default 0
    , HasPhone bit not null constraint DF_Contacts_HasPhone default 0
    , HasWebsite bit not null constraint DF_Contacts_HasWebsite default 0
    , ClientId bigint not null
    , constraint PK_Contacts primary key (Id)
)
go
create nonclustered index IX_Contact_HasPhone_HasWebsite
on ERM.Contact (HasPhone, HasWebsite)
go

-- LegalPerson
create table ERM.LegalPerson(
	Id bigint not null
    , ClientId bigint not null
    , constraint PK_LegalPersons primary key (Id)
)
go

-- Account
create table ERM.Account(
	Id bigint not null
    , Balance decimal(19,4) not null
    , LegalPersonId bit not null
    , constraint PK_Accounts primary key (Id)
)
go

-- Firm
create table ERM.Firm(
	Id bigint not null
    , Name nvarchar(250) not null
    , CreatedOn datetimeoffset(2) not null
    , LastDisqualifiedOn datetimeoffset(2) null
    , LastDistributedOn datetimeoffset(2) null
    , HasPhone bit not null constraint DF_Firms_HasPhone default 0
    , HasWebsite bit not null constraint DF_Firms_HasWebsite default 0
    , AddressCount int not null constraint DF_Firms_AddressCount default 0
    , ClientId bigint null
    , OrganizationUnitId bigint not null
    , TerritoryId bigint not null
    , constraint PK_Firms primary key (Id)
)
go

-- FirmAddress
create table ERM.FirmAddress(
	Id bigint not null
    , FirmId bigint not null
    , constraint PK_FirmAddresses primary key (Id)
)
go

-- FirmContact
create table ERM.FirmContact(
	Id bigint not null
    , HasPhone bit not null constraint DF_FirmContacts_HasPhone default 0
    , HasWebsite bit not null constraint DF_FirmContacts_HasWebsite default 0
    , FirmAddressId bigint not null
    , constraint PK_FirmContacts primary key (Id)
)
go
create nonclustered index IX_FirmContact_HasPhone_FirmAddressId
on ERM.FirmContact (HasPhone, FirmAddressId)
go
create nonclustered index IX_FirmContact_HasWebsite_FirmAddressId
on ERM.FirmContact (HasWebsite,FirmAddressId)
go

-- CategoryFirmAddress
create table ERM.CategoryFirmAddress(
	Id bigint not null
    , CategoryId bigint not null
    , FirmAddressId bigint not null
    , constraint PK_CategoryFirmAddresses primary key (Id)
)
go

-- CategoryOrganizationUnit
create table ERM.CategoryOrganizationUnit(
	Id bigint not null
	, CategoryId bigint not null
    , CategoryGroupId bigint not null
    , OrganizationUnitId bigint not null
    , constraint PK_CategoryOrganizationUnits primary key (Id)
)
go

-- Order
create table ERM.[Order](
	Id bigint not null
    , EndDistributionDateFact datetimeoffset(2) not null
    , FirmId bigint null
    , constraint PK_Orders primary key (Id)
)
go
