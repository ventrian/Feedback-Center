'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System.IO
Imports System.Web
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities
Imports DotNetNuke.Security.Roles

Namespace Ventrian.FeedbackCenter

    Public MustInherit Class EditProduct

        Inherits FeedbackCenterBase

#Region " Controls "

        Protected WithEvents lblProductSettingsHelp As System.Web.UI.WebControls.Label
        Protected WithEvents txtName As System.Web.UI.WebControls.TextBox
        Protected WithEvents valName As System.Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents chkIsActive As System.Web.UI.WebControls.CheckBox
        Protected WithEvents pnlSettings As System.Web.UI.WebControls.Panel
        Protected WithEvents cmdUpdate As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdCancel As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdDelete As System.Web.UI.WebControls.LinkButton
        Protected WithEvents tblProduct As System.Web.UI.HtmlControls.HtmlTable
        Protected WithEvents rptBreadCrumbs As System.Web.UI.WebControls.Repeater
        Protected WithEvents drpParentProduct As System.Web.UI.WebControls.DropDownList
        Protected WithEvents valInvalidParentProduct As System.Web.UI.WebControls.CustomValidator
        Protected WithEvents txtEmailNotification As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkInheritSecurity As System.Web.UI.WebControls.CheckBox
        Protected WithEvents trPermissions As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents grdProductPermissions As System.Web.UI.WebControls.DataGrid


#End Region

#Region " Private Members "

        Private _productID As Integer = Null.NullInteger
        Private _parentID As Integer = Null.NullInteger

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If Not (Request("ProductID") Is Nothing) Then
                _productID = Convert.ToInt32(Request("ProductID"))
            End If

            If Not (Request("ParentID") Is Nothing) Then
                _parentID = Convert.ToInt32(Request("ParentID"))
            End If

        End Sub

        Private Sub BindProduct()

            If (_productID = Null.NullInteger) Then

                cmdDelete.Visible = False

            Else

                cmdDelete.Visible = True
                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('Are You Sure You Wish To Delete This Item ?');")

                Dim objProductController As New ProductController
                Dim objProduct As ProductInfo = objProductController.Get(_productID)

                If Not (objProduct Is Nothing) Then
                    txtName.Text = objProduct.Name
                    chkIsActive.Checked = objProduct.IsActive
                    If (drpParentProduct.Items.FindByValue(objProduct.ParentID.ToString) IsNot Nothing) Then
                        drpParentProduct.SelectedValue = objProduct.ParentID.ToString()
                    End If
                    txtEmailNotification.Text = objProduct.Email
                    chkInheritSecurity.Checked = objProduct.InheritSecurity
                    trPermissions.Visible = Not chkInheritSecurity.Checked
                End If

            End If

        End Sub

        Private Sub BindParentProducts()

            Dim objProductController As New ProductController

            drpParentProduct.DataSource = objProductController.ListAll(ModuleId)
            drpParentProduct.DataBind()

            drpParentProduct.Items.Insert(0, New ListItem(Localization.GetString("SelectParentProduct", Me.LocalResourceFile), "-1"))

            If (_parentID <> Null.NullInteger) Then
                If (drpParentProduct.Items.FindByValue(_parentID.ToString()) IsNot Nothing) Then
                    drpParentProduct.SelectedValue = _parentID.ToString()
                End If
            End If

        End Sub

        Private Sub BindRoles()

            Dim objRole As New RoleController
            Dim availableRoles As New ArrayList
            Dim roles As ArrayList = objRole.GetPortalRoles(PortalId)

            If Not roles Is Nothing Then
                For Each Role As RoleInfo In roles
                    availableRoles.Add(New ListItem(Role.RoleName, Role.RoleName))
                Next
            End If

            grdProductPermissions.DataSource = availableRoles
            grdProductPermissions.DataBind()

        End Sub

        Private Function IsInRole(ByVal roleName As String, ByVal roles As String()) As Boolean

            For Each role As String In roles
                If (roleName = role) Then
                    Return True
                End If
            Next

            Return False

        End Function

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ReadQueryString()

            If (IsPostBack = False) Then

                BindRoles()
                BindParentProducts()
                BindProduct()

            End If

        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            If (Page.IsValid) Then

                Dim objProductController As New ProductController

                Dim objProduct As New ProductInfo

                If (_productID <> Null.NullInteger) Then

                    objProduct = objProductController.Get(_productID)

                Else

                    objProduct = CType(CBO.InitializeObject(objProduct, GetType(ProductInfo)), ProductInfo)

                End If

                objProduct.ModuleID = Me.ModuleId
                objProduct.Name = txtName.Text
                objProduct.IsActive = chkIsActive.Checked
                objProduct.ParentID = Convert.ToInt32(drpParentProduct.SelectedValue)
                objProduct.Email = txtEmailNotification.Text
                objProduct.InheritSecurity = chkInheritSecurity.Checked

                If (_productID = Null.NullInteger) Then
                    objProduct.ProductID = objProductController.Add(objProduct)
                Else
                    objProductController.Update(objProduct)
                End If

                Dim viewRoles As String = ""
                Dim submitRoles As String = ""

                For Each item As DataGridItem In grdProductPermissions.Items
                    Dim role As String = grdProductPermissions.DataKeys(item.ItemIndex).ToString()

                    Dim chkView As CheckBox = CType(item.FindControl("chkView"), CheckBox)
                    If (chkView.Checked) Then
                        If (viewRoles = "") Then
                            viewRoles = role
                        Else
                            viewRoles = viewRoles & ";" & role
                        End If
                    End If

                    Dim chkSubmit As CheckBox = CType(item.FindControl("chkSubmit"), CheckBox)
                    If (chkSubmit.Checked) Then
                        If (submitRoles = "") Then
                            submitRoles = role
                        Else
                            submitRoles = submitRoles & ";" & role
                        End If
                    End If
                Next

                Dim objModuleController As New ModuleController()
                objModuleController.UpdateModuleSetting(Me.ModuleId, objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING, viewRoles)
                objModuleController.UpdateModuleSetting(Me.ModuleId, objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_SUBMIT_SETTING, submitRoles)


                Response.Redirect(EditUrl("EditProducts"), True)

            End If

        End Sub

        Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click

            Dim objProductController As New ProductController
            objProductController.Delete(_productID)

            Response.Redirect(EditUrl("EditProducts"), True)

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click

            Response.Redirect(EditUrl("EditProducts"), True)

        End Sub

        Private Sub valInvalidParentProduct_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valInvalidParentProduct.ServerValidate

            Try

                If (_productID = Null.NullInteger Or drpParentProduct.SelectedValue = "-1") Then
                    args.IsValid = True
                    Return
                End If

                Dim objProductController As New ProductController
                Dim objProduct As ProductInfo = objProductController.Get(Convert.ToInt32(drpParentProduct.SelectedValue))

                While Not objProduct Is Nothing
                    If (_productID = objProduct.ProductID) Then
                        args.IsValid = False
                        Return
                    End If
                    objProduct = objProductController.Get(objProduct.ParentID)
                End While

                args.IsValid = True

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub grdProductPermissions_ItemDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdProductPermissions.ItemDataBound

            Try

                If (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then
                    Dim objListItem As ListItem = CType(e.Item.DataItem, ListItem)

                    If Not (objListItem Is Nothing) Then

                        Dim role As String = CType(e.Item.DataItem, ListItem).Value

                        Dim chkView As CheckBox = CType(e.Item.FindControl("chkView"), CheckBox)
                        Dim chkSubmit As CheckBox = CType(e.Item.FindControl("chkSubmit"), CheckBox)

                        If (objListItem.Value = PortalSettings.AdministratorRoleName.ToString()) Then
                            chkView.Enabled = False
                            chkView.Checked = True
                            chkSubmit.Enabled = False
                            chkSubmit.Checked = True
                        Else
                            If (_productID <> Null.NullInteger) Then
                                If (Settings.Contains(_productID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING)) Then
                                    chkView.Checked = IsInRole(role, Settings(_productID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING).ToString().Split(";"c))
                                End If
                                If (Settings.Contains(_productID.ToString() & "-" & Constants.PERMISSION_CATEGORY_SUBMIT_SETTING)) Then
                                    chkSubmit.Checked = IsInRole(role, Settings(_productID.ToString() & "-" & Constants.PERMISSION_CATEGORY_SUBMIT_SETTING).ToString().Split(";"c))
                                End If
                            End If
                        End If

                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Protected Sub chkInheritSecurity_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkInheritSecurity.CheckedChanged

            Try

                trPermissions.Visible = Not chkInheritSecurity.Checked

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

        End Sub

#End Region

    End Class

End Namespace