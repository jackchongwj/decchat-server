IF OBJECT_ID('dbo.QuitGroup', 'P') IS NOT NULL
    DROP PROCEDURE dbo.QuitGroup;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QuitGroup]
(
    @ChatRoomID INT,
    @UserID INT,
	@Result INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
	SET @Result = 0;

    -- Delete the user from the group
      UPDATE dbo.UserChatRooms WITH (ROWLOCK)
	SET  IsDeleted = 1
    WHERE ChatRoomId = @ChatRoomID AND UserId = @UserID;

	Declare @initialBy INT
		SELECT  @initialBy = cr.InitiatedBy 
		FROM dbo.ChatRooms cr WITH (NOLOCK) 
		WHERE cr.ChatRoomId = @ChatRoomID
	
	if(@initialBy = @UserID)
		BEGIN
			DECLARE @newUser INT
				SELECT TOP 1 @newUser = ucr.UserId 
				FROM dbo.UserChatRooms ucr WITH (NOLOCK) 
				WHERE ucr.ChatRoomId = @ChatRoomID AND IsDeleted = 0
				
				IF (@@ROWCOUNT > 0)
					BEGIN
						UPDATE dbo.ChatRooms WITH (ROWLOCK)
						SET InitiatedBy = @newUser
						WHERE ChatRoomId = @ChatRoomID
						SET @Result = @newUser
					END
				ELSE
					BEGIN
						UPDATE dbo.ChatRooms WITH (ROWLOCK)
						SET IsDeleted = 1
						WHERE ChatRoomId = @ChatRoomID
						SET @Result = 0
					END

					--SET @Result = 1
					
		END

		ELSE 
			BEGIN
				SET @Result = 0
			END
END;
GO
