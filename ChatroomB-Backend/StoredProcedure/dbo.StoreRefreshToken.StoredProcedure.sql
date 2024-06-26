IF OBJECT_ID('dbo.StoreRefreshToken', 'P') IS NOT NULL
    DROP PROCEDURE dbo.StoreRefreshToken;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[StoreRefreshToken]
    @UserId INT,
    @Token VARCHAR(256),
    @ExpiredDateTime DATETIME,
    @Success BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
	
    BEGIN TRY
        MERGE INTO dbo.RefreshToken AS target
        USING (VALUES (@UserId)) AS source (UserId)
        ON target.UserId = source.UserId
        WHEN MATCHED THEN
            UPDATE SET Token = @Token, ExpiredDateTime = @ExpiredDateTime, IsDeleted = 0
        WHEN NOT MATCHED THEN
            INSERT (TokenId, UserId, Token, ExpiredDateTime, IsDeleted)
            VALUES (NEWID(), @UserId, @Token, @ExpiredDateTime, 0);

        SET @Success = 1; -- Indicate success
    END TRY
    BEGIN CATCH
        SET @Success = 0; -- Indicate failure
    END CATCH;
END;
GO
