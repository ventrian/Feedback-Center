Imports Ventrian.FeedbackCenter.Entities.Layout

Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Namespace Ventrian.FeedbackCenter

    Partial Public Class EditLayoutFiles
        Inherits FeedbackCenterBase

#Region " Private Methods "

        Private Sub BindLayoutTypes()

            For Each value As Integer In System.Enum.GetValues(GetType(LayoutType))
                Dim li As New ListItem
                li.Value = System.Enum.GetName(GetType(LayoutType), value)
                li.Text = System.Enum.GetName(GetType(LayoutType), value)
                drpTemplate.Items.Add(li)
            Next

        End Sub

        Private Sub BindTemplate()

            Dim objLayoutController As New LayoutController()
            Dim objLayout As LayoutInfo = objLayoutController.GetLayout(System.Enum.Parse(GetType(LayoutType), drpTemplate.SelectedValue), ModuleId, Settings)
            Dim layoutType As LayoutType = System.Enum.Parse(GetType(LayoutType), drpTemplate.SelectedValue)

            txtTemplate.Text = objLayout.Template


            Select Case layoutType

                Case Entities.Layout.LayoutType.Edit_Layout_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenEditType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Entities.Layout.LayoutType.Landing_FeedbackTracking_Item_Html, Entities.Layout.LayoutType.Landing_HighestRated_Item_Html, Entities.Layout.LayoutType.Landing_MyFeedback_Item_Html, Entities.Layout.LayoutType.Landing_RecentlyCommented_Item_Html, Entities.Layout.LayoutType.Landing_RecentlyImplemented_Item_Html, Entities.Layout.LayoutType.Landing_WhatsNew_Item_Html, Entities.Layout.LayoutType.Search_Listing_Item_Html, Entities.Layout.LayoutType.View_Item_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenFeedbackType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Entities.Layout.LayoutType.Landing_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenLandingType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Entities.Layout.LayoutType.Landing_Make_Suggestion_Html, Entities.Layout.LayoutType.Search_Make_Suggestion_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenSuggestionType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Entities.Layout.LayoutType.Landing_Statistics_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenStatisticsType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Entities.Layout.LayoutType.Landing_ProductList_Item_Html, Entities.Layout.LayoutType.Search_ChildProductList_Item_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenProductType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Entities.Layout.LayoutType.View_Comment_Item_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenCommentType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Entities.Layout.LayoutType.View_Comment_Form_Html
                    rptTokens.DataSource = System.Enum.GetValues(GetType(TokenCommentFormType))
                    rptTokens.DataBind()
                    rptTokens.Visible = True

                Case Else
                    rptTokens.Visible = False

            End Select

        End Sub

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (IsPostBack = False) Then

                BindLayoutTypes()
                BindTemplate()

            End If

        End Sub

        Private Sub drpTemplate_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles drpTemplate.SelectedIndexChanged

            Try

                BindTemplate()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            Dim objLayoutController As New LayoutController()
            objLayoutController.UpdateLayout(System.Enum.Parse(GetType(LayoutType), drpTemplate.SelectedValue), ModuleId, txtTemplate.Text)
            lblTemplateUpdated.Visible = True

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click

            Response.Redirect(NavigateUrl(TabId), True)

        End Sub

#End Region

    End Class

End Namespace