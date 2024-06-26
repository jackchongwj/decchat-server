IF OBJECT_ID('dbo.ChangePassword', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ChangePassword;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ChangePassword]
    @UserId INT,
    @NewHashedPassword VARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE Users WITH (ROWLOCK)
    SET HashedPassword = @NewHashedPassword
    WHERE UserId = @UserId;
END

GO
