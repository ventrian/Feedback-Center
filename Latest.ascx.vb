Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules

Imports Ventrian.FeedbackCenter.Entities

Namespace Ventrian.FeedbackCenter

    Partial Public Class Latest
        Inherits PortalModuleBase

#Region " Private Methods "

        Private Sub BindFeedback()

            Dim maxCount As Integer = Entities.Constants.FEEDBACK_LATEST_MAX_COUNT_DEFAULT
            If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_MAX_COUNT)) Then
                maxCount = Convert.ToInt32(Settings(Entities.Constants.FEEDBACK_LATEST_MAX_COUNT).ToString())
            End If

            Dim userFilter As Boolean = False
            Dim userID As Integer = Null.NullInteger
            If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_USER_ID_FILTER)) Then
                If (Convert.ToBoolean(Settings(Entities.Constants.FEEDBACK_LATEST_USER_ID_FILTER).ToString())) Then
                    If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_USER_ID_PARAM)) Then
                        If (IsNumeric(Request(Settings(Entities.Constants.FEEDBACK_LATEST_USER_ID_PARAM).ToString()))) Then
                            userID = Convert.ToInt32(Request(Settings(Entities.Constants.FEEDBACK_LATEST_USER_ID_PARAM).ToString()))
                        End If
                    End If
                End If
            End If

            Dim headerTemplate As String = Entities.Constants.FEEDBACK_LATEST_HTML_HEADER_DEFAULT
            If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_HEADER)) Then
                headerTemplate = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_HEADER).ToString()
            End If

            Dim bodyTemplate As String = Entities.Constants.FEEDBACK_LATEST_HTML_BODY_DEFAULT
            If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_BODY)) Then
                bodyTemplate = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_BODY).ToString()
            End If

            Dim footerTemplate As String = Entities.Constants.FEEDBACK_LATEST_HTML_FOOTER_DEFAULT
            If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_FOOTER)) Then
                footerTemplate = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_FOOTER).ToString()
            End If

            Dim noFeedbackTemplate As String = Entities.Constants.FEEDBACK_LATEST_HTML_NO_FEEDBACK_DEFAULT
            If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_HTML_NO_FEEDBACK)) Then
                noFeedbackTemplate = Settings(Entities.Constants.FEEDBACK_LATEST_HTML_NO_FEEDBACK).ToString()
            End If

            Dim moduleID As Integer = Convert.ToInt32(Settings(Entities.Constants.FEEDBACK_LATEST_MODULE_ID).ToString())

            Dim objFeedbackController As New FeedbackController()
            Dim objFeedbackList As ArrayList = objFeedbackController.List(moduleID, Null.NullInteger, False, Null.NullString, SortType.CreateDate, SortDirection.Descending, maxCount, userID, Null.NullInteger, True, Nothing)

            If (objFeedbackList.Count = 0) Then
                Dim objLiteral As New Literal
                objLiteral.Text = noFeedbackTemplate
                phFeedback.Controls.Add(objLiteral)
            Else

                Dim objHeader As New Literal
                objHeader.Text = headerTemplate
                phFeedback.Controls.Add(objHeader)

                For Each objFeedback As FeedbackInfo In objFeedbackList

                    Dim delimStr As String = "[]"
                    Dim delimiter As Char() = delimStr.ToCharArray()

                    ProcessFeedback(objFeedback, phFeedback.Controls, bodyTemplate.Split(delimiter))

                Next

                Dim objFooter As New Literal
                objFooter.Text = footerTemplate
                phFeedback.Controls.Add(objFooter)

            End If

        End Sub

        Private Sub ProcessFeedback(ByVal objFeedback As FeedbackInfo, ByRef objPlaceHolder As ControlCollection, ByVal templateArray As String())

            For iPtr As Integer = 0 To templateArray.Length - 1 Step 2

                objPlaceHolder.Add(New LiteralControl(templateArray(iPtr).ToString()))

                If iPtr < templateArray.Length - 1 Then
                    Select Case templateArray(iPtr + 1)
                        Case "CREATEDATE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.CreateDate.ToShortDateString()
                            objPlaceHolder.Add(objLiteral)
                        Case "LINK"
                            Dim objLiteral As New Literal
                            objLiteral.Text = NavigateURL(Convert.ToInt32(Settings(Entities.Constants.FEEDBACK_LATEST_TAB_ID).ToString()), "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                            objPlaceHolder.Add(objLiteral)
                        Case "PRODUCT"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.ProductName
                            objPlaceHolder.Add(objLiteral)
                        Case "TITLE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.Title
                            objPlaceHolder.Add(objLiteral)
                        Case "VOTETOTAL"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.VoteTotal.ToString()
                            objPlaceHolder.Add(objLiteral)
                        Case "VOTETOTALNEGATIVE"
                            Dim objLiteral As New Literal
                            objLiteral.Text = objFeedback.VoteTotalNegative.ToString()
                            objPlaceHolder.Add(objLiteral)
                    End Select
                End If

            Next

        End Sub

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (IsPostBack = False) Then

                If (Settings.Contains(Entities.Constants.FEEDBACK_LATEST_MODULE_ID) = False) Then
                    lblNotConfigured.Visible = True
                    Return
                End If

            End If

            BindFeedback()

        End Sub

#End Region

    End Class

End Namespace
