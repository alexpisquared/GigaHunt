global using System;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Data.SqlTypes;
global using System.Diagnostics;
global using Microsoft.EntityFrameworkCore; // <== instead of  System.Data.Entity  or System.InvalidOperationException: The source IQueryable doesn't implement IDbAsyncEnumerable. Only sources that implement IDbAsyncEnumerable can be used for Entity Framework asynchronous operations. For more details see http://go.microsoft.com/fwlink/?LinkId=287068. ==> https://stackoverflow.com/questions/26296091/idbasyncenumerable-not-implemented ==> use 
global using Microsoft.EntityFrameworkCore.ChangeTracking;
