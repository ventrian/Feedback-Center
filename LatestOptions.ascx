<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="LatestOptions.ascx.vb" Inherits="Ventrian.FeedbackCenter.LatestOptions" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>

<table id="tblLatestCommentsDetail" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
	border="0" runat="server">
	<tr valign="top">
		<td class="SubHead" width="150">
			<dnn:label id="plModuleID" runat="server" resourcekey="Module" suffix=":" controlname="drpModuleID"></dnn:label></td>
		<td align="left" width="325">
			<asp:dropdownlist id="drpModuleID" Runat="server" Width="325" datavaluefield="ModuleID" datatextfield="ModuleTitle" 
				CssClass="NormalTextBox" AutoPostBack="True"></asp:dropdownlist></td>
	</tr>
    <tr valign="top">
        <td class="SubHead" width="150">
	        <dnn:label id="plFeedbackCount" runat="server" resourcekey="Count" suffix=":" controlname="txtFeedbackCount"></dnn:label></td>
        <td align="left">
	        <asp:textbox id="txtFeedbackCount" Runat="server" Width="50" CssClass="NormalTextBox">10</asp:textbox>
	        <asp:RequiredFieldValidator id="valCount" runat="server" ResourceKey="valCount.ErrorMessage" ErrorMessage="<br>* Required"
		        Display="Dynamic" ControlToValidate="txtFeedbackCount" CssClass="NormalRed"></asp:RequiredFieldValidator>
	        <asp:CompareValidator id="valCountType" runat="server" ResourceKey="valCountType.ErrorMessage" ErrorMessage="<br>* Must be a Number"
		        Display="Dynamic" ControlToValidate="txtFeedbackCount" Type="Integer" Operator="DataTypeCheck" CssClass="NormalRed"></asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plQueryStringFilter" resourcekey="plQueryStringFilter" runat="server" controlname="chkQueryStringFilter"></dnn:label></td>
        <td valign="top"><asp:checkbox id="chkQueryStringFilter" Runat="server" CssClass="NormalTextBox"></asp:checkbox></td>
    </tr>
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plQueryStringParam" resourcekey="plQueryStringParam" runat="server" controlname="txtQueryStringParam"></dnn:label></td>
        <td valign="top"><asp:textbox id="txtQueryStringParam" Runat="server" Width="150" CssClass="NormalTextBox"></asp:textbox></td>
    </tr>
	<tr>
	    <td colspan="2">
            <br />
	        <dnn:sectionhead id="dshTemplate" runat="server" cssclass="Head" includerule="True" isExpanded="false"
	            resourcekey="Template" section="tblTemplate" text="Template" />
	        <table id="tblTemplate" runat="server" cellspacing="0" cellpadding="2" width="100%" summary="Template Design Table" border="0">
            <TR vAlign="top">
	            <TD class="SubHead" width="150">
		            <dnn:label id="plHtmlHeader" runat="server" resourcekey="HtmlHeader" suffix=":" controlname="txtHtmlHeader"></dnn:label></TD>
	            <TD align="left" width="325">
		            <asp:textbox id="txtHtmlHeader" cssclass="NormalTextBox" runat="server" Rows="2" TextMode="MultiLine"
			            maxlength="50" width="300"></asp:textbox></TD>
            </TR>
	        <TR valign="top">
		        <TD class="SubHead" width="150">
			        <dnn:label id="plHtmlBody" runat="server" resourcekey="HtmlBody" suffix=":" controlname="txtHtmlBody"></dnn:label></TD>
		        <TD align="left" width="325">
			        <asp:textbox id="txtHtmlBody" cssclass="NormalTextBox" runat="server" Rows="6" TextMode="MultiLine"
				        maxlength="50" width="300"></asp:textbox></TD>
	        </TR>
	        <TR valign="top">
		        <TD class="SubHead" width="150">
			        <dnn:label id="plHtmlFooter" runat="server" resourcekey="HtmlFooter" suffix=":" controlname="txtHtmlFooter"></dnn:label></TD>
		        <TD align="left" width="325">
			        <asp:textbox id="txtHtmlFooter" cssclass="NormalTextBox" runat="server" Rows="2" TextMode="MultiLine"
				        maxlength="50" width="300"></asp:textbox></TD>
	        </TR>
	        <TR valign="top">
		        <TD class="SubHead" width="150">
			        <dnn:label id="plHtmlNoFeedback" runat="server" resourcekey="HtmlNoFeedback" suffix=":" controlname="txtHtmlNoFeedback"></dnn:label></TD>
		        <TD align="left" width="325">
			        <asp:textbox id="txtHtmlNoFeedback" cssclass="NormalTextBox" runat="server" Rows="6" TextMode="MultiLine"
				        maxlength="50" width="300"></asp:textbox></TD>
	        </TR>
            </table>
	        <dnn:sectionhead id="dshTokens" runat="server" cssclass="Head" includerule="True" isExpanded="false"
	            resourcekey="Tokens" section="tblTokens" text="Template" />
            <table id="tblTokens" runat="server" cellspacing="0" cellpadding="2" width="100%"
                summary="Tokens Help Design Table" border="0">
            <tr valign="top">
                <td class="SubHead" width="150">[CREATEDATE]</td>
                <td class="Normal" align="left">
                    <asp:label id="lblCreateDate" resourcekey="CreateDate" cssclass="Normal" runat="server" enableviewstate="False"></asp:label>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead" width="150">[LINK]</td>
                <td class="Normal" align="left">
                    <asp:label id="lblLink" resourcekey="Link" cssclass="Normal" runat="server" enableviewstate="False"></asp:label>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead" width="150">[PRODUCT]</td>
                <td class="Normal" align="left">
                    <asp:label id="lblProduct" resourcekey="Product" cssclass="Normal" runat="server" enableviewstate="False"></asp:label>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead" width="150">[TITLE]</td>
                <td class="Normal" align="left">
                    <asp:label id="lblTitle" resourcekey="Title" cssclass="Normal" runat="server" enableviewstate="False"></asp:label>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead" width="150">[VOTETOTAL]</td>
                <td class="Normal" align="left">
                    <asp:label id="lblVoteTotal" resourcekey="VoteTotal" cssclass="Normal" runat="server" enableviewstate="False"></asp:label>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead" width="150">[VOTETOTALNEGATIVE]</td>
                <td class="Normal" align="left">
                    <asp:label id="lblVoteTotalNegative" resourcekey="VoteTotalNegative" cssclass="Normal" runat="server" enableviewstate="False"></asp:label>
                </td>
            </tr>
            </table>
            
	    </td>
	</tr>
</table>
