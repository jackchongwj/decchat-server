IF OBJECT_ID('dbo.GetSaltByUserName', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetSaltByUserName;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSaltByUserName]
    @UserName NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Salt
    FROM dbo.Users WITH(NOLOCK)
    WHERE UserName = @UserName
	AND IsDeleted = 0;
END;
GO
