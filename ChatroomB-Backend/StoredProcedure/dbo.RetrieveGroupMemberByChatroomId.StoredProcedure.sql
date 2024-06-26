IF OBJECT_ID('dbo.RetrieveGroupMemberByChatroomId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RetrieveGroupMemberByChatroomId;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[RetrieveGroupMemberByChatroomId]
(
	@ChatRoomID INT,
	@UserId INT
)
AS
BEGIN

    SET NOCOUNT ON

    SELECT ucr.ChatRoomId, ucr.UserId, u.ProfileName, u.ProfilePicture 
	FROM dbo.UserChatRooms ucr WITH (NOLOCK)
	INNER JOIN dbo.Users u ON u.UserId = ucr.UserId 
	WHERE ucr.ChatRoomId = @ChatRoomID AND ucr.IsDeleted = 0 
END
GO
