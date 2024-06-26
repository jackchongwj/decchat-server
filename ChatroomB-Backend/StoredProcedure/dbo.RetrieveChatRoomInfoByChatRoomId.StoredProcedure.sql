IF OBJECT_ID('dbo.RetrieveChatRoomInfoByChatRoomId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RetrieveChatRoomInfoByChatRoomId;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RetrieveChatRoomInfoByChatRoomId]
(
	@ChatRoomId INT,
	@SelectedUsers IntListTableType READONLY 
)
AS
BEGIN

    SET NOCOUNT ON


SELECT 
cr.ChatRoomId, ucr.UserChatRoomId,cr.RoomProfilePic AS ProfilePicture, cr.RoomName AS ChatRoomName,  cr.InitiatedBy, cr.RoomType, ucr.UserId
    FROM ChatRooms cr WITH(NOLOCK)
	JOIN UserChatRooms ucr WITH(NOLOCK) ON cr.ChatRoomId = ucr.ChatRoomId
	LEFT JOIN Users u WITH(NOLOCK) ON ucr.UserId = u.UserId
	WHERE ucr.ChatRoomID = @ChatRoomId AND ucr.IsDeleted = 0 AND ucr.UserId IN (SELECT UserId FROM @SelectedUsers);
END
GO
