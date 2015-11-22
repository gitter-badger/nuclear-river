-- create schema
if not exists (select * from sys.schemas where name = 'CustomerIntelligence') exec('create schema CustomerIntelligence')

-- drop tables
if object_id('CustomerIntelligence.CategoryGroup') is not null drop table CustomerIntelligence.CategoryGroup
if object_id('CustomerIntelligence.Project') is not null drop table CustomerIntelligence.Project
if object_id('CustomerIntelligence.ProjectCategory') is not null drop table CustomerIntelligence.ProjectCategory
if object_id('CustomerIntelligence.Territory') is not null drop table CustomerIntelligence.Territory
if object_id('CustomerIntelligence.Firm') is not null drop table CustomerIntelligence.Firm
if object_id('CustomerIntelligence.FirmBalance') is not null drop table CustomerIntelligence.FirmBalance
if object_id('CustomerIntelligence.FirmActivity') is not null drop table CustomerIntelligence.FirmActivity
if object_id('CustomerIntelligence.FirmTerritory') is not null drop table CustomerIntelligence.FirmTerritory
if object_id('CustomerIntelligence.FirmCategoryPartFirm') is not null drop table CustomerIntelligence.FirmCategoryPartFirm
if object_id('CustomerIntelligence.Client') is not null drop table CustomerIntelligence.Client
if object_id('CustomerIntelligence.Contact') is not null drop table CustomerIntelligence.Contact
if object_id('CustomerIntelligence.ClientContact') is not null drop table CustomerIntelligence.ClientContact
if object_id('CustomerIntelligence.FirmCategoryPartProject') is not null drop table CustomerIntelligence.FirmCategoryPartProject
if object_id('CustomerIntelligence.FirmCategory') is not null drop table CustomerIntelligence.FirmCategory
if object_id('CustomerIntelligence.FirmTerritory') is not null drop table CustomerIntelligence.FirmTerritory
if object_id('CustomerIntelligence.FirmView', 'view') is not null drop view CustomerIntelligence.FirmView

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
    , HasPhone bit not null constraint DF_Firms_HasPhone default 0
    , HasWebsite bit not null constraint DF_Firms_HasWebsite default 0
    , AddressCount int not null constraint DF_Firms_AddressCount default 0
    , CategoryGroupId bigint not null
    , ClientId bigint null
    , ProjectId bigint not null
    , OwnerId bigint not null
    , constraint PK_Firms primary key (Id)
)
go

-- FirmActivity
create table CustomerIntelligence.FirmActivity(
	FirmId bigint not null
    , LastActivityOn datetimeoffset(2) null
    , constraint PK_FirmActivities primary key (FirmId)
)
go

-- FirmBalance
create table CustomerIntelligence.FirmBalance(
    FirmId bigint not null
    , AccountId bigint not null
	, ProjectId bigint not null
    , Balance decimal(19,4) not null
    , constraint PK_FirmBalances primary key (FirmId, AccountId)
)
go

-- FirmCategory
create table CustomerIntelligence.FirmCategory(
	FirmId bigint not null
	, CategoryId bigint not null
    , Hits bigint null
    , Shows bigint null
    , FirmCount int null
    , AdvertisersShare float null
    , constraint PK_FirmCategoryPartFirm primary key (FirmId, CategoryId)
)
go

-- FirmTerritory
create table CustomerIntelligence.FirmTerritory(
	FirmId bigint not null
    , FirmAddressId bigint not null
	, TerritoryId bigint null
    , constraint PK_FirmTerritory primary key (FirmId, FirmAddressId)
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

-- ClientContact
create table CustomerIntelligence.ClientContact(
	ClientId bigint not null
	, ContactId bigint not null
	, [Role] int not null
    , constraint PK_ClientContact primary key (ClientId, ContactId)
)
go


-- FirmView
create view CustomerIntelligence.FirmView
as
select Firm.*, FirmActivity.LastActivityOn
from CustomerIntelligence.Firm
	inner join CustomerIntelligence.FirmActivity on FirmActivity.FirmId = Firm.Id
go


-- Идексы для клиента
create nonclustered index IX_Quering_1
on CustomerIntelligence.Firm (ProjectId,OwnerId,CreatedOn)
include (Id)
go
create nonclustered index IX_Quering_2
on CustomerIntelligence.Firm (ProjectId,OwnerId,LastDistributedOn)
include (Id)
go
create nonclustered index IX_Quering_3
on CustomerIntelligence.Firm (ProjectId,OwnerId,LastDisqualifiedOn)
include (Id)
go
create nonclustered index IX_Quering_4
on CustomerIntelligence.Firm (LastDistributedOn,ProjectId,OwnerId)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_5
on CustomerIntelligence.Firm (ProjectId,OwnerId)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_6
on CustomerIntelligence.Firm (HasWebsite,ProjectId,OwnerId)
include (Id)
go
create nonclustered index IX_Quering_7
on CustomerIntelligence.Firm (HasWebsite,ProjectId,OwnerId)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_8
on CustomerIntelligence.Firm (ProjectId,OwnerId,AddressCount)
include (Id)
go
create nonclustered index IX_Quering_9
on CustomerIntelligence.Firm (ProjectId,OwnerId,AddressCount)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_10
on CustomerIntelligence.FirmBalance (ProjectId,Balance)
include (FirmId)
go
create nonclustered index IX_Quering_11
on CustomerIntelligence.FirmActivity(LastActivityOn)
include (FirmId)
go
create nonclustered index IX_Quering_12
on CustomerIntelligence.ClientContact (Role)
include (ClientId)
go


-- FirmCategory
if object_id('BIT.FirmCategory', 'view') is not null drop view BIT.FirmCategory
go

create view BIT.FirmCategory
as
select distinct ERM.Project.Id as ProjectId, ERM.Firm.Id as FirmId, ERM.CategoryFirmAddress.CategoryId
from ERM.Firm 
  inner join ERM.Project on ERM.Project.OrganizationUnitId = ERM.Firm.OrganizationUnitId 
  inner join ERM.FirmAddress on ERM.FirmAddress.FirmId = ERM.Firm.Id 
  inner join ERM.CategoryFirmAddress on ERM.CategoryFirmAddress.FirmAddressId = ERM.FirmAddress.Id
go

if object_id('ERM.ViewClient', 'view') is not null drop view ERM.ViewClient
go

create view ERM.ViewClient
with schemabinding
as
select 
	Firm.ClientId,
	CategoryFirmAddress.FirmAddressId,
	CategoryOrganizationUnit.CategoryId,
	CategoryOrganizationUnit.CategoryGroupId,
	CategoryGroup.Rate
from ERM.Firm
	inner join ERM.FirmAddress on Firm.Id = FirmAddress.FirmId
	inner join ERM.CategoryFirmAddress on FirmAddress.Id = CategoryFirmAddress.FirmAddressId
	inner join ERM.CategoryOrganizationUnit on CategoryFirmAddress.CategoryId = CategoryOrganizationUnit.CategoryId AND Firm.OrganizationUnitId = CategoryOrganizationUnit.OrganizationUnitId
	inner join ERM.CategoryGroup on CategoryOrganizationUnit.CategoryGroupId = CategoryGroup.Id
where Firm.ClientId is not null
go
create unique clustered index PK_ViewClient
    on ERM.ViewClient (FirmAddressId, CategoryId)
go
create nonclustered index IX_ViewClient_ClientId_CategoryGroupId_Rate
	on ERM.ViewClient (ClientId, Rate)
	include (CategoryGroupId)
go
