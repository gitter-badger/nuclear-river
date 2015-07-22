-- create schema
if not exists (select * from sys.schemas where name = 'BIT')
	exec('create schema BIT')
go

-- drop views
if object_id('BIT.FirmCategory', 'view') is not null drop view BIT.FirmCategory;
go

-- drop tables
if object_id('BIT.ProjectCategoryStatistics') is not null drop table BIT.ProjectCategoryStatistics;
if object_id('BIT.FirmCategoryStatistics') is not null drop table BIT.FirmCategoryStatistics;
go


-- ProjectCategoryStatistics
create table BIT.ProjectCategoryStatistics(
	ProjectId bigint not null,
	CategoryId bigint not null,
    AdvertisersCount bigint not null, 
	constraint PK_ProjectCategoryStatistics primary key (ProjectId, CategoryId)
)
go

-- FirmCategoryStatistics
create table BIT.FirmCategoryStatistics(
	ProjectId bigint not null,
	FirmId bigint not null,
	CategoryId bigint not null,
	Hits bigint not null,
	Shows bigint not null,
	constraint PK_FirmCategoryStatistics primary key (FirmId, CategoryId)
)
go

-- FirmCategory
create view BIT.FirmCategory
as
select distinct ERM.Project.Id as ProjectId, ERM.Firm.Id as FirmId, ERM.CategoryFirmAddress.CategoryId
from ERM.Firm 
  inner join ERM.Project on ERM.Project.OrganizationUnitId = ERM.Firm.OrganizationUnitId 
  inner join ERM.FirmAddress on ERM.FirmAddress.FirmId = ERM.Firm.Id 
  inner join ERM.CategoryFirmAddress on ERM.CategoryFirmAddress.FirmAddressId = ERM.FirmAddress.Id
go