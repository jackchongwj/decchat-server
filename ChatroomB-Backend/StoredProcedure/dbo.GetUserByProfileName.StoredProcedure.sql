IF OBJECT_ID('dbo.GetUserByProfileName', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetUserByProfileName;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserByProfileName]
	@profileName nvarchar(15),
	@userId int

AS

BEGIN
	SELECT
    u.UserId,
    u.UserName,
    u.ProfileName,
    u.ProfilePicture,
    f.Status
FROM
    dbo.Users u WITH (NOLOCK)
left JOIN
    Friends f ON (f.ReceiverId = u.UserId AND f.SenderId = @userId AND (f.Status = 1 OR f.Status = 2))
                OR (f.SenderId = u.UserId AND f.ReceiverId = @userId AND (f.Status = 1 OR f.Status = 2))
WHERE
    u.ProfileName LIKE '%' + @profileName + '%' AND
    u.IsDeleted = 0 AND U.UserId <> @userId;
END
GO
