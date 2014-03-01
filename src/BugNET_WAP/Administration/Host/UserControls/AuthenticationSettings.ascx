﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuthenticationSettings.ascx.cs"
    Inherits="BugNET.Administration.Host.UserControls.AuthenticationSettings" %>
<h2>
    <asp:Literal ID="Title" runat="Server" Text="<%$ Resources:AuthenticationSettings %>" /></h2>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="form-horizontal">
            <div class="form-group">
                <asp:Label ID="label2" runat="server" CssClass="col-md-2 control-label" AssociatedControlID="UserAccountSource" Text="<%$ Resources:UserAccountSource %>" />
                <div class="col-md-10">
                    <asp:RadioButtonList RepeatDirection="Horizontal" CssClass="radio" OnSelectedIndexChanged="UserAccountSource_SelectedIndexChanged"
                        AutoPostBack="true" RepeatLayout="Flow" ID="UserAccountSource" runat="server">
                        <asp:ListItem runat="server" Value="None" meta:resourceKey="UserAccountSource_None" />
                        <asp:ListItem runat="server" Value="WindowsSAM" meta:resourceKey="UserAccountSource_WindowsSAM" />
                        <asp:ListItem runat="server" Value="ActiveDirectory" meta:resourceKey="UserAccountSource_AD" />
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="form-group">
                <asp:Label ID="label8" CssClass="col-md-2 control-label" runat="server" AssociatedControlID="UserRegistration"
                    Text="User Registration" meta:resourceKey="UserRegistration" />
                <div class="col-md-10">
                    <asp:RadioButtonList RepeatDirection="Horizontal" CssClass="radio" RepeatLayout="Flow" ID="UserRegistration" runat="server">
                        <asp:ListItem runat="server" meta:resourceKey="UserRegistration_None" Text="None" Value="0" />
                        <asp:ListItem runat="server" meta:resourceKey="UserRegistration_Public" Text="Public" Value="1" />
                        <asp:ListItem runat="server" meta:resourceKey="UserRegistration_Verified" Text="Verified" Value="2" />
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="form-group" id="trADPath" runat="server">
                <asp:Label ID="label25" CssClass="col-md-2 control-label" runat="server" AssociatedControlID="ADPath" Text="<%$ Resources:DomainPath %>" />
                <div class="col-md-10">
                    <asp:TextBox ID="ADPath" CssClass="form-control" runat="Server" />
                </div>
            </div>
            <div class="form-group" id="trADUserName" runat="server">
                <asp:Label ID="label4" CssClass="col-md-2 control-label" runat="server" AssociatedControlID="ADUserName" Text="<%$ Resources:SharedResources, Username %>" />
                <div class="col-md-10">
                    <asp:TextBox ID="ADUserName" CssClass="form-control" runat="Server" />
                </div>
            </div>
            <div class="form-group" id="trADPassword" runat="server">
                <asp:Label ID="label5" CssClass="col-md-2 control-label" runat="server" AssociatedControlID="ADPassword" Text="<%$ Resources:SharedResources, Password %>" />
                <div class="col-md-10">
                    <asp:TextBox TextMode="Password" ID="ADPassword" CssClass="form-control" runat="Server" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label ID="label7" CssClass="col-md-2 control-label" runat="server" AssociatedControlID="AnonymousAccess"
                    Text="Anonymous User Access" meta:resourceKey="AnonymousAccess" />
                <div class="col-md-10">
                    <asp:RadioButtonList ID="AnonymousAccess" CssClass="radio" RepeatLayout="Flow" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="<%$ Resources:SharedResources, Enable %>" Value="True" Selected="True" />
                        <asp:ListItem Text="<%$ Resources:SharedResources, Disable %>" Value="False" />
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="form-group">
                <asp:Label ID="label1" CssClass="col-md-2 control-label" runat="server" AssociatedControlID="OpenIdAuthentication"
                    Text="<%$ Resources:OpenIdAuthentication %>" />
                <div class="col-md-10">
                    <asp:RadioButtonList ID="OpenIdAuthentication" CssClass="radio" RepeatLayout="Flow" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="<%$ Resources:SharedResources, Enable %>" Value="True" />
                        <asp:ListItem Text="<%$ Resources:SharedResources, Disable %>" Value="False" Selected="True" />
                    </asp:RadioButtonList>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
