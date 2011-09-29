namespace BugNET.Administration.Projects.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using BugNET.BLL;
    using BugNET.Common;
    using BugNET.Entities;
    using BugNET.UserInterfaceLayer;

    /// <summary>
	///	Summary description for ProjectMemberRoles.
	/// </summary>
	public partial class ProjectRoles : System.Web.UI.UserControl, IEditProjectControl
	{
        private int _RoleId = -1;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
		
		}
        /// <summary>
        /// Role Id
        /// </summary>
        int RoleId
        {
            get
            {
                if (ViewState["RoleId"] == null)
                    return 0;
                else
                    return (int)ViewState["RoleId"];
            }
            set { ViewState["RoleId"] = value; }
        }

		#region IEditProjectControl Members

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
		public int ProjectId
		{
            get { return ((BasePage)Page).ProjectId; }
            set { ((BasePage)Page).ProjectId = value; }
		}

        /// <summary>
        /// Inits this instance.
        /// </summary>
		public void Initialize()
		{
            SecurityRoles.SelectParameters.Clear();
            SecurityRoles.SelectParameters.Add("projectId",ProjectId.ToString());
            gvRoles.DataBind();
		}

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns></returns>
		public bool Update()
		{
            
			return true;
		}

        /// <summary>
        /// Gets a value indicating whether [show save button].
        /// </summary>
        /// <value><c>true</c> if [show save button]; otherwise, <c>false</c>.</value>
        public bool ShowSaveButton
        {
            get { return false; }
        }
		#endregion

		#region Private Methods  

        /// <summary>
        /// Handles the Click event of the cmdAddUpdateRole control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void cmdAddUpdateRole_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (RoleId == 0)
                    {
                        string RoleName = txtRoleName.Text.Trim();
                        RoleId = RoleManager.CreateRole(RoleName, ProjectId, txtDescription.Text, chkAutoAssign.Checked);
                        UpdatePermissions(RoleId);
                    }
                    else
                    {
                        Role r = RoleManager.GetRoleById(RoleId);
                        r.Description = txtDescription.Text.Trim();
                        r.Name = txtRoleName.Text.Trim();
                        r.AutoAssign = chkAutoAssign.Checked;
                        RoleManager.SaveRole(r);
                        UpdatePermissions(RoleId);
                    }

                    AddRole.Visible = !AddRole.Visible;
                    Roles.Visible = !Roles.Visible;
                    Initialize();
                }
                catch
                {
                    lblError.Text = LoggingManager.GetErrorMessageResource("AddRoleError");
                }
            }	
        }
        /// <summary>
        /// Binds the role details.
        /// </summary>
        /// <param name="roleId">The role id.</param>
		private void BindRoleDetails(int roleId)
		{
            if (roleId == -1)
            {
                cmdAddUpdateRole.Text = "Add Role";
                cmdDelete.Visible = false;
                cancel.Visible = false;
                txtRoleName.Enabled = true;
                txtRoleName.Text = string.Empty;
                txtDescription.Text = string.Empty;
                txtDescription.Enabled = true;
                chkAddSubIssue.Checked = false;
                chkAddSubIssue.Enabled = true;
                chkAutoAssign.Enabled = true;
                chkAutoAssign.Checked = false;
                chkAssignIssue.Enabled = true;
                chkAssignIssue.Checked = false;
                chkCloseIssue.Enabled = true;
                chkCloseIssue.Checked = false;
                chkAddAttachment.Enabled = true;
                chkAddAttachment.Checked = false;
                chkAddComment.Enabled = true;
                chkAddComment.Checked = false;
                chkAddIssue.Enabled = true;
                chkAddIssue.Checked = false;
                chkAddRelated.Enabled = true;
                chkAddRelated.Checked = false;
                chkAddTimeEntry.Enabled = true;
                chkAddTimeEntry.Checked = false;
                chkAssignIssue.Enabled = true;
                chkAssignIssue.Checked = false;
                chkDeleteAttachment.Enabled = true;
                chkDeleteAttachment.Checked = false;
                chkDeleteComment.Enabled = true;
                chkDeleteComment.Checked = false;
                chkDeleteIssue.Enabled = true;
                chkDeleteIssue.Checked = false;
                chkDeleteRelated.Enabled = true;
                chkDeleteRelated.Checked = false;
                chkDeleteTimeEntry.Enabled = true;
                chkDeleteTimeEntry.Checked = false;
                chkEditComment.Enabled = true;
                chkEditComment.Checked = false;
                chkEditIssue.Enabled = true;
                chkEditIssue.Checked = false;
                chkEditIssueDescription.Enabled = true;
                chkEditIssueDescription.Checked = false;
                chkEditIssueSummary.Enabled = true;
                chkEditIssueSummary.Checked = false;
                chkEditOwnComment.Enabled = true;
                chkEditOwnComment.Checked = false;
                chkEditQuery.Enabled = true;
                chkEditQuery.Checked = false;
                chkReOpenIssue.Enabled = true;
                chkReOpenIssue.Checked = false;
                chkSubscribeIssue.Enabled = true;
                chkSubscribeIssue.Checked = false;
                chkDeleteQuery.Enabled = true;
                chkDeleteQuery.Checked = false;
                chkAddQuery.Enabled = true;
                chkAddQuery.Checked = false;
                chkEditProject.Checked = false;
                chkChangeIssueStatus.Checked = false;
                chkAddParentIssue.Checked = false;
                chkDeleteSubIssue.Checked = false;
                chkDeleteParentIssue.Checked = false;
                chkEditProject.Checked = false;
                chkDeleteProject.Checked = false;
                chkCloneProject.Checked = false;
                chkCreateProject.Checked = false;
                chkViewProjectCalendar.Checked = false;
            }
            else
            {
                RoleId = roleId;
                Role r = RoleManager.GetRoleById(roleId);

                foreach (string s in Globals.DefaultRoles)
                {
                    //if default role lock record
                    if (r.Name == s)
                    {
                        cmdDelete.Visible = false;
                        cancel.Visible = false;
                        txtRoleName.Enabled = false;
                        txtDescription.Enabled = false;
                    }
                }
                string message = string.Format(GetLocalResourceObject("ConfirmDelete").ToString(), r.Name);
                cmdDelete.OnClientClick = String.Format("return confirm('{0}');", message);
                cancel.OnClientClick = String.Format("return confirm('{0}');", message);
                AddRole.Visible = !AddRole.Visible;
                Roles.Visible = !Roles.Visible;
                cmdAddUpdateRole.Text = GetLocalResourceObject("UpdateRole").ToString();
                txtRoleName.Text = r.Name;
                txtDescription.Text = r.Description;
                chkAutoAssign.Checked = r.AutoAssign;
                RoleNameTitle.Text = GetLocalResourceObject("RoleNameTitle.Text").ToString() + " " + r.Name;
                ReBind();
            }
		}
		#endregion
   
        /// <summary>
        /// Handles the Click event of the AddRole control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddRole_Click(object sender, EventArgs e)
        {
            AddRole.Visible = !AddRole.Visible;
            Roles.Visible= !Roles.Visible;          
            txtRoleName.Visible = true;
            txtRoleName.Text = string.Empty;
            RoleNameTitle.Text = GetLocalResourceObject("AddNewRole.Text").ToString();       
            cmdDelete.Visible = false;
            cancel.Visible = false;
            BindRoleDetails(-1);
        }

        /// <summary>
        /// Handles the RowCommand event of the gvUsers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.GridViewCommandEventArgs"/> instance containing the event data.</param>
        protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "EditRole":
                    //get roles details and bind to form
                    BindRoleDetails(Convert.ToInt32(e.CommandArgument));
                    break;
            }
        }

        /// <summary>
        /// Updates the permissions.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        private void UpdatePermissions(int roleId)
        {
            //adds
            if (chkAddIssue.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddIssue); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AddIssue); }
            if (chkAddComment.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddComment); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AddComment); }
            if (chkAddAttachment.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddAttachment); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AddAttachment); }
            if (chkAddRelated.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddRelated); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AddRelated); }
            if (chkAddTimeEntry.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddTimeEntry); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AddTimeEntry); }

            if (chkAddQuery.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddQuery); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AddQuery);
            if (chkAddSubIssue.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddSubIssue); else RoleManager.DeleteRolePermission(roleId,(int)Globals.Permission.AddSubIssue);
            if (chkAddParentIssue.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AddParentIssue); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AddParentIssue);

            //edits
            if (chkEditProject.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AdminEditProject); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AdminEditProject);
            if (chkDeleteProject.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AdminDeleteProject); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AdminDeleteProject);
            if (chkDeleteProject.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AdminCloneProject); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AdminCloneProject);
            if (chkCreateProject.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AdminCreateProject); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AdminCreateProject);
            if (chkViewProjectCalendar.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.ViewProjectCalendar); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.ViewProjectCalendar);
            if (chkChangeIssueStatus.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.ChangeIssueStatus); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.ChangeIssueStatus);
            if (chkEditQuery.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.EditQuery); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.EditQuery);

            if (chkEditIssue.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.EditIssue); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.EditIssue); }
            if (chkEditComment.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.EditComment); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.EditComment); }
            if (chkEditOwnComment.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.OwnerEditComment); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.OwnerEditComment); }
            if (chkEditIssueDescription.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.EditIssueDescription); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.EditIssueDescription); }
            if (chkEditIssueSummary.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.EditIssueTitle); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.EditIssueTitle); }

            //deletes
            if (chkDeleteIssue.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteIssue); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteIssue); }
            if (chkDeleteComment.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteComment); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteComment); }
            if (chkDeleteAttachment.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteAttachment); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteAttachment); }
            if (chkDeleteRelated.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteRelated); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteRelated); }

            if (chkDeleteQuery.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteQuery); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteQuery);
            if (chkDeleteParentIssue.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteParentIssue); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteParentIssue);
            if (chkDeleteSubIssue.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteSubIssue); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteSubIssue);


            //misc
            if (chkAssignIssue.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.AssignIssue); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.AssignIssue); }
            if (chkSubscribeIssue.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.SubscribeIssue); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.SubscribeIssue); }

            if (chkReOpenIssue.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.ReopenIssue); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.ReopenIssue); }
            
            if (chkCloseIssue.Checked) RoleManager.AddRolePermission(roleId, (int)Globals.Permission.CloseIssue); else RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.CloseIssue); 

            if (chkDeleteTimeEntry.Checked)
            { RoleManager.AddRolePermission(roleId, (int)Globals.Permission.DeleteTimeEntry); }
            else
            { RoleManager.DeleteRolePermission(roleId, (int)Globals.Permission.DeleteTimeEntry); }
        }

        /// <summary>
        /// Rebinds the permission checkboxes.
        /// </summary>
        private void ReBind()
        {
            chkChangeIssueStatus.Checked = false;
            chkAssignIssue.Checked = false;
            chkCloseIssue.Checked = false;
            chkAddAttachment.Checked = false;
            chkAddComment.Checked = false;
            chkAddIssue.Checked = false;
            chkAddRelated.Checked = false;
            chkAddTimeEntry.Checked = false;            
            chkDeleteAttachment.Checked = false;
            chkDeleteComment.Checked = false;
            chkDeleteIssue.Checked = false;
            chkDeleteRelated.Checked = false;
            chkDeleteTimeEntry.Checked = false;
            chkEditComment.Checked = false;
            chkEditIssue.Checked = false;
            chkEditIssueDescription.Checked = false;
            chkEditIssueSummary.Checked = false;
            chkEditOwnComment.Checked = false;
            chkReOpenIssue.Checked = false;
            chkSubscribeIssue.Checked = false;
            chkAddQuery.Checked = false;
            chkDeleteQuery.Checked = false;
            chkEditProject.Checked = false;
            chkChangeIssueStatus.Checked = false;
            chkAddParentIssue.Checked = false;
            chkDeleteSubIssue.Checked = false;
            chkDeleteParentIssue.Checked = false;
            chkEditProject.Checked = false;
            chkDeleteProject.Checked = false;
            chkCloneProject.Checked = false;
            chkCreateProject.Checked = false;
            chkViewProjectCalendar.Checked = false;

            IEnumerable<Permission> permissions = RoleManager.GetPermissionsByRoleId(RoleId);

            foreach (Permission p in permissions)
            {
                switch (p.Key)
                {                 
                    case "ADD_TIME_ENTRY":
                        chkAddTimeEntry.Checked = true;
                        break;
                    case "ADD_QUERY":
                        chkAddQuery.Checked = true;
                        break;
                    case "ADD_ISSUE":
                        chkAddIssue.Checked = true;
                        break;
                    case "ADD_PARENT_ISSUE":
                        chkAddParentIssue.Checked = true;
                        break;
                    case "ADD_SUB_ISSUE":
                        chkAddSubIssue.Checked = true;
                        break;
                    case "ADD_COMMENT":
                        chkAddComment.Checked = true;
                        break;
                    case "ADD_ATTACHMENT":
                        chkAddAttachment.Checked = true;
                        break;
                    case "ADD_RELATED":
                        chkAddRelated.Checked = true;
                        break;
                    case "EDIT_ISSUE":
                        chkEditIssue.Checked = true;
                        break;
                    case "EDIT_QUERY":
                        chkEditQuery.Checked = true;
                        break;
                    case "EDIT_COMMENT":
                        chkEditComment.Checked = true;
                        break;
                    case "OWNER_EDIT_COMMENT":
                        chkEditOwnComment.Checked = true;
                        break;
                    case "DELETE_ISSUE":
                        chkDeleteIssue.Checked = true;
                        break;
                    case "DELETE_COMMENT":
                        chkDeleteComment.Checked = true;
                        break;
                    case "DELETE_ATTACHMENT":
                        chkDeleteAttachment.Checked = true;
                        break;
                    case "DELETE_RELATED":
                        chkDeleteRelated.Checked = true;
                        break;
                    case "DELETE_PARENT_ISSUE":
                        chkDeleteParentIssue.Checked = true;
                        break;
                    case "DELETE_SUB_ISSUE":
                        chkDeleteSubIssue.Checked = true;
                        break;
                    case "DELETE_TIME_ENTRY":
                        chkDeleteTimeEntry.Checked = true;
                        break;
                    case "DELETE_QUERY":
                        chkDeleteQuery.Checked = true;
                        break;
                    case "ASSIGN_ISSUE":
                        chkAssignIssue.Checked = true;
                        break;
                    case "SUBSCRIBE_ISSUE":
                        chkSubscribeIssue.Checked = true;
                        break;
                    case "CHANGE_ISSUE_STATUS":
                        chkChangeIssueStatus.Checked = true;
                        break;
                    case "REOPEN_ISSUE":
                        chkReOpenIssue.Checked = true;
                        break;
                    case "CLOSE_ISSUE":
                        chkCloseIssue.Checked = true;
                        break;
                    case "EDIT_ISSUE_DESCRIPTION":
                        chkEditIssueDescription.Checked = true;
                        break;
                    case "EDIT_ISSUE_TITLE":
                        chkEditIssueSummary.Checked = true;
                        break;
                    case "ADMIN_EDIT_PROJECT":
                        chkEditProject.Checked = true;
                        break;
                    case "ADMIN_DELETE_PROJECT":
                        chkDeleteProject.Checked = true;
                        break;
                    case "ADMIN_CLONE_PROJECT":
                        chkCloneProject.Checked = true;
                        break;
                    case "ADMIN_CREATE_PROJECT":
                        chkCreateProject.Checked = true;
                        break;
                    case "VIEW_PROJECT_CALENDAR":
                        chkViewProjectCalendar.Checked = true;
                        break;
                }

            }
        }
        /// <summary>
        /// Handles the Click event of the cmdCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            AddRole.Visible = !AddRole.Visible;
            Roles.Visible = !Roles.Visible;
            RoleId = -1;
        }

        /// <summary>
        /// Handles the Click event of the cmdDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                RoleManager.DeleteRole(RoleId);
                AddRole.Visible = !AddRole.Visible;
                Roles.Visible = !Roles.Visible;
                Initialize();
            }
            catch
            {
                lblError.Text = LoggingManager.GetErrorMessageResource("DeleteRoleError"); 
            }
        }
	}
}
