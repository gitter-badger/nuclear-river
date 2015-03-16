-- create schema
if not exists (select * from sys.schemas where name = 'CustomerIntelligence')
	exec('create schema CustomerIntelligence')
go

-- drop tables
if object_id('CustomerIntelligence.FirmCategoryGroups') is not null drop table CustomerIntelligence.FirmCategoryGroups;
if object_id('CustomerIntelligence.FirmCategories') is not null drop table CustomerIntelligence.FirmCategories;
if object_id('CustomerIntelligence.FirmAccount') is not null drop table CustomerIntelligence.FirmAccount;
if object_id('CustomerIntelligence.Firm') is not null drop table CustomerIntelligence.Firm;

if object_id('CustomerIntelligence.Contact') is not null drop table CustomerIntelligence.Contact;
if object_id('CustomerIntelligence.Client') is not null drop table CustomerIntelligence.Client;
go


-- Client
create table CustomerIntelligence.Client(
	Id bigint not null
    , Name nvarchar(250) not null
    , CategoryGroupId bigint not null
    , constraint PK_Clients primary key (Id)
)
go

-- Contact
create table CustomerIntelligence.Contact(
	Id bigint not null
	, [Role] int not null
	, IsFired bit not null constraint DF_Contacts_IsFired default 0
    , ClientId bigint not null
    , constraint PK_Contacts primary key (Id)
)
go

-- Firm
create table CustomerIntelligence.Firm(
	Id bigint not null
    , Name nvarchar(250) not null
    , CreatedOn datetimeoffset(2) not null
    , LastDisqualifiedOn datetimeoffset(2) not null
    , LastDistributedOn datetimeoffset(2) not null
    , HasPhone bit not null constraint DF_Firms_HasPhone default 0
    , HasWebsite bit not null constraint DF_Firms_HasWebsite default 0
    , AddressCount int not null constraint DF_Firms_AddressCount default 0
    , CategoryGroupId bigint not null
    , ClientId bigint null
    , OrganizationUnitId bigint not null
    , TerritoryId bigint not null
    , constraint PK_Firms primary key (Id)
)
go

-- FirmAccount
create table CustomerIntelligence.FirmAccount(
	AccountId bigint not null
    , FirmId bigint not null
    , Balance decimal(19,4) not null
    , constraint PK_FirmAccounts primary key (AccountId, FirmId)
)
go

-- FirmCategories
create table CustomerIntelligence.FirmCategories(
	FirmId bigint not null
    , CategoryId bigint not null
    , constraint PK_FirmCategories primary key (FirmId, CategoryId)
)
go

-- FirmCategoryGroups
create table CustomerIntelligence.FirmCategoryGroups(
	FirmId bigint not null
    , CategoryGroupId bigint not null
    , constraint PK_FirmCategoryGroups primary key (FirmId, CategoryGroupId)
)  
go