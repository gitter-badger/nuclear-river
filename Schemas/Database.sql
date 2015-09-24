-- create databse
if not exists (select * from sys.databases where name = '$(Database)') exec ('create database $(Database)')

exec ('alter database $(Database) set READ_COMMITTED_SNAPSHOT on')