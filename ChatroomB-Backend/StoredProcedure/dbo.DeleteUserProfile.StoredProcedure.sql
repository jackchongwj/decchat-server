IF OBJECT_ID('dbo.DeleteUserProfile', 'P') IS NOT NULL
    DROP PROCEDURE dbo.DeleteUserProfile;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteUserProfile]
    @UserId INT
AS
BEGIN
    UPDATE Users WITH (ROWLOCK)
    SET IsDeleted = 1
    WHERE UserId = @UserId;
END
GO
