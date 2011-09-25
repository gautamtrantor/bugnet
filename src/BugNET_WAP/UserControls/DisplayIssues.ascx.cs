namespace BugNET.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using BugNET.BLL;
    using BugNET.Common;
    using BugNET.Entities;
    using BugNET.UserInterfaceLayer;
    /// <summary>
	///	Display Issues grid
	/// </summary>
	public partial class DisplayIssues : System.Web.UI.UserControl
	{
        /// <summary>
        /// Datasource 
        /// </summary>
		private List<Issue> _DataSource;
        /// <summary>
        /// Event that fires on a databind
        /// </summary>
		public event EventHandler RebindCommand;
        /// <summary>
        /// Array of issue columns
        /// </summary>
        private string[] _arrIssueColumns = new string[] { "4", "5", "6", "7", "8", "9", "10","11","12","13","14","15","16","17","18","19","20","21", "22"};



        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Init(object sender, System.EventArgs e)
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(WebProfile.Current.SelectedIssueColumns))
                    _arrIssueColumns = WebProfile.Current.SelectedIssueColumns.Trim().Split();
            }
            else
            {
                if ((Request.Cookies[Globals.IssueColumns] != null) && (Request.Cookies[Globals.IssueColumns].Value != String.Empty))
                    _arrIssueColumns = Request.Cookies[Globals.IssueColumns].Value.Split();
            }  
        }

        /// <summary>
        /// Sets the RSS  URL.
        /// </summary>
        /// <value>The RSS  URL.</value>
        public string RssUrl
        {
            set { lnkRSS.NavigateUrl = value; }
        }
   
        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
		public List<Issue> DataSource 
		{
			get { return _DataSource; }
			set { _DataSource = value; }
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //need to rebind these on every postback because of dynamic controls
            int projectId;
            if (Request.QueryString["pid"] != null)
            {
                if (Int32.TryParse(Request.QueryString["pid"], out projectId))
                {
                    List<CustomField> customFields = CustomFieldManager.GetCustomFieldsByProjectId(projectId);

                    if (customFields.Count > 0)
                    {
                        ctlCustomFields.DataSource = customFields;
                        ctlCustomFields.DataBind();
                    }
                }
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind() 
		{
			if(this.DataSource.Count > 0)
			{
                if (!string.IsNullOrEmpty(gvIssues.SortField))
                {
                    //_DataSource.Sort(gvIssues.SortField);
                    _DataSource.Sort(new IssueComparer(SortField, SortAscending));
                   //_DataSource.Sort(new ObjectComparer<Issue>(gvIssues.SortField, true));
                }

				gvIssues.Visible = true;
                pager.Visible = true;
                DisplayColumns();

              
              

                SelectColumnsPanel.Visible = true;

                //Table tb = (Table)gvIssues.Controls[0];
                //TableRow pagerRow = tb.Rows[tb.Rows.Count - 1]; //last row in the table is for bottom pager
                //pagerRow.Cells[0].Attributes.Remove("colspan");
                //pagerRow.Cells[0].Attributes.Add("colspan",_arrIssueColumns.Length.ToString() + 4);

				lblResults.Visible=false;

                if (ShowProjectColumn)
                {
                    gvIssues.Columns[0].Visible = false;
                    LeftButtonContainerPanel.Visible = false;
                }
                else
                {
                    gvIssues.Columns[4].Visible = false;
                    lstIssueColumns.Items.Remove(lstIssueColumns.Items.FindByValue("4"));

                    int projectId = ((Issue)_DataSource[0]).ProjectId;


                      //foreach(CustomField cf in CustomField.GetCustomFieldsByProjectId(projectId))
                      //{

                      //    TemplateField ckhColumn = new TemplateField();

                      //    ckhColumn.HeaderTemplate = new GridViewTemplate(DataControlRowType.Header, cf.Name);

                      //    ckhColumn.ItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, cf.Name);
                      //    ckhColumn.Visible = true;
                      //    gvIssues.Columns.Add(ckhColumn);

                      //}

                    //hide votes column if issue voting is disabled
                    if (!ProjectManager.GetProjectById(projectId).AllowIssueVoting)
                    {
                        gvIssues.Columns[4].Visible = false;
                        lstIssueColumns.Items.Remove(lstIssueColumns.Items.FindByValue("4"));
                    }

                    if ((!Page.User.Identity.IsAuthenticated) || (UserManager.HasPermission(projectId, Globals.Permission.EDIT_ISSUE.ToString()) == false) ||
                        (UserManager.IsInRole(projectId, Globals.DefaultRoles[0]) == false))
                    {
                        //hide selection column for unauthenticated users 
                        gvIssues.Columns[0].Visible = false;
                        LeftButtonContainerPanel.Visible = false;
                    }
                    else
                        LeftButtonContainerPanel.Visible = true;

                    CategoryTree categories = new CategoryTree();
                    dropCategory.DataSource = categories.GetCategoryTreeByProjectId(projectId);
                    dropCategory.DataBind();
                    dropMilestone.DataSource = MilestoneManager.GetMilestoneByProjectId(projectId);
                    dropMilestone.DataBind();
                    dropAffectedMilestone.DataSource = MilestoneManager.GetMilestoneByProjectId(projectId);
                    dropAffectedMilestone.DataBind();
                    dropOwner.DataSource = UserManager.GetUsersByProjectId(projectId);
                    dropOwner.DataBind();
                    dropPriority.DataSource = PriorityManager.GetPrioritiesByProjectId(projectId);
                    dropPriority.DataBind();
                    dropStatus.DataSource = StatusManager.GetStatusByProjectId(projectId);
                    dropStatus.DataBind();
                    dropType.DataSource = IssueTypeManager.GetIssueTypesByProjectId(projectId);
                    dropType.DataBind();
                    dropAssigned.DataSource = UserManager.GetUsersByProjectId(projectId);
                    dropAssigned.DataBind();
                    dropResolution.DataSource = ResolutionManager.GetResolutionsByProjectId(projectId);
                    dropResolution.DataBind();
                }

                foreach (string colIndex in _arrIssueColumns)
                {
                    ListItem item = lstIssueColumns.Items.FindByValue(colIndex);
                    if (item != null)
                        item.Selected = true;
                }

                gvIssues.DataSource = this.DataSource;
                gvIssues.DataBind();

			}
            else
            {
                OptionsContainerPanel.Visible = false;
                lblResults.Visible = true;
                gvIssues.Visible = false;
                pager.Visible = false;
            }
			
		}

        /// <summary>
        /// Handles the Click event of the ExportExcelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ExportExcelButton_Click(object sender, EventArgs e)
        {
            GridViewExportUtil.Export("Issues.xls", this.gvIssues);
        }

        /// <summary>
        /// Displays the columns.
        /// </summary>
        private void DisplayColumns()
        {
            // Hide all the DataGrid columns
            for (int index = 4; index < gvIssues.Columns.Count; index++)
                gvIssues.Columns[index].Visible = false;

            // Display columns based on the _arrIssueColumns array (retrieved from cookie)
            foreach (string colIndex in _arrIssueColumns)
                gvIssues.Columns[Int32.Parse(colIndex)].Visible = true;
        }

        /// <summary>
        /// Handles the Click event of the SaveIssues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveIssues_Click(object sender, EventArgs e)
        {
            //TODO: Ajax progress bar when this is running;
            string ids = GetSelectedIssueIds();
            if (ids.Length > 0)
            {
                 //prune out all values that must not change
                var customFieldValues = ctlCustomFields.Values;
                for (int i = customFieldValues.Count - 1; i >= 0; i--)
                {
                    var value = customFieldValues[i];
                    if (string.IsNullOrEmpty(value.Value))
                    {
                        customFieldValues.RemoveAt(i);
                    }
                }

                foreach (string s in ids.Split(new char[] { ',' }))
                {
                    Issue issue = IssueManager.GetIssueById(Convert.ToInt32(s));
                    if (issue != null)
                    {
                        DateTime dueDate = (DateTime)DueDate.SelectedValue;
                  
                        Issue newIssue = new Issue(issue.Id, issue.ProjectId, string.Empty, string.Empty,issue.Title, issue.Description,
                            dropCategory.SelectedValue != 0 ? dropCategory.SelectedValue : issue.CategoryId, dropCategory.SelectedValue != 0 ? dropCategory.SelectedText : issue.CategoryName,
                            dropPriority.SelectedValue != 0 ? dropPriority.SelectedValue : issue.PriorityId, dropPriority.SelectedValue != 0 ? dropPriority.SelectedText : issue.PriorityName,
                            string.Empty,
                            dropStatus.SelectedValue != 0 ? dropStatus.SelectedValue : issue.StatusId,
                            dropStatus.SelectedValue != 0 ? dropStatus.SelectedText : issue.StatusName, string.Empty,
                            dropType.SelectedValue != 0 ? dropType.SelectedValue : issue.IssueTypeId,
                            dropType.SelectedValue != 0 ? dropType.SelectedText : issue.IssueTypeName, string.Empty,
                            dropResolution.SelectedValue != 0 ? dropResolution.SelectedValue : issue.ResolutionId,
                            dropResolution.SelectedValue != 0 ? dropResolution.SelectedText : issue.ResolutionName, string.Empty,
                            dropAssigned.SelectedValue != string.Empty ? dropAssigned.SelectedText : issue.AssignedDisplayName,
                            dropAssigned.SelectedValue != string.Empty ? dropAssigned.SelectedValue : issue.AssignedUserName, 
                            Guid.Empty, Security.GetDisplayName(),Security.GetUserName(), Guid.Empty,
                            dropOwner.SelectedValue !=string.Empty ? dropOwner.SelectedText : issue.OwnerDisplayName,
                            dropOwner.SelectedValue != string.Empty ? dropOwner.SelectedValue : issue.OwnerUserName, Guid.Empty, dueDate != DateTime.MinValue ? dueDate : issue.DueDate,
                            dropMilestone.SelectedValue != 0 ? dropMilestone.SelectedValue : issue.MilestoneId, dropMilestone.SelectedValue != 0 ? dropMilestone.SelectedText : issue.MilestoneName, string.Empty, null, 
                            dropAffectedMilestone.SelectedValue != 0 ? dropAffectedMilestone.SelectedValue : issue.AffectedMilestoneId, dropAffectedMilestone.SelectedValue != 0 ? dropAffectedMilestone.SelectedText : issue.AffectedMilestoneName,
                            string.Empty, issue.Visibility,0, issue.Estimation, DateTime.MinValue, DateTime.MinValue, Security.GetUserName(), Security.GetDisplayName(),
                            issue.Progress, false, 0);

                        IssueManager.SaveIssue(newIssue);
                        CustomFieldManager.SaveCustomFieldValues(issue.Id, customFieldValues);
                    }
                }
            }

            OnRebindCommand(EventArgs.Empty);         
        }
        /// <summary>
        /// Gets the selected issues.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedIssueIds()
        {
            string ids = string.Empty;
             foreach (GridViewRow gvr in gvIssues.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    if (((CheckBox)gvr.Cells[0].Controls[1]).Checked)
                        ids += gvIssues.DataKeys[gvr.RowIndex].Value.ToString() + ",";
                }
            }
             return ids.EndsWith(",") == true ? ids.TrimEnd(new char[] { ',' }) : ids;
        }
       
        /// <summary>
        /// Saves the click.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveClick(Object s, EventArgs e)
        {
            string strIssueColumns = " 0";
            foreach (ListItem item in lstIssueColumns.Items)
                if (item.Selected)
                    strIssueColumns += " " + item.Value;
            strIssueColumns = strIssueColumns.Trim();

            _arrIssueColumns = strIssueColumns.Split();

            if (Page.User.Identity.IsAuthenticated)
            {
                WebProfile.Current.SelectedIssueColumns = strIssueColumns.Trim();
                WebProfile.Current.Save();
            }
            else
            {
                Response.Cookies[Globals.IssueColumns].Value = strIssueColumns;
                Response.Cookies[Globals.IssueColumns].Path = "/";
                Response.Cookies[Globals.IssueColumns].Expires = DateTime.MaxValue;
            }

            OnRebindCommand(EventArgs.Empty);
        }
     
        /// <summary>
        /// Raises the rebind command event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		void OnRebindCommand(EventArgs e) 
		{
			if (RebindCommand != null)
				RebindCommand(this, e);
		}

        /// <summary>
        /// Gets or sets the index of the current page.
        /// </summary>
        /// <value>The index of the current page.</value>
		public int CurrentPageIndex 
		{
			get { return gvIssues.PageIndex; }
			set { gvIssues.PageIndex = value; }
		}

        /// <summary>
        /// Gets or sets a value indicating whether [show project column].
        /// </summary>
        /// <value><c>true</c> if [show project column]; otherwise, <c>false</c>.</value>
        public bool ShowProjectColumn
        {
            get
            {
                object o = ViewState["ShowProjectColumn"];
                if (o == null)
                {
                    return false;
                }
                return (bool)o;
            }
            set
            {
                ViewState["ShowProjectColumn"] = value;
            }

        }

        /// <summary>
        /// Gets or sets the sort field.
        /// </summary>
        /// <value>The sort field.</value>
		public string SortField 
		{
			get 
			{
				object o = ViewState["SortField"];
				if (o == null) 
				{
					return String.Empty;
				}
				return (string)o;
			}
    
			set 
			{
				if (value == SortField) 
				{
					// same as current sort file, toggle sort direction
					SortAscending = !SortAscending;
				}
				ViewState["SortField"] = value;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether [sort ascending].
        /// </summary>
        /// <value><c>true</c> if [sort ascending]; otherwise, <c>false</c>.</value>
		public bool SortAscending 
		{
			get 
			{
				object o = ViewState["SortAscending"];
				if (o == null) 
				{
					return true;
				}
				return (bool)o;
			}
    
			set 
			{
				ViewState["SortAscending"] = value;
			}
		}


        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get { return pager.PageSize; }
            set
            {
                pager.PageSize = value;
            }      
        }


        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if(gvIssues.HeaderRow !=null)
                gvIssues.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        /// <summary>
        /// Handles the RowDataBound event of the gvIssues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gvIssues_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.background='#F7F7EC'");

                if( e.Row.RowState == DataControlRowState.Normal)
                    e.Row.Attributes.Add("onmouseout", "this.style.background=''");
                else if( e.Row.RowState == DataControlRowState.Alternate)
                    e.Row.Attributes.Add("onmouseout", "this.style.background='#fafafa'");
 
                Issue b = ((Issue)e.Row.DataItem);

                //Private issue check
                if (b.Visibility == (int)Globals.IssueVisibility.Private && b.AssignedDisplayName != Security.GetUserName() && b.CreatorDisplayName != Security.GetUserName() && (!UserManager.IsInRole(Globals.SuperUserRole) || !UserManager.IsInRole(Globals.ProjectAdminRole)))
                    e.Row.Visible = false;

                e.Row.FindControl("imgPrivate").Visible = b.Visibility == 0 ? false : true;

                double warnPeriod = 7; //TODO: Add this to be configurable in the users profile
                bool isDue = b.DueDate <= DateTime.Now.AddDays(warnPeriod) && b.DueDate > DateTime.MinValue;
                bool noOwner = b.AssignedUserId == Guid.Empty;
                //if (noOwner || isDue)
                //{
                //    e.Row.Attributes.Add("style", "background-color:#ffdddc");
                //    e.Row.Attributes.Add("onmouseout", "this.style.background='#ffdddc'");
                //}
                ((HtmlControl)e.Row.FindControl("ProgressBar")).Attributes.CssStyle.Add("width", b.Progress.ToString() + "%");
                //CheckBox cb = e.Row.Cells[23].Controls[0] as CheckBox;
                //string val = CustomField.GetCustomFieldsByIssueId(b.Id)[0].Value;
                //cb.Checked = true;
            }
        }
   
        /// <summary>
        /// Handles the Sorting event of the gvIssues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
        protected void gvIssues_Sorting(object sender, GridViewSortEventArgs e)
        {
            gvIssues.SortField = e.SortExpression;
            OnRebindCommand(EventArgs.Empty);
        }

        /// <summary>
        /// Handles the PageIndexChanging event of the gvIssues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.GridViewPageEventArgs"/> instance containing the event data.</param>
        protected void gvIssues_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvIssues.PageIndex = e.NewPageIndex;
            OnRebindCommand(EventArgs.Empty);
        }
	}

    public class GridViewTemplate : System.Web.UI.Page, ITemplate
    {

        private DataControlRowType templateType;

        string columnName;

        public GridViewTemplate(DataControlRowType type, string colname)
        {

            templateType = type;

            columnName = colname;

        }

        /// <summary>
        /// Handles the DataBind event of the tb1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tb1_DataBind(Object sender, EventArgs e)
        {
            TextBox hpl = (TextBox)sender;
            GridViewRow row = (GridViewRow)hpl.NamingContainer;
            //hpl.NavigateUrl = DataBinder.Eval(row.DataItem, _ColNameURL).ToString();
            Issue i = row.DataItem as Issue;
            string value = CustomFieldManager.GetCustomFieldsByIssueId(i.Id).Find(cf => cf.Name == columnName).Value;
            hpl.Text = "<div class=\"Post\"><div class=\"PostTitle\">" + value + "</div></div>";
        }

        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn(System.Web.UI.Control container)
        {

            Literal lc = new Literal();

            LinkButton lb = new LinkButton();
            CheckBox ckh = new CheckBox();

            TextBox tb1 = new TextBox();

            switch (templateType)
            {

                case  DataControlRowType.Header:

                    lc.Text = "<B>" + columnName + "</B>";

                    lb.Text = columnName;

                    lb.CommandName = "EditButton";

                    container.Controls.Add(lb);

                    container.Controls.Add(lc);

                    break;

                case DataControlRowType.DataRow:


                    container.Controls.Add(tb1);
                    container.Controls.Add(ckh);
                    tb1.DataBinding += new EventHandler(this.tb1_DataBind);

                    break;

                //case ListItemType.EditItem:

                //    TextBox tb = new TextBox();

                //    tb.Text = "";

                //    container.Controls.Add(tb);

                //    break;

                //case ListItemType.Footer:

                //    lc.Text = "<I>" + columnName + "</I>";

                //    container.Controls.Add(lc);

                //    break;

            }



        }
    }
}
