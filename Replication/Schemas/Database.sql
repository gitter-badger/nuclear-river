-- create databse
if not exists (select * from sys.databases where name = '$(Database)') exec ('create database $(Database)')

declare @is_read_committed_snapshot_on as bit
declare @snapshot_isolation_state_desc as nvarchar(max)

SELECT @is_read_committed_snapshot_on = is_read_committed_snapshot_on, @snapshot_isolation_state_desc = snapshot_isolation_state_desc FROM sys.databases WHERE name='$(Database)'

if (NOT (@is_read_committed_snapshot_on = 1 AND @snapshot_isolation_state_desc = 'ON'))
begin
	exec ('alter database $(Database) set allow_snapshot_isolation on')
	exec ('alter database $(Database) set single_user with rollback immediate')
	exec ('alter database $(Database) set read_committed_snapshot on')
	exec ('alter database $(Database) set multi_user')
end
