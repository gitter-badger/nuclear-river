-- create schema
if not exists (select * from sys.schemas where name = 'BIT')
	exec('create schema BIT')
go

-- drop tables
if object_id('BIT.ProjectCategoryStatististics') is not null drop table BIT.ProjectCategoryStatististics;
if object_id('BIT.FirmCategoryStatistics') is not null drop table BIT.FirmCategoryStatistics;
go


-- ProjectCategoryStatististics
create table BIT.ProjectCategoryStatististics(
	ProjectId bigint not null,
	CategoryId bigint not null,
    AdvertisersCount bigint not null, 
	constraint PK_CategoryStatististics primary key (ProjectId, CategoryId)
)
go

-- FirmCategoryStatistics
create table BIT.FirmCategoryStatistics(
	ProjectId bigint not null,
	FirmId bigint not null,
	CategoryId bigint not null,
	Hits bigint not null,
	Shows bigint not null,
	constraint PK_FirmStatistics primary key (FirmId, CategoryId)
)
go
