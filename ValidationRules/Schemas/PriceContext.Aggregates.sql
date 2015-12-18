if not exists (select * from sys.schemas where name = 'PriceAggregate') exec('create schema PriceAggregate')

if object_id('PriceAggregate.XXX') is not null drop table PriceAggregate.XXX


go

create table PriceAggregate.Price(
    Id bigint not null
)
go

create table PriceAggregate.PricePosition(
    Id bigint not null,
    PositionId bigint not null,
    PriceId bigint not null,
)
go

create table PriceAggregate.Position(
    Id bigint not null
)
go

create table PriceAggregate.PositionAssociation(
    PriceId bigint not null,
    MasterPositionId bigint not null,
    AssociatedPositionId bigint not null
)
go

create table PriceAggregate.PositionDenial(
    PriceId bigint not null,
    MasterPositionId bigint not null,
    DeniedPositionId bigint not null
)
go