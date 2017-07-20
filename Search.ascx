<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Search.ascx.vb" Inherits="Ventrian.FeedbackCenter.Search" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
			<table cellSpacing="1" cellPadding="3" width="100%" border="0" ID="Table2">
				<tr align="left">
					<td class="feedbackTopCell" align="left" id="FeedbackHeader"><asp:Label ID="lblSearchOptions" Runat="server" ResourceKey="SearchOptions" CssClass="Normal" /></td>
				</tr>
				<tr>
					<td class="feedbackContentCell">
						<table cellpadding="4" cellspacing="0">
							<tr>
								<td align="right">
									<asp:Label ID="lblProduct" Runat="server" ResourceKey="Product" CssClass="NormalBold"></asp:Label>
								</td>
								<td>
									<asp:DropDownList ID="drpProducts" Runat="server" DataValueField="ProductID" DataTextField="Name"
										CssClass="NormalTextBox" />
									&nbsp;
									<asp:RadioButtonList ID="lstStatus" Runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal"
										RepeatLayout="Flow">
										<asp:ListItem Value="open" Selected="True">Open</asp:ListItem>
										<asp:ListItem Value="closed">Closed</asp:ListItem>
									</asp:RadioButtonList>
									&nbsp;
									<asp:DropDownList ID="drpSortBy" Runat="server" CssClass="NormalTextBox" />
									&nbsp;
									<asp:DropDownList ID="drpSortDirection" Runat="server" CssClass="NormalTextBox" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label ID="lblKeyword" Runat="server" ResourceKey="Keyword" CssClass="NormalBold"></asp:Label>
								</td>
								<td>
									<asp:TextBox ID="txtKeyword" Runat="server" CssClass="NormalTextBox" width="300px" />
									<asp:Button ID="btnSearch" Runat="server" ResourceKey="btnSearch" CssClass="Button" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<br style="LINE-HEIGHT: 5px">
<asp:PlaceHolder ID="phControls" runat="server"></asp:PlaceHolder>

