CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int output,
	@CashierId nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS
BEGIN
    set nocount on;

	insert into dbo.Sale(CashierId, SaleDate, SubTotal, Tax, Total)
	values (@CashierId,@SaleDate, @SubTotal, @Tax, @Total);

	SELECT @Id = Scope_Identity(); --THIS WILL GRAB THE LAST ID THAT HAS BEEN CREATED INSIDE THIS TRANSACTION AND PUTS IT INTO OUR ID VALUE VIA OUTPUT VAR INTO THE METHOD THAT CALLS THIS(ACCEDINTLY PUT IT AS CAP CANT BOTHERED TO RETYPE)
	--Scope = this procedure
END
