Namespace Ventrian.FeedbackCenter.Entities.CustomFields

    Public Class CustomValueInfo

#Region " Private Members "

        Dim _customValueID As Integer
        Dim _feedbackID As Integer
        Dim _customFieldID As Integer
        Dim _customValue As String

#End Region

#Region " Public Properties "

        Public Property CustomValueID() As Integer
            Get
                Return _customValueID
            End Get
            Set(ByVal Value As Integer)
                _customValueID = Value
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

        Public Property CustomFieldID() As Integer
            Get
                Return _customFieldID
            End Get
            Set(ByVal Value As Integer)
                _customFieldID = Value
            End Set
        End Property

        Public Property CustomValue() As String
            Get
                Return _customValue
            End Get
            Set(ByVal Value As String)
                _customValue = Value
            End Set
        End Property

#End Region

    End Class

End Namespace


