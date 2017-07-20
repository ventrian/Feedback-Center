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
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities
Imports Ventrian.FeedbackCenter.Entities.Layout

Namespace Ventrian.FeedbackCenter

    Public MustInherit Class MyFeedback

        Inherits FeedbackCenterBase

#Region " Controls "

        Protected WithEvents rptBreadCrumbs As System.Web.UI.WebControls.Repeater
        Protected WithEvents lblStatus As System.Web.UI.WebControls.Label
        Protected WithEvents lstStatus As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents lblSubmitComplete As System.Web.UI.WebControls.Label
        Protected WithEvents phControls As System.Web.UI.WebControls.PlaceHolder

#End Region

#Region " Private Methods "

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim crumbAllAlbums As New CrumbInfo
            crumbAllAlbums.Caption = Localization.GetString("AllFeedback", LocalResourceFile)
            crumbAllAlbums.Url = NavigateURL()
            crumbs.Add(crumbAllAlbums)

            Dim currentCrumb As New CrumbInfo
            currentCrumb.Caption = Localization.GetString("MyFeedback", LocalResourceFile)
            currentCrumb.Url = Request.Url.ToString()
            crumbs.Add(currentCrumb)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

        Private Sub BindFeedback()

            Dim objLayoutController As New LayoutController()
            Dim objFeedbackController As New FeedbackController
            Dim objFeedbackList As ArrayList = objFeedbackController.List(Me.ModuleId, Null.NullInteger, (lstStatus.SelectedValue = "Closed"), Null.NullString, SortType.CreateDate, SortDirection.Descending, Null.NullInteger, Me.UserId, Null.NullInteger, True, Nothing)

            If (objFeedbackList.Count = 0) Then

                Dim noResults As String = Localization.GetString("NoResults", Me.LocalResourceFile)
                Dim objMessage As LayoutInfo = objLayoutController.GetLayout(LayoutType.Message_Html, ModuleId, Settings)
                ProcessMessage(phControls.Controls, objMessage.Tokens, noResults)

            Else

                Dim objPagedDataSource As New PagedDataSource

                objPagedDataSource.AllowPaging = True
                objPagedDataSource.DataSource = objFeedbackList
                objPagedDataSource.PageSize = 10

                If (Request("Page") <> "") Then
                    Try
                        objPagedDataSource.CurrentPageIndex = Convert.ToInt32(Request("Page"))
                    Catch ex As Exception
                        objPagedDataSource.CurrentPageIndex = 1
                    End Try
                End If

                Dim startSet As Integer = objPagedDataSource.CurrentPageIndex * objPagedDataSource.PageSize + 1
                Dim endSet As Integer = (objPagedDataSource.CurrentPageIndex + 1) * objPagedDataSource.PageSize

                If (endSet > objFeedbackList.Count) Then
                    endSet = objFeedbackList.Count
                End If

                Dim results As String = String.Format(Localization.GetString("Paging", LocalResourceFile), startSet.ToString(), endSet.ToString(), objFeedbackList.Count.ToString())

                Dim strPages As String
                If objPagedDataSource.PageCount = 1 Then
                    strPages = Localization.GetString("Page", LocalResourceFile)
                Else
                    strPages = Localization.GetString("Pages", LocalResourceFile)
                End If
                For i As Integer = 1 To objPagedDataSource.PageCount
                    If i = (objPagedDataSource.CurrentPageIndex + 1) Then
                        strPages = strPages + i.ToString() & " "
                    Else
                        strPages = strPages + "<a href=" & NavigateURL(Me.TabId, "", "fbType=MyFeedback", "Status=" & lstStatus.SelectedValue, "Page=" & (i - 1).ToString()) & " class=""Normal"">" & i.ToString() & "</a> "
                    End If
                Next

                Dim objListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Search_Listing_Header_Html, ModuleId, Settings)
                Dim objListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Search_Listing_Item_Html, ModuleId, Settings)
                Dim objListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Search_Listing_Footer_Html, ModuleId, Settings)

                ProcessPaging(phControls.Controls, objListHeader.Tokens, strPages, results)

                For Each objFeedback As FeedbackInfo In objPagedDataSource
                    ProcessFeedbackItem(phControls.Controls, objListItem.Tokens, objFeedback)
                Next

                ProcessPaging(phControls.Controls, objListFooter.Tokens, startSet, endSet)

            End If

        End Sub

        Private Sub SetCriteria()

            If (Request("Status") <> "") Then
                If Not (lstStatus.Items.FindByValue(Request("Status")) Is Nothing) Then
                    lstStatus.SelectedValue = Request("Status")
                End If
            End If

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            BindCrumbs()
            If (IsPostBack = False) Then
                SetCriteria()
                BindFeedback()
            End If

        End Sub

        Private Sub lstStatus_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstStatus.SelectedIndexChanged

            Response.Redirect(NavigateURL(Me.TabId, "", "fbType=MyFeedback", "Status=" & lstStatus.SelectedValue), True)

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