Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Exceptions

Namespace Ventrian.FeedbackCenter

    Partial Public Class LatestOptions
        Inherits ModuleSettingsBase

#Region " Private Methods "

        Private Sub BindModules()

            Dim objDesktopModuleController As New DesktopModuleController
            Dim objDesktopModuleInfo As DesktopModuleInfo = objDesktopModuleController.GetDesktopModuleByModuleName("FeedbackCenter")

            If Not (objDesktopModuleInfo Is Nothing) Then

                Dim objTabController As New TabController()
                Dim objTabs As ArrayList = objTabController.GetTabs(PortalId)
                For Each objTab As DotNetNuke.Entities.Tabs.TabInfo In objTabs
                    If Not (objTab Is Nothing) Then
                        If (objTab.IsDeleted = False) Then
                            Dim objModules As New ModuleController
                            For Each pair As KeyValuePair(Of Integer, ModuleInfo) In objModules.GetTabModules(objTab.TabID)
                                Dim objModule As ModuleInfo = pair.Value
                                If (objModule.IsDeleted = False) Then
                                    If (objModule.DesktopModuleID = objDesktopModuleInfo.DesktopModuleID) Then
                                        If PortalSecurity.IsInRoles(objModule.AuthorizedEditRoles) = True And objModule.IsDeleted = False Then
                                            Dim strPath As String = objTab.TabName
                                            Dim objTabSelected As DotNetNuke.Entities.Tabs.TabInfo = objTab
                                            While objTabSelected.ParentId <> Null.NullInteger
                                                objTabSelected = objTabController.GetTab(objTabSelected.ParentId, objTab.PortalID, False)
                                                If (objTabSelected Is Nothing) Then
                                                    Exit While
                                                End If
                                                strPath = objTabSelected.TabName & " -> " & strPath
                                            End While

                                            Dim objListItem As New ListItem

                                            objListItem.Value = objModule.TabID.ToString() & "-" & objModule.ModuleID.ToString()
                                            objListItem.Text = strPath & " -> " & objModule.ModuleTitle

                                            drpModuleID.Items.Add(objListItem)
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next

            End If

        End Sub

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

#End Region

#Region " Base Method Implementations "

        Public Overrides Sub LoadSettings()

            If (IsPostBack = False) Then
                BindModules()

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_MODULE_ID) And Settings.Contains(Entities.Constants.FEEDBACK_LATEST_TAB_ID)) Then
                    If Not (drpModuleID.Items.FindByValue(Settings(Entities.Constants.FEEDBACK_LATEST_TAB_ID).ToString() & "-" & Settings(Entities.Constants.FEEDBACK_LATEST_MODULE_ID).ToString()) Is Nothing) Then
                        drpModuleID.SelectedValue = Settings(Entities.Constants.FEEDBACK_LATEST_TAB_ID).ToString() & "-" & Settings(Entities.Constants.FEEDBACK_LATEST_MODULE_ID).ToString()
                    End If
                End If

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_HEADER)) Then
                    txtHtmlHeader.Text = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_HEADER).ToString()
                Else
                    txtHtmlHeader.Text = Entities.Constants.FEEDBACK_LATEST_HTML_HEADER_DEFAULT
                End If

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_BODY)) Then
                    txtHtmlBody.Text = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_BODY).ToString()
                Else
                    txtHtmlBody.Text = Entities.Constants.FEEDBACK_LATEST_HTML_BODY_DEFAULT
                End If

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_FOOTER)) Then
                    txtHtmlFooter.Text = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_FOOTER).ToString()
                Else
                    txtHtmlFooter.Text = Entities.Constants.FEEDBACK_LATEST_HTML_FOOTER_DEFAULT
                End If

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_NO_FEEDBACK)) Then
                    txtHtmlNoFeedback.Text = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_NO_FEEDBACK).ToString()
                Else
                    txtHtmlNoFeedback.Text = Entities.Constants.FEEDBACK_LATEST_HTML_NO_FEEDBACK_DEFAULT
                End If

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_MAX_COUNT)) Then
                    txtFeedbackCount.Text = Settings(Entities.Constants.FEEDBACK_LATEST_MAX_COUNT).ToString()
                Else
                    txtFeedbackCount.Text = Entities.Constants.FEEDBACK_LATEST_MAX_COUNT_DEFAULT.ToString()
                End If

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_USER_ID_FILTER)) Then
                    chkQueryStringFilter.Checked = Convert.ToBoolean(Settings(Entities.Constants.FEEDBACK_LATEST_USER_ID_FILTER).ToString())
                Else
                    chkQueryStringFilter.Checked = Entities.Constants.FEEDBACK_LATEST_USER_ID_FILTER_DEFAULT
                End If

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_USER_ID_PARAM)) Then
                    txtQueryStringParam.Text = Settings(Entities.Constants.FEEDBACK_LATEST_USER_ID_PARAM).ToString()
                Else
                    txtQueryStringParam.Text = Entities.Constants.FEEDBACK_LATEST_USER_ID_PARAM_DEFAULT
                End If

            End If

        End Sub

        Public Overrides Sub UpdateSettings()

            Try

                Dim objModuleController As New ModuleController

                If (drpModuleID.Items.Count > 0) Then

                    Dim values As String() = drpModuleID.SelectedValue.Split(Convert.ToChar("-"))

                    If (values.Length = 2) Then
                        objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_TAB_ID, values(0))
                        objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_MODULE_ID, values(1))
                    End If

                    objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_HTML_HEADER, txtHtmlHeader.Text)
                    objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_HTML_BODY, txtHtmlBody.Text)
                    objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_HTML_FOOTER, txtHtmlFooter.Text)
                    objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_HTML_NO_FEEDBACK, txtHtmlNoFeedback.Text)

                    If (Convert.ToInt32(txtFeedbackCount.Text) > 0) Then
                        objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_MAX_COUNT, txtFeedbackCount.Text)
                    End If

                    objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_USER_ID_FILTER, chkQueryStringFilter.Checked.ToString())
                    objModuleController.UpdateTabModuleSetting(Me.TabModuleId, Entities.Constants.FEEDBACK_LATEST_USER_ID_PARAM, txtQueryStringParam.Text)

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

    End Class

End Namespace