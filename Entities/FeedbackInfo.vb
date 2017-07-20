Imports Ventrian.FeedbackCenter.Entities.CustomFields

'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class FeedbackInfo

#Region " Private Members "

        Dim _feedbackID As Integer
        Dim _moduleID As Integer
        Dim _typeID As Integer
        Dim _productID As Integer
        Dim _userID As Integer
        Dim _isClosed As Boolean
        Dim _isResolved As Boolean
        Dim _createDate As DateTime
        Dim _title As String
        Dim _details As String
        Dim _voteTotal As Integer
        Dim _voteTotalNegative As Integer
        Dim _closedDate As DateTime
        Dim _username As String
        Dim _firstName As String
        Dim _lastName As String
        Dim _displayName As String
        Dim _productName As String
        Dim _popularity As Integer
        Dim _lastCommentDate As DateTime

        Dim _anonymousName As String
        Dim _anonymousEmail As String
        Dim _anonymousUrl As String
        Dim _isApproved As Boolean

        Dim _customList As Hashtable

#End Region

#Region " Private Methods "

        Private Sub InitializePropertyList()

            ' Add Caching 
            Dim objCustomFieldController As New CustomFieldController
            Dim objCustomFields As ArrayList = objCustomFieldController.List(Me.ModuleID)

            Dim objCustomValueController As New CustomValueController
            Dim objCustomValues As List(Of CustomValueInfo) = objCustomValueController.List(Me.FeedbackID)

            _customList = New Hashtable

            For Each objCustomField As CustomFieldInfo In objCustomFields
                Dim value As String = ""
                For Each objCustomValue As CustomValueInfo In objCustomValues
                    If (objCustomValue.CustomFieldID = objCustomField.CustomFieldID) Then
                        value = objCustomValue.CustomValue
                    End If
                Next
                _customList.Add(objCustomField.CustomFieldID, value)
            Next

        End Sub

#End Region

#Region " Public Properties "
        Public Property FeedbackID() As Integer
            Get
                Return _feedbackID
            End Get
            Set(ByVal Value As Integer)
                _feedbackID = Value
            End Set
        End Property

        Public Property ModuleID() As Integer
            Get
                Return _moduleID
            End Get
            Set(ByVal Value As Integer)
                _moduleID = Value
            End Set
        End Property

        Public Property TypeID() As Integer
            Get
                Return _typeID
            End Get
            Set(ByVal Value As Integer)
                _typeID = Value
            End Set
        End Property

        Public Property ProductID() As Integer
            Get
                Return _productID
            End Get
            Set(ByVal Value As Integer)
                _productID = Value
            End Set
        End Property

        Public Property UserID() As Integer
            Get
                Return _userID
            End Get
            Set(ByVal Value As Integer)
                _userID = Value
            End Set
        End Property

        Public Property IsClosed() As Boolean
            Get
                Return _isClosed
            End Get
            Set(ByVal Value As Boolean)
                _isClosed = Value
            End Set
        End Property

        Public Property IsResolved() As Boolean
            Get
                Return _isResolved
            End Get
            Set(ByVal Value As Boolean)
                _isResolved = Value
            End Set
        End Property

        Public Property CreateDate() As DateTime
            Get
                Return _createDate
            End Get
            Set(ByVal Value As DateTime)
                _createDate = Value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal Value As String)
                _title = Value
            End Set
        End Property

        Public Property Details() As String
            Get
                Return _details
            End Get
            Set(ByVal Value As String)
                _details = Value
            End Set
        End Property

        Public Property VoteTotal() As Integer
            Get
                Return _voteTotal
            End Get
            Set(ByVal Value As Integer)
                _voteTotal = Value
            End Set
        End Property

        Public Property VoteTotalNegative() As Integer
            Get
                Return _voteTotalNegative
            End Get
            Set(ByVal Value As Integer)
                _voteTotalNegative = Value
            End Set
        End Property

        Public Property ClosedDate() As DateTime
            Get
                Return _closedDate
            End Get
            Set(ByVal Value As DateTime)
                _closedDate = Value
            End Set
        End Property

        Public Property Username() As String
            Get
                Return _username
            End Get
            Set(ByVal Value As String)
                _username = Value
            End Set
        End Property

        Public Property FirstName() As String
            Get
                Return _firstName
            End Get
            Set(ByVal Value As String)
                _firstName = Value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return _lastName
            End Get
            Set(ByVal Value As String)
                _lastName = Value
            End Set
        End Property

        Public Property DisplayName() As String
            Get
                Return _displayName
            End Get
            Set(ByVal Value As String)
                _displayName = Value
            End Set
        End Property

        Public Property ProductName() As String
            Get
                Return _productName
            End Get
            Set(ByVal Value As String)
                _productName = Value
            End Set
        End Property

        Public Property Popularity() As Integer
            Get
                Return _popularity
            End Get
            Set(ByVal Value As Integer)
                _popularity = Value
            End Set
        End Property

        Public Property LastCommentDate() As DateTime
            Get
                Return _lastCommentDate
            End Get
            Set(ByVal Value As DateTime)
                _lastCommentDate = Value
            End Set
        End Property

        Public Property AnonymousName() As String
            Get
                Return _anonymousName
            End Get
            Set(ByVal Value As String)
                _anonymousName = Value
            End Set
        End Property

        Public Property AnonymousEmail() As String
            Get
                Return _anonymousEmail
            End Get
            Set(ByVal Value As String)
                _anonymousEmail = Value
            End Set
        End Property

        Public Property AnonymousUrl() As String
            Get
                Return _anonymousUrl
            End Get
            Set(ByVal Value As String)
                _anonymousUrl = Value
            End Set
        End Property

        Public Property IsApproved() As Boolean
            Get
                Return _isApproved
            End Get
            Set(ByVal Value As Boolean)
                _isApproved = Value
            End Set
        End Property

        Public ReadOnly Property CustomList() As Hashtable
            Get
                If (_customList Is Nothing) Then
                    InitializePropertyList()
                End If
                Return _customList
            End Get
        End Property

#End Region

    End Class

End Namespace
