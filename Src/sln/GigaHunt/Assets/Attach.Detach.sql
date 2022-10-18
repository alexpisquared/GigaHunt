USE [master]
GO
CREATE DATABASE [QStats.Ofc] ON 
( FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\QStats.Ofc.mdf' ),
( FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\QStats.Ofc.ldf' )
 FOR ATTACH
GO


USE [master]
GO
ALTER DATABASE [QStats.Ofc] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
GO
USE [master]
GO
EXEC master.dbo.sp_detach_db @dbname = N'QStats.Ofc', @skipchecks = 'false'
GO
