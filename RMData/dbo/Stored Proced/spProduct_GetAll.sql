CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
BEGIN
    SET NOCOUNT ON; 
	SELECT *
    FROM dbo.Product
	ORDER BY ProductName;
END