IF OBJECT_ID('dbo.AddFriend', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AddFriend;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddFriend]
@SenderId int,
@ReceiverId int

AS

BEGIN
    SET NOCOUNT ON

	INSERT INTO dbo.Friends
		(
			SenderId,
			ReceiverId,
			Status
		)
    VALUES
		(
			@SenderId,
			@ReceiverId,
			1
		)


	Select u.UserId, u.ProfileName,u.ProfilePicture, u.UserName,u.IsDeleted
	FROM dbo.Friends f WITH (NOLOCK)
	INNER JOIN dbo.Users u ON f.SenderId = u.UserId AND u.IsDeleted = 0 
	Where f.ReceiverId = @ReceiverId AND f.Status = 1 
END
GO
