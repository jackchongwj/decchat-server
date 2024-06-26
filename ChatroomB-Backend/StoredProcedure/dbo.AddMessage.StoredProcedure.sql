IF OBJECT_ID('dbo.AddMessage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AddMessage;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddMessage]
	@Content nvarchar(200),
	@UserChatRoomId int,
	@TimeStamp DateTime,
	@ResourceUrl nvarchar(256),
	@IsDeleted bit
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @MessageId int
	INSERT INTO dbo.Messages
	(
		Content,
		UserChatRoomId,
		TimeStamp,
		ResourceUrl,
		IsDeleted
	)
	VALUES
	(
		@Content,
		@UserChatRoomId,
		@TimeStamp,
		@ResourceUrl,
		@IsDeleted
	)

	SET @MessageId = SCOPE_IDENTITY();

	SELECT
		m.MessageId,
		m.Content, 
		m.UserChatRoomId,
		m.ResourceUrl,
		m.TimeStamp,
		m.IsDeleted,
		ucr.ChatRoomId,
		u.ProfileName,
		u.ProfilePicture,
		u.UserId
	FROM 
		dbo.Messages m WITH (NOLOCK)
		INNER JOIN dbo.UserChatRooms ucr WITH (NOLOCK) on ucr.UserChatRoomId = m.UserChatRoomId
		INNER JOIN dbo.Users u WITH (NOLOCK) on u.UserId = ucr.UserId
	WHERE 
		m.MessageId = @MessageId 
END
GO
