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

Namespace Ventrian.FeedbackCenter

    Public MustInherit Class EditFeedbackComplete

        Inherits FeedbackCenterBase

#Region " Controls "

        Protected WithEvents lblSubmitComplete As System.Web.UI.WebControls.Label
        Protected WithEvents lblSubmitMessage As System.Web.UI.WebControls.Label
        Protected WithEvents rptBreadCrumbs As System.Web.UI.WebControls.Repeater

#End Region

#Region " Private Members "

        Private _feedbackID As Integer = Null.NullInteger

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If Not (Request("FeedbackID") Is Nothing) Then
                _feedbackID = Convert.ToInt32(Request("FeedbackID"))
            End If

        End Sub

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim crumbAllAlbums As New CrumbInfo
            crumbAllAlbums.Caption = Localization.GetString("AllFeedback", LocalResourceFile)
            crumbAllAlbums.Url = NavigateURL()
            crumbs.Add(crumbAllAlbums)

            Dim addtCrumb As New CrumbInfo
            addtCrumb.Caption = Localization.GetString("AddNewFeedback", LocalResourceFile)
            addtCrumb.Url = NavigateURL(Me.TabId, "", "fbType=Edit")
            crumbs.Add(addtCrumb)

            Dim currentCrumb As New CrumbInfo
            currentCrumb.Caption = Localization.GetString("AddNewFeedbackComplete", LocalResourceFile)
            currentCrumb.Url = Request.Url.ToString()
            crumbs.Add(currentCrumb)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ReadQueryString()
            BindCrumbs()

            If (Request("approval") <> "") Then
                lblSubmitMessage.Text = String.Format(Localization.GetString("SubmitMessageApproval", LocalResourceFile), NavigateURL(Me.TabId, "", "fbType=Edit"), NavigateURL(Me.TabId, "", "fbType=Search"))
            Else
                lblSubmitMessage.Text = String.Format(Localization.GetString("SubmitMessage", LocalResourceFile), NavigateURL(Me.TabId, "", "fbType=Edit"), NavigateURL(Me.TabId, "", "fbType=Search"))
            End If
            
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