<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Settings.ascx.vb" Inherits="Ventrian.FeedbackCenter.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plEmailNotification" runat="server" controlname="txtEmailNotification" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtEmailNotification" runat="server" maxlength="255" Columns="30" width="300" cssclass="NormalTextBox"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plEnableCaptcha" runat="server" controlname="chkEnableCaptcha" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkEnableCaptcha" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plEnableRSS" runat="server" controlname="chkEnableRSS" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkEnableRSS" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plEnableStatistics" runat="server" controlname="chkEnableStatistics" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkEnableStatistics" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plCommentAttachments" runat="server" controlname="chkCommentAttachments" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkCommentAttachments" runat="server" />
        </td>
    </tr>
</table>
<br />
<dnn:SectionHead id="dshActiveSocial" cssclass="Head" runat="server" text="Active Social Settings" section="tblActiveSocial" IsExpanded="False" />
<table id="tblActiveSocial" cellspacing="2" cellpadding="2" summary="Active Social Design Table" border="0" runat="server">
<tr>
	<td class="SubHead" width="150"><dnn:label id="plActiveSocialSubmissionKey" runat="server" suffix=":" controlname="grdPermissions"></dnn:label></td>
	<td>
	    <asp:TextBox ID="txtActiveSocialSubmissionKey" runat="server" CssClass="NormalTextBox" Width="300" />
	</td>
</tr>
<tr>
	<td class="SubHead" width="150"><dnn:label id="plActiveSocialSubscribeKey" runat="server" suffix=":" controlname="grdPermissions"></dnn:label></td>
	<td>
	    <asp:TextBox ID="txtActiveSocialSubscribeKey" runat="server" CssClass="NormalTextBox" Width="300" />
	</td>
</tr>
<tr>
	<td class="SubHead" width="150"><dnn:label id="plActiveSocialVoteKey" runat="server" suffix=":" controlname="grdPermissions"></dnn:label></td>
	<td>
	    <asp:TextBox ID="txtActiveSocialVoteKey" runat="server" CssClass="NormalTextBox" Width="300" />
	</td>
</tr>
<tr>
	<td class="SubHead" width="150"><dnn:label id="plActiveSocialCommentKey" runat="server" suffix=":" controlname="grdPermissions"></dnn:label></td>
	<td>
	    <asp:TextBox ID="txtActiveSocialCommentKey" runat="server" CssClass="NormalTextBox" Width="300" />
	</td>
</tr>
</table>
<br />
<dnn:SectionHead id="dshSecurity" cssclass="Head" runat="server" text="Security Settings" section="tblSecurity" IsExpanded="False" />
<table id="tblSecurity" cellspacing="2" cellpadding="2" summary="Security Design Table" border="0" runat="server">
<tr>
	<td class="SubHead" width="150"><dnn:label id="plPermissions" runat="server" suffix=":" controlname="grdPermissions"></dnn:label></td>
	<td>
        <asp:DataGrid ID="grdPermissions" Runat="server" AutoGenerateColumns="False" ItemStyle-CssClass="Normal"
            ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center"
            HeaderStyle-CssClass="NormalBold" CellSpacing="0" CellPadding="0" GridLines="None" BorderWidth="1"
            BorderStyle="None" DataKeyField="Value">
            <Columns>
                <asp:TemplateColumn>
	                <ItemStyle HorizontalAlign="Left" />
	                <HeaderTemplate>
	                </HeaderTemplate>
	                <ItemTemplate>
		                <%# DataBinder.Eval(Container.DataItem, "Text") %>
	                </ItemTemplate>
                </asp:TemplateColumn>	
                <asp:TemplateColumn>
	                <HeaderTemplate>
		                &nbsp;
		                <asp:Label ID="lblSubmit" Runat="server" EnableViewState="False" ResourceKey="Submit" />&nbsp;
	                </HeaderTemplate>
	                <ItemTemplate>
		                <asp:CheckBox ID="chkSubmit" Runat="server" />
	                </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn>
	                <HeaderTemplate>
		                &nbsp;
		                <asp:Label ID="lblVote" Runat="server" EnableViewState="False" ResourceKey="Vote" />&nbsp;
	                </HeaderTemplate>
	                <ItemTemplate>
		                <asp:CheckBox ID="chkVote" Runat="server" />
	                </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn>
	                <HeaderTemplate>
		                &nbsp;
		                <asp:Label ID="lblComment" Runat="server" EnableViewState="False" ResourceKey="Comment" />&nbsp;
	                </HeaderTemplate>
	                <ItemTemplate>
		                <asp:CheckBox ID="chkComment" Runat="server" />
	                </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn>
	                <HeaderTemplate>
		                &nbsp;
		                <asp:Label ID="lblAutoApprove" Runat="server" EnableViewState="False" ResourceKey="AutoApprove" />&nbsp;
	                </HeaderTemplate>
	                <ItemTemplate>
		                <asp:CheckBox ID="chkAutoApprove" Runat="server" />
	                </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn>
	                <HeaderTemplate>
		                &nbsp;
		                <asp:Label ID="lblApprove" Runat="server" EnableViewState="False" ResourceKey="Approve" />&nbsp;
	                </HeaderTemplate>
	                <ItemTemplate>
		                <asp:CheckBox ID="chkApprove" Runat="server" />
	                </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
	</td>
</tr>
</table>