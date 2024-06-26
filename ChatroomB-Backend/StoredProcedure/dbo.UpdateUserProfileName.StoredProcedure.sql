IF OBJECT_ID('dbo.UpdateUserProfileName', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateUserProfileName;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateUserProfileName]
    @UserId INT,
    @NewProfileName NVARCHAR(15)
AS
BEGIN
    UPDATE Users WITH (ROWLOCK)
    SET ProfileName = @NewProfileName
    WHERE UserId = @UserId;
END
GO
