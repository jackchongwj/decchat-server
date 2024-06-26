IF OBJECT_ID('dbo.EditMessage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.EditMessage;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EditMessage]
(
    @MessageId INT,
	@Content nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    UPDATE Messages WITH (ROWLOCK)
    SET Content = @Content
    WHERE MessageId = @MessageId
END
GO
