IF OBJECT_ID('dbo.DeleteUserFromGroup', 'P') IS NOT NULL
    DROP PROCEDURE dbo.DeleteUserFromGroup;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteUserFromGroup]
(
    @ChatRoomID INT,
    @UserID INT,
	@Result INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Delete the user from the group
    UPDATE dbo.UserChatRooms 
	SET  IsDeleted = 1
    WHERE ChatRoomId = @ChatRoomID AND UserId = @UserID;

	SET @Result = 1
END;
GO
