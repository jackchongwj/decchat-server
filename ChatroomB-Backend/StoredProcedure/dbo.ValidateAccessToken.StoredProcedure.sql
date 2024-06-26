IF OBJECT_ID('dbo.ValidateAccessToken', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ValidateAccessToken;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ValidateAccessToken]
    @UserId INT,
    @UserName VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @isValid BIT;

    -- Check if a user with the specified userId and username exists
    SELECT @isValid = CASE 
                        WHEN COUNT(*) > 0 THEN 1 
                        ELSE 0 
                      END
    FROM dbo.Users WITH (NOLOCK)
    WHERE UserId = @UserId
      AND UserName = @UserName;

    SELECT @isValid AS IsValid;
END
GO
