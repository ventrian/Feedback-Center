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
Imports DotNetNuke.Security
Imports Ventrian.FeedbackCenter.Entities.Layout

Namespace Ventrian.FeedbackCenter

    Public MustInherit Class LandingPage

        Inherits FeedbackCenterBase

#Region " Controls "

        Protected WithEvents lblVotes As System.Web.UI.WebControls.Label
        Protected WithEvents rptLatest As System.Web.UI.WebControls.Repeater
        Protected WithEvents lblMakeSuggestion As System.Web.UI.WebControls.Label
        Protected WithEvents lblMakeSuggestionDetail As System.Web.UI.WebControls.Label
        Protected WithEvents lblMakeSuggestionClick As System.Web.UI.WebControls.Label
        Protected WithEvents rptHighestRated As System.Web.UI.WebControls.Repeater
        Protected WithEvents Label1 As System.Web.UI.WebControls.Label
        Protected WithEvents rptRecentlyImplemented As System.Web.UI.WebControls.Repeater
        Protected WithEvents Label2 As System.Web.UI.WebControls.Label
        Protected WithEvents rptRecentlyCommented As System.Web.UI.WebControls.Repeater
        Protected WithEvents lblMyFeedback1 As System.Web.UI.WebControls.Label
        Protected WithEvents rptMyFeedback As System.Web.UI.WebControls.Repeater
        Protected WithEvents lblMyFeedback2 As System.Web.UI.WebControls.Label
        Protected WithEvents lblTrackingFeedback1 As System.Web.UI.WebControls.Label
        Protected WithEvents rptTrackingFeeedback As System.Web.UI.WebControls.Repeater
        Protected WithEvents lblTrackingFeedback2 As System.Web.UI.WebControls.Label
        Protected WithEvents trAuthFeedback As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lblNoMyFeedback As System.Web.UI.WebControls.Label
        Protected WithEvents lblNoTrackingFeedback As System.Web.UI.WebControls.Label
        Protected WithEvents lblProductList As System.Web.UI.WebControls.Label
        Protected WithEvents dlProducts As System.Web.UI.WebControls.DataList
        Protected WithEvents lblSearchAllFeedback As System.Web.UI.WebControls.Label
        Protected WithEvents lblStatistics As System.Web.UI.WebControls.Label
        Protected WithEvents lblPeriod30 As System.Web.UI.WebControls.Label
        Protected WithEvents lblFeedbackCreated30 As System.Web.UI.WebControls.Label
        Protected WithEvents Label3 As System.Web.UI.WebControls.Label
        Protected WithEvents Label4 As System.Web.UI.WebControls.Label
        Protected WithEvents lblFeedbackResolved30 As System.Web.UI.WebControls.Label
        Protected WithEvents Label5 As System.Web.UI.WebControls.Label
        Protected WithEvents lblFeedbackTime30 As System.Web.UI.WebControls.Label
        Protected WithEvents lblPeriodAll As System.Web.UI.WebControls.Label
        Protected WithEvents Label6 As System.Web.UI.WebControls.Label
        Protected WithEvents lblFeedbackCreatedAll As System.Web.UI.WebControls.Label
        Protected WithEvents Label7 As System.Web.UI.WebControls.Label
        Protected WithEvents lblFeedbackResolvedAll As System.Web.UI.WebControls.Label
        Protected WithEvents Label8 As System.Web.UI.WebControls.Label
        Protected WithEvents lblFeedbackTimeAll As System.Web.UI.WebControls.Label
        Protected WithEvents lblMoreFeedback1 As System.Web.UI.WebControls.Label
        Protected WithEvents lblMoreFeedback2 As System.Web.UI.WebControls.Label
        Protected WithEvents lblMoreFeedback3 As System.Web.UI.WebControls.Label
        Protected WithEvents lblMoreFeedback4 As System.Web.UI.WebControls.Label
        Protected WithEvents lblHighestRated As System.Web.UI.WebControls.Label
        Protected WithEvents trStatistics As System.Web.UI.HtmlControls.HtmlTableRow

        Protected WithEvents phTemplate As System.Web.UI.WebControls.PlaceHolder

#End Region

#Region " Private Methods "

        Private Sub BindTemplate()

            Dim objLayoutController As New LayoutController()
            Dim objLanding As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_Html, ModuleId, Settings)

            ProcessLayout(phTemplate.Controls, objLanding.Tokens)

        End Sub

        Private Sub ProcessLayout(ByRef controls As ControlCollection, ByVal layoutArray As String())

            Dim objProductController As New ProductController

            Dim objProducts As ArrayList = objProductController.List(Me.ModuleId, True, Null.NullInteger)
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

            For iPtr As Integer = 0 To layoutArray.Length - 1 Step 2
                controls.Add(New LiteralControl(layoutArray(iPtr).ToString()))

                If iPtr < layoutArray.Length - 1 Then
                    Select Case layoutArray(iPtr + 1)

                        Case "FEEDBACKTRACKING"

                            If (Request.IsAuthenticated) Then

                                Dim objFeedbackController As New FeedbackController()
                                Dim trackingFeedback As ArrayList = objFeedbackController.List(Me.ModuleId, Null.NullInteger, False, Null.NullString, SortType.CreateDate, SortDirection.Descending, 5, Null.NullInteger, Me.UserId, True, products)

                                Dim objLayoutController As New LayoutController()

                                Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_FeedbackTracking_Header_Html, ModuleId, Settings)
                                Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_FeedbackTracking_Item_Html, ModuleId, Settings)
                                Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_FeedbackTracking_Footer_Html, ModuleId, Settings)

                                ProcessLayoutProductListHeaderFooter(controls, objProductListHeader.Tokens)

                                For Each objFeedback As FeedbackInfo In trackingFeedback
                                    ProcessFeedbackItem(controls, objProductListItem.Tokens, objFeedback)
                                Next

                                ProcessLayoutProductListHeaderFooter(controls, objProductListFooter.Tokens)

                            End If

                        Case "HIGHESTRATED"

                            Dim objFeedbackController As New FeedbackController()
                            Dim highestRatedFeedback As ArrayList = objFeedbackController.List(Me.ModuleId, Null.NullInteger, False, Null.NullString, SortType.Popularity, SortDirection.Descending, 5, Null.NullInteger, Null.NullInteger, True, products)

                            Dim objLayoutController As New LayoutController()

                            Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_HighestRated_Header_Html, ModuleId, Settings)
                            Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_HighestRated_Item_Html, ModuleId, Settings)
                            Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_HighestRated_Footer_Html, ModuleId, Settings)

                            ProcessLayoutProductListHeaderFooter(controls, objProductListHeader.Tokens)

                            For Each objFeedback As FeedbackInfo In highestRatedFeedback
                                ProcessFeedbackItem(controls, objProductListItem.Tokens, objFeedback)
                            Next

                            ProcessLayoutProductListHeaderFooter(controls, objProductListFooter.Tokens)

                        Case "MAKESUGGESTION"

                            Dim objLayoutController As New LayoutController()
                            Dim objMakeSuggestion As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_Make_Suggestion_Html, ModuleId, Settings)
                            ProcessLayoutMakeSuggestion(controls, objMakeSuggestion.Tokens)

                        Case "MYFEEDBACK"

                            If (Request.IsAuthenticated) Then

                                Dim objFeedbackController As New FeedbackController()
                                Dim myFeedback As ArrayList = objFeedbackController.List(Me.ModuleId, Null.NullInteger, False, Null.NullString, SortType.CreateDate, SortDirection.Descending, 5, Me.UserId, Null.NullInteger, True, products)

                                Dim objLayoutController As New LayoutController()

                                Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_MyFeedback_Header_Html, ModuleId, Settings)
                                Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_MyFeedback_Item_Html, ModuleId, Settings)
                                Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_MyFeedback_Footer_Html, ModuleId, Settings)

                                ProcessLayoutProductListHeaderFooter(controls, objProductListHeader.Tokens)

                                For Each objFeedback As FeedbackInfo In myFeedback
                                    ProcessFeedbackItem(controls, objProductListItem.Tokens, objFeedback)
                                Next

                                ProcessLayoutProductListHeaderFooter(controls, objProductListFooter.Tokens)

                            End If

                        Case "PRODUCTLIST"

                            Dim objLayoutController As New LayoutController()
                            Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_ProductList_Header_Html, ModuleId, Settings)
                            Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_ProductList_Item_Html, ModuleId, Settings)
                            Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_ProductList_Footer_Html, ModuleId, Settings)

                            ProcessLayoutProductListHeaderFooter(controls, objProductListHeader.Tokens)

                            For Each objProduct As ProductInfo In objProductsToSelect
                                ProcessProductItem(controls, objProductListItem.Tokens, objProduct)
                            Next

                            ProcessLayoutProductListHeaderFooter(controls, objProductListFooter.Tokens)

                        Case "RECENTLYCOMMENTED"

                            Dim objFeedbackController As New FeedbackController()
                            Dim recentlyCommented As ArrayList = objFeedbackController.List(Me.ModuleId, Null.NullInteger, False, Null.NullString, SortType.LastCommentDate, SortDirection.Descending, 5, Null.NullInteger, Null.NullInteger, True, products)

                            Dim objLayoutController As New LayoutController()

                            Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_RecentlyCommented_Header_Html, ModuleId, Settings)
                            Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_RecentlyCommented_Item_Html, ModuleId, Settings)
                            Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_RecentlyCommented_Footer_Html, ModuleId, Settings)

                            ProcessLayoutProductListHeaderFooter(controls, objProductListHeader.Tokens)

                            For Each objFeedback As FeedbackInfo In recentlyCommented
                                ProcessFeedbackItem(controls, objProductListItem.Tokens, objFeedback)
                            Next

                            ProcessLayoutProductListHeaderFooter(controls, objProductListFooter.Tokens)

                        Case "RECENTLYIMPLEMENTED"

                            Dim objFeedbackController As New FeedbackController()
                            Dim recentlyImplemented As ArrayList = objFeedbackController.List(Me.ModuleId, Null.NullInteger, True, Null.NullString, SortType.ClosedDate, SortDirection.Descending, 5, Null.NullInteger, Null.NullInteger, True, products)

                            Dim objLayoutController As New LayoutController()

                            Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_RecentlyImplemented_Header_Html, ModuleId, Settings)
                            Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_RecentlyImplemented_Item_Html, ModuleId, Settings)
                            Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_RecentlyImplemented_Footer_Html, ModuleId, Settings)

                            ProcessLayoutProductListHeaderFooter(controls, objProductListHeader.Tokens)

                            For Each objFeedback As FeedbackInfo In recentlyImplemented
                                ProcessFeedbackItem(controls, objProductListItem.Tokens, objFeedback)
                            Next

                            ProcessLayoutProductListHeaderFooter(controls, objProductListFooter.Tokens)

                        Case "STATISTICS"

                            Dim objLayoutController As New LayoutController()
                            Dim objStatistics As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_Statistics_Html, ModuleId, Settings)
                            ProcessLayoutStatistics(controls, objStatistics.Tokens)

                        Case "WHATSNEW"

                            Dim objFeedbackController As New FeedbackController()
                            Dim whatsNewFeedback As ArrayList = objFeedbackController.List(Me.ModuleId, Null.NullInteger, False, Null.NullString, SortType.CreateDate, SortDirection.Descending, 5, Null.NullInteger, Null.NullInteger, True, products)

                            Dim objLayoutController As New LayoutController()

                            Dim objProductListHeader As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_WhatsNew_Header_Html, ModuleId, Settings)
                            Dim objProductListItem As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_WhatsNew_Item_Html, ModuleId, Settings)
                            Dim objProductListFooter As LayoutInfo = objLayoutController.GetLayout(LayoutType.Landing_WhatsNew_Footer_Html, ModuleId, Settings)

                            ProcessLayoutProductListHeaderFooter(controls, objProductListHeader.Tokens)

                            For Each objFeedback As FeedbackInfo In whatsNewFeedback
                                ProcessFeedbackItem(controls, objProductListItem.Tokens, objFeedback)
                            Next

                            ProcessLayoutProductListHeaderFooter(controls, objProductListFooter.Tokens)

                        Case Else

                            Dim objLiteralOther As New Literal
                            objLiteralOther.Text = "[" & layoutArray(iPtr + 1) & "]"
                            objLiteralOther.EnableViewState = False
                            controls.Add(objLiteralOther)

                    End Select
                End If
            Next

        End Sub


#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            BindTemplate()

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