Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities

Namespace Ventrian.FeedbackCenter

    Partial Public Class Settings
        Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region " Private Members "

        Private _feedbackSettings As FeedbackSettings

#End Region

#Region " Private Methods "

        Private Sub BindRoles()

            Dim objRoleController As New RoleController
            Dim availableRoles As New ArrayList
            Dim objRoles As ArrayList = objRoleController.GetPortalRoles(Me.PortalId)

            For Each objRole As RoleInfo In objRoles
                availableRoles.Add(New ListItem(objRole.RoleName, objRole.RoleName))
            Next

            availableRoles.Add(New ListItem(Localization.GetString("UnauthenticatedUsers", Me.LocalResourceFile), glbRoleUnauthUserName))

            grdPermissions.DataSource = availableRoles
            grdPermissions.DataBind()

        End Sub

        Private Function IsInRoles(ByVal roleName As String, ByVal roles As String()) As Boolean

            For Each role As String In roles
                If (roleName = role) Then
                    Return True
                End If
            Next

            Return False

        End Function

#End Region

#Region " Private Properties "

        Public ReadOnly Property FeedbackSettings() As FeedbackSettings
            Get
                If (_feedbackSettings Is Nothing) Then
                    _feedbackSettings = New FeedbackSettings(Me.Settings)
                End If
                Return _feedbackSettings
            End Get
        End Property

#End Region

#Region " Event Handlers "

        Private Sub grdPermissions_ItemDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdPermissions.ItemDataBound

            If (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then
                Dim objListItem As ListItem = CType(e.Item.DataItem, ListItem)

                If Not (objListItem Is Nothing) Then

                    Dim role As String = CType(e.Item.DataItem, ListItem).Value

                    Dim chkSubmit As CheckBox = CType(e.Item.FindControl("chkSubmit"), CheckBox)
                    Dim chkVote As CheckBox = CType(e.Item.FindControl("chkVote"), CheckBox)
                    Dim chkComment As CheckBox = CType(e.Item.FindControl("chkComment"), CheckBox)
                    Dim chkAutoApprove As CheckBox = CType(e.Item.FindControl("chkAutoApprove"), CheckBox)
                    Dim chkApprove As CheckBox = CType(e.Item.FindControl("chkApprove"), CheckBox)

                    If (objListItem.Value = PortalSettings.AdministratorRoleName.ToString()) Then

                        chkSubmit.Enabled = False
                        chkSubmit.Checked = True

                        chkComment.Checked = IsInRoles(role, FeedbackSettings.PermissionComment.Split(";"c))

                        ' Check submit for registered users if settings has not been set
                        If (Settings.Contains(Constants.PERMISSION_COMMENT_SETTING) = False) Then
                            chkComment.Checked = True
                        End If

                        chkVote.Checked = IsInRoles(role, FeedbackSettings.PermissionVote.Split(";"c))

                        ' Check submit for registered users if settings has not been set
                        If (Settings.Contains(Constants.PERMISSION_VOTE_SETTING) = False) Then
                            chkVote.Checked = True
                        End If

                        chkAutoApprove.Enabled = False
                        chkAutoApprove.Checked = True

                        chkApprove.Enabled = False
                        chkApprove.Checked = True

                    Else

                        chkSubmit.Checked = IsInRoles(role, FeedbackSettings.PermissionSubmit.Split(";"c))
                        chkVote.Checked = IsInRoles(role, FeedbackSettings.PermissionVote.Split(";"c))
                        chkComment.Checked = IsInRoles(role, FeedbackSettings.PermissionComment.Split(";"c))
                        chkAutoApprove.Checked = IsInRoles(role, FeedbackSettings.PermissionAutoApprove.Split(";"c))
                        chkApprove.Checked = IsInRoles(role, FeedbackSettings.PermissionApprove.Split(";"c))

                        ' Check submit for registered users if settings has not been set
                        If (Settings.Contains(Constants.PERMISSION_SUBMIT_SETTING) = False) Then
                            If (objListItem.Value = PortalSettings.RegisteredRoleName) Then
                                chkSubmit.Checked = True
                            End If
                        End If

                        If (Settings.Contains(Constants.PERMISSION_VOTE_SETTING) = False) Then
                            If (objListItem.Value = PortalSettings.RegisteredRoleName) Then
                                chkVote.Checked = True
                            End If
                        End If

                        If (Settings.Contains(Constants.PERMISSION_COMMENT_SETTING) = False) Then
                            If (objListItem.Value = PortalSettings.RegisteredRoleName) Then
                                chkComment.Checked = True
                            End If
                        End If

                        If (Settings.Contains(Constants.PERMISSION_AUTO_APPROVE_SETTING) = False) Then
                            If (objListItem.Value = PortalSettings.RegisteredRoleName) Then
                                chkAutoApprove.Checked = True
                            End If
                        End If

                    End If

                End If

            End If

        End Sub

#End Region

#Region " Base Method Implementations "

        Public Overrides Sub LoadSettings()
            Try
                If (Page.IsPostBack = False) Then
                    BindRoles()
                    chkEnableCaptcha.Checked = FeedbackSettings.EnableCaptcha
                    chkEnableRSS.Checked = FeedbackSettings.EnableRSS
                    chkEnableStatistics.Checked = FeedbackSettings.EnableStatistics
                    chkCommentAttachments.Checked = FeedbackSettings.CommentAttachments
                    If CType(TabModuleSettings("EmailNotification"), String) <> "" Then
                        txtEmailNotification.Text = CType(TabModuleSettings("EmailNotification"), String)
                    End If
                    txtActiveSocialSubmissionKey.Text = FeedbackSettings.ActiveSocialSubmissionKey
                    txtActiveSocialSubscribeKey.Text = FeedbackSettings.ActiveSocialSubscribeKey
                    txtActiveSocialVoteKey.Text = FeedbackSettings.ActiveSocialVoteKey
                    txtActiveSocialCommentKey.Text = FeedbackSettings.ActiveSocialCommentKey
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Overrides Sub UpdateSettings()
            Try

                Dim submitRoles As String = ""
                Dim voteRoles As String = ""
                Dim commentRoles As String = ""
                Dim autoApproveRoles As String = ""
                Dim approveRoles As String = ""

                For Each item As DataGridItem In grdPermissions.Items

                    Dim role As String = grdPermissions.DataKeys(item.ItemIndex).ToString()

                    Dim chkSubmit As CheckBox = CType(item.FindControl("chkSubmit"), CheckBox)
                    Dim chkVote As CheckBox = CType(item.FindControl("chkVote"), CheckBox)
                    Dim chkComment As CheckBox = CType(item.FindControl("chkComment"), CheckBox)
                    Dim chkAutoApprove As CheckBox = CType(item.FindControl("chkAutoApprove"), CheckBox)
                    Dim chkApprove As CheckBox = CType(item.FindControl("chkApprove"), CheckBox)

                    If (chkSubmit.Checked) Then
                        If (submitRoles = "") Then
                            submitRoles = role
                        Else
                            submitRoles = submitRoles & ";" & role
                        End If
                    End If

                    If (chkVote.Checked) Then
                        If (voteRoles = "") Then
                            voteRoles = role
                        Else
                            voteRoles = voteRoles & ";" & role
                        End If
                    End If

                    If (chkComment.Checked) Then
                        If (commentRoles = "") Then
                            commentRoles = role
                        Else
                            commentRoles = commentRoles & ";" & role
                        End If
                    End If

                    If (chkAutoApprove.Checked) Then
                        If (autoApproveRoles = "") Then
                            autoApproveRoles = role
                        Else
                            autoApproveRoles = autoApproveRoles & ";" & role
                        End If
                    End If

                    If (chkApprove.Checked) Then
                        If (approveRoles = "") Then
                            approveRoles = role
                        Else
                            approveRoles = approveRoles & ";" & role
                        End If
                    End If

                Next

                Dim objModules As New ModuleController

                objModules.UpdateModuleSetting(ModuleId, Constants.ENABLE_CAPTCHA_SETTING, chkEnableCaptcha.Checked.ToString())
                objModules.UpdateModuleSetting(ModuleId, Constants.ENABLE_RSS_SETTING, chkEnableRSS.Checked.ToString())
                objModules.UpdateModuleSetting(ModuleId, Constants.ENABLE_STATISTICS_SETTING, chkEnableStatistics.Checked.ToString())
                objModules.UpdateModuleSetting(ModuleId, Constants.COMMENT_ATTACHMENT_SETTING, chkCommentAttachments.Checked.ToString())

                objModules.UpdateModuleSetting(ModuleId, Constants.PERMISSION_SUBMIT_SETTING, submitRoles)
                objModules.UpdateModuleSetting(ModuleId, Constants.PERMISSION_VOTE_SETTING, voteRoles)
                objModules.UpdateModuleSetting(ModuleId, Constants.PERMISSION_COMMENT_SETTING, commentRoles)
                objModules.UpdateModuleSetting(ModuleId, Constants.PERMISSION_AUTO_APPROVE_SETTING, autoApproveRoles)
                objModules.UpdateModuleSetting(ModuleId, Constants.PERMISSION_APPROVE_SETTING, approveRoles)
                objModules.UpdateTabModuleSetting(TabModuleId, "EmailNotification", txtEmailNotification.Text)

                objModules.UpdateModuleSetting(ModuleId, Constants.ACTIVE_SOCIAL_SUBMISSION_SETTING, txtActiveSocialSubmissionKey.Text.Trim())
                objModules.UpdateModuleSetting(ModuleId, Constants.ACTIVE_SOCIAL_SUBSCRIBE_SETTING, txtActiveSocialSubscribeKey.Text.Trim())
                objModules.UpdateModuleSetting(ModuleId, Constants.ACTIVE_SOCIAL_VOTE_SETTING, txtActiveSocialVoteKey.Text.Trim())
                objModules.UpdateModuleSetting(ModuleId, Constants.ACTIVE_SOCIAL_COMMENT_SETTING, txtActiveSocialCommentKey.Text.Trim())

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace