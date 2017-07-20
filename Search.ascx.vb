'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System.Web.UI.WebControls

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities
Imports Ventrian.FeedbackCenter.Entities.Layout

Namespace Ventrian.FeedbackCenter

    Public Class Search
        Inherits FeedbackCenterBase
        Implements IActionable

#Region " Controls "

        Protected WithEvents lblProduct As System.Web.UI.WebControls.Label
        Protected WithEvents drpProducts As System.Web.UI.WebControls.DropDownList
        Protected WithEvents lblKeyword As System.Web.UI.WebControls.Label
        Protected WithEvents txtKeyword As System.Web.UI.WebControls.TextBox
        Protected WithEvents btnSearch As System.Web.UI.WebControls.Button
        Protected WithEvents rptFeedback As System.Web.UI.WebControls.Repeater
        Protected WithEvents lblPaging As System.Web.UI.WebControls.Label
        Protected WithEvents lblCurrentPage As System.Web.UI.WebControls.Label
        Protected WithEvents lblNoResults As System.Web.UI.WebControls.Label
        Protected WithEvents pnlResults As System.Web.UI.WebControls.Panel
        Protected WithEvents rptBreadCrumbs As System.Web.UI.WebControls.Repeater
        Protected WithEvents plSortBy As System.Web.UI.WebControls.Label
        Protected WithEvents drpSortBy As System.Web.UI.WebControls.DropDownList
        Protected WithEvents drpSortDirection As System.Web.UI.WebControls.DropDownList
        Protected WithEvents lblMakeSuggestion As System.Web.UI.WebControls.Label
        Protected WithEvents lblMakeSuggestionDetail As System.Web.UI.WebControls.Label
        Protected WithEvents lblMakeSuggestionClick As System.Web.UI.WebControls.Label
        Protected WithEvents lstStatus As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents phControls As System.Web.UI.WebControls.PlaceHolder

#End Region

#Region " Private Methods "

        Private Sub BindProducts()

            Dim objProductController As New ProductController

            Dim objProducts As ArrayList = objProductController.ListAll(Me.ModuleId)
            Dim objProductsToSelect As New ArrayList

            For Each objProduct As ProductInfo In objProducts
                If (objProduct.InheritSecurity) Then
                    objProductsToSelect.Add(objProduct)
                Else
                    If (Settings.Contains(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING)) Then
                        If (PortalSecurity.IsInRoles(Settings(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING).ToString())) Then
                            objProductsToSelect.Add(objProduct)
                        End If
                    End If
                End If
            Next

            drpProducts.DataSource = objProductsToSelect
            drpProducts.DataBind()

            drpProducts.Items.Insert(0, New ListItem(Localization.GetString("AllProducts", LocalResourceFile), Null.NullInteger))

        End Sub

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim crumbAllAlbums As New CrumbInfo
            crumbAllAlbums.Caption = Localization.GetString("AllFeedback", LocalResourceFile)
            crumbAllAlbums.Url = NavigateURL()
            crumbs.Add(crumbAllAlbums)

            Dim currentCrumb As New CrumbInfo
            currentCrumb.Caption = Localization.GetString("Search", LocalResourceFile)
            currentCrumb.Url = Request.Url.ToString()
            crumbs.Add(currentCrumb)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

        Private Sub BindSortBy()

            For Each value As Integer In System.Enum.GetValues(GetType(SortType))
                Dim li As New ListItem
                li.Value = System.Enum.GetName(GetType(SortType), value)
                li.Text = Localization.GetString("SortBy", Me.LocalResourceFile) & " " & Localization.GetString(System.Enum.GetName(GetType(SortType), value), Me.LocalResourceFile)
                drpSortBy.Items.Add(li)
            Next

            For Each value As Integer In System.Enum.GetValues(GetType(SortDirection))
                Dim li As New ListItem
                li.Value = System.Enum.GetName(GetType(SortDirection), value)
                li.Text = Localization.GetString(System.Enum.GetName(GetType(SortDirection), value), Me.LocalResourceFile)
                drpSortDirection.Items.Add(li)
            Next

        End Sub

        Private Sub BindCriteria()

            If Not (Request("ProductID") Is Nothing) Then
                If Not (drpProducts.Items.FindByValue(Request("ProductID")) Is Nothing) Then
                    drpProducts.SelectedValue = Request("ProductID")
                End If
            End If

            If Not (Request("Status") Is Nothing) Then
                If Not (lstStatus.Items.FindByValue(Request("Status").ToLower()) Is Nothing) Then
                    lstStatus.SelectedValue = Request("Status").ToLower()
                End If
            End If

            If Not (Request("Keyword") Is Nothing) Then
                txtKeyword.Text = Server.UrlDecode(Request("Keyword")).Trim()
            End If

            If Not (Request("Sort") Is Nothing) Then
                If Not (drpSortBy.Items.FindByValue(Request("Sort")) Is Nothing) Then
                    drpSortBy.SelectedValue = Request("Sort")
                End If
            End If

            drpSortDirection.SelectedIndex = 1
            If Not (Request("SortDir") Is Nothing) Then
                If Not (drpSortDirection.Items.FindByValue(Request("SortDir")) Is Nothing) Then
                    drpSortDirection.SelectedValue = Request("SortDir")
                End If
            End If

        End Sub

        Private Sub BindFeedback()

            Dim objProductController As New ProductController
            Dim products As New List(Of Integer)
            Dim objProductsToSecure As ArrayList = objProductController.ListAll(ModuleId)

            For Each objProduct As ProductInfo In objProductsToSecure
                If (objProduct.InheritSecurity) Then
                    products.Add(objProduct.ProductID)
                Else
                    If (Settings.Contains(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING)) Then
                        If (PortalSecurity.IsInRoles(Settings(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING).ToString())) Then
                            products.Add(objProduct.ProductID)
                        End If
                    End If
                End If
            Next

            If (products.Count = objProductsToSecure.Count) Then
                products = Nothing
            Else
                If (products.Count = 0) Then
                    products.Add(-1)
                End If
            End If

            Dim objSortType As SortType = System.Enum.Parse(GetType(SortType), drpSortBy.SelectedValue, True)
            Dim objSortDirection As SortDirection = System.Enum.Parse(GetType(SortDirection), drpSortDirection.SelectedValue, True)

            Dim objFeedbackController As New FeedbackController
            Dim objFeedbackList As ArrayList = objFeedbackController.List(Me.ModuleId, Convert.ToInt32(drpProducts.SelectedValue), (lstStatus.SelectedValue = "closed"), txtKeyword.Text.Trim(), objSortType, objSortDirection, Null.NullInteger, Null.NullInteger, Null.NullInteger, True, products)

            Dim objLayoutController As New LayoutController()

            If (Convert.ToInt32(drpProducts.SelectedValue) <> Null.NullInteger) Then

                Dim objProducts As ArrayList = objProductController.List(ModuleId, True, Convert.ToInt32(drpProducts.SelectedValue))

                Dim objProductsToSelect As New ArrayList

                For Each objProduct As ProductInfo In objProducts
                    If (objProduct.InheritSecurity) Then
                        objProductsToSelect.Add(objProduct)
                    Else
                        If (Settings.Contains(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING)) Then
                            If (PortalSecurity.IsInRoles(Settings(objProduct.ProductID.ToString() & "-" & Constants.PERMISSION_CATEGORY_VIEW_SETTING).ToString())) Then
                                objProductsToSelect.Add(objProduct)
                            End If
                        End If
                    End If
                Next

                If (objProductsToSelect.Count > 0) Then

                    Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Search_ChildProductList_Header_Html, ModuleId, Settings)
                    Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Search_ChildProductList_Item_Html, ModuleId, Settings)
                    Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Search_ChildProductList_Footer_Html, ModuleId, Settings)

                    ProcessLayoutProductListHeaderFooter(phControls.Controls, objProductListHeader.Tokens)

                    For Each objProduct As ProductInfo In objProductsToSelect
                        ProcessProductItem(phControls.Controls, objProductListItem.Tokens, objProduct)
                    Next

                    ProcessLayoutProductListHeaderFooter(phControls.Controls, objProductListFooter.Tokens)

                End If

            End If

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
                        If (txtKeyword.Text <> "") Then
                            strPages = strPages + "<a href=" & NavigateURL(Me.TabId, "", "fbType=Search", "Page=" & (i - 1).ToString(), "ProductID=" & drpProducts.SelectedValue, "Status=" & lstStatus.SelectedValue, "Keyword=" & QueryStringEncode(txtKeyword.Text), "Sort=" & QueryStringEncode(drpSortBy.SelectedValue)) & " class=""Normal"">" & i.ToString() & "</a> "
                        Else
                            strPages = strPages + "<a href=" & NavigateURL(Me.TabId, "", "fbType=Search", "Page=" & (i - 1).ToString(), "ProductID=" & drpProducts.SelectedValue, "Status=" & lstStatus.SelectedValue, "Sort=" & QueryStringEncode(drpSortBy.SelectedValue)) & " class=""Normal"">" & i.ToString() & "</a> "
                        End If
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

            Dim objMakeSuggestion As LayoutInfo = objLayoutController.GetLayout(LayoutType.Search_Make_Suggestion_Html, ModuleId, Settings)
            ProcessLayoutMakeSuggestion(phControls.Controls, objMakeSuggestion.Tokens)

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            BindCrumbs()

            If (IsPostBack = False) Then
                lstStatus.Items(0).Text = Localization.GetString("Open", Me.LocalResourceFile)
                lstStatus.Items(1).Text = Localization.GetString("Closed", Me.LocalResourceFile)
                BindProducts()
                BindSortBy()
                BindCriteria()
                BindFeedback()
            End If

        End Sub

        Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

            If (txtKeyword.Text <> "") Then
                Response.Redirect(NavigateURL(Me.TabId, "", "fbType=Search", "ProductID=" & drpProducts.SelectedValue, "Status=" & lstStatus.SelectedValue, "Keyword=" & Server.UrlEncode(txtKeyword.Text), "Sort=" & drpSortBy.SelectedValue, "SortDir=" & drpSortDirection.SelectedValue), True)
            Else
                Response.Redirect(NavigateURL(Me.TabId, "", "fbType=Search", "ProductID=" & drpProducts.SelectedValue, "Status=" & lstStatus.SelectedValue, "Sort=" & drpSortBy.SelectedValue, "SortDir=" & drpSortDirection.SelectedValue), True)
            End If

        End Sub

#End Region

#Region " Option Interfaces "

        Public ReadOnly Property ModuleActions() As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Implements DotNetNuke.Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localization.GetString("EditProducts.Text", LocalResourceFile), ModuleActionType.ContentOptions, "", "", EditUrl("EditProducts"), False, SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property

#End Region

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        'NOTE: The following placeholder declaration is required by the Web Form Designer.
        'Do not delete or move it.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

    End Class

End Namespace
