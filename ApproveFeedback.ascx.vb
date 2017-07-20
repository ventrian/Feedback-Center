Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities
Imports DotNetNuke.Entities.Users
Imports Ventrian.FeedbackCenter.Entities.Emails

Namespace Ventrian.FeedbackCenter

    Partial Public Class ApproveFeedback
        Inherits FeedbackCenterBase

#Region " Private Methods "

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim crumbAllAlbums As New CrumbInfo
            crumbAllAlbums.Caption = Localization.GetString("AllFeedback", LocalResourceFile)
            crumbAllAlbums.Url = NavigateURL()
            crumbs.Add(crumbAllAlbums)

            Dim currentCrumb As New CrumbInfo
            currentCrumb.Caption = Localization.GetString("ApproveFeedback", LocalResourceFile)
            currentCrumb.Url = Request.RawUrl.ToString()
            crumbs.Add(currentCrumb)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

        Private Sub CheckSecurity()

            If (IsEditable = False And PortalSecurity.IsInRoles(FeedbackSettings.PermissionApprove) = False) Then
                Response.Redirect(NavigateURL(Me.TabId, "", "fbType=NotAuthorized"), True)
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

#End Region

#Region " Protected Methods "

        Protected Function GetAuthor(ByVal objItem As Object)

            Dim objFeedback As FeedbackInfo = CType(objItem, FeedbackInfo)

            If (objFeedback IsNot Nothing) Then
                If (objFeedback.UserID <> Null.NullInteger) Then
                    Return objFeedback.DisplayName
                Else
                    Return objFeedback.AnonymousName
                End If
            Else
                Return ""
            End If

        End Function

        Protected Function GetFeedbackTitle(ByVal objItem As Object) As String

            Dim objFeedback As FeedbackInfo = CType(objItem, FeedbackInfo)

            If (objFeedback IsNot Nothing) Then
                Return objFeedback.Title
            Else
                Return ""
            End If

        End Function

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            CheckSecurity()
            BindCrumbs()

            If (Page.IsPostBack = False) Then

                Dim objFeedbackController As New FeedbackController

                grdApproveFeedback.DataSource = objFeedbackController.List(ModuleId, Null.NullInteger, False, Null.NullString, SortType.CreateDate, SortDirection.Descending, Null.NullInteger, Null.NullInteger, Null.NullInteger, False, Nothing)
                grdApproveFeedback.DataBind()

                If (grdApproveFeedback.Items.Count = 0) Then
                    phFeedback.Visible = False
                    lblNoFeedback.Visible = True
                End If

            End If

        End Sub

        Protected Sub cmdApprove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdApprove.Click

            For Each item As DataGridItem In grdApproveFeedback.Items
                If (item.ItemType = ListItemType.Item Or item.ItemType = ListItemType.AlternatingItem) Then
                    Dim chkSelected As CheckBox = CType(item.FindControl("chkSelected"), CheckBox)
                    If Not (chkSelected Is Nothing) Then
                        If (chkSelected.Checked) Then
                            Dim objFeedbackController As New FeedbackController()
                            Dim objFeedback As FeedbackInfo = objFeedbackController.Get(Convert.ToInt32(grdApproveFeedback.DataKeys(item.ItemIndex)))
                            If Not (objFeedback Is Nothing) Then
                                objFeedback.IsApproved = True
                                objFeedbackController.Update(objFeedback)

                                ' Send Notification..
                                Dim objEmailController As New EmailController()

                                Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Approved_Subject_Html, ModuleId, Settings)
                                Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Feedback_Approved_Body_Html, ModuleId, Settings)

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
                                    If (objFeedback.UserID = Null.NullInteger) Then
                                        DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objFeedback.AnonymousEmail, "", subject, template, "", "", "", "", "", "")
                                    Else
                                        Dim objUserController As New UserController
                                        Dim objUser As UserInfo = objUserController.GetUser(Me.PortalId, objFeedback.UserID)

                                        If Not (objUser Is Nothing) Then
                                            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUser.Membership.Email, "", subject, template, "", "", "", "", "", "")
                                        End If
                                    End If

                                    Dim objProductController As New ProductController
                                    Dim objProduct As ProductInfo = objProductController.Get(objFeedback.ProductID)

                                    If (objProduct IsNot Nothing) Then
                                        If (objProduct.Email <> "") Then
                                            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objProduct.Email, "", subject, template, "", "", "", "", "", "")
                                        End If
                                    End If

                                Catch
                                End Try

                                If (FeedbackSettings.ActiveSocialSubmissionKey <> "" And Request.IsAuthenticated = True) Then
                                    If IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")) Then
                                        Dim ai As Object = Nothing
                                        Dim asm As System.Reflection.Assembly
                                        Dim ac As Object = Nothing
                                        Try
                                            asm = System.Reflection.Assembly.Load("Active.Modules.Social")
                                            ac = asm.CreateInstance("Active.Modules.Social.API.Journal")
                                            If Not ac Is Nothing Then
                                                ac.AddProfileItem(FeedbackSettings.ActiveSocialSubmissionKey, objFeedback.UserID, link, objFeedback.Title, objFeedback.Details, objFeedback.Details, 1, "", True)
                                            End If
                                        Catch ex As Exception
                                        End Try
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Next

            Response.Redirect(Request.RawUrl, True)

        End Sub

        Protected Sub cmdReject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReject.Click

            For Each item As DataGridItem In grdApproveFeedback.Items
                If (item.ItemType = ListItemType.Item Or item.ItemType = ListItemType.AlternatingItem) Then
                    Dim chkSelected As CheckBox = CType(item.FindControl("chkSelected"), CheckBox)
                    If Not (chkSelected Is Nothing) Then
                        If (chkSelected.Checked) Then
                            Dim objFeedbackController As New FeedbackController()
                            Dim objFeedback As FeedbackInfo = objFeedbackController.Get(Convert.ToInt32(grdApproveFeedback.DataKeys(item.ItemIndex)))
                            If Not (objFeedback Is Nothing) Then
                                objFeedbackController.Delete(objFeedback.FeedbackID)
                            End If
                        End If
                    End If
                End If
            Next

            Response.Redirect(Request.RawUrl, True)

        End Sub

#End Region

    End Class

End Namespace