IF OBJECT_ID('dbo.RetrieveChatRoomListById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RetrieveChatRoomListById;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[RetrieveChatRoomListById]
(
	@UserId INT
)
AS
BEGIN

    SET NOCOUNT ON

	SELECT 
		cr.ChatRoomID,
		ucr.UserChatRoomId,
		COALESCE(u.ProfileName, cr.RoomName) AS ChatRoomName,
		COALESCE(u.ProfilePicture, cr.RoomProfilePic) AS ProfilePicture,
		COALESCE(u.UserId, ucr.UserID) AS UserId,

		cr.RoomType,
		initiator.ProfileName AS InitiatorProfileName,
		cr.InitiatedBy
	FROM 
		ChatRooms cr
	INNER JOIN 
		UserChatRooms ucr ON cr.ChatRoomID = ucr.ChatRoomID
	INNER JOIN 
		Users initiator ON cr.InitiatedBy = initiator.UserID
	LEFT JOIN 
		Users u ON cr.RoomType = 0 AND u.UserID = (
			SELECT TOP 1 u2.UserID
			FROM Users u2
			INNER JOIN UserChatRooms ucr2 ON u2.UserID = ucr2.UserID
			WHERE ucr2.ChatRoomID = cr.ChatRoomID AND u2.UserID != @UserId
		)
	WHERE 
		ucr.UserID = @UserId AND ucr.IsDeleted = 0
END
GO
