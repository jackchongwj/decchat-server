IF OBJECT_ID('dbo.VerifyUserCredentials', 'P') IS NOT NULL
    DROP PROCEDURE dbo.VerifyUserCredentials;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[VerifyUserCredentials]
    @UserName NVARCHAR(255),
    @HashedPassword NVARCHAR(255)
AS
BEGIN
    -- Check if the provided username and hashed password match the records in the Users table
    IF EXISTS (SELECT 1 FROM Users WITH (NOLOCK) WHERE UserName = @UserName AND HashedPassword = @HashedPassword)
        SELECT 1 AS CredentialsVerified; -- Return 1 if the credentials are verified
    ELSE
        SELECT 0 AS CredentialsVerified; -- Return 0 if the credentials are not verified
END;
GO
