-- create schema
if not exists (select * from sys.schemas where name = 'CustomerIntelligence')
	exec('create schema CustomerIntelligence')
go

-- drop views
if object_id('CustomerIntelligence.ViewFirmCategory', 'view') is not null drop view ERM.ViewFirmCategory;
go

-- drop tables
if object_id('CustomerIntelligence.CategoryGroup') is not null drop table CustomerIntelligence.CategoryGroup;
if object_id('CustomerIntelligence.Project') is not null drop table CustomerIntelligence.Project;
if object_id('CustomerIntelligence.ProjectCategory') is not null drop table CustomerIntelligence.ProjectCategory;
if object_id('CustomerIntelligence.Territory') is not null drop table CustomerIntelligence.Territory;
if object_id('CustomerIntelligence.Firm') is not null drop table CustomerIntelligence.Firm;
if object_id('CustomerIntelligence.FirmBalance') is not null drop table CustomerIntelligence.FirmBalance;
if object_id('CustomerIntelligence.FirmCategory') is not null drop table CustomerIntelligence.FirmCategory;
if object_id('CustomerIntelligence.Client') is not null drop table CustomerIntelligence.Client;
if object_id('CustomerIntelligence.Contact') is not null drop table CustomerIntelligence.Contact;
if object_id('CustomerIntelligence.FirmCategoryStatistics') is not null drop table CustomerIntelligence.FirmCategoryStatistics;
go


-- CategoryGroup
create table CustomerIntelligence.CategoryGroup(
	Id bigint not null
    , Name nvarchar(256) not null
    , Rate float not null
    , constraint PK_CategoryGroups primary key (Id)
)
go

-- Project
create table CustomerIntelligence.Project(
	Id bigint not null
    , Name nvarchar(256) not null
    , constraint PK_Projects primary key (Id)
)
go

-- ProjectCategory
create table CustomerIntelligence.ProjectCategory(
	ProjectId bigint not null
    , CategoryId bigint not null
    , Name nvarchar(256) not null
    , [Level] int not null
    , ParentId bigint null
    , constraint PK_ProjectCategories primary key (ProjectId, CategoryId)
)
go

-- Territory
create table CustomerIntelligence.Territory(
	Id bigint not null
    , Name nvarchar(256) not null
    , ProjectId bigint not null
    , constraint PK_Territories primary key (Id)
)
go

-- Firm
create table CustomerIntelligence.Firm(
	Id bigint not null
    , Name nvarchar(256) not null
    , CreatedOn datetimeoffset(2) not null
    , LastDisqualifiedOn datetimeoffset(2) null
    , LastDistributedOn datetimeoffset(2) null
    , LastActivityOn datetimeoffset(2) null
    , HasPhone bit not null constraint DF_Firms_HasPhone default 0
    , HasWebsite bit not null constraint DF_Firms_HasWebsite default 0
    , AddressCount int not null constraint DF_Firms_AddressCount default 0
    , CategoryGroupId bigint not null
    , ClientId bigint null
    , ProjectId bigint not null
    , OwnerId bigint not null
    , TerritoryId bigint not null
    , constraint PK_Firms primary key (Id)
)
go

-- FirmBalance
create table CustomerIntelligence.FirmBalance(
    FirmId bigint not null
    , AccountId bigint not null
    , Balance decimal(19,4) not null
    , constraint PK_FirmBalances primary key (FirmId, AccountId)
)
go

-- FirmCategory
create table CustomerIntelligence.FirmCategory(
	FirmId bigint not null
    , Hits bigint not null constraint DF_FirmCategories_Hits default 0
    , Shows bigint not null constraint DF_FirmCategories_Shows default 0
    , CategoryId bigint not null
    , constraint PK_FirmCategories primary key (FirmId, CategoryId)
)
go

-- FirmCategoryStatistics
create table CustomerIntelligence.FirmCategoryStatistics(
	ProjectId bigint not null
    , CategoryId bigint not null
    , FirmCount int not null
    , AdvertisersShare float null
    , constraint PK_FirmCategoryStatistics primary key (ProjectId, CategoryId)
)
go

-- Client
create table CustomerIntelligence.Client(
	Id bigint not null
    , Name nvarchar(256) not null
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

-- ¬ьюха дл€ сведени€ FirmCategory и FirmCategoryStatistics в единую бизнес-сущность
create view CustomerIntelligence.ViewFirmCategory
as
select
	FirmCategory.FirmId
	, FirmCategory.CategoryId
	, FirmCategory.Hits
	, FirmCategory.Shows
	, FirmCategoryStatistics.FirmCount
	, FirmCategoryStatistics.AdvertisersShare
from
	CustomerIntelligence.Firm 
	inner join CustomerIntelligence.FirmCategory on Firm.Id = FirmCategory.FirmId 
	left join CustomerIntelligence.FirmCategoryStatistics on Firm.ProjectId = FirmCategoryStatistics.ProjectId and FirmCategory.CategoryId = FirmCategoryStatistics.CategoryId
go
