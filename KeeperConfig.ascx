<%@ Control Language="VB" AutoEventWireup="false" Inherits="Aricie.DNN.Modules.PortalKeeper.UI.KeeperConfig"
    CodeBehind="KeeperConfig.ascx.vb" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register Assembly="Aricie.DNN" Namespace="Aricie.DNN.UI.WebControls" TagPrefix="aricie" %>
<div class="divPKP">
    <div id="divInstall" runat="server" class="divcenter">
        <dnn:CommandButton ID="cmdInstall" runat="server" ImageUrl="~/images/icon_wizard_16px.gif"
            ResourceKey="cmdInstall" />
        <dnn:CommandButton ID="cmdUninstall" runat="server" ImageUrl="~/images/delete.gif"
            ResourceKey="cmdUninstall" />
    </div>
    <div id="divConfig" runat="server">
        <div id="divHostConfig" runat="server">
            <asp:Label ID="lblConfig" CssClass="Head" runat="server" ResourceKey="lblConfig" /><br />
            <aricie:AriciePropertyEditorControl ID="KC" EditControlStyle-CssClass="NormalTextBox"
                EnableClientValidation="true" ErrorStyle-CssClass="NormalRed" GroupHeaderStyle-CssClass="Head"
                GroupHeaderIncludeRule="true" LabelStyle-CssClass="SubHead" VisibilityStyle-CssClass="Normal"
                GroupByMode="Section" DisplayMode="Div" EnabledOnDemandSections="True" runat="server"
                CssClass="pkSettings" LabelWidth="200px" />
            <div class="divcenter">
                <dnn:CommandButton ID="cmdClearProbes" runat="server" visible="false" ImageUrl="~/images/fwd.gif"
                    ResourceKey="cmdClearProbes" />
                <dnn:CommandButton ID="cmdDebug" Visible="False" runat="server" ImageUrl="~/images/icon_wizard_16px.gif"
                    ResourceKey="cmdDebug" />
            </div>
        </div>
        <div id="divPortalSettings" runat="server" visible="false">
            <asp:Label ID="lblSettings" CssClass="Head" runat="server" ResourceKey="lblSettings" /><br />
            <aricie:AriciePropertyEditorControl ID="KS" EditControlStyle-CssClass="NormalTextBox"
                EnableClientValidation="true" ErrorStyle-CssClass="NormalRed" GroupHeaderStyle-CssClass="Head"
                GroupHeaderIncludeRule="True" LabelStyle-CssClass="SubHead" VisibilityStyle-CssClass="Normal"
                GroupByMode="Section" DisplayMode="Div" runat="server" CssClass="pkSettings"
                LabelWidth="200px" />
            <div class="divcenter">
                <dnn:CommandButton ID="cmdSave" runat="server" ImageUrl="~/images/save.gif" ResourceKey="cmdSave" />
                <dnn:CommandButton ID="cmdCancelSettings" runat="server" ImageUrl="~/images/cancel.gif"
                    ResourceKey="cmdCancel" />
            </div>
        </div>
        <div id="divUserBot" runat="server" visible="false" class="SubDiv">
            <asp:Label ID="lblUserBot" CssClass="Head" runat="server" ResourceKey="lblUserBot" /><br />
            <div class="SubDiv">
                <aricie:AriciePropertyEditorControl ID="ctUserBotEntities" EditControlStyle-CssClass="NormalTextBox"
                    EnableClientValidation="true" ErrorStyle-CssClass="NormalRed" GroupHeaderStyle-CssClass="Head"
                    GroupHeaderIncludeRule="True" LabelStyle-CssClass="SubHead" VisibilityStyle-CssClass="Normal"
                    GroupByMode="Section" DisplayMode="Div" runat="server" CssClass="pkSettings"
                    LabelWidth="200px" DisableExports="True" />
            </div>
            <div class="CommandsButtons DNNAligncenter" id="divUserBotCmds" runat="server">
                <div class="SubDiv" id="divActionCommands" Visible="False" runat="server"/>
                <div class="SubDiv">
                    <dnn:CommandButton ID="cmdSaveUserBot" runat="server" ImageUrl="~/images/save.gif"
                        ResourceKey="cmdSaveUserBot" />
                    <dnn:CommandButton ID="cmdCancelUserBot" runat="server" ImageUrl="~/images/cancel.gif"
                        ResourceKey="cmdCancel" CausesValidation="false" />
                    <dnn:CommandButton ID="cmdDeleteUserBot" runat="server" ImageUrl="~/images/delete.gif"
                        ResourceKey="cmdDeleteUserBot" CausesValidation="false" />
                </div>
            </div>
            <asp:Label ID="lblBotDefinition" CssClass="Head" runat="server" Visible="false" ResourceKey="lblBotDefinition" /><br />
            <div class="SubDiv">
                <aricie:AriciePropertyEditorControl ID="ctlUserBot" EditControlStyle-CssClass="NormalTextBox"
                    EnableClientValidation="true" ErrorStyle-CssClass="NormalRed" GroupHeaderStyle-CssClass="Head"
                    GroupHeaderIncludeRule="True" LabelStyle-CssClass="SubHead" VisibilityStyle-CssClass="Normal"
                    GroupByMode="Section" DisplayMode="Div" runat="server" CssClass="pkSettings"
                    LabelWidth="200px" />
            </div>
        </div>
    </div>
</div>
