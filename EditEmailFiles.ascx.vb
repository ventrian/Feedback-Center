Imports Ventrian.FeedbackCenter.Entities.Layout

Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities.Emails

Namespace Ventrian.FeedbackCenter

    Partial Public Class EditEmailFiles
        Inherits FeedbackCenterBase

#Region " Private Methods "

        Private Sub BindEmailTypes()

            For Each value As Integer In System.Enum.GetValues(GetType(EmailType))
                Dim li As New ListItem
                li.Value = System.Enum.GetName(GetType(EmailType), value)
                li.Text = System.Enum.GetName(GetType(EmailType), value)
                drpTemplate.Items.Add(li)
            Next

        End Sub

        Private Sub BindTemplate()

            Dim objEmailController As New EmailController()
            Dim objLayout As EmailInfo = objEmailController.GetLayout(System.Enum.Parse(GetType(EmailType), drpTemplate.SelectedValue), ModuleId, Settings)
            Dim layoutType As EmailType = System.Enum.Parse(GetType(EmailType), drpTemplate.SelectedValue)

            txtTemplate.Text = objLayout.Template

            rptTokens.DataSource = System.Enum.GetValues(GetType(EmailTokensType))
            rptTokens.DataBind()
            rptTokens.Visible = True

        End Sub

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (IsPostBack = False) Then

                BindEmailTypes()
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

            Dim objEmailController As New EmailController()
            objEmailController.UpdateLayout(System.Enum.Parse(GetType(EmailType), drpTemplate.SelectedValue), ModuleId, txtTemplate.Text)
            lblTemplateUpdated.Visible = True

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click

            Response.Redirect(NavigateURL(TabId), True)

        End Sub

#End Region

    End Class

End Namespace