﻿CREATE PROCEDURE [dbo].[BugNet_IssueWorkReport_GetIssueWorkReportsByIssueId]
	@IssueId INT
AS
SELECT      
	IssueWorkReportId,
	BugNet_IssueWorkReports.IssueId,
	WorkDate,
	Duration,
	BugNet_IssueWorkReports.IssueCommentId,
	BugNet_IssueWorkReports.UserId CreatorUserId, 
	U.UserName CreatorUserName,
	IsNull(DisplayName,'') CreatorDisplayName,
    ISNULL(BugNet_IssueComments.Comment, '') Comment
FROM         
	BugNet_IssueWorkReports
	INNER JOIN AspNetUsers U ON BugNet_IssueWorkReports.UserId = U.Id
	LEFT OUTER JOIN BugNet_IssueComments ON BugNet_IssueComments.IssueCommentId =  BugNet_IssueWorkReports.IssueCommentId
WHERE
	 BugNet_IssueWorkReports.IssueId = @IssueId
ORDER BY WorkDate DESC
