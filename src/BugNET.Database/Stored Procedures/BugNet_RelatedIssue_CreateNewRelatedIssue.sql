﻿CREATE PROCEDURE [dbo].[BugNet_RelatedIssue_CreateNewRelatedIssue] 
	@PrimaryIssueId Int,
	@SecondaryIssueId Int,
	@RelationType Int
AS
IF NOT EXISTS(SELECT PrimaryIssueId FROM BugNet_RelatedIssues WHERE (PrimaryIssueId = @PrimaryIssueId OR PrimaryIssueId = @SecondaryIssueId) AND (SecondaryIssueId = @SecondaryIssueId OR SecondaryIssueId = @PrimaryIssueId) AND RelationType = @RelationType)
BEGIN
	INSERT BugNet_RelatedIssues
	(
		PrimaryIssueId,
		SecondaryIssueId,
		RelationType
	)
	VALUES
	(
		@SecondaryIssueId,
		@PrimaryIssueId,
		@RelationType
	)
END
