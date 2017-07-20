'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System
Imports System.Web.Caching
Imports System.Reflection

Imports DotNetNuke
Imports DotNetNuke.Common.Utilities

Namespace Ventrian.FeedbackCenter.Data

    Public MustInherit Class DataProvider

#Region " Shared/Static Methods "

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "Ventrian.FeedbackCenter", "Ventrian.FeedbackCenter"), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region " Abstract methods "

        Public MustOverride Function GetFeedback(ByVal feedbackID As Integer) As IDataReader
        Public MustOverride Function ListFeedback(ByVal moduleId As Integer, ByVal productID As Integer, ByVal isClosed As Boolean, ByVal keywords As String, ByVal sortOrder As Integer, ByVal sortDirection As Integer, ByVal maxCount As Integer, ByVal userID As Integer, ByVal trackingID As Integer, ByVal isApproved As Boolean, ByVal products As String) As IDataReader
        Public MustOverride Function AddFeedback(ByVal moduleID As Integer, ByVal typeID As Integer, ByVal productID As Integer, ByVal userID As Integer, ByVal isClosed As Boolean, ByVal isResolved As Boolean, ByVal createDate As DateTime, ByVal title As String, ByVal details As String, ByVal voteTotal As Integer, ByVal voteTotalNegative As Integer, ByVal closedDate As DateTime, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean) As Integer
        Public MustOverride Sub UpdateFeedback(ByVal feedbackID As Integer, ByVal moduleID As Integer, ByVal typeID As Integer, ByVal productID As Integer, ByVal userID As Integer, ByVal isClosed As Boolean, ByVal isResolved As Boolean, ByVal createDate As DateTime, ByVal title As String, ByVal details As String, ByVal voteTotal As Integer, ByVal voteTotalNegative As Integer, ByVal closedDate As DateTime, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean)
        Public MustOverride Sub DeleteFeedback(ByVal feedbackID As Integer)

        Public MustOverride Function GetProduct(ByVal productID As Integer) As IDataReader
        Public MustOverride Function ListProduct(ByVal moduleId As Integer, ByVal showActiveOnly As Boolean, ByVal parentID As Integer) As IDataReader
        Public MustOverride Function ListProductAll(ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function AddProduct(ByVal moduleID As Integer, ByVal name As String, ByVal isActive As Boolean, ByVal parentID As Integer, ByVal email As String, ByVal inheritSecurity As Boolean) As Integer
        Public MustOverride Sub UpdateProduct(ByVal productID As Integer, ByVal moduleID As Integer, ByVal name As String, ByVal isActive As Boolean, ByVal parentID As Integer, ByVal email As String, ByVal inheritSecurity As Boolean)
        Public MustOverride Sub DeleteProduct(ByVal productID As Integer)

        Public MustOverride Function ListType() As IDataReader

        Public MustOverride Function GetComment(ByVal commentID As Integer) As IDataReader
        Public MustOverride Function ListComment(ByVal moduleID As Integer, ByVal feedbackID As Integer, ByVal isApproved As Boolean) As IDataReader
        Public MustOverride Function AddComment(ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime, ByVal comment As String, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean, ByVal fileAttachment As String) As Integer
        Public MustOverride Sub UpdateComment(ByVal commentID As Integer, ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime, ByVal comment As String, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean, ByVal fileAttachment As String)
        Public MustOverride Sub DeleteComment(ByVal commentID As Integer)

        Public MustOverride Function GetVote(ByVal feedbackID As Integer, ByVal userID As Integer) As IDataReader
        Public MustOverride Function AddVote(ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime, ByVal isPositive As Boolean) As Integer

        Public MustOverride Function GetTracking(ByVal feedbackID As Integer, ByVal userID As Integer) As IDataReader
        Public MustOverride Function ListTracking(ByVal feedbackID As Integer) As IDataReader
        Public MustOverride Function AddTracking(ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime) As Integer
        Public MustOverride Sub DeleteTracking(ByVal feedbackID As Integer, ByVal userID As Integer)

        Public MustOverride Function GetStatistics(ByVal moduleID As Integer, ByVal period As Integer) As IDataReader

        Public MustOverride Function GetCustomField(ByVal customFieldID As Integer) As IDataReader
        Public MustOverride Function GetCustomFieldList(ByVal moduleID As Integer) As IDataReader
        Public MustOverride Sub DeleteCustomField(ByVal commentID As Integer)
        Public MustOverride Function AddCustomField(ByVal moduleID As Integer, ByVal name As String, ByVal fieldType As Integer, ByVal fieldElements As String, ByVal defaultValue As String, ByVal caption As String, ByVal captionHelp As String, ByVal isRequired As Boolean, ByVal isVisible As Boolean, ByVal sortOrder As Integer, ByVal validationType As Integer, ByVal length As Integer, ByVal regularExpression As String) As Integer
        Public MustOverride Sub UpdateCustomField(ByVal customFieldID As Integer, ByVal moduleID As Integer, ByVal name As String, ByVal fieldType As Integer, ByVal fieldElements As String, ByVal defaultValue As String, ByVal caption As String, ByVal captionHelp As String, ByVal isRequired As Boolean, ByVal isVisible As Boolean, ByVal sortOrder As Integer, ByVal validationType As Integer, ByVal length As Integer, ByVal regularExpression As String)

        Public MustOverride Function GetCustomValueList(ByVal feedbackID As Integer) As IDataReader
        Public MustOverride Function AddCustomValue(ByVal feedbackID As Integer, ByVal customFieldID As Integer, ByVal customValue As String) As Integer
        Public MustOverride Sub UpdateCustomValue(ByVal customValueID As Integer, ByVal feedbackID As Integer, ByVal customFieldID As Integer, ByVal customValue As String)
        Public MustOverride Sub DeleteCustomValue(ByVal feedbackID As Integer, ByVal customFieldID As Integer)

#End Region

    End Class

End Namespace