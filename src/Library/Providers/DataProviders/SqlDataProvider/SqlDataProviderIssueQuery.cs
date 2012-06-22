﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BugNET.Common;
using BugNET.Entities;

namespace BugNET.Providers.DataProviders
{
    public partial class SqlDataProvider
    {
        /// <summary>
        /// Perform a query agains the issues stored
        /// </summary>
        /// <param name="queryClauses">Criteria for the query</param>
        /// <param name="sortFields">The name / sort direction of the sort fields</param>
        /// <param name="projectId">The project id</param>
        /// <returns></returns>
        /// <remarks>This mehtod does not filter the issues by any criteria, that means disabled and closed issues will be returned by default.
        /// It is up to the client caller to define the filters before hand.  For filtering closed and disabled issues use iv.[IsClosed] = 0 and 
        /// iv.[Disabled] = 0
        /// </remarks>
        public override List<Issue> PerformQuery(List<QueryClause> queryClauses, ICollection<KeyValuePair<string, string>> sortFields, int projectId = 0)
        {
            // build the custom field view name
            var customFieldViewName = string.Format(Globals.PROJECT_CUSTOM_FIELDS_VIEW_NAME, projectId);

            // start out with some default values
            var customFieldsJoin = string.Empty;
            var startWhere = "WHERE 1=1 ";

            // store the sql we will be building
            var commandBuilder = new StringBuilder();

            // this is our default select statement, the placeholders will be swapped out as we go
            commandBuilder.Append("SELECT * FROM BugNet_IssuesView iv @PROJECT_CF_JOIN@ @START_WHERE@ @CRITERIA@ @SORT_FIELDS@");

            // if we have a project id then we can take advantage of the custom field view to help with performance
            if (projectId > 0)
            {
                customFieldsJoin = string.Format("INNER JOIN {0} cf ON cf.IssueId = iv.IssueId AND cf.ProjectId = iv.ProjectId", customFieldViewName);
                startWhere = string.Format("WHERE iv.[ProjectId] = {0} ", projectId);
            }
            else
            {
                var disabledPresent = queryClauses.Any(queryClause => queryClause.FieldName.ToLowerInvariant().Contains("projectdisabled"));

                if (!disabledPresent)
                {
                    startWhere = string.Format("WHERE iv.[ProjectDisabled] = {0} ", 0);
                }
            }

            // swap out the placeholders for the custom field view join logic and the were criteria
            commandBuilder = commandBuilder.Replace("@PROJECT_CF_JOIN@", customFieldsJoin);
            commandBuilder = commandBuilder.Replace("@START_WHERE@", startWhere);

            var sortSql = string.Empty;
            var issueList = new List<Issue>();

            try
            {
                // build the sort string (if any)
                if (sortFields != null)
                {
                    foreach (var keyValuePair in sortFields)
                    {
                        var field = keyValuePair.Key.Trim();

                        // no field then no sort option
                        if (field.Length.Equals(0)) continue;

                        // lower the direction
                        var direction = keyValuePair.Value.Trim().ToLowerInvariant();

                        // check if the direction is valid
                        if (!direction.Equals("asc") && !direction.Equals("desc"))
                            direction = "asc";

                        // if the field contains a period then they might be passing in and alias so dont try and clean up
                        if (!field.Contains("."))
                        {
                            if (!field.EndsWith("]"))
                                field = string.Concat(field, "]");

                            if (!field.EndsWith("["))
                                field = string.Concat("[", field);
                        }

                        // build proper sort string
                        sortSql = string.Concat(sortSql, " ", field, " ", direction, ",").Trim();
                    }
                }

                // set a default sort if no sort fields
                if (sortFields == null || sortFields.Count.Equals(0))
                {
                    sortSql = "iv.[IssueId] desc";
                }

                sortSql = sortSql.TrimEnd(',');

                sortSql = sortSql.Insert(0, "ORDER BY ");

                // do we have query clauses if so then process them
                if (queryClauses != null)
                {
                    var i = 0;
                    var criteriaBuilder = new StringBuilder();

                    foreach (var qc in queryClauses)
                    {
                        var fieldName = qc.FieldName.Trim();
                        var fieldValue = qc.FieldValue;
                        var boolOper = qc.BooleanOperator.ToLower().Trim();
                        var compareOper = qc.ComparisonOperator.Trim();

                        // if the field contains a period then they might be passing in and alias so dont try and clean up
                        // this puts properness in the hands of the UI making the call
                        if (!fieldName.Contains(".") && !fieldName.Equals("1"))
                        {
                            fieldName = fieldName.Replace("[", "").Replace("]", "").Trim();

                            if (fieldName.Length > 0)
                            {
                                if (!fieldName.StartsWith("["))
                                    fieldName = string.Concat("[", fieldName);

                                if (!fieldName.EndsWith("]"))
                                    fieldName = string.Concat(fieldName, "]");   
                            }
                        }

                        // handle when we want to create nested boolean logic in the query clauses
                        // this of course means the order of the query clauses must be correct
                        if (boolOper.EndsWith(")"))
                        {
                            criteriaBuilder.AppendFormat(" {0}", boolOper);
                        }

                        // if the field name is empty they we must be closing a nested criteria
                        if (fieldName.Length.Equals(0)) continue;

                        // if the custom field value is null empty then setup for a null value
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            criteriaBuilder.AppendFormat(" {0} {1} {2} NULL", boolOper, fieldName, compareOper);
                        }
                        else if (qc.DataType == SqlDbType.DateTime)
                        {
                            criteriaBuilder.AppendFormat(" {0} datediff(day, {1}, @p{3}) {2} 0", boolOper, fieldName, compareOper, i);
                        }
                        else
                        {
                            criteriaBuilder.AppendFormat(" {0} {1} {2} @p{3}", boolOper, fieldName, compareOper, i);
                        }

                        i++;
                    }

                    // swap out the placeholders for the critiera and the sort fields
                    commandBuilder = commandBuilder.Replace("@CRITERIA@", criteriaBuilder.ToString());
                    commandBuilder = commandBuilder.Replace("@SORT_FIELDS@", sortSql);

                    using (var sqlCmd = new SqlCommand())
                    {
                        sqlCmd.CommandText = commandBuilder.ToString();

#if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("NEW - {0}", sqlCmd.CommandText);
#endif
                        commandBuilder.Clear();

                        i = 0;

                        //RW loop thru and add non custom field queries parameters.
                        foreach (var qc in queryClauses)
                        {
                            //skip if value null
                            if (string.IsNullOrEmpty(qc.FieldValue)) continue;
                            var par = new SqlParameter("@p" + i, qc.DataType) { Value = qc.FieldValue };
                            sqlCmd.Parameters.Add(par);
                            commandBuilder.AppendFormat("{0} = {1} | ", par, par.Value);
                            i++;
                        }
#if (DEBUG)
                        System.Diagnostics.Debug.WriteLine(commandBuilder);
#endif
                        ExecuteReaderCmd(sqlCmd, GenerateIssueListFromReader, ref issueList);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }

            return issueList;
        }

        /// <summary>
        /// Combined Perform Custom Query Method
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="queryClauses">The query clauses.</param>
        /// <returns></returns>
        [Obsolete("Please use the PerformQuery, you may have to update how the query clauses get built first")]
        public override List<Issue> PerformQuery(int projectId, List<QueryClause> queryClauses)
        {
            // Validate Parameters
            int queryClauseCount;

            //assign the queryClauses Count to our variable and then check the result.
            if ((queryClauseCount = queryClauses.Count) == 0) throw (new ArgumentOutOfRangeException("queryClauses"));

            try
            {
                // Build Command Text
                var commandBuilder = new StringBuilder();
                //'DSS customfields in the same query   
                commandBuilder.Append(projectId != 0
                                          ? "SELECT DISTINCT * FROM BugNet_GetIssuesByProjectIdAndCustomFieldView WHERE ProjectId = @ProjectId AND IssueId IN (SELECT IssueId FROM BugNet_IssuesView WHERE 1 = 1 "
                                          : "SELECT DISTINCT * FROM BugNet_GetIssuesByProjectIdAndCustomFieldView WHERE IssueId IN (SELECT IssueId FROM BugNet_IssuesView WHERE 1 = 1 ");

                var i = 0;

                //RW check for Standard Query
                foreach (var qc in queryClauses)
                {
                    if (!qc.CustomFieldQuery)
                    {
                        if (qc.BooleanOperator.Trim().Equals(")"))
                            commandBuilder.AppendFormat(" {0}", qc.BooleanOperator);
                        else if (string.IsNullOrEmpty(qc.FieldValue))
                        {
                            commandBuilder.AppendFormat(" {0} {1} {2} NULL", qc.BooleanOperator, qc.FieldName, qc.ComparisonOperator);
                        }
                        else if (qc.DataType == SqlDbType.DateTime)
                        {
                            commandBuilder.AppendFormat(" {0} datediff(day, {1}, @p{3}) {2} 0", qc.BooleanOperator, qc.FieldName, qc.ComparisonOperator, i);
                        }
                        else
                        {
                            commandBuilder.AppendFormat(" {0} {1} {2} @p{3}", qc.BooleanOperator, qc.FieldName, qc.ComparisonOperator, i);
                        }
                        i++;
                    }
                    else
                    {
                        //'DSS customfields in the same query
                        commandBuilder.AppendFormat(" {0} {1} {2} {3} {4} @p{5}", qc.BooleanOperator, "CustomFieldName=", "'" + qc.FieldName + "'", " AND CustomFieldValue ", qc.ComparisonOperator, i);
                        i += 1;
                    }
                }
                commandBuilder.Append(") ORDER BY IssueId DESC");

                using (var sqlCmd = new SqlCommand())
                {
                    sqlCmd.CommandText = commandBuilder.ToString();

                    // Build Parameter List
                    if (projectId != 0)
                        sqlCmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;

                    i = 0; //RW counter for parameters

                    //RW loop thru and add non custom field queries parameters.
                    foreach (var qc in queryClauses)
                    {
                        if (!qc.CustomFieldQuery) //RW not a custom field query
                        {
                            //skip if value null
                            if (!string.IsNullOrEmpty(qc.FieldValue))
                            {
                                sqlCmd.Parameters.Add("@p" + i.ToString(), qc.DataType).Value = qc.FieldValue;
                            }
                            i++;
                            //sqlCmd.Parameters.Add("@p" + i.ToString(), qc.DataType).Value = qc.FieldValue;
                            //i++;
                        }
                        else
                        {
                            //DSS customfields in the same query
                            sqlCmd.Parameters.Add("@p" + i.ToString(), qc.DataType).Value = qc.FieldValue;
                            i += 1;
                        }
                    }

                    //RW create a new issue collection here
                    var issueList = new List<Issue>();

                    //more queries, but they are not custom.
                    //So, populate the collection with what we have.
                    if (i > 0 && i <= queryClauseCount)
                    {
#if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("OLD - {0}", sqlCmd.CommandText);
#endif
                        ExecuteReaderCmd(sqlCmd, GenerateIssueListFromReader, ref issueList);
                    }

                    //return distinct issues
                    var distinctIssueList = issueList.Distinct(new DistinctIssueComparer()).ToList();
                    return distinctIssueList;
                }
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }

        }


        /// <summary>
        /// Performs the query against any generic List&lt;T/&gtl;
        /// Added SMOSS 11-May-2009
        /// Modified 13-April-2010
        /// </summary>
        /// <param name="list"></param>
        /// <param name="queryClauses">The query clauses.</param>
        /// <param name="sql"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public override void PerformGenericQuery<T>(ref List<T> list, List<QueryClause> queryClauses, string sql, string orderBy)
        {
            SqlDataProvider.GenerateListFromReader<T> gcfr = null;

            // ---------------------------------------------------------------
            //
            // The following Regular Expression yields the lines out of this provider file.
            // 
            // TGenerate\w*FromReader\<T\>\(SqlDataReader returnData, ref List\<\w*\> *\w* *\w*\) *
            //            
            //TGenerateIssueCountListFromReader<T>(SqlDataReader returnData, ref List<IssueCount> issueCountList)
            //TGenerateProjectMemberRoleListFromReader<T>(SqlDataReader returnData, ref List<MemberRoles> memberRoleList)
            //TGenerateRolePermissionListFromReader<T>(SqlDataReader returnData, ref List<RolePermission> rolePermissionList)
            //TGeneratePermissionListFromReader<T>(SqlDataReader returnData, ref List<Permission> permissionList)
            //GenerateIssueRevisionListFromReader<T>(SqlDataReader returnData, ref List<IssueRevision> revisionList)
            //TGenerateRequiredFieldListFromReader<T>(SqlDataReader returnData, ref List<RequiredField> requiredFieldList)
            //GenerateCategoryListFromReader<T>(SqlDataReader returnData, ref List<Category> categoryList)
            //GenerateHostSettingListFromReader<T>(SqlDataReader returnData, ref List<HostSetting> hostsettingList)
            //TGenerateProjectListFromReader<T>(SqlDataReader returnData, ref List<Project> projectList)
            //TGenerateApplicationLogListFromReader<T>(SqlDataReader returnData, ref List<ApplicationLog> applicationLogList)
            //GenerateIssueNotificationListFromReader<T>(SqlDataReader returnData, ref List<IssueNotification> issueNotificationList)
            //TGenerateProjectNotificationListFromReader<T>(SqlDataReader returnData, ref List<ProjectNotification> projectNotificationList)
            //GenerateRelatedIssueListFromReader<T>(SqlDataReader returnData, ref List<RelatedIssue> relatedIssueList)
            //TGenerateUserListFromReader<T>(SqlDataReader returnData, ref List<ITUser> userList)
            //TGenerateStatusListFromReader<T>(SqlDataReader returnData, ref List<Status> statusList)
            //GenerateMilestoneListFromReader<T>(SqlDataReader returnData, ref List<Milestone> milestoneList)
            //TGeneratePriorityListFromReader<T>(SqlDataReader returnData, ref List<Priority> priorityList)            
            //TGenerateQueryClauseListFromReader<T>(SqlDataReader returnData, ref List<QueryClause> queryClauseList)
            //TGenerateIssueTimeEntryListFromReader<T>(SqlDataReader returnData, ref List<IssueWorkReport> issueWorkReportList)
            //TGenerateResolutionListFromReader<T>(SqlDataReader returnData, ref List<Resolution> resolutionList)
            //GenerateRoleListFromReader<T>(SqlDataReader returnData, ref List<Role> roleList)
            //TGenerateCustomFieldListFromReader<T>(SqlDataReader returnData, ref List<CustomField> customFieldList)
            //TGenerateCustomFieldSelectionListFromReader<T>(SqlDataReader returnData, ref List<CustomFieldSelection> customFieldSelectionList)
            //GenerateQueryListFromReader<T>(SqlDataReader returnData, ref List<Query> queryList)
            //TGenerateRoadmapIssueListFromReader<T>(SqlDataReader returnData, ref List<RoadMapIssue> issueList)
            //
            // ---------------------------------------------------------------
            var theType = typeof(T);

            // Wont work in a switch statement due to Type

            //if (theType == typeof(ProjectMailbox))
            //{
            //    System.Reflection.MethodInfo mi = this.GetType().GetMethod("GenerateProjectMailboxListFromReader").MakeGenericMethod(new Type[] { typeof(ProjectMailbox) });
            //    gcfr = (TGenerateListFromReader<T>)Delegate.CreateDelegate(typeof(TGenerateListFromReader<ProjectMailbox>), this, mi);
            //}

            if (theType == typeof(IssueHistory))
            {
                var mi = GetType().GetMethod("GenerateIssueHistoryListFromReader").MakeGenericMethod(new[] { typeof(IssueHistory) });
                gcfr = (SqlDataProvider.GenerateListFromReader<T>)Delegate.CreateDelegate(typeof(SqlDataProvider.GenerateListFromReader<IssueHistory>), this, mi);
            }

            //TGenerateIssueTypeListFromReader<T>(SqlDataReader returnData, ref List<IssueType> issueTypeList)
            if (theType == typeof(IssueHistory))
            {
                var mi = GetType().GetMethod("GenerateIssueHistoryListFromReader").MakeGenericMethod(new[] { typeof(IssueHistory) });
                gcfr = (SqlDataProvider.GenerateListFromReader<T>)Delegate.CreateDelegate(typeof(SqlDataProvider.GenerateListFromReader<IssueHistory>), this, mi);
            }

            if (theType == typeof(IssueAttachment))
            {
                var mi = GetType().GetMethod("GenerateIssueAttachmentListFromReader").MakeGenericMethod(new[] { typeof(IssueAttachment) });
                gcfr = (SqlDataProvider.GenerateListFromReader<T>)Delegate.CreateDelegate(typeof(SqlDataProvider.GenerateListFromReader<IssueAttachment>), this, mi);
            }

            if (theType == typeof(Issue))
            {
                var mi = GetType().GetMethod("TGenerateIssueListFromReader").MakeGenericMethod(new[] { typeof(Issue) });
                gcfr = (SqlDataProvider.GenerateListFromReader<T>)Delegate.CreateDelegate(typeof(SqlDataProvider.GenerateListFromReader<Issue>), this, mi);
            }

            if (theType == typeof(IssueComment))
            {
                var mi = GetType().GetMethod("GenerateIssueCommentListFromReader").MakeGenericMethod(new[] { typeof(IssueComment) });
                gcfr = (SqlDataProvider.GenerateListFromReader<T>)Delegate.CreateDelegate(typeof(SqlDataProvider.GenerateListFromReader<IssueComment>), this, mi);
            }

            //assign the queryClauses Count to our variable and then check the result.
            if ((queryClauses.Count) == 0)
                throw (new ArgumentOutOfRangeException("queryClauses", 0, "queryClauses == 0"));

            // Build Command Text
            var commandBuilder = new StringBuilder();
            commandBuilder.Append(sql);

            var i = 0;

            //RW check for Standard Query
            foreach (var qc in queryClauses.Where(qc => !qc.CustomFieldQuery))
            {
                // if Field Value is null or empty do a null comparision
                // But only if the operator is not blank
                // "William Highfield" Method
                if (string.IsNullOrEmpty(qc.FieldValue))
                {
                    commandBuilder.AppendFormat(
                        !string.IsNullOrEmpty(qc.ComparisonOperator) ? " {0} {1} {2} NULL" : " {0} {1} {2}",
                        qc.BooleanOperator, qc.FieldName, qc.ComparisonOperator);
                }
                else
                {
                    commandBuilder.AppendFormat(" {0} {1} {2} @p{3}", qc.BooleanOperator, qc.FieldName, qc.ComparisonOperator, i);
                }
                i++;
            }

            commandBuilder.Append(orderBy);

            using (var sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandText = commandBuilder.ToString();

                // Build Parameter List
                //  sqlCmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;
                i = 0; //RW counter for parameters			

                //RW loop thru and add non custom field queries parameters.
                foreach (var qc in queryClauses)
                {
                    if (qc.CustomFieldQuery) continue;

                    //skip if value null
                    if (!string.IsNullOrEmpty(qc.FieldValue))
                    {
                        sqlCmd.Parameters.Add("@p" + i, qc.DataType).Value = qc.FieldValue;
                    }
                    i++;
                }

                if (gcfr != null)
                {
                    ExecuteReaderCmd(sqlCmd, gcfr, ref list);
                    //note we are passing this methods objects by reference because we want
                    //the collection to be added to. If we don't pass by reference, we lose the collection
                    //when the method returns.
                }
            }
        } //end PerformGenericQuery

    }
}
