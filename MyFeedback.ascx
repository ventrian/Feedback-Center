<%@ Control language="vb" Inherits="Ventrian.FeedbackCenter.MyFeedback" CodeBehind="MyFeedback.ascx.vb" AutoEventWireup="false" Explicit="True" %>
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
<table cellSpacing=1 cellPadding=0 width="100%" align=center border=0 ID="Table1"><tr><td class=feedbackTable>
<table cellSpacing=1 cellPadding=3 width="100%" border=0 ID="Table2">
    <tr>
		<td class="feedbackContentCell">
		<table cellpadding="4" cellspacing="0">
			<tr>
				<td align="right">
					<asp:Label ID="lblStatus" Runat="server" ResourceKey="StatusLabel" CssClass="NormalBold"></asp:Label>
				</td>
				<td>
					<asp:RadioButtonList ID="lstStatus" Runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True">
						<asp:ListItem Value="Open" Selected="True">Open</asp:ListItem>
						<asp:ListItem Value="Closed">Closed</asp:ListItem>
					</asp:RadioButtonList>
				</td>
			</tr>
		</table>
		</td>
    </tr>
</table>
</TD></TR></TBODY></TABLE>
<br style="line-height: 5px;" />
<asp:PlaceHolder ID="phControls" runat="server"></asp:PlaceHolder>
