IF OBJECT_ID('dbo.AddUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AddUser;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddUser]
    @UserName varchar(15),
    @HashedPassword varchar(256),
    @Salt varchar(256),
	@ProfileName nvarchar(15),
	@ProfilePicture varchar(256)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users (UserName, HashedPassword, Salt, ProfileName, ProfilePicture, IsDeleted)
    VALUES (@UserName, @HashedPassword, @Salt, @ProfileName, @ProfilePicture, 0);
END;
GO
