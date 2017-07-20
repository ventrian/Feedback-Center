'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient

Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports Microsoft.ApplicationBlocks.Data

Imports Ventrian.FeedbackCenter.Data

Namespace Ventrian.FeedbackCenter

    Public Class SqlDataProvider

        Inherits DataProvider

#Region " Private Members "

        Private Const ProviderType As String = "data"

        Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String

#End Region

#Region " Constructors "

        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' Read the attributes for this provider
            _connectionString = Config.GetConnectionString()

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub

#End Region

#Region " Properties "

        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property

#End Region

#Region " Public Methods "

        Private Function GetNull(ByVal Field As Object) As Object
            Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

#Region " Feedback Methods "

        Public Overrides Function GetFeedback(ByVal feedbackID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_FeedbackGet", feedbackID), IDataReader)
        End Function

        Public Overrides Function ListFeedback(ByVal moduleId As Integer, ByVal productID As Integer, ByVal isClosed As Boolean, ByVal keywords As String, ByVal sortOrder As Integer, ByVal sortDirection As Integer, ByVal maxCount As Integer, ByVal userID As Integer, ByVal trackingID As Integer, ByVal isApproved As Boolean, ByVal products As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_FeedbackList", moduleId, productID, isClosed, GetNull(keywords), sortOrder, sortDirection, GetNull(maxCount), GetNull(userID), GetNull(trackingID), isApproved, GetNull(products)), IDataReader)
        End Function

        Public Overrides Function AddFeedback(ByVal moduleID As Integer, ByVal typeID As Integer, ByVal productID As Integer, ByVal userID As Integer, ByVal isClosed As Boolean, ByVal isResolved As Boolean, ByVal createDate As DateTime, ByVal title As String, ByVal details As String, ByVal voteTotal As Integer, ByVal voteTotalNegative As Integer, ByVal closedDate As DateTime, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_FeedbackAdd", moduleID, typeID, productID, userID, isClosed, isResolved, createDate, title, details, voteTotal, voteTotalNegative, GetNull(closedDate), GetNull(anonymousName), GetNull(anonymousEmail), GetNull(anonymousUrl), isApproved), Integer)
        End Function

        Public Overrides Sub UpdateFeedback(ByVal feedbackID As Integer, ByVal moduleID As Integer, ByVal typeID As Integer, ByVal productID As Integer, ByVal userID As Integer, ByVal isClosed As Boolean, ByVal isResolved As Boolean, ByVal createDate As DateTime, ByVal title As String, ByVal details As String, ByVal voteTotal As Integer, ByVal voteTotalNegative As Integer, ByVal closedDate As DateTime, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_FeedbackUpdate", feedbackID, moduleID, typeID, productID, userID, isClosed, isResolved, createDate, title, details, voteTotal, voteTotalNegative, GetNull(closedDate), GetNull(anonymousName), GetNull(anonymousEmail), GetNull(anonymousUrl), isApproved)
        End Sub

        Public Overrides Sub DeleteFeedback(ByVal feedbackID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_FeedbackDelete", feedbackID)
        End Sub

#End Region

#Region " Product Methods "

        Public Overrides Function GetProduct(ByVal productID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_ProductGet", productID), IDataReader)
        End Function

        Public Overrides Function ListProduct(ByVal moduleId As Integer, ByVal showActiveOnly As Boolean, ByVal parentID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_ProductList", moduleId, showActiveOnly, parentID), IDataReader)
        End Function

        Public Overrides Function ListProductAll(ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_ProductListAll", moduleId), IDataReader)
        End Function

        Public Overrides Function AddProduct(ByVal moduleID As Integer, ByVal name As String, ByVal isActive As Boolean, ByVal parentID As Integer, ByVal email As String, ByVal inheritSecurity As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_ProductAdd", moduleID, name, isActive, parentID, email, inheritSecurity), Integer)
        End Function

        Public Overrides Sub UpdateProduct(ByVal productID As Integer, ByVal moduleID As Integer, ByVal name As String, ByVal isActive As Boolean, ByVal parentID As Integer, ByVal email As String, ByVal inheritSecurity As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_ProductUpdate", productID, moduleID, name, isActive, parentID, email, inheritSecurity)
        End Sub

        Public Overrides Sub DeleteProduct(ByVal productID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_ProductDelete", productID)
        End Sub

#End Region

#Region " Type Methods "

        Public Overrides Function ListType() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_TypeList"), IDataReader)
        End Function

#End Region

#Region " Comment Methods "

        Public Overrides Function GetComment(ByVal commentID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CommentGet", commentID), IDataReader)
        End Function

        Public Overrides Function ListComment(ByVal moduleID As Integer, ByVal feedbackID As Integer, ByVal isApproved As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CommentList", moduleID, GetNull(feedbackID), isApproved), IDataReader)
        End Function

        Public Overrides Function AddComment(ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime, ByVal comment As String, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean, ByVal fileAttachment As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CommentAdd", feedbackID, userID, createDate, comment, GetNull(anonymousName), GetNull(anonymousEmail), GetNull(anonymousUrl), isApproved, GetNull(fileAttachment)), Integer)
        End Function

        Public Overrides Sub UpdateComment(ByVal commentID As Integer, ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime, ByVal comment As String, ByVal anonymousName As String, ByVal anonymousEmail As String, ByVal anonymousUrl As String, ByVal isApproved As Boolean, ByVal fileAttachment As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CommentUpdate", commentID, feedbackID, userID, createDate, comment, GetNull(anonymousName), GetNull(anonymousEmail), GetNull(anonymousUrl), isApproved, GetNull(fileAttachment))
        End Sub

        Public Overrides Sub DeleteComment(ByVal commentID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CommentDelete", commentID)
        End Sub

#End Region

#Region " Custom Field Methods "
        Public Overrides Function GetCustomField(ByVal customFieldID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomFieldGet", customFieldID), IDataReader)
        End Function

        Public Overrides Function GetCustomFieldList(ByVal moduleID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomFieldList", moduleID), IDataReader)
        End Function

        Public Overrides Sub DeleteCustomField(ByVal customFieldID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomFieldDelete", customFieldID)
        End Sub

        Public Overrides Function AddCustomField(ByVal moduleID As Integer, ByVal name As String, ByVal fieldType As Integer, ByVal fieldElements As String, ByVal defaultValue As String, ByVal caption As String, ByVal captionHelp As String, ByVal isRequired As Boolean, ByVal isVisible As Boolean, ByVal sortOrder As Integer, ByVal validationType As Integer, ByVal length As Integer, ByVal regularExpression As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomFieldAdd", moduleID, name, fieldType, GetNull(fieldElements), GetNull(defaultValue), GetNull(caption), GetNull(captionHelp), isRequired, isVisible, sortOrder, validationType, length, GetNull(regularExpression)), Integer)
        End Function

        Public Overrides Sub UpdateCustomField(ByVal customFieldID As Integer, ByVal moduleID As Integer, ByVal name As String, ByVal fieldType As Integer, ByVal fieldElements As String, ByVal defaultValue As String, ByVal caption As String, ByVal captionHelp As String, ByVal isRequired As Boolean, ByVal isVisible As Boolean, ByVal sortOrder As Integer, ByVal validationType As Integer, ByVal length As Integer, ByVal regularExpression As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomFieldUpdate", customFieldID, moduleID, name, fieldType, GetNull(fieldElements), GetNull(defaultValue), GetNull(caption), GetNull(captionHelp), isRequired, isVisible, sortOrder, validationType, length, GetNull(regularExpression))
        End Sub
#End Region

#Region " Custom Value Methods "

        Public Overrides Function GetCustomValueList(ByVal feedbackID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomValueList", feedbackID), IDataReader)
        End Function

        Public Overrides Sub DeleteCustomValue(ByVal feedbackID As Integer, ByVal customFieldID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomValueDelete", feedbackID, customFieldID)
        End Sub

        Public Overrides Function AddCustomValue(ByVal feedbackID As Integer, ByVal customFieldID As Integer, ByVal customValue As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomValueAdd", feedbackID, customFieldID, customValue), Integer)
        End Function

        Public Overrides Sub UpdateCustomValue(ByVal customValueID As Integer, ByVal feedbackID As Integer, ByVal customFieldID As Integer, ByVal customValue As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_CustomValueUpdate", customValueID, feedbackID, customFieldID, customValue)
        End Sub

#End Region

#Region " Vote Methods "

        Public Overrides Function GetVote(ByVal feedbackID As Integer, ByVal userID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_VoteGet", feedbackID, userID), IDataReader)
        End Function

        Public Overrides Function AddVote(ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime, ByVal isPositive As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_VoteAdd", feedbackID, userID, createDate, isPositive), Integer)
        End Function

#End Region

#Region " Tracking Methods "
        Public Overrides Function GetTracking(ByVal feedbackID As Integer, ByVal userID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_TrackingGet", feedbackID, userID), IDataReader)
        End Function

        Public Overrides Function ListTracking(ByVal feedbackID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_TrackingList", feedbackID), IDataReader)
        End Function

        Public Overrides Function AddTracking(ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_TrackingAdd", feedbackID, userID, createDate), Integer)
        End Function

        Public Overrides Sub DeleteTracking(ByVal feedbackID As Integer, ByVal userID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_TrackingDelete", feedbackID, userID)
        End Sub

#End Region

#Region " Statistic Methods "

        Public Overrides Function GetStatistics(ByVal moduleID As Integer, ByVal period As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "DnnForge_FeedbackCenter_GetStatistics", moduleID, GetNull(period)), IDataReader)
        End Function

#End Region

#End Region

    End Class

End Namespace