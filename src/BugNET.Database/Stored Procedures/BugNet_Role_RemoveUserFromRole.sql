﻿CREATE PROCEDURE [dbo].[BugNet_Role_RemoveUserFromRole]
	@UserName	nvarchar(256),
	@RoleId		Int 
AS
DECLARE @UserId UNIQUEIDENTIFIER
SELECT	@UserId = Id FROM AspNetUsers WHERE UserName = @UserName

DELETE BugNet_UserRoles WHERE UserId = @UserId AND RoleId = @RoleId
