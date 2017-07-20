Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities

Imports Ventrian.FeedbackCenter.Entities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Roles
Imports System.IO
Imports Ventrian.FeedbackCenter.Entities.Emails

Namespace Ventrian.FeedbackCenter.Controls

    Partial Public Class PostComment
        Inherits FeedbackCenterControlBase

#Region " Private Properties "

        Private ReadOnly Property LocalResourceFile() As String
            Get
                Return "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx"
            End Get
        End Property

#End Region

#Region " Private Methods "

        Private Function FilterInput(ByVal stringToFilter As String) As String

            Dim tempString As String = stringToFilter

            Dim objPortalSecurity As New PortalSecurity

            stringToFilter = objPortalSecurity.InputFilter(stringToFilter, PortalSecurity.FilterFlag.NoScripting)

            stringToFilter = Replace(stringToFilter, Chr(13), "")
            stringToFilter = Replace(stringToFilter, ControlChars.Lf, "<br>")

            Return stringToFilter

        End Function

        Public Function FormatCommentEmail(ByVal template As String, ByVal link As String, ByVal objFeedback As FeedbackInfo, ByVal objComment As CommentInfo) As String

            Dim formatted As String = template

            formatted = formatted.Replace("[PORTALNAME]", FeedbackCenterBase.PortalSettings.PortalName)
            formatted = formatted.Replace("[POSTDATE]", DateTime.Now.ToShortDateString & " " & DateTime.Now.ToShortTimeString)

            formatted = formatted.Replace("[TITLE]", objFeedback.Title)
            formatted = formatted.Replace("[COMMENT]", objComment.Comment)
            formatted = formatted.Replace("[LINK]", link)

            formatted = formatted.Replace("<br>", vbCrLf)

            Return formatted

        End Function

        Private Function FormatEmail(ByVal template As String, ByVal link As String, ByVal objFeedback As FeedbackInfo) As String

            Dim formatted As String = template

            formatted = formatted.Replace("[PORTALNAME]", FeedbackCenterBase.PortalSettings.PortalName)
            formatted = formatted.Replace("[POSTDATE]", DateTime.Now.ToShortDateString & " " & DateTime.Now.ToShortTimeString)

            formatted = formatted.Replace("[TITLE]", objFeedback.Title)
            formatted = formatted.Replace("[LINK]", link)

            Return formatted

        End Function

        Private Function GetApproverDistributionList() As String

            Dim distributionList As String = ""

            If (FeedbackCenterBase.Settings.Contains(Constants.PERMISSION_APPROVE_SETTING)) Then

                Dim roles As String = FeedbackCenterBase.Settings(Constants.PERMISSION_APPROVE_SETTING).ToString()
                Dim rolesArray() As String = roles.Split(Convert.ToChar(";"))
                Dim userList As Hashtable = New Hashtable

                For Each role As String In rolesArray
                    If (role.Length > 0) Then
                        Dim objRoleController As RoleController = New RoleController
                        Dim objRole As RoleInfo = objRoleController.GetRoleByName(FeedbackCenterBase.PortalId, role)

                        If Not (objRole Is Nothing) Then
                            Dim objUsers As ArrayList = objRoleController.GetUserRolesByRoleName(FeedbackCenterBase.PortalId, objRole.RoleName)
                            For Each objUser As UserRoleInfo In objUsers
                                Dim objUserController As UserController = New UserController
                                Dim objSelectedUser As UserInfo = objUserController.GetUser(FeedbackCenterBase.PortalId, objUser.UserID)
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

                distributionList = FeedbackCenterBase.PortalSettings.Email

            End If

            Return distributionList

        End Function


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

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            phAnonymousUser.Visible = (Request.IsAuthenticated = False)
            phCaptcha.Visible = FeedbackCenterBase.FeedbackSettings.EnableCaptcha
            phAttachment.Visible = FeedbackCenterBase.FeedbackSettings.CommentAttachments
            ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", Me.LocalResourceFile)

        End Sub

        Private Sub btnPostComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPostComment.Click

            Try
                If (Page.IsValid) Then

                    If (FeedbackCenterBase.FeedbackSettings.EnableCaptcha) Then
                        If (ctlCaptcha.IsValid = False) Then
                            Return
                        End If
                    End If

                    Dim objComment As New CommentInfo

                    If (Request.IsAuthenticated) Then
                        objComment.UserID = FeedbackCenterBase.UserId
                    Else
                        objComment.UserID = Null.NullInteger
                        objComment.AnonymousName = txtName.Text
                        objComment.AnonymousEmail = txtEmail.Text
                        objComment.AnonymousUrl = txtURL.Text
                        SetCookie()
                    End If

                    objComment.FeedbackID = FeedbackID
                    objComment.CreateDate = DateTime.Now
                    objComment.Comment = FilterInput(txtComment.Text)

                    If (FeedbackCenterBase.Settings.Contains(Constants.PERMISSION_AUTO_APPROVE_SETTING)) Then
                        If (FeedbackCenterBase.IsEditable Or PortalSecurity.IsInRoles(FeedbackCenterBase.FeedbackSettings.PermissionApprove) Or PortalSecurity.IsInRoles(FeedbackCenterBase.FeedbackSettings.PermissionAutoApprove)) Then
                            objComment.IsApproved = True
                        Else
                            If (FeedbackCenterBase.FeedbackSettings.PermissionAutoApprove.ToLower().Contains(glbRoleUnauthUserName.ToLower()) = False) Then
                                objComment.IsApproved = False
                            Else
                                objComment.IsApproved = True
                            End If
                        End If
                    Else
                        ' Auto-Approve
                        objComment.IsApproved = True
                    End If

                    objComment.FileAttachment = Null.NullString

                    Dim objCommentController As New CommentController
                    objComment.CommentID = objCommentController.Add(objComment)

                    If (phAttachment.Visible And txtCommentAttachment.HasFile) Then
                        Dim path As String = Server.MapPath("~/DesktopModules/FeedbackCenter/Attachments/" & objComment.CommentID.ToString() & "/")
                        If (Directory.Exists(path) = False) Then
                            Directory.CreateDirectory(path)
                        End If
                        txtCommentAttachment.SaveAs(path & txtCommentAttachment.FileName)
                        objComment.FileAttachment = txtCommentAttachment.FileName
                        objCommentController.Update(objComment)
                    Else
                        objComment.FileAttachment = Null.NullString
                    End If

                    Dim objFeedbackController As New FeedbackController
                    Dim objFeedback As FeedbackInfo = objFeedbackController.Get(FeedbackID)

                    Dim emails As String = FeedbackCenterBase.EmailNotification

                    If (objComment.IsApproved) Then

                        Dim objTrackingController As New TrackingController
                        Dim objTrackingList As ArrayList = objTrackingController.List(FeedbackID)

                        For Each objTracking As TrackingInfo In objTrackingList

                            If (objTracking.UserID <> FeedbackCenterBase.UserId) Then

                                Dim objEmailController As New EmailController()

                                Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Subject_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)
                                Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Body_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)

                                Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", FeedbackCenterBase.PortalSettings.PortalName)
                                Dim template As String = objEmailBodyInfo.Template

                                Dim objUserController As New UserController
                                Dim objUser As UserInfo = objUserController.GetUser(FeedbackCenterBase.PortalId, objTracking.UserID)

                                If Not (objUser Is Nothing) Then

                                    Dim link As String = ""

                                    If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                        link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & FeedbackID.ToString())
                                    Else
                                        link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                        If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                            link = AddHTTP(System.Web.HttpContext.Current.Request.Url.Host & link)
                                        End If
                                    End If

                                    template = FormatCommentEmail(template, link, objFeedback, objComment)

                                    Try
                                        DotNetNuke.Services.Mail.Mail.SendMail(FeedbackCenterBase.PortalSettings.Email, objUser.Membership.Email, "", subject, template, "", "", "", "", "", "")
                                    Catch
                                    End Try

                                End If

                            End If

                        Next

                        For Each email As String In emails.Split(",")

                            Dim objEmailController As New EmailController()

                            Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Subject_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)
                            Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Body_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)

                            Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", FeedbackCenterBase.PortalSettings.PortalName)
                            Dim template As String = objEmailBodyInfo.Template

                            Dim link As String = ""

                            If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & FeedbackID.ToString())
                            Else
                                link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                    link = AddHTTP(System.Web.HttpContext.Current.Request.Url.Host & link)
                                End If
                            End If

                            template = FormatCommentEmail(template, link, objFeedback, objComment)
                            Try
                                DotNetNuke.Services.Mail.Mail.SendMail(FeedbackCenterBase.PortalSettings.Email, email, "", subject, template, "", "", "", "", "", "")
                            Catch
                            End Try
                        Next

                        If (FeedbackCenterBase.FeedbackSettings.ActiveSocialCommentKey <> "" And Request.IsAuthenticated = True) Then
                            If IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")) Then
                                Dim ai As Object = Nothing
                                Dim asm As System.Reflection.Assembly
                                Dim ac As Object = Nothing
                                Try
                                    asm = System.Reflection.Assembly.Load("Active.Modules.Social")
                                    ac = asm.CreateInstance("Active.Modules.Social.API.Journal")
                                    If Not ac Is Nothing Then
                                        Dim link As String = ""
                                        link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                        If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                            link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                        End If
                                        ac.AddProfileItem(New Guid(FeedbackCenterBase.FeedbackSettings.ActiveSocialCommentKey), objComment.UserID, link, objFeedback.Title, objComment.Comment, objComment.Comment, 1, "", True)
                                    End If
                                Catch ex As Exception
                                End Try
                            End If
                        End If

                        Dim objProductController As New ProductController
                        Dim objProduct As ProductInfo = objProductController.Get(objFeedback.ProductID)

                        If (objProduct IsNot Nothing) Then
                            If (objProduct.Email <> "") Then
                                Dim objEmailController As New EmailController()

                                Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Subject_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)
                                Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Body_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)

                                Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", FeedbackCenterBase.PortalSettings.PortalName)
                                Dim template As String = objEmailBodyInfo.Template

                                Dim link As String = ""

                                If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                    link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & FeedbackID.ToString())
                                Else
                                    link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                                    If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                        link = AddHTTP(System.Web.HttpContext.Current.Request.Url.Host & link)
                                    End If
                                End If

                                template = FormatCommentEmail(template, link, objFeedback, objComment)
                                Try
                                    DotNetNuke.Services.Mail.Mail.SendMail(FeedbackCenterBase.PortalSettings.Email, objProduct.Email, "", subject, template, "", "", "", "", "", "")
                                Catch
                                End Try
                            End If
                        End If

                    End If

                    If (objComment.IsApproved = False) Then

                        ' Send approval notice.
                        emails = GetApproverDistributionList()

                        For Each email As String In emails.Split(";")

                            Dim objEmailController As New EmailController()

                            Dim objEmailSubjectInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Require_Approval_Subject_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)
                            Dim objEmailBodyInfo As EmailInfo = objEmailController.GetLayout(EmailType.Comment_Require_Approval_Body_Html, FeedbackCenterBase.ModuleId, FeedbackCenterBase.Settings)

                            Dim subject As String = objEmailSubjectInfo.Template.Replace("[PORTALNAME]", FeedbackCenterBase.PortalSettings.PortalName)
                            Dim template As String = objEmailBodyInfo.Template

                            Dim link As String = ""

                            If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                                link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=ApproveComments")
                            Else
                                link = NavigateURL(FeedbackCenterBase.TabId, "", "fbType=ApproveComments")
                                If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                                    link = "http://" & System.Web.HttpContext.Current.Request.Url.Host & link
                                End If
                            End If

                            template = FormatEmail(template, link, objFeedback)
                            Try
                                DotNetNuke.Services.Mail.Mail.SendMail(FeedbackCenterBase.PortalSettings.Email, email, "", subject, template, "", "", "", "", "", "")
                            Catch
                            End Try
                        Next

                    End If

                    If (objComment.IsApproved) Then
                        Response.Redirect(NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & FeedbackID.ToString()), True)
                    Else
                        Response.Redirect(NavigateURL(FeedbackCenterBase.TabId, "", "fbType=View", "FeedbackID=" & FeedbackID.ToString(), "Approval=1"), True)
                    End If

                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

    End Class

End Namespace