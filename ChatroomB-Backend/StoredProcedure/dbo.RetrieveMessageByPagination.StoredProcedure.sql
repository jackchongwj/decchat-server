IF OBJECT_ID('dbo.RetrieveMessageByPagination', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RetrieveMessageByPagination;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RetrieveMessageByPagination]
	@ChatRoomId INT,
    @MessageId  INT
AS

BEGIN
		WITH SortedMessages AS (
			SELECT *,
				   ROW_NUMBER() OVER (ORDER BY TimeStamp DESC, MessageId DESC) AS rn
			FROM Messages
		)
		SELECT TOP 30
			m.MessageId, m.Content, m.UserChatRoomId, m.TimeStamp, m.ResourceUrl, m.IsDeleted, m.rn,
			usr.UserId, u.ProfileName, u.ProfilePicture, usr.ChatRoomId
		FROM
			SortedMessages m
		JOIN
			UserChatRooms usr WITH (NOLOCK) ON m.UserChatRoomId = usr.UserChatRoomId
		JOIN
			Users u WITH (NOLOCK) ON usr.UserId = u.UserId
		WHERE
			usr.ChatRoomId = @ChatRoomId AND m.IsDeleted = 0
			AND m.MessageId <= CASE WHEN @messageId = 0 THEN (SELECT TOP 1 MessageId FROM SortedMessages ORDER BY TimeStamp DESC, MessageId DESC)
								ELSE @messageId
						   END
		ORDER BY
			m.TimeStamp DESC, m.MessageId DESC;
END
GO
