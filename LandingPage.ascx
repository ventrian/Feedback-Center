<%@ Control language="vb" Inherits="Ventrian.FeedbackCenter.LandingPage" CodeBehind="LandingPage.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="Feedback" TagName="Approval" Src="Controls\Approval.ascx" %>
<div align="right">
    <Feedback:Approval id="Approval1" runat="server" />
</div>

<asp:PlaceHolder ID="phTemplate" runat="server" />
