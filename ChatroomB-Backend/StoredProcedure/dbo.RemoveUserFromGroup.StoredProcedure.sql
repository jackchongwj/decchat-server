IF OBJECT_ID('dbo.RemoveUserFromGroup', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RemoveUserFromGroup;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RemoveUserFromGroup]
(
    @ChatRoomID INT,
    @UserID INT,
	@Result INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Delete the user from the group
    UPDATE dbo.UserChatRooms WITH (ROWLOCK)
	SET  IsDeleted = 1
    WHERE ChatRoomId = @ChatRoomID AND UserId = @UserID;

	SET @Result = 1
END;
GO
