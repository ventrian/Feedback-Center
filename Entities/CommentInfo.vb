'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class CommentInfo

#Region "Private Members"

        Dim _commentID As Integer
        Dim _feedbackID As Integer
        Dim _userID As Integer
        Dim _createDate As DateTime
        Dim _comment As String
        Dim _username As String
        Dim _firstName As String
        Dim _lastName As String
        Dim _displayName As String
        Dim _fileAttachment As String

        Dim _anonymousName As String
        Dim _anonymousEmail As String
        Dim _anonymousUrl As String
        Dim _isApproved As Boolean

#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal commentID As Integer, ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime, ByVal comment As String)
            Me.CommentID = commentID
            Me.FeedbackID = feedbackID
            Me.UserID = userID
            Me.CreateDate = createDate
            Me.Comment = comment
        End Sub
#End Region

#Region "Public Properties"
        Public Property CommentID() As Integer
            Get
                Return _commentID
            End Get
            Set(ByVal Value As Integer)
                _commentID = Value
            End Set
        End Property

        Public Property FeedbackID() As Integer
            Get
                Return _feedbackID
            End Get
            Set(ByVal Value As Integer)
                _feedbackID = Value
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

        Public Property CreateDate() As DateTime
            Get
                Return _createDate
            End Get
            Set(ByVal Value As DateTime)
                _createDate = Value
            End Set
        End Property

        Public Property Comment() As String
            Get
                Return _comment
            End Get
            Set(ByVal Value As String)
                _comment = Value
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

        Public Property FileAttachment() As String
            Get
                Return _fileAttachment
            End Get
            Set(ByVal Value As String)
                _fileAttachment = Value
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

#End Region

    End Class

End Namespace
