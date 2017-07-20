Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities

Namespace Ventrian.FeedbackCenter

    Partial Public Class EditProducts
        Inherits FeedbackCenterBase

#Region " Private Methods "

        Private Sub BindProducts()

            Dim objProductController As New ProductController

            lstChildCategories.DataSource = objProductController.List(ModuleId, False, Convert.ToInt32(drpParentCategory.SelectedValue))
            lstChildCategories.DataBind()

            If (lstChildCategories.Items.Count = 0) Then
                pnlSortOrder.Visible = False
                lblNoCategories.Visible = True
            Else
                pnlSortOrder.Visible = True
                lblNoCategories.Visible = False
            End If

        End Sub

        Private Sub BindParentCategories()

            Dim objProductController As New ProductController

            drpParentCategory.DataSource = objProductController.ListAll(ModuleId)
            drpParentCategory.DataBind()

            drpParentCategory.Items.Insert(0, New ListItem(Localization.GetString("NoParentProduct", Me.LocalResourceFile), "-1"))

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                If (IsPostBack = False) Then
                    BindParentCategories()
                    BindProducts()
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdAddNewCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddNewProduct.Click

            Try

                If (drpParentCategory.SelectedValue <> "-1") Then
                    Response.Redirect(EditUrl("ParentID", drpParentCategory.SelectedValue, "EditProduct"), True)
                Else
                    Response.Redirect(EditUrl("EditProduct"), True)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Protected Sub drpParentCategory_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles drpParentCategory.SelectedIndexChanged

            Try

                BindProducts()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Protected Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdEdit.Click

            If (lstChildCategories.Items.Count > 0) Then
                If Not (lstChildCategories.SelectedItem Is Nothing) Then
                    Response.Redirect(EditUrl("ProductID", lstChildCategories.SelectedValue, "EditProduct"), True)
                End If
            End If

        End Sub

        Protected Sub cmdView_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdView.Click

            If (lstChildCategories.Items.Count > 0) Then
                If Not (lstChildCategories.SelectedItem Is Nothing) Then
                    Response.Redirect(NavigateURl(), True)
                    ' (Convert.ToInt32(lstChildCategories.SelectedValue))
                End If
            End If

        End Sub

#End Region

    End Class

End Namespace