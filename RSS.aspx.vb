'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'
Imports System.Web
Imports System.IO
Imports System.Text
Imports System.Xml

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.FeedbackCenter.Entities

Namespace Ventrian.FeedbackCenter

    Public MustInherit Class RSS
        Inherits DotNetNuke.Framework.PageBase

#Region " Controls "

#End Region

#Region " Private Members "

        Dim _tabID As Integer = Null.NullInteger
        Dim _moduleID As Integer = Null.NullInteger
        Dim _productID As Integer = Null.NullInteger
        Dim _status As String = Null.NullString
        Dim _keyword As String = Null.NullString
        Dim _sort As String = "CreateDate"
        Dim _maxCount As Integer = 10
        Dim _userID As Integer = Null.NullInteger
        Dim _trackingID As Integer = Null.NullInteger

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If Not (Request("T") Is Nothing) Then
                _tabID = Convert.ToInt32(Request("T"))
            End If

            If Not (Request("M") Is Nothing) Then
                _moduleID = Convert.ToInt32(Request("M"))
            End If

            If Not (Request("P") Is Nothing) Then
                _productID = Convert.ToInt32(Request("P"))
            End If

            If Not (Request("S") Is Nothing) Then
                _status = Request("S")
            End If

            If Not (Request("Keyword") Is Nothing) Then
                _keyword = Request("Keyword")
            End If

            If Not (Request("Sort") Is Nothing) Then
                _sort = Request("Sort")
            End If

            If Not (Request("MaxCount") Is Nothing) Then
                _maxCount = Convert.ToInt32(Request("MaxCount"))
            End If

            If Not (Request("UserID") Is Nothing) Then
                _userID = Convert.ToInt32(Request("UserID"))
            End If

            If Not (Request("TrackingID") Is Nothing) Then
                _trackingID = Convert.ToInt32(Request("TrackingID"))
            End If

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ReadQueryString()

            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)

            Dim objModuleController As New ModuleController()
            Dim objSettings As Hashtable = objModuleController.GetModuleSettings(_moduleID)

            If (objSettings.Contains(Constants.ENABLE_RSS_SETTING)) Then
                If (Convert.ToBoolean(objSettings(Constants.ENABLE_RSS_SETTING).ToString()) = False) Then
                    Response.Clear()
                    Response.Redirect("~", True)
                End If
            End If

            Response.ContentType = "text/xml"
            Response.ContentEncoding = Encoding.UTF8

            Dim sw As StringWriter = New StringWriter
            Dim writer As XmlTextWriter = New XmlTextWriter(sw)

            writer.WriteProcessingInstruction("xml-stylesheet", "type=""text/xsl"" href=""" & AddHTTP(System.Web.HttpContext.Current.Request.Url.Host & Me.ResolveUrl("RSS.xsl")) & """ media=""screen""")

            writer.WriteStartElement("rss")
            writer.WriteAttributeString("version", "2.0")
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/")

            writer.WriteStartElement("channel")

            Dim objModule As ModuleInfo = objModuleController.GetModule(_moduleID, _tabID)

            If Not (objModule Is Nothing) Then
                writer.WriteElementString("title", objModule.ModuleTitle)
            End If

            If (_portalSettings.PortalAlias.HTTPAlias.IndexOf("http://") = -1) Then
                writer.WriteElementString("link", AddHTTP(System.Web.HttpContext.Current.Request.Url.Host & Page.ResolveUrl("~/Default.aspx?TabID=" & _tabID.ToString())))
            Else
                writer.WriteElementString("link", AddHTTP(System.Web.HttpContext.Current.Request.Url.Host & Page.ResolveUrl("~/Default.aspx?TabID=" & _tabID.ToString())))
            End If

            If (_productID <> Null.NullInteger) Then
                Dim objProductController As New ProductController
                Dim objProduct As ProductInfo = objProductController.Get(_productID)
                If Not (objProduct Is Nothing) Then
                    writer.WriteElementString("description", "Feedback for " & objProduct.Name)
                End If
            Else
                writer.WriteElementString("description", "Feedback for " & _portalSettings.PortalName)
            End If

            writer.WriteElementString("ttl", "60")

            Dim objSortType As SortType = System.Enum.Parse(GetType(SortType), _sort)

            Dim objFeedbackController As New FeedbackController
            Dim objFeedbackList As ArrayList = objFeedbackController.List(Me._moduleID, Me._productID, (_status.ToLower() = "closed"), _keyword, objSortType, SortDirection.Descending, _maxCount, _userID, _trackingID, True, Nothing)

            For Each objFeedback As FeedbackInfo In objFeedbackList

                writer.WriteStartElement("item")

                writer.WriteElementString("title", objFeedback.Title)


                Dim link As String = ""

                If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") = "Y" Then
                    link = NavigateURL(_tabID, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                Else
                    link = NavigateURL(_tabID, "", "fbType=View", "FeedbackID=" & objFeedback.FeedbackID.ToString())
                    If Not (link.ToLower().StartsWith("http://") Or link.ToLower().StartsWith("https://")) Then
                        link = AddHTTP(System.Web.HttpContext.Current.Request.Url.Host & link)
                    End If
                End If

                writer.WriteElementString("link", link)

                writer.WriteElementString("dc:creator", objFeedback.DisplayName)
                writer.WriteElementString("description", objFeedback.Details)
                writer.WriteElementString("pubDate", objFeedback.CreateDate.ToString("r"))

                writer.WriteEndElement()

            Next

            writer.WriteEndElement()
            writer.WriteEndElement()

            Response.Write(sw.ToString)

        End Sub

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
