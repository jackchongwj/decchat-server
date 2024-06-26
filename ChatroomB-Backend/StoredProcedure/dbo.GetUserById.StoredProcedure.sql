IF OBJECT_ID('dbo.GetUserById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetUserById;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserById]
    @UserId INT
AS
BEGIN
    SELECT UserName, ProfileName, ProfilePicture
    FROM Users WITH (NOLOCK)
    WHERE UserId = @UserId AND IsDeleted = 0;
END
GO
