IF OBJECT_ID('dbo.UpdateFriendRequest', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateFriendRequest;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateFriendRequest]
    @senderId INT,
	@receiverId INT,
    @status INT 

AS

BEGIN
    UPDATE dbo.Friends WITH (ROWLOCK)
    SET Status = @status
    WHERE SenderId = @senderId AND ReceiverId = @receiverId AND Status <> 4 AND Status <> 3;
END
GO
