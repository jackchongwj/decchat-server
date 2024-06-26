IF OBJECT_ID('dbo.DeleteFriend', 'P') IS NOT NULL
    DROP PROCEDURE dbo.DeleteFriend;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteFriend]
    @ChatRoomId INT,
	@User1 INT,
	@User2 INT,
	@result INT OUTPUT


AS

BEGIN
	SET @result = 0;
	/* doing try catch to ensure data can update success*/
	BEGIN TRY
		BEGIN TRANSACTION;


			UPDATE dbo.UserChatRooms WITH (ROWLOCK)
			SET IsDeleted = 1
			WHERE ChatRoomId = @ChatRoomId

			UPDATE dbo.ChatRooms WITH (ROWLOCK)
			SET IsDeleted = 1
			WHERE ChatRoomId = @ChatRoomId


			/*select userchatroom id for delete meesage*/
			DECLARE @UserChatRoom1 INT;
			DECLARE @UserChatRoom2 INT;
			SELECT @UserChatRoom1 = UserChatRoomId  FROM dbo.UserChatRooms WITH (NOLOCK) WHERE UserId = @User1 AND ChatRoomId = @ChatRoomId
			SELECT @UserChatRoom1 = UserChatRoomId FROM dbo.UserChatRooms WITH (NOLOCK) WHERE UserId = @User2 AND ChatRoomId = @ChatRoomId

			UPDATE dbo.Messages WITH (ROWLOCK)
			SET IsDeleted = 1
			WHERE UserChatRoomId = @UserChatRoom1

			UPDATE dbo.Messages WITH (ROWLOCK)
			SET IsDeleted = 1
			WHERE UserChatRoomId = @UserChatRoom2

			/*declare sid for delete friend */
			DECLARE @SId int;
			SELECT @SId = InitiatedBy
			FROM ChatRooms  WITH (NOLOCK)
			WHERE ChatRoomId = @ChatRoomId

			IF (@SId = @User1) 
			BEGIN
				UPDATE dbo.Friends WITH (ROWLOCK)
				SET Status = 4
				WHERE SenderId = @SId AND ReceiverId = @User2;
			END
			ELSE IF (@SId = @User2)

			BEGIN
				UPDATE dbo.Friends WITH (ROWLOCK)
				SET Status = 4
				WHERE SenderId = @SId AND ReceiverId = @User1;
			END 
			COMMIT; /*commit means is this all update function will update successful then the db will update its*/

			/* set  result to 1 status*/
			SET @result = 1;
	END TRY

	BEGIN CATCH
		ROLLBACK; /*ROLLBACK means is return that all update data before updated*/
		SET @result = 0;
	END CATCH
END
GO
