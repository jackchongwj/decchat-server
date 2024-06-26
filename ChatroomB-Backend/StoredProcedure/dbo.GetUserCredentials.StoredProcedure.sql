IF OBJECT_ID('dbo.GetUserCredentials', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetUserCredentials;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserCredentials]
    @UserName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, UserName, HashedPassword, Salt
    FROM Users WITH (NOLOCK)
    WHERE UserName = @UserName;
END;
GO
