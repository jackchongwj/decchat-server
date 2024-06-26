IF OBJECT_ID('dbo.UpdateUserProfilePicture', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateUserProfilePicture;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateUserProfilePicture]
    @UserId INT,
    @NewProfilePicture VARCHAR(256)
AS
BEGIN
    UPDATE Users WITH (ROWLOCK)
    SET ProfilePicture = @NewProfilePicture
    WHERE UserId = @UserId;
END
GO
