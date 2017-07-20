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
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities
Imports DotNetNuke.Security.Roles
Imports Ventrian.FeedbackCenter.Entities.Layout


Namespace Ventrian.FeedbackCenter

    Partial Public Class ViewFeedback
        Inherits FeedbackCenterBase

#Region " Private Members "

        Private _feedbackID As Integer = Null.NullInteger

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If Not (Request("FeedbackID") Is Nothing) Then
                _feedbackID = Convert.ToInt32(Request("FeedbackID"))
            End If

        End Sub

        Private Sub SetVisibility()

            cmdEditFeedback.Visible = (Me.IsEditable Or PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove))

        End Sub

        Private Sub BindTemplate()

            Dim objFeedbackController As New FeedbackController
            Dim objFeedback As FeedbackInfo = objFeedbackController.Get(_feedbackID)

            If Not (objFeedback Is Nothing) Then

                If (objFeedback.IsApproved = False) Then
                    If (Settings.Contains(Constants.PERMISSION_APPROVE_SETTING)) Then
                        If (PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove) = False) Then
                            Response.Redirect(NavigateURL(Me.TabId, "", "fbType=NotAuthorized"), True)
                        End If
                    End If
                End If

                Dim objProductController As New ProductController()
                Dim objProduct As ProductInfo = objProductController.Get(objFeedback.ProductID)

                If (objProduct IsNot Nothing) Then
                    If (objProduct.InheritSecurity = False) Then
                        If (Settings.Contains(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING)) Then
                            If (PortalSecurity.IsInRoles(Settings(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING).ToString()) = False) Then
                                Response.Redirect(NavigateURL(Me.TabId, "", "fbType=NotAuthorized"), True)
                            End If
                        End If
                    End If
                End If

                Dim objLayoutController As New LayoutController()
                Dim objView As LayoutInfo = objLayoutController.GetLayout(LayoutType.View_Item_Html, ModuleId, Settings)
                ProcessFeedbackItem(phControls.Controls, objView.Tokens, objFeedback)

                Dim crumbs As New ArrayList

                Dim crumbAll As New CrumbInfo
                crumbAll.Caption = Localization.GetString("AllFeedback", LocalResourceFile)
                crumbAll.Url = NavigateURL()
                crumbs.Add(crumbAll)

                If (Request("Return") <> "") Then
                    Dim crumbReturn As New CrumbInfo
                    crumbReturn.Caption = Localization.GetString(Request("Return"), Me.LocalResourceFile)
                    crumbReturn.Url = NavigateURL(Me.TabId, "", "fbType=" & Request("Return"))
                    crumbs.Add(crumbReturn)
                End If

                Dim crumbProduct As New CrumbInfo
                crumbProduct.Caption = objFeedback.ProductName
                crumbProduct.Url = Request.RawUrl.ToString()
                crumbs.Add(crumbProduct)

                rptBreadCrumbs.DataSource = crumbs
                rptBreadCrumbs.DataBind()

                If (Request.IsAuthenticated) Then

                    Dim objTrackingController As New TrackingController
                    Dim objTracking As TrackingInfo = objTrackingController.Get(_feedbackID, UserId)

                    lblNoSubscribe.Visible = False
                    If (objTracking Is Nothing) Then
                        chkSubscribe.Checked = False
                    Else
                        chkSubscribe.Checked = True
                    End If

                Else

                    lblNoSubscribe.Visible = True
                    chkSubscribe.Visible = False

                End If

            End If

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try
                ReadQueryString()

                If (Request("Approval") <> "") Then
                    divApprovalMessage.Visible = True
                End If

                BindTemplate()

                If (IsPostBack = False) Then
                    SetVisibility()
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub chkSubscribe_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSubscribe.CheckedChanged

            Try

                If (chkSubscribe.Checked) Then

                    Dim objTracking As New TrackingInfo

                    objTracking.CreateDate = DateTime.Now
                    objTracking.UserID = UserId
                    objTracking.FeedbackID = _feedbackID

                    Dim objTrackingController As New TrackingController
                    objTrackingController.Add(objTracking)

                    If (FeedbackSettings.ActiveSocialSubscribeKey <> "" And Request.IsAuthenticated = True) Then
                        If IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")) Then
                            Dim ai As Object = Nothing
                            Dim asm As System.Reflection.Assembly
                            Dim ac As Object = Nothing
                            Try
                                asm = System.Reflection.Assembly.Load("Active.Modules.Social")
                                ac = asm.CreateInstance("Active.Modules.Social.API.Journal")
                                If Not ac Is Nothing Then
                                    Dim objFeedbackController As New FeedbackController()
                                    Dim objFeedback As FeedbackInfo = objFeedbackController.Get(_feedbackID)

                                    If (objFeedback IsNot Nothing) Then
                                        Dim link As String = ""
                                        link = NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                        If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                            link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                        End If
                                        ac.AddProfileItem(New Guid(FeedbackSettings.ActiveSocialSubscribeKey), objTracking.UserID, link, objFeedback.Title, objFeedback.Details, objFeedback.Details, 1, "", True)
                                    End If
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                    End If

                Else

                    Dim objTrackingController As New TrackingController
                    objTrackingController.Delete(_feedbackID, UserId)

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdEditFeedback_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEditFeedback.Click

            Response.Redirect(NavigateURL(Me.TabId, "", "fbType=Edit", "FeedbackID=" & _feedbackID.ToString()), True)

        End Sub

#End Region

    End Class

End Namespace