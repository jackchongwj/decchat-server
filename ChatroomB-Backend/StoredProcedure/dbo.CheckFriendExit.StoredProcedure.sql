IF OBJECT_ID('dbo.CheckFriendExit', 'P') IS NOT NULL
    DROP PROCEDURE dbo.CheckFriendExit;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [dbo].[RetrieveChatRoomListById]    Script Date: 2/8/2024 11:06:47 AM ******/
CREATE PROCEDURE [dbo].[CheckFriendExit]
	@SenderId INT,
	@ReceiverId INT,
	@Result INT OUTPUT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    SET NOCOUNT ON

    IF EXISTS (
        SELECT 1
        FROM dbo.Friends m WITH (NOLOCK)
        WHERE
            (@SenderId = m.SenderId AND @ReceiverId = m.ReceiverId AND (m.Status <> 4 AND m.Status <> 3))
            OR
            (@SenderId = m.ReceiverId AND @ReceiverId = m.SenderId AND (m.Status <> 4 AND m.Status <> 3))
    )
    BEGIN
        SET @Result = 1
    END
    ELSE
    BEGIN
        SET @Result = 0
    END

END
GO
