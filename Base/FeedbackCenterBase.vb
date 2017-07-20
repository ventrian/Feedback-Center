'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Xml

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities
Imports Ventrian.FeedbackCenter.Entities.Layout
Imports Ventrian.FeedbackCenter.Entities.CustomFields

Namespace Ventrian.FeedbackCenter

    Public Class FeedbackCenterBase
        Inherits PortalModuleBase

#Region " Private Members "

        Private _feedbackSettings As FeedbackSettings

#End Region

#Region " Private Methods "

        Private Function GetFieldValue(ByVal objCustomField As CustomFieldInfo, ByVal objFeedback As FeedbackInfo) As String

            Dim value As String = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
            If (objCustomField.FieldType = CustomFieldType.RichTextBox) Then
                value = Page.Server.HtmlDecode(objFeedback.CustomList(objCustomField.CustomFieldID).ToString())
            Else
                If (objCustomField.FieldType = CustomFieldType.MultiCheckBox) Then
                    value = objFeedback.CustomList(objCustomField.CustomFieldID).ToString().Replace("|", ", ")
                End If
                If (objCustomField.FieldType = CustomFieldType.MultiLineTextBox) Then
                    value = objFeedback.CustomList(objCustomField.CustomFieldID).ToString().Replace(vbCrLf, "<br />")
                End If
                If (objCustomField.FieldType = CustomFieldType.FileUpload) Then
                    value = PortalSettings.HomeDirectory & objFeedback.CustomList(objCustomField.CustomFieldID).ToString().Replace("\", "/")
                End If
                If (value <> "" And objCustomField.ValidationType = CustomFieldValidationType.Date) Then
                    Try
                        value = DateTime.Parse(value).ToShortDateString()
                    Catch
                        value = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                    End Try
                End If

                If (value <> "" And objCustomField.ValidationType = CustomFieldValidationType.Currency) Then
                    Try
                        Dim culture As String = "en-US"

                        Dim portalFormat As System.Globalization.CultureInfo = New System.Globalization.CultureInfo(culture)
                        Dim format As String = "{0:C2}"
                        value = String.Format(portalFormat.NumberFormat, format, Double.Parse(value))

                    Catch
                        value = objFeedback.CustomList(objCustomField.CustomFieldID).ToString()
                    End Try
                End If
            End If

            Return value

        End Function

#End Region

#Region " Protected Properties "

        Public ReadOnly Property EmailNotification() As String
            Get
                If (Settings.Contains("EmailNotification")) Then
                    Return CType(Settings("EmailNotification"), String)
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property FeedbackSettings() As FeedbackSettings
            Get
                If (_feedbackSettings Is Nothing) Then
                    _feedbackSettings = New FeedbackSettings(Me.Settings)
                End If
                Return _feedbackSettings
            End Get
        End Property

#End Region

#Region " Protected Methods "

        Protected Function HasEditPermissions() As Boolean

            Return _
                (PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedEditRoles) = True) Or _
                (PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles) = True) Or _
                (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) = True)

        End Function

        Protected Sub ProcessPaging(ByRef controls As ControlCollection, ByVal layoutArray As String(), ByVal paging As String, ByVal results As String)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "PAGING"
                            Dim objLiteral As New Literal
                            objLiteral.Text = paging
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "RESULTS"
                            Dim objLiteral As New Literal
                            objLiteral.Text = results
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else
                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessPostComment(ByRef controls As ControlCollection, ByVal layoutArray As String(), ByVal objFeedback As FeedbackInfo)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "COMMENTFORM"
                            Dim objControl As Control = Page.LoadControl("~/DesktopModules/FeedbackCenter/Controls/PostComment.ascx")
                            CType(objControl, FeedbackCenterControlBase).FeedbackID = objFeedback.FeedbackID
                            controls.Add(objControl)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else
                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessLayoutMakeSuggestion(ByRef controls As ControlCollection, ByVal layoutArray As String())

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "SUGGESTIONLINK"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetEditUrl()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessLayoutProductListHeaderFooter(ByRef controls As ControlCollection, ByVal layoutArray As String())

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "FEEDBACKTRACKINGRSS"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = "<a href='" & GetRssTrackingFeedbackUrl() & "'><img src='" & Me.ResolveUrl("Images/xml_small.gif") & "' alt='' border='0' align='absBottom'></a>"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "FEEDBACKTRACKINGURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetTrackingFeedbackUrl()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "HIGHESTRATEDRSS"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = "<a href='" & GetRssHighestRatedUrl() & "'><img src='" & Me.ResolveUrl("Images/xml_small.gif") & "' alt='' border='0' align='absBottom'></a>"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "HIGHESTRATEDURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetSearchUrlSort("Popularity", "Open")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "MYFEEDBACKRSS"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = "<a href='" & GetRssMyFeedbackUrl() & "'><img src='" & Me.ResolveUrl("Images/xml_small.gif") & "' alt='' border='0' align='absBottom'></a>"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "MYFEEDBACKURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetMyFeedbackUrl()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "RECENTLYCOMMENTEDRSS"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = "<a href='" & GetRssRecentlyCommentedUrl() & "'><img src='" & Me.ResolveUrl("Images/xml_small.gif") & "' alt='' border='0' align='absBottom'></a>"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "RECENTLYCOMMENTEDURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetSearchUrlSort("LastCommentDate", "Open")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "RECENTLYIMPLEMENTEDRSS"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = "<a href='" & GetRssRecentlyImplementedUrl() & "'><img src='" & Me.ResolveUrl("Images/xml_small.gif") & "' alt='' border='0' align='absBottom'></a>"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "RECENTLYIMPLEMENTEDURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetSearchUrlSort("ClosedDate", "Closed")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "SEARCHLINK"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetMyFeedbackUrl()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "WHATSNEWRSS"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = "<a href='" & GetRssWhatsNewUrl() & "'><img src='" & Me.ResolveUrl("Images/xml_small.gif") & "' alt='' border='0' align='absBottom'></a>"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "WHATSNEWURL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetSearchUrlSort("CreateDate", "Open")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessMessage(ByRef controls As ControlCollection, ByVal layoutArray As String(), ByVal message As String)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "MESSAGE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = message
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessProductItem(ByRef controls As ControlCollection, ByVal layoutArray As String(), ByVal objProduct As ProductInfo)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "ID"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objProduct.ProductID.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "NAME"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objProduct.Name
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "RSS"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = "<a href='" & GetRssUrl(objProduct.ProductID.ToString()) & "'><img src='" & Me.ResolveUrl("Images/xml_small.gif") & "' alt='' border='0' align='absmiddle'></a>"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "RSSLINK"
                            If (FeedbackSettings.EnableRSS) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = GetRssUrl(objProduct.ProductID.ToString())
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "SEARCHLINK"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetSearchUrl(objProduct.ProductID.ToString(), "Open")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessComment(ByRef controls As ControlCollection, ByVal layoutArray As String(), ByVal objComment As CommentInfo)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "ATTACHMENT"
                            If (objComment.FileAttachment <> Null.NullString()) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = objComment.FileAttachment
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "ATTACHMENTLINK"
                            If (objComment.FileAttachment <> Null.NullString()) Then
                                Dim objLiteral As New Literal
                                objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Attachments/" & objComment.CommentID.ToString() & "/" & objComment.FileAttachment)
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)
                            End If

                        Case "COMMENT"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objComment.Comment
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "COMMENTDATE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objComment.CreateDate.ToString("d")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "DELETE"
                            Dim objLinkDelete As New LinkButton
                            objLinkDelete.Text = Localization.GetString("cmdDeleteComment", "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx")
                            objLinkDelete.EnableViewState = False
                            objLinkDelete.CssClass = "CommandButton"
                            objLinkDelete.CausesValidation = False
                            objLinkDelete.Attributes("CommentID") = objComment.CommentID
                            objLinkDelete.Visible = IsEditable
                            objLinkDelete.OnClientClick = "return confirm('" & Localization.GetString("DeleteItem", "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx") & "');"
                            AddHandler objLinkDelete.Click, AddressOf btnDeleteComment_Click
                            controls.Add(objLinkDelete)

                        Case "DISPLAYNAME"
                            Dim objLiteral As New Literal
                            If (objComment.UserID = -1) Then
                                If (objComment.AnonymousUrl <> "") Then
                                    objLiteral.Text = objComment.AnonymousName & " (<a href='" & AddHTTP(objComment.AnonymousUrl) & "' rel='nofollow' target='_blank'>" & objComment.AnonymousUrl & "</a>)"
                                Else
                                    objLiteral.Text = objComment.AnonymousName
                                End If
                            Else
                                objLiteral.Text = objComment.DisplayName
                            End If
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "HASATTACHMENT"
                            If (objComment.FileAttachment = Null.NullString) Then
                                While (iPtr < layoutArray.Length - 1)
                                    If (layoutArray(iPtr + 1) = "/HASATTACHMENT") Then
                                        Exit While
                                    End If
                                    iPtr = iPtr + 1
                                End While
                            End If

                        Case "/HASATTACHMENT"

                        Case "ICON"
                            Dim objLiteral As New Literal
                            objLiteral.Text = "<img src='" & Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Images/icon_comment.gif") & "' align='absmiddle' />"
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessFeedbackItem(ByRef controls As ControlCollection, ByVal layoutArray As String(), ByVal objFeedback As FeedbackInfo)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "CLOSEDDATE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.ClosedDate.ToString("d")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "COMMENTS"
                            Dim objCommentController As New CommentController()
                            Dim objComments As ArrayList = objCommentController.List(ModuleId, objFeedback.FeedbackID, True)

                            If (objComments.Count > 0) Then

                                Dim objLayoutController As New LayoutController()

                                Dim objListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.View_Comment_Header_Html, ModuleId, Settings)
                                Dim objListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.View_Comment_Item_Html, ModuleId, Settings)
                                Dim objListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.View_Comment_Footer_Html, ModuleId, Settings)

                                ProcessLayoutMakeSuggestion(controls, objListHeader.Tokens)

                                For Each objComment As CommentInfo In objComments
                                    ProcessComment(controls, objListItem.Tokens, objComment)
                                Next

                                ProcessLayoutMakeSuggestion(controls, objListFooter.Tokens)

                            End If

                        Case "CREATEDATE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.CreateDate.ToString("d")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "DESCRIPTION"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.Details
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "DISPLAYNAME"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetDisplayName(objFeedback)
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "EDIT"
                            Dim objLiteral As New Literal
                            objLiteral.Text = "<a href='" & GetEditUrl(objFeedback.FeedbackID) & "'><img title='Edit' src='" & Page.ResolveUrl("~/images/edit.gif") & "' alt='Edit' style='border-width:0px;'/></a>"
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "FEEDBACKID"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.FeedbackID.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "FEEDBACKLINK"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetFeedbackUrl(objFeedback.FeedbackID.ToString())
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "IMPLEMENTED"
                            Dim objLiteral As New Literal
                            If (objFeedback.IsResolved) Then
                                objLiteral.Text = "Yes"
                            Else
                                objLiteral.Text = "No"
                            End If
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "POSTCOMMENT"

                            Dim hasCommentPermission As Boolean = True
                            If (Settings.Contains(Constants.PERMISSION_COMMENT_SETTING)) Then
                                If (PortalSecurity.IsInRoles(FeedbackSettings.PermissionComment) = False) Then
                                    If (FeedbackSettings.PermissionComment.ToLower().Contains(glbRoleUnauthUserName.ToLower()) = False) Then
                                        hasCommentPermission = False
                                    End If
                                End If
                            Else
                                ' Setting not set.
                                If (Request.IsAuthenticated = False) Then
                                    hasCommentPermission = False
                                End If
                            End If

                            If (hasCommentPermission And objFeedback.IsClosed = False) Then
                                Dim objLayoutController As New LayoutController()
                                Dim objPostComment As LayoutInfo = objLayoutController.GetLayout(LayoutType.View_Comment_Form_Html, ModuleId, Settings)
                                ProcessPostComment(controls, objPostComment.Tokens, objFeedback)
                            End If

                        Case "PRODUCT"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.ProductName
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "PRODUCTLINK"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetSearchUrl(objFeedback.ProductID.ToString(), "Open")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "STATUS"
                            Dim objLiteral As New Literal
                            objLiteral.Text = GetStatus(objFeedback)
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "THUMBSDOWN"
                            Dim objLiteral As New Literal
                            objLiteral.Text = "<img src='" & Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Images/ThumbsDown.gif") & "' alt='Votes Against' align='absmiddle'>"
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "THUMBSUP"
                            Dim objLiteral As New Literal
                            objLiteral.Text = "<img src='" & Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Images/ThumbsUp.gif") & "' alt='Votes For' align='absmiddle'>"
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TITLE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.Title
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "VOTE"
                            Dim hasVotePermission As Boolean = True
                            If (Settings.Contains(Constants.PERMISSION_VOTE_SETTING)) Then
                                If (PortalSecurity.IsInRoles(FeedbackSettings.PermissionVote) = False) Then
                                    If (FeedbackSettings.PermissionVote.ToLower().Contains(glbRoleUnauthUserName.ToLower()) = False) Then
                                        hasVotePermission = False
                                    Else
                                        hasVotePermission = False
                                    End If
                                End If
                            Else
                                ' Setting not set.
                                If (Request.IsAuthenticated = False) Then
                                    hasVotePermission = False
                                End If
                            End If

                            If (hasVotePermission) Then

                                Dim alreadyVoted As Boolean = False

                                If (Request.IsAuthenticated) Then

                                    If (Request.IsAuthenticated) Then

                                        Dim objVoteController As New VoteController
                                        Dim objVote As VoteInfo = objVoteController.Get(objFeedback.FeedbackID, UserId)

                                        If (objVote IsNot Nothing) Then

                                            alreadyVoted = True

                                        End If

                                    Else

                                        Dim cookie As HttpCookie = Request.Cookies("Feedback-" & Me.ModuleId.ToString() & "-" & objFeedback.FeedbackID.ToString())

                                        If (cookie IsNot Nothing) Then

                                            alreadyVoted = True

                                        End If

                                    End If

                                End If

                                If (alreadyVoted = False) Then

                                    Dim objLiteral As New Literal
                                    objLiteral.Text = Localization.GetString("HowWouldYouVote", "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx") & "&nbsp;"
                                    objLiteral.EnableViewState = False
                                    controls.Add(objLiteral)

                                    Dim objImageThumbsUp As New ImageButton
                                    objImageThumbsUp.ImageUrl = "~/DesktopModules/FeedbackCenter/Images/ThumbsUp.gif"
                                    objImageThumbsUp.EnableViewState = False
                                    objImageThumbsUp.ImageAlign = ImageAlign.AbsMiddle
                                    objImageThumbsUp.CausesValidation = False
                                    objImageThumbsUp.Attributes("FeedbackID") = objFeedback.FeedbackID
                                    AddHandler objImageThumbsUp.Click, AddressOf btnVoteUp_Click
                                    controls.Add(objImageThumbsUp)

                                    Dim objLinkThumbsUp As New LinkButton
                                    objLinkThumbsUp.Text = Localization.GetString("ThumbsUp", "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx")
                                    objLinkThumbsUp.EnableViewState = False
                                    objLinkThumbsUp.CssClass = "CommandButton"
                                    objLinkThumbsUp.CausesValidation = False
                                    objLinkThumbsUp.Attributes("FeedbackID") = objFeedback.FeedbackID
                                    AddHandler objLinkThumbsUp.Click, AddressOf btnVoteUp_Click
                                    controls.Add(objLinkThumbsUp)

                                    Dim objLiteralSpacer As New Literal
                                    objLiteralSpacer.Text = "&nbsp;"
                                    objLiteralSpacer.EnableViewState = False
                                    controls.Add(objLiteralSpacer)

                                    Dim objImageThumbsDown As New ImageButton
                                    objImageThumbsDown.ImageUrl = "~/DesktopModules/FeedbackCenter/Images/ThumbsDown.gif"
                                    objImageThumbsDown.EnableViewState = False
                                    objImageThumbsDown.ImageAlign = ImageAlign.AbsMiddle
                                    objImageThumbsDown.CausesValidation = False
                                    objImageThumbsDown.Attributes("FeedbackID") = objFeedback.FeedbackID
                                    AddHandler objImageThumbsDown.Click, AddressOf btnVoteDown_Click
                                    controls.Add(objImageThumbsDown)

                                    Dim objLinkThumbsDown As New LinkButton
                                    objLinkThumbsDown.Text = Localization.GetString("ThumbsDown", "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx")
                                    objLinkThumbsDown.EnableViewState = False
                                    objLinkThumbsDown.CssClass = "CommandButton"
                                    objLinkThumbsDown.CausesValidation = False
                                    objLinkThumbsDown.Attributes("FeedbackID") = objFeedback.FeedbackID
                                    AddHandler objLinkThumbsDown.Click, AddressOf btnVoteDown_Click
                                    controls.Add(objLinkThumbsDown)

                                Else

                                    Dim objLiteral As New Literal
                                    objLiteral.Text = Localization.GetString("AlreadyVoted", "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx") & "&nbsp;"
                                    objLiteral.EnableViewState = False
                                    controls.Add(objLiteral)

                                End If

                            Else

                                Dim objLiteral As New Literal
                                objLiteral.Text = Localization.GetString("VoteNotAuthorized", "~/DesktopModules/FeedbackCenter/App_LocalResources/ViewFeedback.ascx.resx") & "&nbsp;"
                                objLiteral.EnableViewState = False
                                controls.Add(objLiteral)

                            End If

                        Case "VOTENEGATIVE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.VoteTotalNegative.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "VOTEPOSITIVE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.VoteTotal.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "VOTETOTAL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = (objFeedback.VoteTotalNegative + objFeedback.VoteTotal).ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else

                            If (layoutArray(iPtr + 1).ToUpper().StartsWith("CUSTOM:")) Then
                                Dim field As String = layoutArray(iPtr + 1).Substring(7, layoutArray(iPtr + 1).Length - 7).ToLower()

                                Dim customFieldID As Integer = Null.NullInteger
                                Dim objCustomFieldSelected As New CustomFieldInfo
                                Dim isLink As Boolean = False

                                Dim objCustomFieldController As New CustomFieldController
                                Dim objCustomFields As ArrayList = objCustomFieldController.List(objFeedback.ModuleID)

                                Dim maxLength As Integer = Null.NullInteger
                                If (field.IndexOf(":"c) <> -1) Then
                                    Try
                                        maxLength = Convert.ToInt32(field.Split(":"c)(1))
                                    Catch
                                        maxLength = Null.NullInteger
                                    End Try
                                    field = field.Split(":"c)(0)
                                End If
                                If (customFieldID = Null.NullInteger) Then
                                    For Each objCustomField As CustomFieldInfo In objCustomFields
                                        If (objCustomField.Name.ToLower() = field.ToLower()) Then
                                            customFieldID = objCustomField.CustomFieldID
                                            objCustomFieldSelected = objCustomField
                                        End If
                                    Next
                                End If

                                If (customFieldID <> Null.NullInteger) Then

                                    Dim i As Integer = 0
                                    If (objFeedback.CustomList.Contains(customFieldID)) Then
                                        Dim objLiteral As New Literal
                                        Dim fieldValue As String = GetFieldValue(objCustomFieldSelected, objFeedback)
                                        If (maxLength <> Null.NullInteger) Then
                                            If (fieldValue.Length > maxLength) Then
                                                fieldValue = fieldValue.Substring(0, maxLength)
                                            End If
                                        End If
                                        objLiteral.Text = fieldValue.TrimStart("#"c)
                                        objLiteral.EnableViewState = False
                                        controls.Add(objLiteral)
                                        i = i + 1
                                    End If
                                End If
                                Exit Select
                            End If

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub

        Protected Sub ProcessLayoutStatistics(ByRef controls As ControlCollection, ByVal layoutArray As String())

            Dim objStatisticController As New StatisticsController
            Dim objStatistics As StatisticsInfo = objStatisticController.Get(Me.ModuleId, 30)
            Dim objStatisticsAll As StatisticsInfo = objStatisticController.Get(Me.ModuleId, Null.NullInteger)

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "AVGTIMEOPEN"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objStatistics.AverageTime.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "AVGTIMEOPENALL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objStatisticsAll.AverageTime.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "FEEDBACKSUBMITTED"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objStatistics.FeedbackCreated.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "FEEDBACKSUBMITTEDALL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objStatisticsAll.FeedbackCreated.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "FEEDBACKRESOLVED"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objStatistics.FeedbackResolved.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "FEEDBACKRESOLVEDALL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objStatisticsAll.FeedbackResolved.ToString()
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case "TEMPLATEPATH"
                            Dim objLiteral As New Literal
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/FeedbackCenter/Templates/" & ModuleId.ToString() & "/")
                            objLiteral.EnableViewState = False
                            controls.Add(objLiteral)

                        Case Else

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub


#Region " Protected Methods "

        Protected Function GetEditUrl()

            Return NavigateURL(Me.TabId, "", "fbType=Edit")

        End Function

        Protected Function GetEditUrl(ByVal feedbackID As Integer)

            Return NavigateURL(Me.TabId, "", "fbType=Edit", "FeedbackID=" & feedbackID.ToString())

        End Function

        Protected Function GetMyFeedbackUrl()

            Return NavigateURL(Me.TabId, "", "fbType=MyFeedback")

        End Function

        Protected Function GetTrackingFeedbackUrl()

            Return NavigateURL(Me.TabId, "", "fbType=Tracking")

        End Function

        Protected Function GetFeedbackUrl(ByVal feedbackID As String)

            Return NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & feedbackID)

        End Function

        Protected Function GetFeedbackUrl(ByVal feedbackID As String, ByVal ret As String)

            Return NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & feedbackID, "Return=" & ret)

        End Function

        Protected Function GetSearchUrl()

            Return NavigateURL(Me.TabId, "", "fbType=Search")

        End Function

        Protected Function GetSearchUrl(ByVal productID As String)

            Return NavigateURL(Me.TabId, "", "fbType=Search", "ProductID=" & productID)

        End Function

        Protected Function GetSearchUrl(ByVal productID As String, ByVal status As String)

            Return NavigateURL(Me.TabId, "", "fbType=Search", "ProductID=" & productID, "Status=" & status)

        End Function

        Protected Function GetSearchUrlSort(ByVal sort As String, ByVal status As String)

            Return NavigateURL(Me.TabId, "", "fbType=Search", "Sort=" & sort, "Status=" & status)

        End Function

        Protected Function GetStatus(ByVal dataItem As Object) As String

            Dim objFeedback As FeedbackInfo = CType(dataItem, FeedbackInfo)

            If Not (objFeedback Is Nothing) Then
                If (objFeedback.IsClosed) Then
                    Return Localization.GetString("Closed", LocalResourceFile)
                Else
                    Return Localization.GetString("Open", LocalResourceFile)
                End If
            End If

            Return ""

        End Function

        Protected Function GetDateString(ByVal objItem As Object)

            Dim objFeedback As FeedbackInfo = CType(objItem, FeedbackInfo)

            If Not (objFeedback Is Nothing) Then
                If (objFeedback.LastCommentDate = Null.NullDate) Then
                    Return Localization.GetString("NA", Me.LocalResourceFile)
                Else
                    Return objFeedback.LastCommentDate.ToShortDateString()
                End If
            Else
                Return ""
            End If

        End Function

        Protected Function GetDisplayName(ByVal dataItem As Object) As String

            Dim objFeedback As FeedbackInfo = CType(dataItem, FeedbackInfo)

            If Not (objFeedback Is Nothing) Then
                If (objFeedback.DisplayName <> "") Then
                    Return objFeedback.DisplayName
                Else
                    Return objFeedback.AnonymousName
                End If
            End If

            Return ""

        End Function

        Protected Function GetRssUrl(ByVal productID As String) As String

            Return Me.ResolveUrl("RSS.aspx?T=" & Me.TabId.ToString() & "&M=" & Me.ModuleId.ToString() & "&p=" & productID)

        End Function

        Protected Function GetRssMyFeedbackUrl() As String

            Return Me.ResolveUrl("RSS.aspx?T=" & Me.TabId.ToString() & "&M=" & Me.ModuleId.ToString() & "&UserID=" & Me.UserId.ToString())

        End Function

        Protected Function GetRssTrackingFeedbackUrl() As String

            Return Me.ResolveUrl("RSS.aspx?T=" & Me.TabId.ToString() & "&M=" & Me.ModuleId.ToString() & "&TrackingID=" & Me.UserId.ToString())

        End Function

        Protected Function GetRssWhatsNewUrl() As String

            Return Me.ResolveUrl("RSS.aspx?T=" & Me.TabId.ToString() & "&M=" & Me.ModuleId.ToString())

        End Function

        Protected Function GetRssHighestRatedUrl() As String

            Return Me.ResolveUrl("RSS.aspx?T=" & Me.TabId.ToString() & "&M=" & Me.ModuleId.ToString() & "&Sort=Popularity")

        End Function

        Protected Function GetRssRecentlyImplementedUrl() As String

            Return Me.ResolveUrl("RSS.aspx?T=" & Me.TabId.ToString() & "&M=" & Me.ModuleId.ToString() & "&Sort=ClosedDate&S=Closed")

        End Function

        Protected Function GetRssRecentlyCommentedUrl() As String

            Return Me.ResolveUrl("RSS.aspx?T=" & Me.TabId.ToString() & "&M=" & Me.ModuleId.ToString() & "&Sort=LastCommentDate")

        End Function

#End Region

#End Region

#Region " Event Handlers "

        Private Sub btnVoteUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

            Dim feedbackID As Integer = Null.NullInteger

            If (TypeOf sender Is WebControl) Then
                feedbackID = Convert.ToInt32(CType(sender, WebControl).Attributes("FeedbackID").ToString())
            End If

            Dim objVote As New VoteInfo

            If (Request.IsAuthenticated) Then
                objVote.UserID = Me.UserId
            Else
                objVote.UserID = Null.NullInteger
            End If

            objVote.FeedbackID = feedbackID
            objVote.CreateDate = DateTime.Now
            objVote.IsPositive = True

            Dim objVoteController As New VoteController
            objVoteController.Add(objVote)

            Dim objFeedbackController As New FeedbackController
            Dim objFeedback As FeedbackInfo = objFeedbackController.Get(feedbackID)
            If (objVote.IsPositive) Then
                objFeedback.VoteTotal = objFeedback.VoteTotal + 1
            Else
                objFeedback.VoteTotalNegative = objFeedback.VoteTotalNegative + 1
            End If
            objFeedbackController.Update(objFeedback)

            If (Request.IsAuthenticated = False) Then
                Dim cookie As HttpCookie = New HttpCookie("Feedback-" & Me.ModuleId.ToString() & "-" & feedbackID.ToString())
                cookie.Value = feedbackID.ToString()
                cookie.Expires = DateTime.Now.AddYears(1)
                Context.Response.Cookies.Add(cookie)
            End If

            If (FeedbackSettings.ActiveSocialVoteKey <> "" And Request.IsAuthenticated = True) Then
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

                            If (objVote.IsPositive) Then
                                ac.AddProfileItem(New Guid(FeedbackSettings.ActiveSocialVoteKey), objVote.UserID, link, objFeedback.Title, "Yes", "Yes", 1, "", True)
                            Else
                                ac.AddProfileItem(New Guid(FeedbackSettings.ActiveSocialVoteKey), objVote.UserID, link, objFeedback.Title, "No", "No", 1, "", True)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If
            End If

            Response.Redirect(NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & feedbackID.ToString()), True)

        End Sub

        Private Sub btnVoteDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

            Dim feedbackID As Integer = Null.NullInteger

            If (TypeOf sender Is WebControl) Then
                feedbackID = Convert.ToInt32(CType(sender, WebControl).Attributes("FeedbackID").ToString())
            End If

            Dim objVote As New VoteInfo

            If (Request.IsAuthenticated) Then
                objVote.UserID = Me.UserId
            Else
                objVote.UserID = Null.NullInteger
            End If

            objVote.FeedbackID = feedbackID
            objVote.CreateDate = DateTime.Now
            objVote.IsPositive = False

            Dim objVoteController As New VoteController
            objVoteController.Add(objVote)

            Dim objFeedbackController As New FeedbackController
            Dim objFeedback As FeedbackInfo = objFeedbackController.Get(feedbackID)
            If (objVote.IsPositive) Then
                objFeedback.VoteTotal = objFeedback.VoteTotal + 1
            Else
                objFeedback.VoteTotalNegative = objFeedback.VoteTotalNegative + 1
            End If
            objFeedbackController.Update(objFeedback)

            If (Request.IsAuthenticated = False) Then
                Dim cookie As HttpCookie = New HttpCookie("Feedback-" & Me.ModuleId.ToString() & "-" & feedbackID.ToString())
                cookie.Value = feedbackID.ToString()
                cookie.Expires = DateTime.Now.AddYears(1)
                Context.Response.Cookies.Add(cookie)
            End If

            If (FeedbackSettings.ActiveSocialVoteKey <> "" And Request.IsAuthenticated = True) Then
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

                            If (objVote.IsPositive) Then
                                ac.AddProfileItem(New Guid(FeedbackSettings.ActiveSocialVoteKey), objVote.UserID, link, objFeedback.Title, "Yes", "Yes", 1, "", True)
                            Else
                                ac.AddProfileItem(New Guid(FeedbackSettings.ActiveSocialVoteKey), objVote.UserID, link, objFeedback.Title, "No", "No", 1, "", True)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If
            End If

            Response.Redirect(NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & feedbackID.ToString()), True)

        End Sub

        Private Sub btnDeleteComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

            Dim commentID As Integer = Null.NullInteger

            If (TypeOf sender Is WebControl) Then
                commentID = Convert.ToInt32(CType(sender, WebControl).Attributes("CommentID").ToString())
            End If

            Dim objCommentController As New CommentController()
            Dim objComment As CommentInfo = objCommentController.Get(commentID)

            If (objComment IsNot Nothing) Then

                objCommentController.Delete(objComment.CommentID)
                Response.Redirect(NavigateURL(Me.TabId, "", "fbType=View", "FeedbackID=" & objComment.FeedbackID.ToString()), True)

            End If

        End Sub

#End Region

    End Class

End Namespace
