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

    Public MustInherit Class NotAuthorized

        Inherits FeedbackCenterBase

#Region " Controls "

        Protected WithEvents lblNotAuthorizedMessage As System.Web.UI.WebControls.Label
        Protected WithEvents lblNotAuthorized As System.Web.UI.WebControls.Label
        Protected WithEvents rptBreadCrumbs As System.Web.UI.WebControls.Repeater
        Protected WithEvents lblSubmitComplete As System.Web.UI.WebControls.Label

#End Region

#Region " Private Methods "

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim crumbAllAlbums As New CrumbInfo
            crumbAllAlbums.Caption = Localization.GetString("AllFeedback", LocalResourceFile)
            crumbAllAlbums.Url = NavigateURL()
            crumbs.Add(crumbAllAlbums)

            Dim currentCrumb As New CrumbInfo
            currentCrumb.Caption = Localization.GetString("NotAuthorized", LocalResourceFile)
            currentCrumb.Url = Request.Url.ToString()
            crumbs.Add(currentCrumb)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            BindCrumbs()

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