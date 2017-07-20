'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class StatisticsInfo

#Region " Private Members "

        Dim _feedbackCreated As Integer
        Dim _feedbackResolved As Integer
        Dim _averageTime As Integer

#End Region

#Region " Public Properties "

        Public Property FeedbackCreated() As Integer
            Get
                Return _feedbackCreated
            End Get
            Set(ByVal Value As Integer)
                _feedbackCreated = Value
            End Set
        End Property

        Public Property FeedbackResolved() As Integer
            Get
                Return _feedbackResolved
            End Get
            Set(ByVal Value As Integer)
                _feedbackResolved = Value
            End Set
        End Property

        Public Property AverageTime() As Integer
            Get
                Return _averageTime
            End Get
            Set(ByVal Value As Integer)
                _averageTime = Value
            End Set
        End Property

#End Region

    End Class

End Namespace
