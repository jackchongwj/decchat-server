IF OBJECT_ID('dbo.UpdateGroupName', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateGroupName;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateGroupName]
    @ChatRoomId INT,
    @NewGroupName NVARCHAR(15)
AS
BEGIN
    UPDATE ChatRooms WITH (ROWLOCK)
    SET RoomName = @NewGroupName
    WHERE ChatRoomId = @ChatRoomId;
END
GO
