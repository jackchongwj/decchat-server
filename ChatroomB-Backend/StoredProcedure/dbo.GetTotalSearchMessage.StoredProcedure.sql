IF OBJECT_ID('dbo.GetTotalSearchMessage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetTotalSearchMessage;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetTotalSearchMessage]
	@ChatRoomId int,
	@SearchValue nvarchar(15),
	@RowCount INT OUTPUT

AS

BEGIN
   SET NOCOUNT ON;

	SELECT @RowCount = Count(*) 
FROM
    dbo.Messages m WITH (NOLOCK)
	JOIN UserChatRooms usr WITH (NOLOCK) ON m.UserChatRoomId = usr.UserChatRoomId

WHERE
    m.Content LIKE '%' + @SearchValue + '%' AND
    m.IsDeleted = 0 AND usr.ChatRoomId = @ChatRoomId;
END
GO
