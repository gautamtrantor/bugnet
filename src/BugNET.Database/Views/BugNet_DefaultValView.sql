﻿
CREATE VIEW [dbo].[BugNet_DefaultValView]
AS
SELECT     dbo.BugNet_DefaultValues.DefaultType, dbo.BugNet_DefaultValues.StatusId, dbo.BugNet_DefaultValues.IssueOwnerUserId, 
                      dbo.BugNet_DefaultValues.IssuePriorityId, dbo.BugNet_DefaultValues.IssueAffectedMilestoneId, dbo.BugNet_DefaultValues.ProjectId, 
                      ISNULL(OwnerUsers.UserName, N'none') AS OwnerUserName, ISNULL(OwnerUsersProfile.DisplayName, N'none') AS OwnerDisplayName, 
                      ISNULL(AssignedUsers.UserName, N'none') AS AssignedUserName, ISNULL(AssignedUsersProfile.DisplayName, N'none') AS AssignedDisplayName, 
                      dbo.BugNet_DefaultValues.IssueAssignedUserId, dbo.BugNet_DefaultValues.IssueCategoryId, dbo.BugNet_DefaultValues.IssueVisibility, 
                      dbo.BugNet_DefaultValues.IssueDueDate, dbo.BugNet_DefaultValues.IssueProgress, dbo.BugNet_DefaultValues.IssueMilestoneId, 
                      dbo.BugNet_DefaultValues.IssueEstimation, dbo.BugNet_DefaultValues.IssueResolutionId, dbo.BugNet_DefaultValuesVisibility.StatusVisibility, 
                      dbo.BugNet_DefaultValuesVisibility.PriorityVisibility, dbo.BugNet_DefaultValuesVisibility.OwnedByVisibility, 
                      dbo.BugNet_DefaultValuesVisibility.AssignedToVisibility, dbo.BugNet_DefaultValuesVisibility.PrivateVisibility, 
                      dbo.BugNet_DefaultValuesVisibility.CategoryVisibility, dbo.BugNet_DefaultValuesVisibility.DueDateVisibility, 
                      dbo.BugNet_DefaultValuesVisibility.TypeVisibility, dbo.BugNet_DefaultValuesVisibility.PercentCompleteVisibility, 
                      dbo.BugNet_DefaultValuesVisibility.MilestoneVisibility, dbo.BugNet_DefaultValuesVisibility.ResolutionVisibility, 
                      dbo.BugNet_DefaultValuesVisibility.EstimationVisibility, dbo.BugNet_DefaultValuesVisibility.AffectedMilestoneVisibility, 
                      dbo.BugNet_DefaultValues.OwnedByNotify, dbo.BugNet_DefaultValues.AssignedToNotify
FROM         dbo.BugNet_DefaultValues LEFT OUTER JOIN
                      dbo.AspNetUsers AS OwnerUsers ON dbo.BugNet_DefaultValues.IssueOwnerUserId = OwnerUsers.Id LEFT OUTER JOIN
                      dbo.AspNetUsers AS AssignedUsers ON dbo.BugNet_DefaultValues.IssueAssignedUserId = AssignedUsers.Id LEFT OUTER JOIN
                      dbo.BugNet_UserProfiles AS AssignedUsersProfile ON AssignedUsers.UserName = AssignedUsersProfile.UserName LEFT OUTER JOIN
                      dbo.BugNet_UserProfiles AS OwnerUsersProfile ON OwnerUsers.UserName = OwnerUsersProfile.UserName LEFT OUTER JOIN
                      dbo.BugNet_DefaultValuesVisibility ON dbo.BugNet_DefaultValues.ProjectId = dbo.BugNet_DefaultValuesVisibility.ProjectId

