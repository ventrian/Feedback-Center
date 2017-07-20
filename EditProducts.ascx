<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditProducts.ascx.vb" Inherits="Ventrian.FeedbackCenter.EditProducts" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>
<table cellspacing="0" cellpadding="0" width="600" summary="Category Sort Order Design Table">
<tr valign="top">
	<td class="SubHead" width="150" height="30"><dnn:label id="plParentCategory" runat="server" resourcekey="ParentProduct" controlname="drpParentCategory" suffix=":"></dnn:label></td>
	<td><asp:DropDownList ID="drpParentCategory" Runat="server" DataTextField="Name" DataValueField="ProductID" AutoPostBack="True" /></td>
</tr>
<tr valign="top">
	<td class="SubHead" width="150" height="30"><dnn:label id="plChildCategories" runat="server" resourcekey="ChildProducts" controlname="lstChildCategories" suffix=":"></dnn:label></td>
	<td>
		<asp:Label ID="lblNoCategories" Runat="server" CssClass="Normal" Visible="False" ResourceKey="NoProducts" />
		<asp:Panel ID="pnlSortOrder" Runat="server">
			<table cellspacing="0" cellpadding="0" width="450">
			<tr>
				<td>	
					<asp:listbox id="lstChildCategories" runat="server" rows="22" DataTextField="Name" DataValueField="ProductID" cssclass="NormalTextBox" width="290px"></asp:listbox>
					<asp:Label ID="lblCategoryUpdated" Runat="server" CssClass="NormalBold" Visible="False" ResourceKey="CategoryUpdated" EnableViewState="False" />
				</td>
				<td>&nbsp;</td>
				<td width="150px" valign="top">
					<table summary="Tabs Design Table">
						<tr>
							<td colspan="2" valign="top" class="SubHead">
								<asp:label id="lblActions" runat="server" resourcekey="Actions">Actions</asp:label>
								<hr noshade size="1">
							</td>
						</tr>
						<tr>
							<td valign="top" width="10%">
								<asp:imagebutton id="cmdEdit" resourcekey="cmdEdit.Help" runat="server" alternatetext="Edit Category" imageurl="~/images/edit.gif"></asp:imagebutton>
							</td>
							<td valign="top" width="90%">
								<dnn:helpbutton id="hbtnEditHelp" resourcekey="cmdEdit" runat="server" /></dnn:helpbutton>
							</td>
						</tr>
						<tr>
							<td valign="top" width="10%">
								<asp:imagebutton id="cmdView" resourcekey="cmdView.Help" runat="server" alternatetext="View Category" imageurl="~/images/view.gif"></asp:imagebutton>
							</td>
							<td valign="top" width="90%">
								<dnn:helpbutton id="hbtnViewHelp" resourcekey="cmdView" runat="server" /></dnn:helpbutton>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			</table>
		</asp:Panel>
	</td>
</tr>
</table>

<p>
    <asp:linkbutton id="cmdAddNewProduct" resourcekey="AddProduct" runat="server" cssclass="CommandButton" text="Add New Product" borderstyle="none" />
</p>
