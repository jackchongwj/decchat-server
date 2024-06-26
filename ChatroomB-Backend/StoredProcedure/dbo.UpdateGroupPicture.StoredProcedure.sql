IF OBJECT_ID('dbo.UpdateGroupPicture', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateGroupPicture;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateGroupPicture]
    @ChatRoomId INT,
    @NewGroupPicture VARCHAR(256)
AS
BEGIN
    UPDATE ChatRooms WITH (ROWLOCK)
    SET RoomProfilePic = @NewGroupPicture
    WHERE ChatRoomId = @ChatRoomId;
END
GO
