CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int

AS
BEGIN
    set nocount on; --this return no row count when data comes back

	SELECT *
	FROM dbo.Product
	WHERE Id = @Id;
END