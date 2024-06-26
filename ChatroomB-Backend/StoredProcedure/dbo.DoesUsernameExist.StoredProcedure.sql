IF OBJECT_ID('dbo.DoesUsernameExist', 'P') IS NOT NULL
    DROP PROCEDURE dbo.DoesUsernameExist;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DoesUsernameExist]
    @UserName varchar(15)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsExist BIT;

    -- Check if the username is unique
    SELECT @IsExist = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
    FROM dbo.Users WITH (NOLOCK)
    WHERE UserName = @UserName

    SELECT @IsExist AS IsExist;
END;
GO
