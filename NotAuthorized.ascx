<%@ Control language="vb" Inherits="Ventrian.FeedbackCenter.NotAuthorized" CodeBehind="NotAuthorized.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="Feedback" TagName="Approval" Src="Controls\Approval.ascx" %>
<table cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td align="left">
			<asp:Repeater ID="rptBreadCrumbs" Runat="server" EnableViewState="False">
				<ItemTemplate>
					<a href='<%# DataBinder.Eval(Container.DataItem, "Url") %>' class="NormalBold">
						<%# DataBinder.Eval(Container.DataItem, "Caption") %>
					</a>
				</ItemTemplate>
				<SeparatorTemplate>
					&#187;
				</SeparatorTemplate>
			</asp:Repeater>
		</td>
		<td align="right">
		    <Feedback:Approval id="Approval1" runat="server" />
		</td>
	</tr>
</table>
<br style="LINE-HEIGHT: 5px">
<table cellSpacing="1" cellPadding="0" width="100%" align="center" border="0" ID="Table1">
	<tr>
		<td class="feedbackTable">
			<table cellSpacing="1" cellPadding="3" width="100%" border="0" ID="Feedback">
				<tr align="left">
					<td class="feedbackTopCell" align="left" id="FeedbackHeader"><asp:Label ID="lblNotAuthorized" Runat="server" ResourceKey="NotAuthorized" CssClass="Normal"></asp:Label></td>
				</tr>
				<tr>
					<td class="feedbackContentCell">
						<asp:Label ID="lblNotAuthorizedMessage" Runat="server" CssClass="Normal" ResourceKey="NotAuthorizedMessage"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
