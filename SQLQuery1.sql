DECLARE @__userId_0 NVARCHAR(450) = '5802d573-80bf-4c1a-a2e1-3217c64563c9';
DECLARE @__criterion_FieldName_1 NVARCHAR(4000) = 'Company Name';
DECLARE @__Format_3 NVARCHAR(4000) = '%hand%'; -- Assuming this is a LIKE pattern
DECLARE @__startDate_4 DATETIME2 = '2024-04-01'; -- Use the appropriate date
-- Repeat declarations for other parameters as necessary...

SELECT [f].[FormId], [f].[CreatedDate], [f].[Description], [f].[FormName], [f].[UpdatedDate], [f].[UserId], 
       [f3].[FieldId], [f3].[FieldName], [f3].[FieldOptions], [f3].[FieldType], [f3].[FieldValue], [f3].[FormId], [f3].[Required]
FROM [Forms] AS [f]
JOIN [FormFields] AS [f3] ON [f].[FormId] = [f3].[FormId]
WHERE [f].[UserId] = @__userId_0 
AND EXISTS (
    SELECT 1
    FROM [FormFields] AS [f0]
    WHERE [f].[FormId] = [f0].[FormId] AND [f0].[FieldName] = @__criterion_FieldName_1 AND [f0].[FieldValue] LIKE @__Format_3
) 
AND [f].[CreatedDate] >= @__startDate_4 

ORDER BY [f].[FormId];
