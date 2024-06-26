IF OBJECT_ID('dbo.AddMembersToGroup', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AddMembersToGroup;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddMembersToGroup]
    @ChatRoomId INT,
    @SelectedUsers IntListTableType READONLY --ensure datakenot bemodified
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert relationships for the selected users
    INSERT INTO UserChatRooms (UserId, ChatRoomId, IsDeleted)
    SELECT UserId, @ChatRoomId, 0
    FROM @SelectedUsers;

	SELECT cr.ChatRoomId, ucr.UserChatRoomId, ucr.UserId, u.ProfilePicture AS ProfilePicture, u.ProfileName
    FROM ChatRooms cr WITH(NOLOCK)
	JOIN UserChatRooms ucr WITH(NOLOCK) ON cr.ChatRoomId = ucr.ChatRoomId
	LEFT JOIN Users u WITH(NOLOCK) ON ucr.UserId = u.UserId
	WHERE ucr.ChatRoomID = @ChatRoomId AND ucr.IsDeleted = 0 AND ucr.UserId IN (SELECT UserId FROM @SelectedUsers) ;

END
GO
