IF OBJECT_ID('dbo.CreateChatRoomAndUserChatRoomWithPrivate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.CreateChatRoomAndUserChatRoomWithPrivate;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateChatRoomAndUserChatRoomWithPrivate]
@RoomName nvarchar(50),
@RoomType bit,
@RoomProfilePic varchar(256),
@SenderId int,
@ReceiverId int,
@UserId int

AS

BEGIN
	DECLARE @ChatRoomId int
	INSERT INTO dbo.ChatRooms
		(
			RoomName,
			RoomType,
			RoomProfilePic,
			InitiatedBy,
			IsDeleted
		)
    VALUES
		(
			@RoomName,
			@RoomType,
			@RoomProfilePic,
			@SenderId,
			0
		)
		
		SET @ChatRoomId = SCOPE_IDENTITY();

	INSERT INTO dbo.UserChatRooms
		(
			UserId,
			ChatRoomId,
			IsDeleted
		)
    VALUES
		(
			@SenderId,
			@ChatRoomId,
			0
		),
		(
			@ReceiverId,
			@ChatRoomId,
			0
		);

		
	SELECT
		UCR1.[UserChatRoomId] AS UserChatRoomId,
		CR.[ChatRoomId],
		U2.ProfilePicture AS ProfilePicture,
		U2.ProfileName AS ChatRoomName,
		CR.[RoomType],
		U2.UserId AS UserId
	FROM
		dbo.UserChatRooms UCR1 WITH (NOLOCK)
	INNER JOIN
		dbo.ChatRooms CR WITH (NOLOCK) ON UCR1.ChatRoomId = CR.ChatRoomId
	LEFT JOIN
		dbo.Users U1 WITH (NOLOCK) ON UCR1.UserId = U1.UserId
	LEFT JOIN
		dbo.UserChatRooms UCR2 WITH (NOLOCK) ON UCR1.ChatRoomId = UCR2.ChatRoomId AND UCR1.UserId != UCR2.UserId
	LEFT JOIN
		dbo.Users U2 WITH (NOLOCK) ON UCR2.UserId = U2.UserId
	WHERE
		UCR1.ChatRoomId = @ChatRoomId
		AND UCR1.IsDeleted = 0
		AND UCR2.IsDeleted = 0;
END
GO
