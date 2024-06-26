IF OBJECT_ID('dbo.CreateGroup', 'P') IS NOT NULL
    DROP PROCEDURE dbo.CreateGroup;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateGroup]
    @RoomName NVARCHAR(255),
	@RoomProfilePic varchar(256),
    @InitiatedBy INT,
    @SelectedUsers IntListTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert the new group
    INSERT INTO ChatRooms (RoomName, RoomProfilePic, InitiatedBy, RoomType,  IsDeleted)
    VALUES (@RoomName,@RoomProfilePic, @InitiatedBy, 1 , 0);

    -- Get the newly inserted ChatRoomId
    DECLARE @ChatRoomId INT;
    SET @ChatRoomId = SCOPE_IDENTITY();

    -- Insert the user-chat relationships for the initiator
    INSERT INTO UserChatRooms (UserId, ChatRoomId, IsDeleted)
    VALUES (@InitiatedBy, @ChatRoomId, 0);

    -- Insert relationships for the selected users
	--insert data from @ to  UCR
    INSERT INTO UserChatRooms (UserId, ChatRoomId, IsDeleted)
    SELECT UserId, @ChatRoomId, 0
    FROM @SelectedUsers;

	SELECT cr.ChatRoomId, cr.RoomProfilePic AS ProfilePicture, cr.RoomName AS ChatRoomName, RoomType, cr.InitiatedBy, u.ProfileName AS InitiatorProfileName, ucr.UserChatRoomId, ucr.UserId
    FROM ChatRooms cr WITH(NOLOCK)
	JOIN UserChatRooms ucr WITH(NOLOCK) ON cr.ChatRoomId = ucr.ChatRoomId
	LEFT JOIN Users u WITH(NOLOCK) ON cr.InitiatedBy = u.UserId
	WHERE ucr.ChatRoomID = @ChatRoomId;

END
GO
