CREATE PROCEDURE [dbo].[AdminUsersAddorEdit]
@mode nvarchar(10),
@Id int,
@username nvarchar(50),
@password nvarchar(50),
@admin int,
@name nvarchar(50)

AS
	if @mode = 'Add'
	BEGIN
	INSERT INTO Users
	(username,password,admin,name)
	VALUES (@username,@password,@admin,@name)
	END


