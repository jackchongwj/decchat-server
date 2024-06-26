IF OBJECT_ID('dbo.UpdateRefreshTokenExpiry', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateRefreshTokenExpiry;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateRefreshTokenExpiry]
    @Token varchar(256),
    @ExpiredDateTime DATETIME,
    @Success BIT OUTPUT -- Add an output parameter to indicate success or failure
AS
BEGIN
    SET NOCOUNT ON;

    -- Update the expiry date of the refresh token
    UPDATE dbo.RefreshToken WITH (ROWLOCK)
    SET ExpiredDateTime = @ExpiredDateTime -- Roll forward expiry date by 7 days
	, IsDeleted = 0
    WHERE Token = @Token;

    -- Set the output parameter based on the success of the update
    IF @@ROWCOUNT > 0
        SET @Success = 1; -- Update successful
    ELSE
        SET @Success = 0; -- Update failed
END
GO
