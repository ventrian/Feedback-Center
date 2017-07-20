<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditFeedback.ascx.vb" Inherits="Ventrian.FeedbackCenter.EditFeedback" %>
<%@ Register TagPrefix="Feedback" TagName="Approval" Src="Controls/Approval.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
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
<br style="line-height: 5px;" />

<asp:PlaceHolder ID="phForm" runat="server">
<table class="Settings" cellspacing="2" cellpadding="2" summary="Edit Feedback Design Table" border="0" width="100%">
	<tr>
		<td width="100%" valign="top">
		    <dnn:sectionhead id="dshSuggestion" cssclass="Head" runat="server" includerule="True" resourcekey="SuggestionDetails"
			    section="tblSuggestion" text="Suggestion Details"></dnn:sectionhead>
		    <table id="tblSuggestion" cellSpacing="0" cellPadding="2" width="100%" summary="Suggestion Settings Design Table"
			    border="0" runat="server">
			    <tr valign="top" runat="server" visible="false" ID="Tr2">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
					    <dnn:label id="plType" runat="server" resourcekey="Type" suffix=":" controlname="lstType"></dnn:label>
                    </td>
				    <td align="left" width="100%">
				        <asp:RadioButtonList ID="lstTypes" Runat="server" DataValueField="TypeID" DataTextField="Name" CssClass="NormalTextBox" />
				    </td>
			    </tr>
			    <tr valign="top">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
				        <dnn:label id="plProduct" runat="server" resourcekey="Product" suffix=":" controlname="drpProduct"></dnn:label>
                    </td>
				    <td align="left" width="100%">
					    <asp:DropDownList ID="drpProducts" Runat="server" DataTextField="Name" DataValueField="ProductID"
						    CssClass="NormalTextBox"></asp:DropDownList>
					    <asp:requiredfieldvalidator id="valProducts" runat="server" cssclass="NormalRed" resourcekey="valProducts.ErrorMessage"
						    display="Dynamic" errormessage="<br>Product is Required" controltovalidate="drpProducts" InitialValue="-1"></asp:requiredfieldvalidator>
					    <asp:LinkButton ID="cmdEditProducts" Runat="server" ResourceKey="EditProducts" CssClass="CommandButton" CausesValidation="False" />
				    </td>
			    </tr>
			    <tr valign="top">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
					    <dnn:label id="plTitle" runat="server" resourcekey="Title" suffix=":" controlname="txtTitle"></dnn:label>
                    </td>
				    <td align="left" width="100%">
					    <asp:TextBox ID="txtTitle" Runat="server" MaxLength="100" CssClass="NormalTextBox" Width="300" />
					    <asp:requiredfieldvalidator id="valTitle" runat="server" cssclass="NormalRed" resourcekey="valTitle.ErrorMessage"
						    display="Dynamic" errormessage="<br>Title is Required" controltovalidate="txtTitle"></asp:requiredfieldvalidator>
				    </td>
			    </tr>
			    <tr valign="top">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
					    <dnn:label id="plDescription" runat="server" resourcekey="Description" suffix=":" controlname="txtDescription"></dnn:label>
				    </td>
				    <td align="left" width="100%">
					    <asp:TextBox ID="txtDescription" Runat="server" CssClass="NormalTextBox" Width="300" TextMode="MultiLine"
					        Rows="10" />
				        <asp:requiredfieldvalidator id="valDescription" runat="server" cssclass="NormalRed" resourcekey="valDescription.ErrorMessage"
					        display="Dynamic" errormessage="<br>Description is Required" controltovalidate="txtDescription"></asp:requiredfieldvalidator>
                    </td>
			    </tr>
            </table>
            
            <asp:repeater id="rptCustomFields" Runat="server">
                <HeaderTemplate>
                    <table id="Table1" cellspacing="0" cellpadding="2" summary="Custom Fields Design Table" border="0">
                </HeaderTemplate>
				<ItemTemplate>
					<tr valign="top" runat="server" id="trItem">
					    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
						<td class="SubHead" width="150" valign="middle">
							<label id="label" runat="server">
								<asp:linkbutton id="cmdHelp" tabindex="-1" runat="server" CausesValidation="False" enableviewstate="False">
									<asp:image id="imgHelp" tabindex="-1" runat="server" imageurl="~/images/help.gif" enableviewstate="False"></asp:image>
								</asp:linkbutton>
								<asp:label id="lblLabel" runat="server" enableviewstate="False"></asp:label>
							</label>
							<asp:panel id="pnlHelp" runat="server" cssClass="Help" enableviewstate="False">
								<asp:label id="lblHelp" runat="server" enableviewstate="False"></asp:label>
							</asp:panel>
						</td>
						<td align="left">
							<asp:PlaceHolder ID="phValue" Runat="server" />
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
			        </table>
				</FooterTemplate>
			</asp:repeater>
            
		    <table cellSpacing="0" cellPadding="2" width="100%" summary="Suggestion Settings Design Table"
			    border="0" runat="server">
			    <tr valign="top" runat="server" id="trStatus">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
				        <dnn:label id="plStatus" runat="server" resourcekey="Status" suffix=":" controlname="lstStatus"></dnn:label>
				    </td>
				    <td align="left" width="100%">
					    <asp:RadioButtonList ID="lstStatus" Runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal"
						    RepeatLayout="Flow">
						    <asp:ListItem Value="Open" Selected="True">Open</asp:ListItem>
						    <asp:ListItem Value="Closed">Closed</asp:ListItem>
					    </asp:RadioButtonList>
                    </td>
			    </tr>
			    <tr valign="top" runat="server" id="trImplemented">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
				        <dnn:label id="plImplemented" runat="server" resourcekey="Implemented" suffix=":" controlname="chkImplemented"></dnn:label>
				    </td>
				    <td align="left" width="100%">
					    <asp:CheckBox ID="chkImplemented" Runat="server" CssClass="NormalTextBox" />
                    </td>
			    </tr>
			    <tr valign="top" runat="server" id="trIsApproved">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
				        <dnn:label id="plIsApproved" runat="server" resourcekey="IsApproved" suffix=":" controlname="chkIsApproved"></dnn:label>
				    </td>
				    <td align="left" width="100%">
					    <asp:CheckBox ID="chkIsApproved" Runat="server" CssClass="NormalTextBox" />
                    </td>
			    </tr>
			    <tr valign="top" runat="server" id="trCaptcha">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
				        <dnn:label id="plCaptcha" runat="server" suffix=":" controlname="ctlCaptcha"></dnn:label>
				    </td>
				    <td align="left" width="100%">
                        <dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" runat="server" errorstyle-cssclass="NormalRed" />
                    </td>
			    </tr>
		    </table>
		    
		    <asp:PlaceHolder ID="phUser" runat="server" Visible="false">
		    <br /><br />
		     <dnn:sectionhead id="dshUser" cssclass="Head" runat="server" includerule="True" resourcekey="UserDetails"
			    section="tblUser" text="Your Details"></dnn:sectionhead>
		    <table id="tblUser" cellSpacing="0" cellPadding="2" width="100%" summary="User Details Settings Design Table"
			    border="0" runat="server">
			    <tr valign="top">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
					    <dnn:label id="plName" runat="server" suffix=":" controlname="txtName"></dnn:label>
				    </td>
				    <td align="left" width="100%">
					    <asp:textbox id="txtName" cssclass="NormalTextBox" runat="server" Width="300" />
                        <asp:requiredfieldvalidator id="valName" cssclass="NormalRed" runat="server" 
                                    ControlToValidate="txtName" Display="Dynamic" ErrorMessage="Name Is Required" ResourceKey="valName.ErrorMessage" SetFocusOnError="true" />
                    </td>
			    </tr>
			    <tr valign="top">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
					     <dnn:label id="plEmail" runat="server" suffix=":" controlname="txtEmail"></dnn:label>
				    </td>
				    <td align="left" width="100%">
					    <asp:textbox id="txtEmail" CssClass="NormalTextBox" runat="server" Width="300" />
                        <asp:requiredfieldvalidator id="valEmail" cssclass="NormalRed" runat="server" 
                            controltovalidate="txtEmail" display="Dynamic" ErrorMessage="Email Is Required" ResourceKey="valEmail.ErrorMessage" SetFocusOnError="true" />
                        <asp:RegularExpressionValidator ID="valEmailIsValid" CssClass="NormalRed" runat="server"
                            ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Invalid Email Address" ResourceKey="valEmailIsValid.ErrorMessage" SetFocusOnError="true" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$" />    
                    </td>
			    </tr>
			    <tr valign="top">
				    <td width="25"><img height="1" src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width="25" alt="Spacer" /></td>
				    <td class="SubHead" noWrap width="150">
					    <dnn:label id="plUrl" runat="server" suffix=":" controlname="txtURL"></dnn:label>
				    </td>
				    <td align="left" width="100%">
					    <asp:textbox id="txtURL" cssclass="NormalTextBox" runat="server" Width="300" />
                    </td>
			    </tr>
            </table>
            </asp:PlaceHolder>
		</td>
	</tr>
</table>
</asp:PlaceHolder>

<asp:PlaceHolder ID="phFormTemplate" runat="server" Visible="False"></asp:PlaceHolder>

<p align="center">
	<asp:linkbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="CommandButton" text="Update"
		borderstyle="none" />&nbsp;
	<asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" text="Cancel"
		causesvalidation="False" borderstyle="none" />&nbsp;
	<asp:linkbutton id="cmdDelete" resourcekey="cmdDelete" runat="server" cssclass="CommandButton" text="Delete"
		causesvalidation="False" borderstyle="none" />
</p>
