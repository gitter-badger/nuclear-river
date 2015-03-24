-- create schema
if not exists (select * from sys.schemas where name = 'ERM')
	exec('create schema ERM')
go

-- drop tables
if object_id('ERM.FirmCategoryGroups') is not null drop table ERM.FirmCategoryGroups;
if object_id('ERM.FirmCategories') is not null drop table ERM.FirmCategories;
if object_id('ERM.FirmAccount') is not null drop table ERM.FirmAccount;
if object_id('ERM.Firm') is not null drop table ERM.Firm;

if object_id('ERM.Contact') is not null drop table ERM.Contact;
if object_id('ERM.Client') is not null drop table ERM.Client;
go


-- Client
create table ERM.Client(
	Id bigint not null
    , Name nvarchar(250) not null
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
    --, constraint FK_Contacts_Clients foreign key (ClientId) references ERM.Clients (Id)
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
    --, constraint FK_Firms_Clients foreign key (ClientId) references ERM.Clients (Id)
)
go

-- FirmAccount
create table ERM.FirmAccount(
	AccountId bigint not null
    , FirmId bigint not null
    , Balance decimal(19,4) not null
    , constraint PK_FirmAccounts primary key (AccountId, FirmId)
)
go

-- FirmCategories
create table ERM.FirmCategories(
	FirmId bigint not null
    , CategoryId bigint not null
    , constraint PK_FirmCategories primary key (FirmId, CategoryId)
    --, constraint FK_FirmCategories_Firms foreign key (FirmId) references ERM.Firms (Id)
)
go

-- FirmCategoryGroups
create table ERM.FirmCategoryGroups(
	FirmId bigint not null
    , CategoryGroupId bigint not null
    , constraint PK_FirmCategoryGroups primary key (FirmId, CategoryGroupId)
    --, constraint FK_FirmCategoryGroups_Firms foreign key (FirmId) references ERM.Firms (Id)
)
go