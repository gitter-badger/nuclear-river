-- create schema
if not exists (select * from sys.schemas where name = 'BIT')
	exec('create schema BIT')
go

-- drop tables
if object_id('BIT.CategoryStatististics') is not null drop table BIT.CategoryStatististics;
if object_id('BIT.FirmStatistics') is not null drop table BIT.FirmStatistics;
go


-- CategoryStatististics
create table BIT.CategoryStatististics(
	ProjectId bigint not null,
	CategoryId bigint not null,
    AdvertisersCount int not null, 
	constraint PK_CategoryStatististics primary key (ProjectId, CategoryId)
)
go

-- FirmStatistics
create table BIT.FirmStatistics(
	ProjectId bigint not null,
	FirmId bigint not null,
	CategoryId bigint not null,
	Hits int not null,
	Shows int not null,
	constraint PK_FirmStatistics primary key (ProjectId, FirmId, CategoryId)
)
go
