IF OBJECT_ID('dbo.RemoveRefreshToken', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RemoveRefreshToken;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RemoveRefreshToken]
    @Token varchar(256),
    @Success BIT OUTPUT -- Add an output parameter to indicate success or failure
AS
BEGIN
    SET NOCOUNT ON;

    -- Update the refresh token to mark it as deleted
    UPDATE RefreshToken WITH (ROWLOCK)
    SET IsDeleted = 1
    WHERE Token = @Token;

    -- Set the output parameter based on the success of the update
    IF @@ROWCOUNT > 0
        SET @Success = 1; -- Deletion successful
    ELSE
        SET @Success = 0; -- Deletion failed
END;
GO
