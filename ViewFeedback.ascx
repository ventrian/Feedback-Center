<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewFeedback.ascx.vb" Inherits="Ventrian.FeedbackCenter.ViewFeedback" %>
<%@ Register TagPrefix="Feedback" TagName="Approval" Src="Controls\Approval.ascx" %>

<table cellpadding="0" cellspacing="0" width="100%">
<tr>
	<td align="left" class="normal">
		<asp:Repeater ID="rptBreadCrumbs" Runat="server">
		<ItemTemplate>
			<a href='<%# DataBinder.Eval(Container.DataItem, "Url") %>' class="NormalBold"><%# DataBinder.Eval(Container.DataItem, "Caption") %></a>
		</ItemTemplate>
		<SeparatorTemplate>
			&#187;
		</SeparatorTemplate>
		</asp:Repeater>
	</td>
	<td align="right">
		<asp:Label ID="lblNoSubscribe" Runat="server" ResourceKey="NotLoggedInSubscribe" CssClass="Normal" />
		<asp:CheckBox ID="chkSubscribe" Runat="server" ResourceKey="Subscribe" AutoPostBack="True" CssClass="Normal" />
		<asp:LinkButton ID="cmdEditFeedback" runat="server" ResourceKey="btnEditFeedback" CssClass="COmmandButton" CausesValidation="false" />
	    <Feedback:Approval id="Approval1" runat="server" />
	</td>
</tr>
</table>
<br style="line-height: 5px;" />
<div align="center" id="divApprovalMessage" runat="server" visible="false">
    <asp:Label ID="lblApprovalMessage" Runat="server" CssClass="Normal" ResourceKey="ApprovalMessage"></asp:Label><br /><br />
</div>
<asp:PlaceHolder ID="phControls" runat="server" />
