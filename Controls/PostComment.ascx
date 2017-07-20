<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PostComment.ascx.vb" Inherits="Ventrian.FeedbackCenter.Controls.PostComment" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<asp:Panel ID="pnlPostComment" Runat="server">
	<asp:textbox id="txtComment" cssclass="NormalTextBox" runat="server" width="500" maxlength="500"
		rows="6" textmode="MultiLine"></asp:textbox>
	<asp:requiredfieldvalidator id="valComment" cssclass="NormalRed" runat="server" resourcekey="valComment.ErrorMessage"
		controltovalidate="txtComment" errormessage="<br>Comment Is Required" display="Dynamic" ValidationGroup="FeedbackCenterComments"></asp:requiredfieldvalidator>
	<asp:PlaceHolder ID="phAnonymousUser" runat="server">
	<br />
	<br />
    <asp:textbox id="txtName" cssclass="NormalTextBox" runat="server" width="300" />&nbsp;
    <asp:Label ID="lblName" runat="server" CssClass="Normal" Text="Name (required)" />
    <asp:requiredfieldvalidator id="valName" cssclass="NormalRed" runat="server" 
                ControlToValidate="txtName" Display="Dynamic" ResourceKey="valName.ErrorMessage" ErrorMessage="Name Is Required" SetFocusOnError="true" ValidationGroup="FeedbackCenterComments" />
	<br />
    <asp:textbox id="txtEmail" CssClass="NormalTextBox" runat="server" width="300" />&nbsp;
    <asp:Label ID="lblEmail" runat="server" CssClass="Normal" Text="Email (required)" />
    <asp:requiredfieldvalidator id="valEmail" cssclass="NormalRed" runat="server" 
        controltovalidate="txtEmail" display="Dynamic" ResourceKey="valEmail.ErrorMessage" ErrorMessage="Email Is Required" SetFocusOnError="true" ValidationGroup="FeedbackCenterComments" />
    <asp:RegularExpressionValidator ID="valEmailIsValid" CssClass="NormalRed" runat="server"
        ControlToValidate="txtEmail" Display="Dynamic" ResourceKey="valEmailIsValid.ErrorMessage" ErrorMessage="Invalid Email Address" SetFocusOnError="true" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$" ValidationGroup="FeedbackCenterComments" />    
	<br />
    <asp:textbox id="txtURL" cssclass="NormalTextBox" runat="server" width="300" />&nbsp;
    <asp:Label ID="lblUrl" runat="server" CssClass="Normal" Text="Website" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phAttachment" runat="server">
    <br />
    <asp:FileUpload ID="txtCommentAttachment" runat="server" CssClass="Button" />&nbsp;
    <asp:Label ID="lblAttachment" runat="server" CssClass="Normal" Text="Attachment" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phCaptcha" runat="server">
    <br />
    <br />
    <dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" runat="server" errorstyle-cssclass="NormalRed" />
    </asp:PlaceHolder>
    <br />
	<asp:Button ID="btnPostComment" Runat="server" ResourceKey="btnPostComment" CssClass="Button" ValidationGroup="FeedbackCenterComments" />
</asp:Panel>
<asp:Label ID="lblPostCommentNotAuthorized" Runat="server" CssClass="Normal" ResourceKey="PostCommentNotAuthorized" Visible="False" />
