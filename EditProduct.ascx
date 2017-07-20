<%@ Control language="vb" Inherits="Ventrian.FeedbackCenter.EditProduct" CodeBehind="EditProduct.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table class="Settings" cellspacing="2" cellpadding="2" summary="Edit Product Design Table" border="0" width="100%">
  <tr>
    <td width="100%" valign="top">
		<asp:panel id="pnlSettings" runat="server" cssclass="WorkPanel" visible="True">
		<dnn:sectionhead id=dshProduct cssclass="Head" runat="server" text="Product Settings" section="tblProduct" resourcekey="ProductSettings" includerule="True"></dnn:sectionhead>
      <TABLE id=tblProduct cellSpacing=0 cellPadding=2 width="100%" 
      summary="Product Settings Design Table" border=0 runat="server">
        <TR>
          <TD colSpan=3>
               <asp:label id=lblProductSettingsHelp cssclass="Normal" runat="server" resourcekey="ProductSettingsDescription" enableviewstate="False"></asp:label></TD></TR>
         <tr vAlign=top>
          <TD width=25><IMG height=1 
            src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width=25></TD>
          <TD class=SubHead noWrap width=150>
<dnn:label id="plParentProduct" runat="server" resourcekey="ParentProduct" suffix=":" controlname="drpParentProduct"></dnn:label></TD>
          <TD align=left width="100%">
<asp:DropDownList ID="drpParentProduct" Runat="server" DataTextField="Name" DataValueField="ProductID" />
<asp:CustomValidator id="valInvalidParentProduct" runat="server" ErrorMessage="<br>Invalid Parent Product. Possible Loop Detected."
								ResourceKey="valInvalidParentProduct" ControlToValidate="drpParentProduct" CssClass="NormalRed" Display="Dynamic"></asp:CustomValidator>
</TD></tr>

        <TR vAlign=top>
          <TD width=25><IMG height=1 
            src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width=25></TD>
          <TD class=SubHead noWrap width=150>
<dnn:label id=plName runat="server" resourcekey="Name" suffix=":" controlname="txtName"></dnn:label></TD>
          <TD align=left width="100%">
<asp:textbox id=txtName cssclass="NormalTextBox" runat="server" maxlength="100" columns="30" width="325"></asp:textbox>
<asp:requiredfieldvalidator id=valName cssclass="NormalRed" runat="server" resourcekey="valName" controltovalidate="txtName" errormessage="<br>You Must Enter a Valid Name" display="Dynamic"></asp:requiredfieldvalidator></TD></TR>
        <TR vAlign=top>
          <TD width=25><IMG height=1 
            src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width=25></TD>
          <TD class=SubHead noWrap width=150>
<dnn:label id=plIsActive runat="server" resourcekey="IsActive" suffix=":" controlname="chkIsActive"></dnn:label></TD>
          <TD align=left width="100%">
<asp:CheckBox id=chkIsActive cssclass="NormalTextBox" runat="server" Checked="True"></asp:CheckBox></TD></TR>

  <tr vAlign=top>
          <TD width=25><IMG height=1 
            src='<%= Page.ResolveUrl("~/Images/Spacer.gif") %>' width=25></TD>
          <TD class=SubHead noWrap width=150>
<dnn:label id="plEmailNotification" runat="server" resourcekey="EmailNotification" suffix=":" controlname="txtEmailNotification"></dnn:label></TD>
          <TD align=left width="100%">
<asp:textbox id=txtEmailNotification cssclass="NormalTextBox" runat="server" maxlength="100" columns="30" width="325"></asp:textbox>
</TD></tr>
	        <tr>
				<td width="25"></td>
	            <td class="SubHead" width="150"><dnn:label id="plInheritSecurity" resourcekey="InheritSecurity" runat="server" controlname="chkInheritSecurity" suffix=":"></dnn:label></td>
	            <td>
	                <asp:CheckBox ID="chkInheritSecurity" runat="server" AutoPostBack="true" />
	            </td>
	        </tr>
	        <tr runat="server" id="trPermissions">
	            <td width="25"></td>
	            <td class="SubHead" width="150"><dnn:label id="plPermissions" resourcekey="Permissions" runat="server" controlname="chkPermissions" suffix=":"></dnn:label></td>
	            <td>
                    <asp:DataGrid ID="grdProductPermissions" Runat="server" AutoGenerateColumns="False" ItemStyle-CssClass="Normal"
                        ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center"
                        HeaderStyle-CssClass="NormalBold" CellSpacing="0" CellPadding="0" GridLines="None" BorderWidth="1"
                        BorderStyle="None" DataKeyField="Value">
                        <Columns>
                            <asp:TemplateColumn>
	                            <ItemStyle HorizontalAlign="Left" Wrap="False"/>
	                            <ItemTemplate>
		                            <%# DataBinder.Eval(Container.DataItem, "Text") %>
	                            </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
	                            <HeaderTemplate>
		                            &nbsp;
		                            <asp:Label ID="lblView" Runat="server" EnableViewState="False" ResourceKey="View" />&nbsp;
	                            </HeaderTemplate>
	                            <ItemTemplate>
		                            <asp:CheckBox ID="chkView" Runat="server" />
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
                        </Columns>
                    </asp:DataGrid>
	            </td>
	        </tr>
		</TABLE>
      </asp:panel>
    </td>
  </tr>
</table>
<p>
	<asp:linkbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="CommandButton" text="Update" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" text="Cancel" causesvalidation="False" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdDelete" resourcekey="cmdDelete" runat="server" cssclass="CommandButton" text="Delete" causesvalidation="False" borderstyle="none" />	
</p>
