Imports System.Web.UI.WebControls

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization

Namespace Ventrian.FeedbackCenter

    Public MustInherit Class CDefault

        Inherits PortalModuleBase
        Implements IActionable

#Region " Controls "

        Protected WithEvents phControls As System.Web.UI.WebControls.PlaceHolder

#End Region

#Region " Private Members "

        Private _controlToLoad As String

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If Not (Request("fbType") Is Nothing) Then

                ' Load the appropriate Control
                '
                Select Case Request("fbType").ToLower()

                    Case "approvecomments"
                        _controlToLoad = "ApproveComments.ascx"

                    Case "approvefeedback"
                        _controlToLoad = "ApproveFeedback.ascx"

                    Case "edit"
                        _controlToLoad = "EditFeedback.ascx"

                    Case "editcomplete"
                        _controlToLoad = "EditFeedbackComplete.ascx"

                    Case "myfeedback"
                        _controlToLoad = "MyFeedback.ascx"

                    Case "notauthorized"
                        _controlToLoad = "NotAuthorized.ascx"

                    Case "search"
                        _controlToLoad = "Search.ascx"

                    Case "tracking"
                        _controlToLoad = "Tracking.ascx"

                    Case "view"
                        _controlToLoad = "ViewFeedback.ascx"

                    Case Else
                        _controlToLoad = "LandingPage.ascx"

                End Select

            Else

                _controlToLoad = "LandingPage.ascx"

            End If

        End Sub

        Private Sub LoadControlType()

            Dim objPortalModuleBase As PortalModuleBase = CType(Me.LoadControl(_controlToLoad), PortalModuleBase)

            If Not (objPortalModuleBase Is Nothing) Then

                objPortalModuleBase.ModuleConfiguration = Me.ModuleConfiguration
                objPortalModuleBase.ID = System.IO.Path.GetFileNameWithoutExtension(_controlToLoad)
                phControls.Controls.Add(objPortalModuleBase)

            End If

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        End Sub

        Private Sub Page_Initialize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            ReadQueryString()
            LoadControlType()

        End Sub

#End Region

#Region " Optional Interfaces "

        Public ReadOnly Property ModuleActions() As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Implements IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localization.GetString("EditCustomFields.Text", LocalResourceFile), ModuleActionType.ContentOptions, "", "", EditUrl("EditCustomFields"), False, SecurityAccessLevel.Edit, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("EditEmailFiles.Text", LocalResourceFile), ModuleActionType.ContentOptions, "", "", EditUrl("EditEmailFiles"), False, SecurityAccessLevel.Edit, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("EditLayoutFiles.Text", LocalResourceFile), ModuleActionType.ContentOptions, "", "", EditUrl("EditLayoutFiles"), False, SecurityAccessLevel.Edit, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("EditProducts.Text", LocalResourceFile), ModuleActionType.ContentOptions, "", "", EditUrl("EditProducts"), False, SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property

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