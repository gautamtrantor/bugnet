﻿--CREATE TABLE [dbo].[BugNet_UserProfiles] (
--    [UserName]                  NVARCHAR (50)  NOT NULL,
--    [FirstName]                 NVARCHAR (100) NULL,
--    [LastName]                  NVARCHAR (100) NULL,
--    [DisplayName]               NVARCHAR (100) NULL,
--    [IssuesPageSize]            INT            NULL,
--    [PreferredLocale]           NVARCHAR (50)  NULL,
--    [LastUpdate]                DATETIME       NOT NULL,
--    [SelectedIssueColumns]      NVARCHAR (50)  NULL,
--    [ReceiveEmailNotifications] BIT            CONSTRAINT [DF_BugNet_UserProfiles_RecieveEmailNotifications] DEFAULT ((1)) NOT NULL,
--	[PasswordVerificationToken] [nvarchar](128) NULL,
--	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
--    CONSTRAINT [PK_BugNet_UserProfiles] PRIMARY KEY CLUSTERED ([UserName] ASC)
--);

