/**************************************************************************
-- -SqlDataProvider                    
-- -Date/Time: Aug 26, 2010 		
**************************************************************************/
SET NOEXEC OFF
SET ANSI_WARNINGS ON
SET XACT_ABORT ON
SET IMPLICIT_TRANSACTIONS OFF
SET ARITHABORT ON
SET QUOTED_IDENTIFIER ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
GO

BEGIN TRAN
GO

ALTER TABLE [dbo].[BugNet_UserProjects]
ADD   [SelectedIssueColumns] nvarchar(255) NULL  
DEFAULT(0)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[BugNet_GetProjectSelectedColumnsWithUserIdAndProjectId]
	@Username	nvarchar(255),
 	@ProjectId	int,
 	@ReturnValue nvarchar(255) OUT
AS
DECLARE 
	@UserId UNIQUEIDENTIFIER	
SELECT @UserId = UserId FROM aspnet_users WHERE Username = @Username
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	SET @ReturnValue = (SELECT [SelectedIssueColumns] FROM BugNet_UserProjects WHERE UserId = @UserId AND ProjectId = @ProjectId);
	
END
GO

CREATE PROCEDURE [dbo].[BugNet_SetProjectSelectedColumnsWithUserIdAndProjectId]
	@Username	nvarchar(255),
 	@ProjectId	int,
 	@Columns nvarchar(255)
AS
DECLARE 
	@UserId UNIQUEIDENTIFIER	
SELECT @UserId = UserId FROM aspnet_users WHERE Username = @Username
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE BugNet_UserProjects
	SET [SelectedIssueColumns] = @Columns 
	WHERE UserId = @UserId AND ProjectId = @ProjectId;
	
END
GO

UPDATE BugNet_HostSettings SET SettingValue = '2'  WHERE SettingName = 'SMTPEmailFormat'
GO

COMMIT

SET NOEXEC OFF
GO