<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditLayoutFiles.ascx.vb" Inherits="Ventrian.FeedbackCenter.EditLayoutFiles" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>

<asp:Label ID="lblTemplateUpdated" runat="server" resourceKey="TemplateUpdated" Visible="false" EnableViewState="false" CssClass="NormalBold" />
<table cellspacing="0" cellpadding="0" width="600" summary="Layout Files Design Table">
<tr valign="top">
	<td class="SubHead" width="150" height="30"><dnn:label id="plTemplate" runat="server" resourcekey="Template" controlname="drpTemplate" suffix=":"></dnn:label></td>
	<td><asp:DropDownList ID="drpTemplate" Runat="server" AutoPostBack="True" /></td>
</tr>
<tr valign="top">
	<td class="SubHead" width="150" height="30"><dnn:label id="plTemplateText" runat="server" resourcekey="TemplateText" controlname="txtTemplate" suffix=":"></dnn:label></td>
	<td>
        <asp:textbox id="txtTemplate" runat="server" cssclass="NormalTextBox" width="450px" rows="20"
								textmode="MultiLine"></asp:textbox>
	</td>
</tr>
</table>

<p>
    <asp:linkbutton id="cmdUpdate" resourcekey="Update" runat="server" cssclass="CommandButton" text="Update" borderstyle="none" />&nbsp;<asp:linkbutton id="cmdCancel" resourcekey="Cancel" runat="server" cssclass="CommandButton" text="Cancel" borderstyle="none" />
</p>

<table cellspacing="0" cellpadding="0" width="600" summary="Layout Files Design Table">
<tr valign="top">
	<td class="SubHead" width="150" height="30"><dnn:label id="plTokens" runat="server" resourcekey="Tokens" controlname="rptTokens" suffix=":"></dnn:label></td>
	<td>
	
	    <asp:Repeater ID="rptTokens" runat="server">
	        <ItemTemplate>
	            <span class="normal">[<%# Container.DataItem.ToString() %>]</span><br />
	        </ItemTemplate>
	    </asp:Repeater>
	    
	</td>
</tr>
</table>