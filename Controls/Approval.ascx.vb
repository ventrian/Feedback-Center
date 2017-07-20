Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities

Namespace Ventrian.FeedbackCenter.Controls

    Partial Public Class Approval
        Inherits System.Web.UI.UserControl

#Region " Private Properties "

        Private ReadOnly Property FeedbackControl() As FeedbackCenterBase
            Get
                Return CType(Parent, FeedbackCenterBase)
            End Get
        End Property
#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (FeedbackControl.IsEditable = False And PortalSecurity.IsInRoles(FeedbackControl.FeedbackSettings.PermissionApprove) = False) Then
                Me.Visible = False
                Return
            End If

            Dim objFeedbackController As New FeedbackController
            Dim objFeedbackItems As ArrayList = objFeedbackController.List(FeedbackControl.ModuleId, Null.NullInteger, False, Null.NullString, SortType.CreateDate, SortDirection.Descending, 1000, Null.NullInteger, Null.NullInteger, False, Nothing)

            lnkApproveFeedback.Text = String.Format(Localization.GetString("ApproveFeedback", "~/DesktopModules/FeedbackCenter/App_LocalResources/Control-Approval.ascx.resx"), objFeedbackItems.Count.ToString())
            lnkApproveFeedback.NavigateUrl = NavigateURL(FeedbackControl.TabId, "", "fbType=ApproveFeedback")

            Dim objCommentController As New CommentController
            Dim objComments As ArrayList = objCommentController.List(FeedbackControl.ModuleId, Null.NullInteger, False)

            lnkApproveComments.Text = String.Format(Localization.GetString("ApproveComments", "~/DesktopModules/FeedbackCenter/App_LocalResources/Control-Approval.ascx.resx"), objComments.Count.ToString())
            lnkApproveComments.NavigateUrl = NavigateURL(FeedbackControl.TabId, "", "fbType=ApproveComments")

        End Sub

#End Region

    End Class

End Namespace

