IF OBJECT_ID('dbo.GetFriendsRequest', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetFriendsRequest;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetFriendsRequest]
	@userId int

AS

BEGIN
	Select u.UserId, u.ProfileName,u.ProfilePicture, u.UserName,u.IsDeleted
	FROM dbo.Friends f WITH (NOLOCK)
	INNER JOIN dbo.Users u ON f.SenderId = u.UserId AND u.IsDeleted = 0 
	Where f.ReceiverId = @userId AND f.Status = 1 
END
GO
