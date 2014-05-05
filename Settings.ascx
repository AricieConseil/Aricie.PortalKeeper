<%@ Control Language="vb" AutoEventWireup="false" Inherits="Aricie.DNN.Modules.PortalKeeper.UI.Settings" Codebehind="Settings.ascx.vb" %>
<%@ Register Assembly="Aricie.DNN" Namespace="Aricie.DNN.UI.WebControls" TagPrefix="aricie" %>
<asp:Label ID="lblGeneralIntro" CssClass="Normal" runat="server" ResourceKey="GeneralIntro"></asp:Label>
    <aricie:AriciePropertyEditorControl ID="ctS" EditControlStyle-CssClass="NormalTextBox"
        EnableClientValidation="true" ErrorStyle-CssClass="NormalRed" GroupHeaderStyle-CssClass="SubHead"
        GroupHeaderIncludeRule="True" LabelStyle-CssClass="SubHead"
        VisibilityStyle-CssClass="Normal" EditControlWidth="400px" LabelWidth="300px"
        Width="700px" GroupByMode="Section" runat="server">
    </aricie:AriciePropertyEditorControl>
