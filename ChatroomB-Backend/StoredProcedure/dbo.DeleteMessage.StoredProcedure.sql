IF OBJECT_ID('dbo.DeleteMessage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.DeleteMessage;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[DeleteMessage]
(
    @MessageId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	UPDATE Messages WITH (ROWLOCK)
		SET IsDeleted = 1
		WHERE MessageId = @MessageId
END
GO
