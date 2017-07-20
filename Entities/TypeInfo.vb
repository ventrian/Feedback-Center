'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class TypeInfo

#Region "Private Members"
        Dim _typeID As Integer
        Dim _name As String
#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal typeID As Integer, ByVal name As String)
            Me.TypeID = typeID
            Me.Name = name
        End Sub
#End Region

#Region "Public Properties"
        Public Property TypeID() As Integer
            Get
                Return _typeID
            End Get
            Set(ByVal Value As Integer)
                _typeID = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal Value As String)
                _name = Value
            End Set
        End Property
#End Region

    End Class

End Namespace
