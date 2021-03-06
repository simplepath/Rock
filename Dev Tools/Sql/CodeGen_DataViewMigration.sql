create table #codeTable (
    Id int identity(1,1) not null,
    CodeText nvarchar(max),
    CONSTRAINT [pk_codeTable] PRIMARY KEY CLUSTERED  ( [Id]) );

DECLARE 
   --TODO: set this to the dataview that you want to create a Migration up/down for
    @dataViewId int = 12;

/* UP */
/* Data Filters for the DataView in the correct order for migrations */
WITH CTE
AS (
    SELECT dv.NAME [DataView.Name]
        ,dv.[Guid] [DataView.Guid]
        ,dv.[Id] [DataView.Id]
        ,dvf.*
    FROM [DataViewFilter] [dvf]
    JOIN [DataView] dv ON dv.DataViewFilterId = dvf.Id
    
    UNION ALL
    
    SELECT pcte.[DataView.Name]
        ,pcte.[DataView.Guid]
        ,pcte.[DataView.Id]
        ,[a].*
    FROM [DataViewFilter] [a]
    INNER JOIN CTE pcte ON pcte.Id = [a].[ParentId]
    )
insert into #codeTable 
SELECT CONCAT (
        '// Create '
        ,isnull(et.NAME, CASE dvf.ExpressionType
                WHEN 1
                    THEN '[GroupAll]'
                WHEN 2
                    THEN '[GroupAny]'
                WHEN 3
                    THEN '[GroupAllFalse]'
                WHEN 4
                    THEN '[GroupAnyFalse]'
                ELSE convert(NVARCHAR, dvf.ExpressionType)
                END)
        ,' DataViewFilter for DataView: '
        ,dvf.[DataView.Name]
        ,case when isnull(dvf.[Selection], '') != '' then concat('
/* NOTE to Developer. Review that the generated DataViewFilter.Selection ''', dvf.[Selection]  ,''' for ' ,et.NAME ,' will work on different databases */') else null end
        ,'
Sql( @"
IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '''
        ,dvf.[Guid]
        ,''') BEGIN    
    DECLARE
        @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '''
        ,isnull(pdvf.[Guid], '00000000-0000-0000-0000-000000000000')
        ,'''),
        @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '''
        ,isnull(et.[Guid], '00000000-0000-0000-0000-000000000000')
        ,''')

    INSERT INTO [DataViewFilter] (ExpressionType, ParentId, EntityTypeId, Selection, [Guid]) 
    values ('
        ,dvf.ExpressionType
        ,',@ParentDataViewFilterId,@DataViewFilterEntityTypeId,'''
        ,replace(dvf.[Selection], '"', '""')
        ,''','''
        ,[dvf].[Guid]
        ,''')'
        ,'
END
");'
        ) [DataviewFilter_UpMigration]
FROM CTE [dvf]
LEFT JOIN [EntityType] [et] ON dvf.EntityTypeId = et.Id
LEFT JOIN [DataViewFilter] [pdvf] ON dvf.ParentId = pdvf.Id
WHERE dvf.[DataView.Id] = @dataViewId
ORDER BY [DataView.Id] DESC


/* Data View*/
insert into #codeTable 
SELECT CONCAT (
        '// Create DataView: ' + dv.NAME
        ,'
Sql( @"
IF NOT EXISTS (SELECT * FROM DataView where [Guid] = '''
        ,dv.[Guid]
        ,''') BEGIN
DECLARE
    @categoryId int = (select top 1 [Id] from [Category] where [Guid] = '''
        ,isnull(c.[Guid], '00000000-0000-0000-0000-000000000000')
        ,'''),
    @entityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '''
        ,isnull(et.[Guid], '00000000-0000-0000-0000-000000000000')
        ,'''),
    @dataViewFilterId  int = (select top 1 [Id] from [DataViewFilter] where [Guid] = '''
        ,isnull(dvf.[Guid], '00000000-0000-0000-0000-000000000000')
        ,'''),
    @transformEntityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '''
        ,isnull(tet.[Guid], '00000000-0000-0000-0000-000000000000')
        ,''')

INSERT INTO [DataView] ([IsSystem], [Name], [Description], [CategoryId], [EntityTypeId], [DataViewFilterId], [TransformEntityTypeId], [Guid])
VALUES(0,'''
        ,dv.NAME
        ,''','''
        ,dv.[Description]
        ,''',@categoryId,@entityTypeId,@dataViewFilterId,@transformEntityTypeId,'''
        ,dv.[Guid]
        ,''')'
        ,'
END
");'
        ) [Dataview_UpMigration]
FROM [DataView] [dv]
JOIN [Category] [c] ON dv.CategoryId = c.Id
JOIN [EntityType] [et] ON dv.EntityTypeId = et.Id
LEFT JOIN [EntityType] [tet] ON dv.TransformEntityTypeId = tet.Id
JOIN [DataViewFilter] [dvf] ON dv.DataViewFilterId = dvf.Id
WHERE dv.Id = @dataViewId
ORDER BY dv.Id DESC;

select CodeText [Up] from #codeTable 

delete from #codeTable;

/* Down for DataView  */
INSERT INTO #codeTable
SELECT CONCAT (
        '// Delete DataView: '
        ,dv.[Name]
        ,'
Sql( @"DELETE FROM DataView where [Guid] = '''
        ,dv.[Guid]
        ,'''");'
        ) [Dataview_Down_Migration]
FROM [DataView] dv
WHERE dv.Id = @dataViewId
ORDER BY [Id] DESC;

/* Down for DataFilters (in Reverse Order) */
WITH CTE
AS (
    SELECT dv.NAME [DataView.Name]
        ,dv.[Guid] [DataView.Guid]
        ,dv.[Id] [DataView.Id]
        ,dvf.*
    FROM [DataViewFilter] [dvf]
    JOIN [DataView] dv ON dv.DataViewFilterId = dvf.Id
    
    UNION ALL
    
    SELECT pcte.[DataView.Name]
        ,pcte.[DataView.Guid]
        ,pcte.[DataView.Id]
        ,[a].*
    FROM [DataViewFilter] [a]
    INNER JOIN CTE pcte ON pcte.Id = [a].[ParentId]
    )
INSERT INTO #codeTable
SELECT CONCAT (
        '// Delete DataViewFilter for DataView: '
        ,dvf.[DataView.Name]
        ,'
Sql( @"DELETE FROM DataViewFilter where [Guid] = '''
        ,dvf.[Guid]
        ,'''");'
        ) [DataviewFilter_Down_Migration]
FROM CTE [dvf]
LEFT JOIN [DataViewFilter] [pdvf] ON dvf.ParentId = pdvf.Id
WHERE dvf.[DataView.Id] = @dataViewId
ORDER BY [DataView.Id] DESC
    ,dvf.Id DESC



select CodeText [Down] from #codeTable 

IF OBJECT_ID('tempdb..#codeTable') IS NOT NULL
    DROP TABLE #codeTable
