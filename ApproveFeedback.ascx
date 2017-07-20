<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ApproveFeedback.ascx.vb" Inherits="Ventrian.FeedbackCenter.ApproveFeedback" %>
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
<br />
<script type="text/javascript">
function SelectAll(CheckBoxControl)
{
    if (CheckBoxControl.checked == true)
    {
        var i;
        for (i=0; i < document.forms[0].elements.length; i++)
        {
            if ((document.forms[0].elements[i].type == 'checkbox') &&
            (document.forms[0].elements[i].name.indexOf('grdApproveFeedback') > -1))
            {
                document.forms[0].elements[i].checked = true;
            }
        }
    }
    else
    {
        var i;
        for (i=0; i < document.forms[0].elements.length; i++)
        {
            if ((document.forms[0].elements[i].type == 'checkbox') &&
            (document.forms[0].elements[i].name.indexOf('grdApproveFeedback') > -1))
            {
                document.forms[0].elements[i].checked = false;
            }
        }
    }
}
</script>

<asp:PlaceHolder ID="phFeedback" runat="server">
<asp:datagrid id="grdApproveFeedback" Border="0" CellPadding="4" CellSpacing="0" Width="100%" AutoGenerateColumns="false"
	runat="server" summary="Approve Feedback Design Table" GridLines="None" DataKeyField="FeedbackID">
	<ItemStyle CssClass="Normal" HorizontalAlign="center" VerticalAlign="Top" />
	<HeaderStyle CssClass="NormalBold" HorizontalAlign="center" />
	<Columns>
	    <asp:TemplateColumn>
	        <ItemStyle Width="25px" HorizontalAlign="center" VerticalAlign="Top" />
	        <HeaderTemplate>
	            <input type="CheckBox" name="SelectAllCheckBox" onclick="SelectAll(this)">
	        </HeaderTemplate>
	        <ItemTemplate><asp:CheckBox id="chkSelected" runat="server" ></asp:CheckBox></ItemTemplate>
	    </asp:TemplateColumn>
		<asp:TemplateColumn>
		    <HeaderStyle HorizontalAlign="Left" />
		    <ItemStyle Width="200px" HorizontalAlign="Left" />
		    <HeaderTemplate><asp:Label ID="lblFeedback" runat="Server" ResourceKey="Feedback.Header" /></HeaderTemplate>
		    <ItemTemplate><a href="<%# GetFeedbackUrl(DataBinder.Eval(Container.DataItem, "FeedbackID").ToString()) %>" class="Normal" target="_blank"><%#GetFeedbackTitle(Container.DataItem)%></a></ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn HeaderText="Details" DataField="Details" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />
		<asp:TemplateColumn>
		    <ItemStyle Width="100px" />
		    <HeaderTemplate><asp:Label ID="lblHeader" runat="Server" ResourceKey="Author.Header" /></HeaderTemplate>
		    <ItemTemplate><span class="Normal"><%#GetAuthor(Container.DataItem)%></span></ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn HeaderText="Date" DataField="CreateDate" ItemStyle-Width="100px" DataFormatString="{0:d}" />
	</Columns>
</asp:datagrid>
<p align="center">
	<asp:linkbutton id="cmdApprove" resourcekey="cmdApprove" runat="server" cssclass="CommandButton" text="Approve" causesvalidation="False" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdReject" resourcekey="cmdReject" runat="server" cssclass="CommandButton" text="Reject" borderstyle="none" />
</p>
</asp:PlaceHolder>

<asp:label id="lblNoFeedback" ResourceKey="NoFeedback" Runat="server" CssClass="Normal" Visible="False" />