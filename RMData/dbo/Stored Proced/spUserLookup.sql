CREATE PROCEDURE [dbo].[spUserLookup]
	@Id nvarchar(128) --this is the agrs we pass in

AS
begin
    set nocount on;

	SELECT Id, FirstName, LastName, EmailAddress, CreatedDate
	from [dbo].[User]
	WHERE id = @Id
end