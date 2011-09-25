using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BugNET.BLL;
using BugNET.Common;
using BugNET.Entities;
using BugNET.UserInterfaceLayer;
using log4net;

namespace BugNET.Issues.UserControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Attachments : System.Web.UI.UserControl, IIssueTab
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Attachments));
        private int _IssueId = 0;
        private int _ProjectId = 0;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager sman = ScriptManager.GetCurrent(Page);
            sman.RegisterPostBackControl(UploadButton);
        }

        #region IIssueTab Members

        /// <summary>
        /// Gets or sets the bug id.
        /// </summary>
        /// <value>The bug id.</value>
        public int IssueId
        {
            get { return _IssueId; }
            set { _IssueId = value; }
        }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        public int ProjectId
        {
            get { return _ProjectId; }
            set { _ProjectId = value; }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            AttachmentsDataGrid.Columns[0].HeaderText = GetLocalResourceObject("AttachmentsGrid.FileNameHeader.Text").ToString();
            AttachmentsDataGrid.Columns[1].HeaderText = GetLocalResourceObject("AttachmentsGrid.SizeHeader.Text").ToString();
            AttachmentsDataGrid.Columns[2].HeaderText = GetLocalResourceObject("AttachmentsGrid.Description.Text").ToString();

            BindAttachments();

            //check users role permission for adding an attachment
            if (!Page.User.Identity.IsAuthenticated || !UserManager.HasPermission(ProjectId, Globals.Permission.AddAttachment.ToString()))
                pnlAddAttachment.Visible = false;

            if (!Page.User.Identity.IsAuthenticated || !UserManager.HasPermission(ProjectId, Globals.Permission.DeleteAttachment.ToString()))
                AttachmentsDataGrid.Columns[5].Visible = false;
        }

        #endregion

        /// <summary>
        /// Binds the attachments.
        /// </summary>
        private void BindAttachments()
        {
            //Fix tab names after adding or deleting a record.
           //IssueTabs tabs = this.Parent as Issues.UserControls.IssueTabs;
           //tabs.RefreshTabNames();
            List<IssueAttachment> attachments = IssueAttachmentManager.GetIssueAttachmentsByIssueId(_IssueId);

            if (attachments.Count == 0)
            {
                lblAttachments.Text = GetLocalResourceObject("NoAttachments").ToString();
                lblAttachments.Visible = true;
                AttachmentsDataGrid.Visible = false;
            }
            else
            {
                lblAttachments.Visible = false;
                AttachmentsDataGrid.DataSource = attachments;
                AttachmentsDataGrid.DataBind();
                AttachmentsDataGrid.Visible = true;
            }
        }

        /// <summary>
        /// Uploads the document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UploadDocument(object sender, EventArgs e)
        {
            // get the current file
            HttpPostedFile uploadFile = this.AspUploadFile.PostedFile;

            // if there was a file uploaded
            if (uploadFile.ContentLength > 0)
            {

                string returnMessage = IssueAttachmentManager.ValidateFileName(uploadFile.FileName);

                if (string.IsNullOrEmpty(returnMessage))
                {
                    byte[] fileBytes;
                    using (System.IO.Stream input = uploadFile.InputStream)
                    {
                        fileBytes = new byte[uploadFile.ContentLength];
                        input.Read(fileBytes, 0, uploadFile.ContentLength);
                    }

                    IssueAttachment attachment = new IssueAttachment(IssueId, Security.GetUserName(),
                        uploadFile.FileName, uploadFile.ContentType, fileBytes, fileBytes.Length, AttachmentDescription.Text.Trim());

                    if (!IssueAttachmentManager.SaveIssueAttachment(attachment))
                    {
                        AttachmentsMessage.ShowErrorMessage(string.Format(GetGlobalResourceObject("Exceptions", "SaveAttachmentError").ToString(), uploadFile.FileName));
                        if (Log.IsWarnEnabled) Log.Warn(string.Format(GetGlobalResourceObject("Exceptions", "SaveAttachmentError").ToString(), uploadFile.FileName));
                        return;
                    }

                    //add history record and send notifications
                    IssueHistory history = new IssueHistory(_IssueId, Security.GetUserName(), Resources.SharedResources.Attachment.ToString(), string.Empty, Resources.SharedResources.Added);
                    IssueHistoryManager.SaveIssueHistory(history);

                    List<IssueHistory> changes = new List<IssueHistory>();
                    changes.Add(history);

                    IssueNotificationManager.SendIssueNotifications(_IssueId, changes);

                    BindAttachments();
                }
                else
                    AttachmentsMessage.ShowErrorMessage(returnMessage); 
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the AttachmentsDataGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
        protected void AttachmentsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                IssueAttachment currentAttachment = (IssueAttachment)e.Item.DataItem;
                ((HtmlAnchor)e.Item.FindControl("lnkAttachment")).InnerText = currentAttachment.FileName;
                ((HtmlAnchor)e.Item.FindControl("lnkAttachment")).HRef = "DownloadAttachment.axd?id=" + currentAttachment.Id.ToString();
                ImageButton lnkDeleteAttachment = (ImageButton)e.Item.FindControl("lnkDeleteAttachment");
                lnkDeleteAttachment.OnClientClick = string.Format("return confirm('{0}');", GetLocalResourceObject("DeleteAttachment").ToString());
                LinkButton cmdDeleteAttachment = (LinkButton)e.Item.FindControl("cmdDeleteAttachment");
                cmdDeleteAttachment.OnClientClick = string.Format("return confirm('{0}');", GetLocalResourceObject("DeleteAttachment").ToString());

                float size;
                string label;
                if (currentAttachment.Size > 1000)
                {
                    size = currentAttachment.Size / 1000f;
                    label = string.Format("{0} kb", size.ToString("##,##"));
                }
                else
                {
                    size = currentAttachment.Size;
                    label = string.Format("{0} b", size.ToString("##,##"));
                }
                ((Label)e.Item.FindControl("lblSize")).Text = label;
            }
        }

        /// <summary>
        /// Handles the ItemCommand event of the dtgAttachment control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
        protected void AttachmentsDataGrid_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Delete":
                    IssueAttachmentManager.DeleteIssueAttachment(Convert.ToInt32(e.CommandArgument));

                    IssueHistory history = new IssueHistory(_IssueId, Security.GetUserName(), Resources.SharedResources.Attachment.ToString(), string.Empty, Resources.SharedResources.Deleted);
                    IssueHistoryManager.SaveIssueHistory(history);

                    List<IssueHistory> changes = new List<IssueHistory>();
                    changes.Add(history);

                    IssueNotificationManager.SendIssueNotifications(_IssueId, changes);

                    break;
            }
            BindAttachments();
        }

    }
}