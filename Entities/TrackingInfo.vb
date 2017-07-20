'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class TrackingInfo

#Region "Private Members"
        Dim _trackingID As Integer
        Dim _feedbackID As Integer
        Dim _userID As Integer
        Dim _createDate As DateTime
        Dim _email As String
#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal trackingID As Integer, ByVal feedbackID As Integer, ByVal userID As Integer, ByVal createDate As DateTime)
            Me.TrackingID = trackingID
            Me.FeedbackID = feedbackID
            Me.UserID = userID
            Me.CreateDate = createDate
        End Sub
#End Region

#Region "Public Properties"
        Public Property TrackingID() As Integer
            Get
                Return _trackingID
            End Get
            Set(ByVal Value As Integer)
                _trackingID = Value
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

        Public Property Email() As String
            Get
                Return _email
            End Get
            Set(ByVal Value As String)
                _email = Value
            End Set
        End Property
#End Region

    End Class

End Namespace
