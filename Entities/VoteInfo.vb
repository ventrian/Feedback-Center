'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class VoteInfo

#Region " Private Members "

        Dim _voteID As Integer
        Dim _feedbackID As Integer
        Dim _userID As Integer
        Dim _createDate As DateTime
        Dim _isPositive As Boolean

#End Region

#Region "Public Properties"

        Public Property VoteID() As Integer
            Get
                Return _voteID
            End Get
            Set(ByVal Value As Integer)
                _voteID = Value
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

        Public Property IsPositive() As Integer
            Get
                Return _isPositive
            End Get
            Set(ByVal Value As Integer)
                _isPositive = Value
            End Set
        End Property

#End Region

    End Class

End Namespace
