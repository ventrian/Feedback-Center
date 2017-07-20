'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System.IO
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities
Imports Ventrian.FeedbackCenter.Entities.CustomFields
Imports DotNetNuke.UI.UserControls
Imports Ventrian.FeedbackCenter.Entities.Emails
Imports Ventrian.FeedbackCenter.Entities.Layout

Namespace Ventrian.FeedbackCenter


    Partial Public Class EditFeedback
        Inherits FeedbackCenterBase

#Region " Private Members "

        Private _feedbackID As Integer = Null.NullInteger
        Private _productID As Integer = Null.NullInteger
        Private _richTextValues As New NameValueCollection

#End Region

#Region " Private Methods "

        Private Sub BindCustomFields()

            Dim objCustomFieldController As New CustomFieldController()
            Dim objCustomFields As ArrayList = objCustomFieldController.List(Me.ModuleId)

            If (objCustomFields.Count = 0) Then
                rptCustomFields.Visible = False
            Else
                rptCustomFields.DataSource = objCustomFields
                rptCustomFields.DataBind()
            End If

        End Sub

        Private Sub CheckSecurity()

            If (Settings.Contains(Constants.PERMISSION_SUBMIT_SETTING)) Then
                If (PortalSecurity.IsInRoles(FeedbackSettings.PermissionSubmit) = False) Then
                    If (FeedbackSettings.PermissionSubmit.ToLower().Contains(glbRoleUnauthUserName.ToLower()) = False) Then
                        Response.Redirect(NavigateURL(Me.TabId, "", "fbType=NotAuthorized"), True)
                    End If
                End If
            Else
                If (Request.IsAuthenticated = False) Then
                    Response.Redirect(NavigateURL(Me.TabId, "", "fbType=NotAuthorized"), True)
                End If
            End If

        End Sub

        Private Sub ReadQueryString()

            If Not (Request("FeedbackID") Is Nothing) Then
                _feedbackID = Convert.ToInt32(Request("FeedbackID"))
            End If

            If Not (Request("ProductID") Is Nothing) Then
                _productID = Convert.ToInt32(Request("ProductID"))
            End If

        End Sub

        Private Sub SetVisibility()

            trStatus.Visible = HasEditPermissions()
            trImplemented.Visible = HasEditPermissions()
            trCaptcha.Visible = FeedbackSettings.EnableCaptcha
            If (_feedbackID <> Null.NullInteger) Then
                trCaptcha.Visible = False
            End If
            cmdEditProducts.Visible = HasEditPermissions()

        End Sub

        Private Sub BindTypes()

            Dim objTypeController As New TypeController

            lstTypes.DataSource = objTypeController.List()
            lstTypes.DataBind()

            If (lstTypes.Items.Count > 0) Then
                lstTypes.Items(0).Selected = True
            End If

        End Sub

        Private Sub BindProducts(ByRef drpProductsList As DropDownList)

            Dim objProductController As New ProductController

            Dim objProducts As ArrayList = objProductController.ListAll(Me.ModuleId)
            Dim objProductsToSelect As New ArrayList

            For Each objProduct As ProductInfo In objProducts
                If (objProduct.InheritSecurity) Then
                    objProductsToSelect.Add(objProduct)
                Else
                    If (Settings.Contains(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_SUBMIT_SETTING)) Then
                        If (PortalSecurity.IsInRoles(Settings(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_SUBMIT_SETTING).ToString())) Then
                            objProductsToSelect.Add(objProduct)
                        End If
                    End If
                End If
            Next

            drpProductsList.DataSource = objProductsToSelect
            drpProductsList.DataBind()

            If (_productID <> Null.NullInteger) Then
                If Not (drpProducts.Items.FindByValue(_productID.ToString) Is Nothing) Then
                    drpProductsList.SelectedValue = _productID.ToString
                End If
            Else
                If (drpProductsList.Items.Count = 1) Then
                    drpProductsList.SelectedIndex = 0
                End If
            End If

            drpProductsList.Items.Insert(0, New ListItem(Localization.GetString("AllProducts", LocalResourceFile), Null.NullInteger.ToString()))

        End Sub

        Private Sub BindFeedback()

            If (IsEditable Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove)) Then
                trIsApproved.Visible = True
            Else
                trIsApproved.Visible = False
            End If

            If (_feedbackID = Null.NullInteger) Then

                If (Settings.Contains(Constants.PERMISSION_AUTO_APPROVE_SETTING)) Then
                    If (IsEditable Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove) Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionAutoApprove)) Then
                        chkIsApproved.Checked = True
                    Else
                        If (FeedbackSettings.PermissionAutoApprove.ToLower().Contains(glbRoleUnauthUserName.ToLower()) = False) Then
                            chkIsApproved.Checked = False
                        Else
                            chkIsApproved.Checked = True
                        End If
                    End If
                Else
                    ' Auto-Approve
                    chkIsApproved.Checked = True
                End If

                phUser.Visible = Not Request.IsAuthenticated
                cmdDelete.Visible = False
                cmdUpdate.Text = Localization.GetString("Submit", LocalResourceFile)

            Else

                cmdDelete.Visible = True
                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('Are You Sure You Wish To Delete This Item ?');")
                cmdUpdate.Text = Localization.GetString("cmdUpdate")

                Dim objFeedbackController As New FeedbackController
                Dim objFeedback As FeedbackInfo = objFeedbackController.Get(_feedbackID)

                If Not (objFeedback Is Nothing) Then

                    If (IsEditable = False) Then
                        If (PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove) = False) Then
                            If (FeedbackSettings.PermissionApprove.ToLower().Contains(glbRoleUnauthUserName.ToLower()) = False) Then
                                Response.Redirect(NavigateURL(Me.TabId, "", "fbType=NotAuthorized"), True)
                            End If
                        End If
                    End If

                    If (objFeedback.UserID = Null.NullInteger) Then
                        phUser.Visible = True
                    End If

                    If Not (lstTypes.Items.FindByValue(objFeedback.TypeID) Is Nothing) Then
                        lstTypes.Items.FindByValue(objFeedback.TypeID).Selected = True
                    End If

                    If Not (drpProducts.Items.FindByValue(objFeedback.ProductID) Is Nothing) Then
                        drpProducts.Items.FindByValue(objFeedback.ProductID).Selected = True
                    End If

                    txtTitle.Text = objFeedback.Title
                    txtDescription.Text = objFeedback.Details.Replace("<br />", vbCrLf)
                    txtDescription.Text = objFeedback.Details.Replace("<br>", vbCrLf)

                    If (objFeedback.IsClosed) Then
                        lstStatus.SelectedValue = "Closed"
                    Else
                        lstStatus.SelectedValue = "Open"
                    End If

                    chkImplemented.Checked = objFeedback.IsResolved
                    chkIsApproved.Checked = objFeedback.IsApproved

                    txtName.Text = objFeedback.AnonymousName
                    txtEmail.Text = objFeedback.AnonymousEmail
                    txtURL.Text = objFeedback.AnonymousUrl

                End If

            End If

        End Sub

        Private Function FormatEmail(ByVal template As String, ByVal link As String, ByVal objFeedback As FeedbackInfo) As String

            Dim formatted As String = template

            formatted = formatted.Replace("[PORTALNAME]", PortalSettings.PortalName)
            formatted = formatted.Replace("[POSTDATE]", DateTime.Now.ToShortDateString & " " & DateTime.Now.ToShortTimeString)

            formatted = formatted.Replace("[TITLE]", objFeedback.Title)
            formatted = formatted.Replace("[LINK]", link)

            Return formatted

        End Function

        Private Function GetApproverDistributionList() As String

            Dim distributionList As String = ""

            If (Settings.Contains(Constants.PERMISSION_APPROVE_SETTING)) Then

                Dim roles As String = Settings(Constants.PERMISSION_APPROVE_SETTING).ToString()
                Dim rolesArray() As String = roles.Split(Convert.ToChar(";"))
                Dim userList As Hashtable = New Hashtable

                For Each role As String In rolesArray
                    If (role.Length > 0) Then
                        Dim objRoleController As RoleController = New RoleController
                        Dim objRole As RoleInfo = objRoleController.GetRoleByName(PortalId, role)

                        If Not (objRole Is Nothing) Then
                            Dim objUsers As ArrayList = objRoleController.GetUserRolesByRoleName(PortalId, objRole.RoleName)
                            For Each objUser As UserRoleInfo In objUsers
                                Dim objUserController As UserController = New UserController
                                Dim objSelectedUser As UserInfo = objUserController.GetUser(PortalId, objUser.UserID)
                                If Not (objSelectedUser Is Nothing) Then
                                    If (objSelectedUser.Membership.Email.Length > 0) Then
                                        If (userList.Contains(objSelectedUser.Membership.Email) = False) Then
                                            userList.Add(objSelectedUser.Membership.Email, objSelectedUser.Membership.Email)
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next

                For Each email As DictionaryEntry In userList
                    If (distributionList.Length > 0) Then
                        distributionList += ";"
                    End If
                    distributionList += email.Value.ToString()
                Next

            Else

                distributionList = PortalSettings.Email

            End If

            Return distributionList

        End Function

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim crumbAllAlbums As New CrumbInfo
            crumbAllAlbums.Caption = Localization.GetString("AllFeedback", LocalResourceFile)
            crumbAllAlbums.Url = NavigateURL()
            crumbs.Add(crumbAllAlbums)

            Dim currentCrumb As New CrumbInfo
            If (_feedbackID = Null.NullInteger) Then
                currentCrumb.Caption = Localization.GetString("AddNewFeedback", LocalResourceFile)
            Else
                currentCrumb.Caption = Localization.GetString("EditFeedback", LocalResourceFile)
            End If
            currentCrumb.Url = Request.Url.ToString()
            crumbs.Add(currentCrumb)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

        'set focus to any control
        Public Sub SetFormFocus(ByVal control As Control)
            Page.SetFocus(control)
        End Sub

        Private Sub LocalizeTitles()
            If (lstStatus.Items.Count = 2) Then
                lstStatus.Items(0).Text = Localization.GetString("Open", LocalResourceFile)
                lstStatus.Items(1).Text = Localization.GetString("Closed", LocalResourceFile)
            End If
        End Sub

        Private Sub GetCookie()

            If (Request.IsAuthenticated = False) Then
                Dim cookie As HttpCookie = Request.Cookies("comment")

                If (cookie IsNot Nothing) Then
                    txtName.Text = cookie.Values("name")
                    txtEmail.Text = cookie.Values("email")
                    txtURL.Text = cookie.Values("url")
                End If
            End If

        End Sub

        Private Sub SetCookie()

            If (Request.IsAuthenticated = False) Then
                Dim objCookie As New HttpCookie("comment")

                objCookie.Expires = DateTime.Now.AddMonths(24)
                objCookie.Values.Add("name", txtName.Text)
                objCookie.Values.Add("email", txtEmail.Text)
                objCookie.Values.Add("url", txtURL.Text)

                Response.Cookies.Add(objCookie)
            End If

        End Sub

        Private Sub SaveCustomFields(ByVal feedbackID As Integer)

            Dim fieldsToUpdate As New Hashtable

            Dim objCustomFieldController As New CustomFieldController()
            Dim objCustomFields As ArrayList = objCustomFieldController.List(Me.ModuleId)

            If (phForm.Visible) Then
                If (rptCustomFields.Visible) Then

                    For Each item As RepeaterItem In rptCustomFields.Items
                        Dim phValue As PlaceHolder = CType(item.FindControl("phValue"), PlaceHolder)

                        If Not (phValue Is Nothing) Then
                            If (phValue.Controls.Count > 0) Then

                                Dim objControl As System.Web.UI.Control = phValue.Controls(0)
                                Dim customFieldID As Integer = Convert.ToInt32(objControl.ID)

                                For Each objCustomField As CustomFieldInfo In objCustomFields
                                    If (objCustomField.CustomFieldID = customFieldID) Then
                                        Select Case objCustomField.FieldType

                                            Case CustomFieldType.OneLineTextBox
                                                Dim objTextBox As TextBox = CType(objControl, TextBox)
                                                fieldsToUpdate.Add(customFieldID.ToString(), objTextBox.Text)

                                            Case CustomFieldType.MultiLineTextBox
                                                Dim objTextBox As TextBox = CType(objControl, TextBox)
                                                fieldsToUpdate.Add(customFieldID.ToString(), objTextBox.Text)

                                            Case CustomFieldType.RichTextBox
                                                Dim objTextBox As TextEditor = CType(objControl, TextEditor)
                                                fieldsToUpdate.Add(customFieldID.ToString(), objTextBox.Text)

                                            Case CustomFieldType.DropDownList
                                                Dim objDropDownList As DropDownList = CType(objControl, DropDownList)
                                                If (objDropDownList.SelectedValue = "-1") Then
                                                    fieldsToUpdate.Add(customFieldID.ToString(), "")
                                                Else
                                                    fieldsToUpdate.Add(customFieldID.ToString(), objDropDownList.SelectedValue)
                                                End If

                                            Case CustomFieldType.CheckBox
                                                Dim objCheckBox As CheckBox = CType(objControl, CheckBox)
                                                fieldsToUpdate.Add(customFieldID.ToString(), objCheckBox.Checked.ToString())

                                            Case CustomFieldType.MultiCheckBox
                                                Dim objCheckBoxList As CheckBoxList = CType(objControl, CheckBoxList)
                                                Dim values As String = ""
                                                For Each objCheckBox As ListItem In objCheckBoxList.Items
                                                    If (objCheckBox.Selected) Then
                                                        If (values = "") Then
                                                            values = objCheckBox.Value
                                                        Else
                                                            values = values & "|" & objCheckBox.Value
                                                        End If
                                                    End If
                                                Next
                                                fieldsToUpdate.Add(customFieldID.ToString(), values)

                                            Case CustomFieldType.RadioButton
                                                Dim objRadioButtonList As RadioButtonList = CType(objControl, RadioButtonList)
                                                fieldsToUpdate.Add(customFieldID.ToString(), objRadioButtonList.SelectedValue)

                                            Case CustomFieldType.FileUpload
                                                Dim objFileUpload As HtmlInputFile = CType(objControl, HtmlInputFile)
                                                If Not (objFileUpload.PostedFile Is Nothing) Then

                                                    ' Delete old one
                                                    Dim objPropertyValueController As New CustomValueController
                                                    Dim objPropertyValue As CustomValueInfo = objPropertyValueController.GetByCustomField(feedbackID, Convert.ToInt32(customFieldID.ToString()))

                                                    If Not (objPropertyValue Is Nothing) Then
                                                        Dim fileToDelete = PortalSettings.HomeDirectoryMapPath & objPropertyValue.CustomValue
                                                        If (File.Exists(fileToDelete)) Then
                                                            File.Delete(fileToDelete)
                                                        End If
                                                        objPropertyValueController.Delete(feedbackID, objPropertyValue.CustomValueID)
                                                    End If

                                                    If (objFileUpload.PostedFile.ContentLength > 0) Then
                                                        ' Upload new one
                                                        Dim filePath As String = PortalSettings.HomeDirectoryMapPath & "FeedbackCenter\" & ModuleId.ToString() & "\Files\" & feedbackID.ToString() & "\"
                                                        Dim fileName As String = Path.GetFileName(objFileUpload.PostedFile.FileName)
                                                        Dim relativePath As String = "FeedbackCenter\" & ModuleId.ToString() & "\Files\" & feedbackID.ToString() & "\" & fileName
                                                        If (Directory.Exists(filePath) = False) Then
                                                            Directory.CreateDirectory(filePath)
                                                        End If
                                                        objFileUpload.PostedFile.SaveAs(filePath & fileName)
                                                        fieldsToUpdate.Add(customFieldID.ToString(), relativePath)
                                                    End If

                                                End If

                                        End Select

                                        Exit For
                                    End If
                                Next

                            End If
                        End If
                    Next

                End If
            Else


                For Each objControl As Control In phFormTemplate.Controls

                    If (IsNumeric(objControl.ID)) Then

                        Dim customFieldID As Integer = Convert.ToInt32(objControl.ID)

                        For Each objCustomField As CustomFieldInfo In objCustomFields
                            If (objCustomField.CustomFieldID = customFieldID) Then
                                Select Case objCustomField.FieldType

                                    Case CustomFieldType.OneLineTextBox
                                        Dim objTextBox As TextBox = CType(objControl, TextBox)
                                        fieldsToUpdate.Add(customFieldID.ToString(), objTextBox.Text)

                                    Case CustomFieldType.MultiLineTextBox
                                        Dim objTextBox As TextBox = CType(objControl, TextBox)
                                        fieldsToUpdate.Add(customFieldID.ToString(), objTextBox.Text)

                                    Case CustomFieldType.RichTextBox
                                        Dim objTextBox As TextEditor = CType(objControl, TextEditor)
                                        fieldsToUpdate.Add(customFieldID.ToString(), objTextBox.Text)

                                    Case CustomFieldType.DropDownList
                                        Dim objDropDownList As DropDownList = CType(objControl, DropDownList)
                                        If (objDropDownList.SelectedValue = "-1") Then
                                            fieldsToUpdate.Add(customFieldID.ToString(), "")
                                        Else
                                            fieldsToUpdate.Add(customFieldID.ToString(), objDropDownList.SelectedValue)
                                        End If

                                    Case CustomFieldType.CheckBox
                                        Dim objCheckBox As CheckBox = CType(objControl, CheckBox)
                                        fieldsToUpdate.Add(customFieldID.ToString(), objCheckBox.Checked.ToString())

                                    Case CustomFieldType.MultiCheckBox
                                        Dim objCheckBoxList As CheckBoxList = CType(objControl, CheckBoxList)
                                        Dim values As String = ""
                                        For Each objCheckBox As ListItem In objCheckBoxList.Items
                                            If (objCheckBox.Selected) Then
                                                If (values = "") Then
                                                    values = objCheckBox.Value
                                                Else
                                                    values = values & "|" & objCheckBox.Value
                                                End If
                                            End If
                                        Next
                                        fieldsToUpdate.Add(customFieldID.ToString(), values)

                                    Case CustomFieldType.RadioButton
                                        Dim objRadioButtonList As RadioButtonList = CType(objControl, RadioButtonList)
                                        fieldsToUpdate.Add(customFieldID.ToString(), objRadioButtonList.SelectedValue)

                                    Case CustomFieldType.FileUpload
                                        Dim objFileUpload As HtmlInputFile = CType(objControl, HtmlInputFile)
                                        If Not (objFileUpload.PostedFile Is Nothing) Then

                                            ' Delete old one
                                            Dim objPropertyValueController As New CustomValueController
                                            Dim objPropertyValue As CustomValueInfo = objPropertyValueController.GetByCustomField(feedbackID, Convert.ToInt32(customFieldID.ToString()))

                                            If Not (objPropertyValue Is Nothing) Then
                                                Dim fileToDelete = PortalSettings.HomeDirectoryMapPath & objPropertyValue.CustomValue
                                                If (File.Exists(fileToDelete)) Then
                                                    File.Delete(fileToDelete)
                                                End If
                                                objPropertyValueController.Delete(feedbackID, objPropertyValue.CustomValueID)
                                            End If

                                            If (objFileUpload.PostedFile.ContentLength > 0) Then
                                                ' Upload new one
                                                Dim filePath As String = PortalSettings.HomeDirectoryMapPath & "FeedbackCenter\" & ModuleId.ToString() & "\Files\" & feedbackID.ToString() & "\"
                                                Dim fileName As String = Path.GetFileName(objFileUpload.PostedFile.FileName)
                                                Dim relativePath As String = "FeedbackCenter\" & ModuleId.ToString() & "\Files\" & feedbackID.ToString() & "\" & fileName
                                                If (Directory.Exists(filePath) = False) Then
                                                    Directory.CreateDirectory(filePath)
                                                End If
                                                objFileUpload.PostedFile.SaveAs(filePath & fileName)
                                                fieldsToUpdate.Add(customFieldID.ToString(), relativePath)
                                            End If

                                        End If

                                End Select

                                Exit For
                            End If
                        Next

                    End If

                Next

            End If

            For Each key As String In fieldsToUpdate.Keys
                Dim val As String = fieldsToUpdate(key).ToString()

                Dim objCustomValueController As New CustomValueController
                Dim objCustomValue As CustomValueInfo = objCustomValueController.GetByCustomField(feedbackID, Convert.ToInt32(key))

                If (objCustomValue IsNot Nothing) Then
                    objCustomValueController.Delete(feedbackID, Convert.ToInt32(key))
                End If

                objCustomValue = New CustomValueInfo
                objCustomValue.CustomFieldID = Convert.ToInt32(key)
                objCustomValue.CustomValue = val
                objCustomValue.FeedbackID = feedbackID
                objCustomValueController.Add(objCustomValue)
            Next

        End Sub

        Protected Sub ProcessEdiTemplate(ByRef controls As ControlCollection, ByVal layoutArray As String())

            Dim objFeedback As FeedbackInfo
            Dim objFeedbackController As New FeedbackController()
            objFeedback = objFeedbackController.Get(_feedbackID)

            Dim objCustomFieldController As New CustomFieldController()
            Dim objCustomFields As ArrayList = objCustomFieldController.List(Me.ModuleId)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "APPROVED"
                            Dim objApproved As New CheckBox
                            objApproved.EnableViewState = False
                            objApproved.ID = "Approved"

                            If (Settings.Contains(Constants.PERMISSION_AUTO_APPROVE_SETTING)) Then
                                If (IsEditable Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove) Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionAutoApprove)) Then
                                    objApproved.Checked = True
                                Else
                                    If (FeedbackSettings.PermissionAutoApprove.ToLower().Contains(glbRoleUnauthUserName.ToLower()) = False) Then
                                        objApproved.Checked = False
                                    Else
                                        objApproved.Checked = True
                                    End If
                                End If
                            Else
                                ' Auto-Approve
                                objApproved.Checked = True
                            End If

                            If (IsEditable Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove)) Then
                                objApproved.Visible = True
                            Else
                                objApproved.Visible = False
                            End If

                            If (objFeedback IsNot Nothing) Then
                                objApproved.Checked = objFeedback.IsApproved
                            End If

                            controls.Add(objApproved)

                        Case "DESCRIPTION"
                            Dim objDescription As New TextBox
                            objDescription.EnableViewState = False
                            objDescription.ID = "Description"
                            objDescription.CssClass = "NormalTextBox"
                            objDescription.TextMode = TextBoxMode.MultiLine
                            objDescription.Rows = 10

                            If (objFeedback IsNot Nothing) Then
                                objDescription.Text = objFeedback.Details
                            End If

                            controls.Add(objDescription)

                            Dim objRequired As New RequiredFieldValidator
                            objRequired.CssClass = "NormalRed"
                            objRequired.ErrorMessage = Localization.GetString("valDescription.ErrorMessage", Me.LocalResourceFile)
                            objRequired.ControlToValidate = objDescription.ID
                            objRequired.Display = ValidatorDisplay.Dynamic
                            controls.Add(objRequired)

                        Case "EMAIL"
                            If (Request.IsAuthenticated = False) Then
                                Dim objEmail As New TextBox
                                objEmail.EnableViewState = False
                                objEmail.ID = "Email"
                                objEmail.CssClass = "NormalTextBox"

                                If (objFeedback IsNot Nothing) Then
                                    objEmail.Text = objFeedback.Details
                                End If

                                controls.Add(objEmail)

                                Dim objRequired As New RequiredFieldValidator
                                objRequired.CssClass = "NormalRed"
                                objRequired.ErrorMessage = Localization.GetString("valEmail.ErrorMessage", Me.LocalResourceFile)
                                objRequired.ControlToValidate = objEmail.ID
                                objRequired.Display = ValidatorDisplay.Dynamic
                                controls.Add(objRequired)

                                Dim objRegEx As New RegularExpressionValidator
                                objRegEx.CssClass = "NormalRed"
                                objRegEx.ErrorMessage = Localization.GetString("valEmailIsValid.ErrorMessage", Me.LocalResourceFile)
                                objRegEx.ControlToValidate = objEmail.ID
                                objRegEx.Display = ValidatorDisplay.Dynamic
                                objRegEx.ValidationExpression = "^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"
                                controls.Add(objRegEx)
                            End If

                        Case "IMPLEMENTED"
                            Dim objImplemented As New CheckBox
                            objImplemented.EnableViewState = False
                            objImplemented.ID = "Implemented"

                            If (objFeedback IsNot Nothing) Then
                                objImplemented.Checked = objFeedback.IsResolved
                            End If

                            controls.Add(objImplemented)

                        Case "ISANONYMOUS"
                            If (Request.IsAuthenticated) Then
                                While (iPtr < layoutArray.Length - 1)
                                    If (layoutArray(iPtr + 1) = "/ISANONYMOUS") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "/ISANONYMOUS"
                            ' Do Nothing

                        Case "ISAPPROVER"
                            If ((IsEditable Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove)) = False) Then
                                While (iPtr < layoutArray.Length - 1)
                                    If (layoutArray(iPtr + 1) = "/ISAPPROVER") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "/ISAPPROVER"
                            ' Do Nothing

                        Case "NAME"
                            If (Request.IsAuthenticated = False) Then
                                Dim objName As New TextBox
                                objName.EnableViewState = False
                                objName.ID = "Name"
                                objName.CssClass = "NormalTextBox"

                                If (objFeedback IsNot Nothing) Then
                                    objName.Text = objFeedback.AnonymousEmail
                                End If

                                controls.Add(objName)

                                Dim objRequired As New RequiredFieldValidator
                                objRequired.CssClass = "NormalRed"
                                objRequired.ErrorMessage = Localization.GetString("valName.ErrorMessage", Me.LocalResourceFile)
                                objRequired.ControlToValidate = objName.ID
                                objRequired.Display = ValidatorDisplay.Dynamic
                                controls.Add(objRequired)
                            End If

                        Case "PRODUCTS"
                            Dim objDropDownList As New DropDownList
                            objDropDownList.EnableViewState = False
                            objDropDownList.ID = "Products"
                            objDropDownList.DataTextField = "Name"
                            objDropDownList.DataValueField = "ProductID"
                            BindProducts(objDropDownList)

                            If (objFeedback IsNot Nothing) Then
                                If (objDropDownList.Items.FindByValue(objFeedback.ProductID.ToString()) IsNot Nothing) Then
                                    objDropDownList.SelectedValue = objFeedback.ProductID
                                End If
                            End If

                            controls.Add(objDropDownList)

                            Dim objRequired As New RequiredFieldValidator
                            objRequired.CssClass = "NormalRed"
                            objRequired.ErrorMessage = Localization.GetString("valProducts.ErrorMessage", Me.LocalResourceFile)
                            objRequired.ControlToValidate = objDropDownList.ID
                            objRequired.Display = ValidatorDisplay.Dynamic
                            objRequired.InitialValue = "-1"
                            controls.Add(objRequired)

                        Case "STATUS"
                            Dim rdoStatus As New RadioButtonList
                            rdoStatus.ID = "Status"
                            rdoStatus.Items.Add(New ListItem(Localization.GetString("Open", Me.LocalResourceFile), "Open"))
                            rdoStatus.Items.Add(New ListItem(Localization.GetString("Closed", Me.LocalResourceFile), "Closed"))
                            rdoStatus.RepeatDirection = RepeatDirection.Horizontal
                            rdoStatus.RepeatLayout = RepeatLayout.Flow
                            rdoStatus.CssClass = "NormalTextBox"
                            rdoStatus.SelectedIndex = 0

                            If (objFeedback IsNot Nothing) Then
                                If (objFeedback.IsClosed) Then
                                    rdoStatus.SelectedValue = "Closed"
                                Else
                                    rdoStatus.SelectedValue = "Open"
                                End If
                            End If

                            controls.Add(rdoStatus)

                        Case "TITLE"
                            Dim objTitle As New TextBox
                            objTitle.EnableViewState = False
                            objTitle.ID = "Title"
                            objTitle.MaxLength = 100
                            objTitle.CssClass = "NormalTextBox"

                            If (objFeedback IsNot Nothing) Then
                                objTitle.Text = objFeedback.Title
                            End If

                            controls.Add(objTitle)

                            Dim objRequired As New RequiredFieldValidator
                            objRequired.CssClass = "NormalRed"
                            objRequired.ErrorMessage = Localization.GetString("valTitle.ErrorMessage", Me.LocalResourceFile)
                            objRequired.ControlToValidate = objTitle.ID
                            objRequired.Display = ValidatorDisplay.Dynamic
                            controls.Add(objRequired)

                        Case "URL"
                            If (Request.IsAuthenticated = False) Then
                                Dim objUrl As New TextBox
                                objUrl.EnableViewState = False
                                objUrl.ID = "Url"
                                objUrl.CssClass = "NormalTextBox"

                                If (objFeedback IsNot Nothing) Then
                                    objUrl.Text = objFeedback.AnonymousUrl
                                End If

                                controls.Add(objUrl)
                            End If

                        Case Else

                            Dim rendered As Boolean = False
                            If (layoutArray(iPtr + 1).ToUpper().StartsWith("CUSTOM:")) Then
                                Dim field As String = layoutArray(iPtr + 1).Substring(7, layoutArray(iPtr + 1).Length - 7).ToLower()

                                Dim customFieldID As Integer = Null.NullInteger

                                For Each objCustomField As CustomFieldInfo In objCustomFields
                                    If (objCustomField.Name.ToLower() = field.ToLower()) Then

                                        Select (objCustomField.FieldType)

                                            Case CustomFieldType.OneLineTextBox

                                                Dim objTextBox As New TextBox
                                                objTextBox.CssClass = "NormalTextBox"
                                                objTextBox.ID = objCustomField.CustomFieldID.ToString()
                                                If (objCustomField.Length <> Null.NullInteger AndAlso objCustomField.Length > 0) Then
                                                    objTextBox.MaxLength = objCustomField.Length
                                                End If
                                                If (objCustomField.DefaultValue <> "") Then
                                                    objTextBox.Text = objCustomField.DefaultValue
                                                End If
                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objTextBox.Enabled = False)) Then
                                                        objTextBox.Text = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                                    End If
                                                End If
                                                controls.Add(objTextBox)
                                                rendered = True

                                                If (objCustomField.IsRequired) Then
                                                    Dim valRequired As New RequiredFieldValidator
                                                    valRequired.ControlToValidate = objTextBox.ID
                                                    valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                    valRequired.CssClass = "NormalRed"
                                                    valRequired.Display = ValidatorDisplay.Dynamic
                                                    valRequired.SetFocusOnError = True
                                                    controls.Add(valRequired)
                                                End If

                                                If (objCustomField.ValidationType <> CustomFieldValidationType.None) Then
                                                    Dim valCompare As New CompareValidator
                                                    valCompare.ControlToValidate = objTextBox.ID
                                                    valCompare.CssClass = "NormalRed"
                                                    valCompare.Display = ValidatorDisplay.Dynamic
                                                    valCompare.SetFocusOnError = True
                                                    Select Case objCustomField.ValidationType

                                                        Case CustomFieldValidationType.Currency
                                                            valCompare.Type = ValidationDataType.Double
                                                            valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                                            valCompare.ErrorMessage = Localization.GetString("valFieldCurrency", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                            controls.Add(valCompare)

                                                        Case CustomFieldValidationType.Date
                                                            valCompare.Type = ValidationDataType.Date
                                                            valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                                            valCompare.ErrorMessage = Localization.GetString("valFieldDate", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                            controls.Add(valCompare)

                                                            Dim objCalendar As New HyperLink
                                                            objCalendar.CssClass = "CommandButton"
                                                            objCalendar.Text = Localization.GetString("Calendar", Me.LocalResourceFile)
                                                            objCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(objTextBox)
                                                            controls.Add(objCalendar)

                                                        Case CustomFieldValidationType.Double
                                                            valCompare.Type = ValidationDataType.Double
                                                            valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                                            valCompare.ErrorMessage = Localization.GetString("valFieldDecimal", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                            controls.Add(valCompare)

                                                        Case CustomFieldValidationType.Integer
                                                            valCompare.Type = ValidationDataType.Integer
                                                            valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                                            valCompare.ErrorMessage = Localization.GetString("valFieldNumber", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                            controls.Add(valCompare)

                                                        Case CustomFieldValidationType.Email
                                                            Dim valRegular As New RegularExpressionValidator
                                                            valRegular.ControlToValidate = objTextBox.ID
                                                            valRegular.ValidationExpression = "\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                            valRegular.ErrorMessage = Localization.GetString("valFieldEmail", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                            valRegular.CssClass = "NormalRed"
                                                            valRegular.Display = ValidatorDisplay.Dynamic
                                                            controls.Add(valRegular)

                                                        Case CustomFieldValidationType.Regex
                                                            If (objCustomField.RegularExpression <> "") Then
                                                                Dim valRegular As New RegularExpressionValidator
                                                                valRegular.ControlToValidate = objTextBox.ID
                                                                valRegular.ValidationExpression = objCustomField.RegularExpression
                                                                valRegular.ErrorMessage = Localization.GetString("valFieldRegex", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                                valRegular.CssClass = "NormalRed"
                                                                valRegular.Display = ValidatorDisplay.Dynamic
                                                                controls.Add(valRegular)
                                                            End If

                                                    End Select
                                                End If

                                            Case CustomFieldType.MultiLineTextBox

                                                Dim objTextBox As New TextBox
                                                objTextBox.TextMode = TextBoxMode.MultiLine
                                                objTextBox.CssClass = "NormalTextBox"
                                                objTextBox.ID = objCustomField.CustomFieldID.ToString()
                                                objTextBox.Rows = 4
                                                If (objCustomField.Length <> Null.NullInteger AndAlso objCustomField.Length > 0) Then
                                                    objTextBox.MaxLength = objCustomField.Length
                                                End If
                                                If (objCustomField.DefaultValue <> "") Then
                                                    objTextBox.Text = objCustomField.DefaultValue
                                                End If
                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objTextBox.Enabled = False)) Then
                                                        objTextBox.Text = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                                    End If
                                                End If
                                                controls.Add(objTextBox)
                                                rendered = True

                                                If (objCustomField.IsRequired) Then
                                                    Dim valRequired As New RequiredFieldValidator
                                                    valRequired.ControlToValidate = objTextBox.ID
                                                    valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                    valRequired.CssClass = "NormalRed"
                                                    valRequired.Display = ValidatorDisplay.None
                                                    valRequired.SetFocusOnError = True
                                                    controls.Add(valRequired)
                                                End If

                                            Case CustomFieldType.RichTextBox

                                                Dim objTextBox As TextEditor = CType(Me.LoadControl("~/controls/TextEditor.ascx"), TextEditor)
                                                objTextBox.ID = objCustomField.CustomFieldID.ToString()
                                                If (objCustomField.DefaultValue <> "") Then
                                                    ' objTextBox.Text = objCustomField.DefaultValue
                                                End If
                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And Page.IsPostBack = False) Then
                                                        ' There is a problem assigned values at init with the RTE, using ArrayList to assign later.
                                                        _richTextValues.Add(objCustomField.CustomFieldID.ToString(), objFeedback.CustomList(objCustomField.CustomFieldID).ToString())
                                                    End If
                                                End If
                                                objTextBox.Width = Unit.Pixel(300)
                                                objTextBox.Height = Unit.Pixel(400)

                                                controls.Add(objTextBox)
                                                rendered = True

                                                If (objCustomField.IsRequired) Then
                                                    Dim valRequired As New RequiredFieldValidator
                                                    valRequired.ControlToValidate = objTextBox.ID
                                                    valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                    valRequired.CssClass = "NormalRed"
                                                    valRequired.SetFocusOnError = True
                                                    controls.Add(valRequired)
                                                End If

                                            Case CustomFieldType.DropDownList

                                                Dim objDropDownList As New DropDownList
                                                objDropDownList.CssClass = "NormalTextBox"
                                                objDropDownList.ID = objCustomField.CustomFieldID.ToString()

                                                Dim values As String() = objCustomField.FieldElements.Split(Convert.ToChar("|"))
                                                For Each value As String In values
                                                    If (value <> "") Then
                                                        objDropDownList.Items.Add(value)
                                                    End If
                                                Next

                                                Dim selectText As String = Localization.GetString("SelectValue", Me.LocalResourceFile)
                                                selectText = selectText.Replace("[VALUE]", objCustomField.Caption)
                                                objDropDownList.Items.Insert(0, New ListItem(selectText, "-1"))

                                                If (objCustomField.DefaultValue <> "") Then
                                                    If Not (objDropDownList.Items.FindByValue(objCustomField.DefaultValue) Is Nothing) Then
                                                        objDropDownList.SelectedValue = objCustomField.DefaultValue
                                                    End If
                                                End If


                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objDropDownList.Enabled = False)) Then
                                                        If Not (objDropDownList.Items.FindByValue(objFeedback.CustomList(objCustomField.CustomFieldID).ToString()) Is Nothing) Then
                                                            objDropDownList.SelectedValue = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                                        End If
                                                    End If
                                                End If
                                                controls.Add(objDropDownList)
                                                rendered = True

                                                If (objCustomField.IsRequired) Then
                                                    Dim valRequired As New RequiredFieldValidator
                                                    valRequired.ControlToValidate = objDropDownList.ID
                                                    valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                    valRequired.CssClass = "NormalRed"
                                                    valRequired.Display = ValidatorDisplay.None
                                                    valRequired.SetFocusOnError = True
                                                    valRequired.InitialValue = "-1"
                                                    controls.Add(valRequired)
                                                End If

                                            Case CustomFieldType.CheckBox

                                                Dim objCheckBox As New CheckBox
                                                objCheckBox.CssClass = "Normal"
                                                objCheckBox.ID = objCustomField.CustomFieldID.ToString()
                                                If (objCustomField.DefaultValue <> "") Then
                                                    Try
                                                        objCheckBox.Checked = Convert.ToBoolean(objCustomField.DefaultValue)
                                                    Catch
                                                    End Try
                                                End If

                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objCheckBox.Enabled = False)) Then
                                                        If (objFeedback.CustomList(objCustomField.CustomFieldID).ToString() <> "") Then
                                                            Try
                                                                objCheckBox.Checked = Convert.ToBoolean(objFeedback.CustomList(objCustomField.CustomFieldID).ToString())
                                                            Catch
                                                            End Try
                                                        End If
                                                    End If
                                                End If
                                                controls.Add(objCheckBox)
                                                rendered = True

                                            Case CustomFieldType.MultiCheckBox

                                                Dim objCheckBoxList As New CheckBoxList
                                                objCheckBoxList.CssClass = "Normal"
                                                objCheckBoxList.ID = objCustomField.CustomFieldID.ToString()
                                                objCheckBoxList.RepeatColumns = 4
                                                objCheckBoxList.RepeatDirection = RepeatDirection.Horizontal
                                                objCheckBoxList.RepeatLayout = RepeatLayout.Table

                                                Dim values As String() = objCustomField.FieldElements.Split(Convert.ToChar("|"))
                                                For Each value As String In values
                                                    objCheckBoxList.Items.Add(value)
                                                Next

                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objCheckBoxList.Enabled = False)) Then
                                                        Dim vals As String() = objFeedback.CustomList(objCustomField.CustomFieldID).ToString().Split(Convert.ToChar("|"))
                                                        For Each val As String In vals
                                                            For Each item As ListItem In objCheckBoxList.Items
                                                                If (item.Value = val) Then
                                                                    item.Selected = True
                                                                End If
                                                            Next
                                                        Next
                                                    End If
                                                End If

                                                controls.Add(objCheckBoxList)
                                                rendered = True

                                            Case CustomFieldType.RadioButton

                                                Dim objRadioButtonList As New RadioButtonList
                                                objRadioButtonList.CssClass = "Normal"
                                                objRadioButtonList.ID = objCustomField.CustomFieldID.ToString()
                                                objRadioButtonList.RepeatDirection = RepeatDirection.Horizontal
                                                objRadioButtonList.RepeatLayout = RepeatLayout.Table
                                                objRadioButtonList.RepeatColumns = 4

                                                Dim values As String() = objCustomField.FieldElements.Split(Convert.ToChar("|"))
                                                For Each value As String In values
                                                    objRadioButtonList.Items.Add(value)
                                                Next

                                                If (objCustomField.DefaultValue <> "") Then
                                                    If Not (objRadioButtonList.Items.FindByValue(objCustomField.DefaultValue) Is Nothing) Then
                                                        objRadioButtonList.SelectedValue = objCustomField.DefaultValue
                                                    End If
                                                End If

                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objRadioButtonList.Enabled = False)) Then
                                                        If Not (objRadioButtonList.Items.FindByValue(objFeedback.CustomList(objCustomField.CustomFieldID).ToString()) Is Nothing) Then
                                                            objRadioButtonList.SelectedValue = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                                        End If
                                                    End If
                                                End If

                                                controls.Add(objRadioButtonList)
                                                rendered = True

                                                If (objCustomField.IsRequired) Then
                                                    Dim valRequired As New RequiredFieldValidator
                                                    valRequired.ControlToValidate = objRadioButtonList.ID
                                                    valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                    valRequired.CssClass = "NormalRed"
                                                    valRequired.Display = ValidatorDisplay.None
                                                    valRequired.SetFocusOnError = True
                                                    controls.Add(valRequired)
                                                End If


                                            Case CustomFieldType.FileUpload

                                                Dim hasValue As Boolean = False
                                                Dim objHtmlInputFile As New HtmlInputFile
                                                objHtmlInputFile.ID = objCustomField.CustomFieldID.ToString()
                                                controls.Add(objHtmlInputFile)
                                                rendered = True

                                                If Not (objFeedback Is Nothing) Then
                                                    If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) AndAlso objFeedback.CustomList(objCustomField.CustomFieldID).ToString() <> "") Then

                                                        Dim fileName As String = Path.GetFileName(PortalSettings.HomeDirectoryMapPath & objFeedback.CustomList(objCustomField.CustomFieldID).ToString())
                                                        Dim objLabel As New Label
                                                        objLabel.ID = objCustomField.CustomFieldID.ToString() & "-Label"
                                                        objLabel.Text = fileName & "<BR>"
                                                        objLabel.CssClass = "Normal"
                                                        controls.Add(objLabel)

                                                        Dim objDelete As New LinkButton
                                                        objDelete.ID = objCustomField.CustomFieldID.ToString() & "-Button"
                                                        objDelete.Text = Localization.GetString("cmdDelete")
                                                        objDelete.CssClass = "CommandButton"
                                                        objDelete.CommandArgument = objCustomField.CustomFieldID.ToString()
                                                        AddHandler objDelete.Click, AddressOf FileUploadButton2_Click
                                                        controls.Add(objDelete)

                                                        objHtmlInputFile.Visible = False
                                                        hasValue = True
                                                    End If
                                                End If


                                                If (objCustomField.IsRequired) Then
                                                    Dim valRequired As New RequiredFieldValidator
                                                    valRequired.ID = objCustomField.CustomFieldID.ToString() & "-Required"
                                                    valRequired.ControlToValidate = objHtmlInputFile.ID
                                                    valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                                    valRequired.CssClass = "NormalRed"
                                                    valRequired.Display = ValidatorDisplay.None
                                                    valRequired.Visible = Not hasValue
                                                    valRequired.SetFocusOnError = True
                                                    controls.Add(valRequired)
                                                End If

                                        End Select


                                    End If
                                Next

                            End If

                            If (rendered = False) Then
                                Dim objLiteralOther As New Literal
                                objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                                objLiteralOther.EnableViewState = False
                                controls.Add(objLiteralOther)
                            End If

                    End Select
                End If
            Next

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            ReadQueryString()
            BindCustomFields()

            Dim objLayoutController As New LayoutController()
            Dim objEditLayout As LayoutInfo = objLayoutController.GetLayout(LayoutType.Edit_Layout_Html, ModuleId, Settings)

            If (objEditLayout.Template.Trim() <> "") Then
                ProcessEdiTemplate(phFormTemplate.Controls, objEditLayout.Tokens)
                phForm.Visible = False
                phFormTemplate.Visible = True
            Else
                phForm.Visible = True
                phFormTemplate.Visible = False
            End If

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            CheckSecurity()

            BindCrumbs()

            ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", Me.LocalResourceFile)

            If (_feedbackID <> Null.NullInteger) Then
                For Each key As String In _richTextValues
                    For Each item As RepeaterItem In rptCustomFields.Items
                        If Not (item.FindControl(key) Is Nothing) Then
                            Dim objTextBox As TextEditor = CType(item.FindControl(key), TextEditor)
                            objTextBox.Text = _richTextValues(key)
                            Exit For
                        End If
                    Next
                Next
            End If

            If (IsPostBack = False) Then

                SetVisibility()
                BindTypes()
                BindProducts(drpProducts)
                GetCookie()
                BindFeedback()
                SetFormFocus(txtTitle)
                LocalizeTitles()

            End If

        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            If (Page.IsValid) Then

                If (_feedbackID = Null.NullInteger And FeedbackSettings.EnableCaptcha) Then
                    If (ctlCaptcha.IsValid = False) Then
                        Return
                    End If
                End If

                Dim objFeedbackController As New FeedbackController
                Dim objFeedback As New FeedbackInfo
                Dim objSecurity As New DotNetNuke.Security.PortalSecurity

                If (_feedbackID <> Null.NullInteger) Then
                    objFeedback = objFeedbackController.Get(_feedbackID)
                Else
                    objFeedback = CType(CBO.InitializeObject(objFeedback, GetType(FeedbackInfo)), FeedbackInfo)
                End If

                objFeedback.ModuleID = Me.ModuleId
                objFeedback.TypeID = Convert.ToInt32(lstTypes.SelectedValue)

                If (phFormTemplate.Visible) Then
                    For Each objControl As Control In phFormTemplate.Controls
                        If (objControl.ID = "Products") Then
                            objFeedback.ProductID = Convert.ToInt32(CType(objControl, DropDownList).SelectedValue)
                        End If
                        If (objControl.ID = "Title") Then
                            objFeedback.Title = objSecurity.InputFilter(CType(objControl, TextBox).Text, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup Or DotNetNuke.Security.PortalSecurity.FilterFlag.MultiLine)
                        End If
                        If (objControl.ID = "Description") Then
                            objFeedback.Details = objSecurity.InputFilter(CType(objControl, TextBox).Text, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup Or DotNetNuke.Security.PortalSecurity.FilterFlag.MultiLine)
                        End If
                        If (objControl.ID = "Status") Then
                            If (_feedbackID <> Null.NullInteger) Then
                                If (objFeedback.IsClosed = False And (CType(objControl, RadioButtonList).SelectedValue = "Closed")) Then
                                    objFeedback.ClosedDate = DateTime.Now
                                End If

                                If (objFeedback.IsClosed = True And ((CType(objControl, RadioButtonList).SelectedValue = "Closed") = False)) Then
                                    objFeedback.ClosedDate = Null.NullDate
                                End If
                            End If

                            objFeedback.IsClosed = (CType(objControl, RadioButtonList).SelectedValue = "Closed")
                        End If
                        If (objControl.ID = "Implemented") Then
                            objFeedback.IsResolved = CType(objControl, CheckBox).Checked
                        End If
                        If (objControl.ID = "Approved") Then
                            objFeedback.IsApproved = CType(objControl, CheckBox).Checked
                        End If

                        If (objControl.ID = "Name") Then
                            objFeedback.AnonymousName = objSecurity.InputFilter(CType(objControl, TextBox).Text, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup Or DotNetNuke.Security.PortalSecurity.FilterFlag.MultiLine)
                        End If
                        If (objControl.ID = "Email") Then
                            objFeedback.AnonymousEmail = objSecurity.InputFilter(CType(objControl, TextBox).Text, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup Or DotNetNuke.Security.PortalSecurity.FilterFlag.MultiLine)
                        End If
                        If (objControl.ID = "Url") Then
                            objFeedback.AnonymousUrl = objSecurity.InputFilter(CType(objControl, TextBox).Text, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup Or DotNetNuke.Security.PortalSecurity.FilterFlag.MultiLine)
                        End If

                    Next
                Else
                    objFeedback.ProductID = Convert.ToInt32(drpProducts.SelectedValue)
                    objFeedback.Title = objSecurity.InputFilter(txtTitle.Text, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup Or DotNetNuke.Security.PortalSecurity.FilterFlag.MultiLine)
                    objFeedback.Details = objSecurity.InputFilter(txtDescription.Text, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup Or DotNetNuke.Security.PortalSecurity.FilterFlag.MultiLine)

                    If (_feedbackID <> Null.NullInteger) Then
                        If (objFeedback.IsClosed = False And (lstStatus.SelectedValue = "Closed")) Then
                            objFeedback.ClosedDate = DateTime.Now
                        End If

                        If (objFeedback.IsClosed = True And ((lstStatus.SelectedValue = "Closed") = False)) Then
                            objFeedback.ClosedDate = Null.NullDate
                        End If
                    End If

                    objFeedback.IsClosed = (lstStatus.SelectedValue = "Closed")
                    objFeedback.IsResolved = chkImplemented.Checked
                    objFeedback.IsApproved = chkIsApproved.Checked

                    objFeedback.AnonymousName = txtName.Text
                    objFeedback.AnonymousEmail = txtEmail.Text
                    objFeedback.AnonymousUrl = txtURL.Text
                End If


                If (Request.IsAuthenticated = False) Then
                    SetCookie()
                End If

                If (_feedbackID = Null.NullInteger) Then

                    If (Request.IsAuthenticated) Then
                        objFeedback.UserID = Me.UserId
                    Else
                        objFeedback.UserID = Null.NullInteger
                    End If
                    objFeedback.CreateDate = DateTime.Now
                    objFeedback.VoteTotal = 1
                    objFeedback.VoteTotalNegative = 0

                    If (objFeedback.IsClosed) Then
                        objFeedback.ClosedDate = objFeedback.CreateDate
                    End If

                    objFeedback.FeedbackID = objFeedbackController.Add(objFeedback)
                    SaveCustomFields(objFeedback.FeedbackID)

                    If (objFeedback.IsApproved) Then

                        If (FeedbackSettings.ActiveSocialSubmissionKey <> "" And Request.IsAuthenticated = True) Then
                            If IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")) Then
                                Dim ai As Object = Nothing
                                Dim asm As System.Reflection.Assembly
                                Dim ac As Object = Nothing
                                Try
                                    asm = System.Reflection.Assembly.Load("Active.Modules.Social")
                                    ac = asm.CreateInstance("Active.Modules.Social.API.Journal")
                                    If Not ac Is Nothing Then
                                        Dim link As String = ""
                                        link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                        If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                            link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                        End If
                                        ac.AddProfileItem(New Guid(FeedbackSettings.ActiveSocialSubmissionKey), objFeedback.UserID, link, objFeedback.Title, objFeedback.Details, objFeedback.Details, 1, "", True)
                                    End If
                                Catch ex As Exception
                                End Try
                            End If
                        End If

                        If (PortalSettings.AdministratorId <> Me.UserId) Then

                            Dim objEmailController As New EmailController()

                            Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Subject_Html, ModuleId, Settings)
                            Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Body_Html, ModuleId, Settings)

                            Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", PortalSettings.PortalName)
                            Dim template As String = objEmailBodyInfo.Template

                            Dim link As String = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                            If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                            End If

                            template = FormatEmail(template, link, objFeedback)
                            Try
                                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, PortalSettings.Email, "", subject, template, "", "", "", "", "", "")
                            Catch
                            End Try

                        End If

                        Dim objProductController As New ProductController
                        Dim objProduct As ProductInfo = objProductController.Get(objFeedback.ProductID)

                        If (objProduct IsNot Nothing) Then
                            If (objProduct.Email <> "") Then
                                Dim objEmailController As New EmailController()

                                Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Subject_Html, ModuleId, Settings)
                                Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Body_Html, ModuleId, Settings)

                                Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", PortalSettings.PortalName)
                                Dim template As String = objEmailBodyInfo.Template

                                Dim link As String = ""

                                If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                    link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                Else
                                    link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                    If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                        link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                    End If
                                End If

                                template = FormatEmail(template, link, objFeedback)
                                Try
                                    DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objProduct.Email, "", subject, template, "", "", "", "", "", "")
                                Catch
                                End Try
                            End If
                        End If

                        Dim emails As String = Me.EmailNotification

                        For Each email As String In emails.Split(",")
                            If (email <> PortalSettings.Email) Then

                                Dim objEmailController As New EmailController()

                                Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Subject_Html, ModuleId, Settings)
                                Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Body_Html, ModuleId, Settings)

                                Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", PortalSettings.PortalName)
                                Dim template As String = objEmailBodyInfo.Template

                                Dim link As String = ""

                                If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                    link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                Else
                                    link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                    If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                        link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                    End If
                                End If

                                template = FormatEmail(template, link, objFeedback)
                                Try
                                    DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, email, "", subject, template, "", "", "", "", "", "")
                                Catch
                                End Try

                            End If
                        Next

                    Else

                        ' Send approval notice.
                        Dim emails As String = GetApproverDistributionList()

                        For Each email As String In emails.Split(";")

                            Dim objEmailController As New EmailController()

                            Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Require_Approval_Subject_Html, ModuleId, Settings)
                            Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Require_Approval_Body_Html, ModuleId, Settings)

                            Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", PortalSettings.PortalName)
                            Dim template As String = objEmailBodyInfo.Template

                            Dim link As String = ""

                            If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                link = NavigateURL(Me.TabId, "", "fbType=ApproveFeedback")
                            Else
                                link = NavigateURL(Me.TabId, "", "fbType=ApproveFeedback")
                                If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                    link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                End If
                            End If

                            template = FormatEmail(template, link, objFeedback)
                            Try
                                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, email, "", subject, template, "", "", "", "", "", "")
                            Catch
                            End Try
                        Next

                    End If

                    Dim objVote As New VoteInfo

                    objVote.UserID = Me.UserId
                    objVote.FeedbackID = objFeedback.FeedbackID
                    objVote.CreateDate = DateTime.Now

                    Dim objVoteController As New VoteController
                    objVoteController.Add(objVote)

                    If (Request.IsAuthenticated) Then
                        Dim objTracking As New TrackingInfo

                        objTracking.UserID = Me.UserId
                        objTracking.FeedbackID = objFeedback.FeedbackID
                        objTracking.CreateDate = DateTime.Now

                        Dim objTrackingController As New TrackingController
                        objTrackingController.Add(objTracking)
                    End If

                    If (chkIsApproved.Checked) Then
                        Response.Redirect(NavigateURL(Me.TabId, "", "fbType=EditComplete"), True)
                    Else
                        Response.Redirect(NavigateURL(Me.TabId, "", "fbType=EditComplete", "approval=1"), True)
                    End If

                Else

                    Dim objFeedbackCurrent As FeedbackInfo = objFeedbackController.Get(_feedbackID)

                    If Not (objFeedbackCurrent Is Nothing) Then

                        If (objFeedbackCurrent.IsClosed <> objFeedback.IsClosed) Then

                            Dim objEmailController As New EmailController()

                            Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Status_Subject_Html, ModuleId, Settings)
                            Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Notification_Status_Body_Html, ModuleId, Settings)

                            Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", PortalSettings.PortalName)
                            Dim template As String = objEmailBodyInfo.Template

                            Dim link As String = ""

                            If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                            Else
                                link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                    link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                End If
                            End If

                            template = FormatEmail(template, link, objFeedback)
                            template = String.Format(template, IIf(objFeedbackCurrent.IsClosed, "Closed", "Open"), IIf(objFeedback.IsClosed, "Closed", "Open"))

                            Dim objTrackingController As New TrackingController
                            Dim objTrackingList As ArrayList = objTrackingController.List(_feedbackID)

                            For Each objTracking As TrackingInfo In objTrackingList

                                If (objTracking.UserID <> Me.UserId) Then

                                    Dim objUserController As New UserController
                                    Dim objUser As UserInfo = objUserController.GetUser(Me.PortalId, objTracking.UserID)

                                    If Not (objUser Is Nothing) Then
                                        Try
                                            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUser.Membership.Email, "", subject, template, "", "", "", "", "", "")
                                        Catch
                                        End Try
                                    End If

                                End If

                            Next

                        End If

                    End If

                    objFeedbackController.Update(objFeedback)
                    SaveCustomFields(objFeedback.FeedbackID)
                    Response.Redirect(NavigateURL(Me.TabId, "", "fbType=Search", "ProductID=" & objFeedbackCurrent.ProductID.ToString()), True)

                End If

            End If

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click

            Response.Redirect(NavigateURL(), True)

        End Sub

        Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click

            Dim objFeedbackController As New FeedbackController
            objFeedbackController.Delete(_feedbackID)

            Response.Redirect(NavigateURL(), True)

        End Sub

        Private Sub cmdEditProducts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEditProducts.Click

            Response.Redirect(EditUrl("EditProducts"), True)

        End Sub

        Private Sub rptCustomFields_OnItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptCustomFields.ItemDataBound

            If (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then

                Dim objFeedbackController As New FeedbackController()
                Dim objFeedback As FeedbackInfo = Nothing
                If (_feedbackID <> Null.NullInteger) Then
                    objFeedback = objFeedbackController.Get(_feedbackID)
                End If

                Dim objCustomField As CustomFieldInfo = CType(e.Item.DataItem, CustomFieldInfo)
                Dim phValue As PlaceHolder = CType(e.Item.FindControl("phValue"), PlaceHolder)
                Dim phLabel As PlaceHolder = CType(e.Item.FindControl("phLabel"), PlaceHolder)

                Dim cmdHelp As LinkButton = CType(e.Item.FindControl("cmdHelp"), LinkButton)
                Dim pnlHelp As Panel = CType(e.Item.FindControl("pnlHelp"), Panel)
                Dim lblLabel As Label = CType(e.Item.FindControl("lblLabel"), Label)
                Dim lblHelp As Label = CType(e.Item.FindControl("lblHelp"), Label)
                Dim imgHelp As Image = CType(e.Item.FindControl("imgHelp"), Image)

                Dim trItem As HtmlControls.HtmlTableRow = CType(e.Item.FindControl("trItem"), HtmlControls.HtmlTableRow)

                If Not (phValue Is Nothing) Then

                    DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, True, DotNetNuke.UI.Utilities.DNNClientAPI.MinMaxPersistanceType.None)

                    If (objCustomField.IsRequired) Then
                        lblLabel.Text = objCustomField.Caption & "*:"
                    Else
                        lblLabel.Text = objCustomField.Caption & ":"
                    End If
                    lblHelp.Text = objCustomField.CaptionHelp
                    imgHelp.AlternateText = objCustomField.CaptionHelp

                    Select Case (objCustomField.FieldType)

                        Case CustomFieldType.OneLineTextBox

                            Dim objTextBox As New TextBox
                            objTextBox.CssClass = "NormalTextBox"
                            objTextBox.ID = objCustomField.CustomFieldID.ToString()
                            If (objCustomField.Length <> Null.NullInteger AndAlso objCustomField.Length > 0) Then
                                objTextBox.MaxLength = objCustomField.Length
                            End If
                            If (objCustomField.DefaultValue <> "") Then
                                objTextBox.Text = objCustomField.DefaultValue
                            End If
                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objTextBox.Enabled = False)) Then
                                    objTextBox.Text = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                End If
                            End If
                            objTextBox.Width = Unit.Pixel(300)
                            phValue.Controls.Add(objTextBox)

                            If (objCustomField.IsRequired) Then
                                Dim valRequired As New RequiredFieldValidator
                                valRequired.ControlToValidate = objTextBox.ID
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                valRequired.CssClass = "NormalRed"
                                valRequired.Display = ValidatorDisplay.None
                                valRequired.SetFocusOnError = True
                                phValue.Controls.Add(valRequired)
                            End If

                            If (objCustomField.ValidationType <> CustomFieldValidationType.None) Then
                                Dim valCompare As New CompareValidator
                                valCompare.ControlToValidate = objTextBox.ID
                                valCompare.CssClass = "NormalRed"
                                valCompare.Display = ValidatorDisplay.None
                                valCompare.SetFocusOnError = True
                                Select Case objCustomField.ValidationType

                                    Case CustomFieldValidationType.Currency
                                        valCompare.Type = ValidationDataType.Double
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                        valCompare.ErrorMessage = Localization.GetString("valFieldCurrency", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                        phValue.Controls.Add(valCompare)

                                    Case CustomFieldValidationType.Date
                                        valCompare.Type = ValidationDataType.Date
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                        valCompare.ErrorMessage = Localization.GetString("valFieldDate", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                        phValue.Controls.Add(valCompare)

                                        Dim objCalendar As New HyperLink
                                        objCalendar.CssClass = "CommandButton"
                                        objCalendar.Text = Localization.GetString("Calendar", Me.LocalResourceFile)
                                        objCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(objTextBox)
                                        phValue.Controls.Add(objCalendar)

                                    Case CustomFieldValidationType.Double
                                        valCompare.Type = ValidationDataType.Double
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                        valCompare.ErrorMessage = Localization.GetString("valFieldDecimal", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                        phValue.Controls.Add(valCompare)

                                    Case CustomFieldValidationType.Integer
                                        valCompare.Type = ValidationDataType.Integer
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck
                                        valCompare.ErrorMessage = Localization.GetString("valFieldNumber", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                        phValue.Controls.Add(valCompare)

                                    Case CustomFieldValidationType.Email
                                        Dim valRegular As New RegularExpressionValidator
                                        valRegular.ControlToValidate = objTextBox.ID
                                        valRegular.ValidationExpression = "\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                        valRegular.ErrorMessage = Localization.GetString("valFieldEmail", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                        valRegular.CssClass = "NormalRed"
                                        valRegular.Display = ValidatorDisplay.None
                                        phValue.Controls.Add(valRegular)

                                    Case CustomFieldValidationType.Regex
                                        If (objCustomField.RegularExpression <> "") Then
                                            Dim valRegular As New RegularExpressionValidator
                                            valRegular.ControlToValidate = objTextBox.ID
                                            valRegular.ValidationExpression = objCustomField.RegularExpression
                                            valRegular.ErrorMessage = Localization.GetString("valFieldRegex", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                            valRegular.CssClass = "NormalRed"
                                            valRegular.Display = ValidatorDisplay.None
                                            phValue.Controls.Add(valRegular)
                                        End If

                                End Select
                            End If

                        Case CustomFieldType.MultiLineTextBox

                            Dim objTextBox As New TextBox
                            objTextBox.TextMode = TextBoxMode.MultiLine
                            objTextBox.CssClass = "NormalTextBox"
                            objTextBox.ID = objCustomField.CustomFieldID.ToString()
                            objTextBox.Rows = 4
                            If (objCustomField.Length <> Null.NullInteger AndAlso objCustomField.Length > 0) Then
                                objTextBox.MaxLength = objCustomField.Length
                            End If
                            If (objCustomField.DefaultValue <> "") Then
                                objTextBox.Text = objCustomField.DefaultValue
                            End If
                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objTextBox.Enabled = False)) Then
                                    objTextBox.Text = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                End If
                            End If
                            objTextBox.Width = Unit.Pixel(300)
                            phValue.Controls.Add(objTextBox)

                            If (objCustomField.IsRequired) Then
                                Dim valRequired As New RequiredFieldValidator
                                valRequired.ControlToValidate = objTextBox.ID
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                valRequired.CssClass = "NormalRed"
                                valRequired.Display = ValidatorDisplay.None
                                valRequired.SetFocusOnError = True
                                phValue.Controls.Add(valRequired)
                            End If

                        Case CustomFieldType.RichTextBox

                            Dim objTextBox As TextEditor = CType(Me.LoadControl("~/controls/TextEditor.ascx"), TextEditor)
                            objTextBox.ID = objCustomField.CustomFieldID.ToString()
                            If (objCustomField.DefaultValue <> "") Then
                                ' objTextBox.Text = objCustomField.DefaultValue
                            End If
                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And Page.IsPostBack = False) Then
                                    ' There is a problem assigned values at init with the RTE, using ArrayList to assign later.
                                    _richTextValues.Add(objCustomField.CustomFieldID.ToString(), objFeedback.CustomList(objCustomField.CustomFieldID).ToString())
                                End If
                            End If
                            objTextBox.Width = Unit.Pixel(300)
                            objTextBox.Height = Unit.Pixel(400)

                            phValue.Controls.Add(objTextBox)

                            If (objCustomField.IsRequired) Then
                                Dim valRequired As New RequiredFieldValidator
                                valRequired.ControlToValidate = objTextBox.ID
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                valRequired.CssClass = "NormalRed"
                                valRequired.SetFocusOnError = True
                                phValue.Controls.Add(valRequired)
                            End If

                        Case CustomFieldType.DropDownList

                            Dim objDropDownList As New DropDownList
                            objDropDownList.CssClass = "NormalTextBox"
                            objDropDownList.ID = objCustomField.CustomFieldID.ToString()

                            Dim values As String() = objCustomField.FieldElements.Split(Convert.ToChar("|"))
                            For Each value As String In values
                                If (value <> "") Then
                                    objDropDownList.Items.Add(value)
                                End If
                            Next

                            Dim selectText As String = Localization.GetString("SelectValue", Me.LocalResourceFile)
                            selectText = selectText.Replace("[VALUE]", objCustomField.Caption)
                            objDropDownList.Items.Insert(0, New ListItem(selectText, "-1"))

                            If (objCustomField.DefaultValue <> "") Then
                                If Not (objDropDownList.Items.FindByValue(objCustomField.DefaultValue) Is Nothing) Then
                                    objDropDownList.SelectedValue = objCustomField.DefaultValue
                                End If
                            End If

                            objDropDownList.Width = Unit.Pixel(300)

                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objDropDownList.Enabled = False)) Then
                                    If Not (objDropDownList.Items.FindByValue(objFeedback.CustomList(objCustomField.CustomFieldID).ToString()) Is Nothing) Then
                                        objDropDownList.SelectedValue = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                    End If
                                End If
                            End If
                            phValue.Controls.Add(objDropDownList)

                            If (objCustomField.IsRequired) Then
                                Dim valRequired As New RequiredFieldValidator
                                valRequired.ControlToValidate = objDropDownList.ID
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                valRequired.CssClass = "NormalRed"
                                valRequired.Display = ValidatorDisplay.None
                                valRequired.SetFocusOnError = True
                                valRequired.InitialValue = "-1"
                                phValue.Controls.Add(valRequired)
                            End If

                        Case CustomFieldType.CheckBox

                            Dim objCheckBox As New CheckBox
                            objCheckBox.CssClass = "Normal"
                            objCheckBox.ID = objCustomField.CustomFieldID.ToString()
                            If (objCustomField.DefaultValue <> "") Then
                                Try
                                    objCheckBox.Checked = Convert.ToBoolean(objCustomField.DefaultValue)
                                Catch
                                End Try
                            End If

                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objCheckBox.Enabled = False)) Then
                                    If (objFeedback.CustomList(objCustomField.CustomFieldID).ToString() <> "") Then
                                        Try
                                            objCheckBox.Checked = Convert.ToBoolean(objFeedback.CustomList(objCustomField.CustomFieldID).ToString())
                                        Catch
                                        End Try
                                    End If
                                End If
                            End If
                            phValue.Controls.Add(objCheckBox)

                        Case CustomFieldType.MultiCheckBox

                            Dim objCheckBoxList As New CheckBoxList
                            objCheckBoxList.CssClass = "Normal"
                            objCheckBoxList.ID = objCustomField.CustomFieldID.ToString()
                            objCheckBoxList.RepeatColumns = 4
                            objCheckBoxList.RepeatDirection = RepeatDirection.Horizontal
                            objCheckBoxList.RepeatLayout = RepeatLayout.Table

                            Dim values As String() = objCustomField.FieldElements.Split(Convert.ToChar("|"))
                            For Each value As String In values
                                objCheckBoxList.Items.Add(value)
                            Next

                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objCheckBoxList.Enabled = False)) Then
                                    Dim vals As String() = objFeedback.CustomList(objCustomField.CustomFieldID).ToString().Split(Convert.ToChar("|"))
                                    For Each val As String In vals
                                        For Each item As ListItem In objCheckBoxList.Items
                                            If (item.Value = val) Then
                                                item.Selected = True
                                            End If
                                        Next
                                    Next
                                End If
                            End If

                            phValue.Controls.Add(objCheckBoxList)

                        Case CustomFieldType.RadioButton

                            Dim objRadioButtonList As New RadioButtonList
                            objRadioButtonList.CssClass = "Normal"
                            objRadioButtonList.ID = objCustomField.CustomFieldID.ToString()
                            objRadioButtonList.RepeatDirection = RepeatDirection.Horizontal
                            objRadioButtonList.RepeatLayout = RepeatLayout.Table
                            objRadioButtonList.RepeatColumns = 4

                            Dim values As String() = objCustomField.FieldElements.Split(Convert.ToChar("|"))
                            For Each value As String In values
                                objRadioButtonList.Items.Add(value)
                            Next

                            If (objCustomField.DefaultValue <> "") Then
                                If Not (objRadioButtonList.Items.FindByValue(objCustomField.DefaultValue) Is Nothing) Then
                                    objRadioButtonList.SelectedValue = objCustomField.DefaultValue
                                End If
                            End If

                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) And (Page.IsPostBack = False Or objRadioButtonList.Enabled = False)) Then
                                    If Not (objRadioButtonList.Items.FindByValue(objFeedback.CustomList(objCustomField.CustomFieldID).ToString()) Is Nothing) Then
                                        objRadioButtonList.SelectedValue = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                                    End If
                                End If
                            End If

                            phValue.Controls.Add(objRadioButtonList)

                            If (objCustomField.IsRequired) Then
                                Dim valRequired As New RequiredFieldValidator
                                valRequired.ControlToValidate = objRadioButtonList.ID
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                valRequired.CssClass = "NormalRed"
                                valRequired.Display = ValidatorDisplay.None
                                valRequired.SetFocusOnError = True
                                phValue.Controls.Add(valRequired)
                            End If


                        Case CustomFieldType.FileUpload

                            Dim hasValue As Boolean = False
                            Dim objHtmlInputFile As New HtmlInputFile
                            objHtmlInputFile.ID = objCustomField.CustomFieldID.ToString()
                            phValue.Controls.Add(objHtmlInputFile)

                            If Not (objFeedback Is Nothing) Then
                                If (objFeedback.CustomList.Contains(objCustomField.CustomFieldID) AndAlso objFeedback.CustomList(objCustomField.CustomFieldID).ToString() <> "") Then

                                    Dim fileName As String = Path.GetFileName(PortalSettings.HomeDirectoryMapPath & objFeedback.CustomList(objCustomField.CustomFieldID).ToString())
                                    Dim objLabel As New Label
                                    objLabel.ID = objCustomField.CustomFieldID.ToString() & "-Label"
                                    objLabel.Text = fileName & "<BR>"
                                    objLabel.CssClass = "Normal"
                                    phValue.Controls.Add(objLabel)

                                    Dim objDelete As New LinkButton
                                    objDelete.ID = objCustomField.CustomFieldID.ToString() & "-Button"
                                    objDelete.Text = Localization.GetString("cmdDelete")
                                    objDelete.CssClass = "CommandButton"
                                    objDelete.CommandArgument = objCustomField.CustomFieldID.ToString()
                                    AddHandler objDelete.Click, AddressOf FileUploadButton_Click
                                    phValue.Controls.Add(objDelete)

                                    phValue.Controls(0).Visible = False
                                    hasValue = True
                                End If
                            End If


                            If (objCustomField.IsRequired) Then
                                Dim valRequired As New RequiredFieldValidator
                                valRequired.ID = objCustomField.CustomFieldID.ToString() & "-Required"
                                valRequired.ControlToValidate = objHtmlInputFile.ID
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", Me.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name)
                                valRequired.CssClass = "NormalRed"
                                valRequired.Display = ValidatorDisplay.None
                                valRequired.Visible = Not hasValue
                                valRequired.SetFocusOnError = True
                                phValue.Controls.Add(valRequired)
                            End If


                    End Select

                End If

            End If

        End Sub

        Public Sub FileUploadButton_Click(ByVal sender As Object, ByVal e As EventArgs)

            Dim objDelete As LinkButton = CType(sender, LinkButton)

            For Each objControl As Control In rptCustomFields.Controls
                Dim phValue As PlaceHolder = CType(objControl.FindControl("phValue"), PlaceHolder)

                If Not (phValue Is Nothing) Then
                    Dim objHtmlInputFile As HtmlInputFile = CType(phValue.FindControl(objDelete.CommandArgument), HtmlInputFile)
                    If Not (objHtmlInputFile Is Nothing) Then
                        objHtmlInputFile.Visible = True
                    End If

                    Dim valRequired As RequiredFieldValidator = CType(phValue.FindControl(objDelete.CommandArgument & "-Required"), RequiredFieldValidator)
                    If Not (valRequired Is Nothing) Then
                        valRequired.Visible = True
                    End If

                    Dim objLabel As Label = CType(phValue.FindControl(objDelete.CommandArgument & "-Label"), Label)
                    If Not (objLabel Is Nothing) Then
                        objLabel.Visible = False
                    End If

                    Dim objDeleteLink As LinkButton = CType(phValue.FindControl(objDelete.CommandArgument & "-Button"), LinkButton)
                    If Not (objDeleteLink Is Nothing) Then
                        objDeleteLink.Visible = False
                    End If
                End If
            Next

        End Sub

        Public Sub FileUploadButton2_Click(ByVal sender As Object, ByVal e As EventArgs)

            Dim objDelete As LinkButton = CType(sender, LinkButton)

            Dim objHtmlInputFile As HtmlInputFile = CType(phFormTemplate.FindControl(objDelete.CommandArgument), HtmlInputFile)
            If Not (objHtmlInputFile Is Nothing) Then
                objHtmlInputFile.Visible = True
            End If

            Dim valRequired As RequiredFieldValidator = CType(phFormTemplate.FindControl(objDelete.CommandArgument & "-Required"), RequiredFieldValidator)
            If Not (valRequired Is Nothing) Then
                valRequired.Visible = True
            End If

            Dim objLabel As Label = CType(phFormTemplate.FindControl(objDelete.CommandArgument & "-Label"), Label)
            If Not (objLabel Is Nothing) Then
                objLabel.Visible = False
            End If

            Dim objDeleteLink As LinkButton = CType(phFormTemplate.FindControl(objDelete.CommandArgument & "-Button"), LinkButton)
            If Not (objDeleteLink Is Nothing) Then
                objDeleteLink.Visible = False
            End If

        End Sub

#End Region

    End Class

End Namespace