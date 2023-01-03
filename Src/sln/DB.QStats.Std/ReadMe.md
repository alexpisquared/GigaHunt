#Chrono -- from CI  <<

#2019-09: see C:\g\alex-pi\Src\AlexPiApi\ReadMe.md
#2021-02:
  Model gen-n:
      Install-Package Microsoft.EntityFrameworkCore
      Install-Package Microsoft.EntityFrameworkCore.SqlServer
      Install-Package Microsoft.EntityFrameworkCore.Tools
      Install-Package Microsoft.EntityFrameworkCore.Design
  ..installed all 3 straight from the Nuget manager.

  Scaffold-DbContext "Server=mtUATsqldb;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;"  Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\QStatsRls
  Scaffold-DbContext "Server=mtUATsqldb;Database=RMS;Trusted_Connection=True;Encrypt=False;"        Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\RMS
  Scaffold-DbContext "Server=mtUATsqldb;Database=RMS;Trusted_Connection=True;Encrypt=False;"        Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\RMS -force
  Scaffold-DbContext "Server=mtUATsqldb;Database=BR;Trusted_Connection=True;Encrypt=False;"         Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\BR
  
#Running this gives Yellow warnings:
Scaffold-DbContext "Server=MTUATSQLDB;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
#Yellow Warnings:
To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
The column 'dbo.NewIssues.Active'             would normally be mapped to a non-nullable bool property, but it has a default constraint. Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.
The column 'dbo.NewIssues.UploadInitialAlloc' would normally be mapped to a non-nullable bool property, but it has a default constraint. Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.
The column 'dbo.SecurityGroups.Active'        would normally be mapped to a non-nullable bool property, but it has a default constraint. Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.


#2021-05-21
Scaffold-DbContext "Server=.\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force
#2021-06-29
##From CLI:
1. must install EF:  dotnet tool install --global dotnet-ef
2. must be in the project's folder
3. dotnet ef dbcontext scaffold "Server=.\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -o Models --schema dpl
##From VS:
   Scaffold-DbContext           "Server=.\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -o Models  -schema dpl -table BookGroup,BookReport_view -force
Build started...
Build succeeded.
To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
The column 'dbo.DplDatabase.IsActive' would normally be mapped to a non-nullable bool property, but it has a default constraint. Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.
PM>  


USE [QStatsRls]
GO
  INSERT INTO dbo.[DplDatabase]                   (Name, Notes, IsActive)  SELECT     name, '** Initial load. ' AS Expr1, 0 AS Expr2  FROM        sys.databases  WHERE     (database_id > 4)  ORDER BY name


#2021-08-07  dpl schema retired as per MC's request
Scaffold-DbContext "Server=mtUATsqldb;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -o Models -force

#2022-04-29  //tu: discard local changes:  git reset --hard origin

#2022-05-24  regen-ed model to get PermAsgn joins missing in UAT:
Scaffold-DbContext "Server=mtDEVsqldb;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -o Models -force
Scaffold-DbContext "Server=mtDEVsqldb,1625;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -o Models -force
    ^ no diff in results.


#Chrono -- from CI  >>

//entity.HasNoKey(); //tu: ?improper? fix for "The invoked method cannot be used for the entity type 'VEmailAvailProd' because it does not have a primary key."


#2022-11-02
Scaffold-DbContext "Server=.\SqlExpRess;Database=QStatsRls;Trusted_Connection=True;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

#2022-11-07
Include:  
  https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/read-related-data?view=aspnetcore-6.0
EF6 Eage/Lazy/Explicit: 
  https://www.entityframeworktutorial.net/eager-loading-in-entity-framework.aspx
  https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager


2023-01-03  //tu:

dotnet new classlib -n DB.DbName
cd .\DB.DbName\
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet ef dbcontext scaffold "Server=.\SqlExpress;Database=DbName;Trusted_Connection=True;TrustServerCertificate=Yes;" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
