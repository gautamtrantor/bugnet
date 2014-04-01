﻿<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="BugNET.Account.Register" meta:ResourceKey="Page" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <asp:CreateUserWizard runat="server" ID="RegisterUser" ViewStateMode="Disabled" OnCreatedUser="RegisterUser_CreatedUser">
        <LayoutTemplate>
            <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder" />
            <asp:PlaceHolder runat="server" ID="navigationPlaceholder" />
        </LayoutTemplate>
        <WizardSteps>
            <asp:CreateUserWizardStep runat="server" ID="RegisterUserWizardStep">
                <ContentTemplate>
                    <div class="form-horizontal">
                        <h4><asp:Localize runat="server" meta:resourceKey="TitleLabel" Text="[Resource Required]"/></h4>
                        <hr />
                        <asp:ValidationSummary runat="server" CssClass="text-danger" />
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-4 control-label" Text="<%$ Resources:SharedResources, UserName%>">User name</asp:Label>
                            <div class="col-md-5">
                                <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName"
                                    CssClass="text-danger" ErrorMessage="<%$ Resources: UsernameRequiredErrorMessage%>" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-4 control-label" Text="<%$ Resources:SharedResources, EMail%>">Email</asp:Label>
                            <div class="col-md-5">
                                <asp:TextBox runat="server" ID="Email" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                    CssClass="text-danger" ErrorMessage="<%$ Resources: EmailRequiredErrorMessage%>" />
                            </div>
                        </div>
                         <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-4 control-label" Text="<%$ Resources:SharedResources, Password%>">Password</asp:Label>
                            <div class="col-md-5">
                                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                                    CssClass="text-danger" ErrorMessage="<%$ Resources: PasswordRequiredErrorMessage%>" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="ConfirmPassword" CssClass="col-md-4 control-label" Text="<%$ Resources:SharedResources, ConfirmPassword%>">Confirm password</asp:Label>
                            <div class="col-md-5">
                                <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmPassword"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="<%$ Resources: ConfirmPasswordRequiredErrorMessage%>" />
                                <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="<%$ Resources: ConfirmPasswordMismatchErrorMessage%>" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-offset-4 col-md-5">
                                <asp:Button runat="server" CommandName="MoveNext" Text="Register"  CssClass="btn btn-primary" meta:resourcekey="RegisterButton" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <CustomNavigationTemplate />
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="VerificationPanel" runat="server" Visible="false">
                        <p><strong><asp:Localize runat="server" ID="Localize1" Text="Thanks for registering with us" meta:resourcekey="VerificationInstructionsTitle" /></strong></p>
                        <p><asp:Localize runat="server" ID="Localize5" Text="Now wait for an email to be sent to the email address you specified with instructions to enable your account and login." meta:resourcekey="VerificationInstructions" />     </p>
                    </asp:Panel>
                </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
</asp:Content>