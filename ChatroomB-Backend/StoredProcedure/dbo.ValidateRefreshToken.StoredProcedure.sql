
IF OBJECT_ID('dbo.ValidateRefreshToken', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ValidateRefreshToken;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ValidateRefreshToken]
    @Token VARCHAR(256),
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @isValid BIT;

    -- Check if the refresh token is valid, not expired, and not deleted for the specified userId
    SELECT @isValid = CASE 
                        WHEN COUNT(*) > 0 THEN 1 
                        ELSE 0 
                      END
    FROM dbo.RefreshToken WITH (NOLOCK)
    WHERE Token = @Token
      AND UserId = @UserId
      AND ExpiredDateTime > GETDATE()
      AND IsDeleted = 0;

    SELECT @isValid AS IsValid;
END
GO
