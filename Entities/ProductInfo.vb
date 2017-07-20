'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class ProductInfo

#Region " Private Members "
        Dim _productID As Integer
        Dim _moduleID As Integer
        Dim _name As String
        Dim _isActive As Boolean
        Dim _parentID As Integer
        Dim _email As String
        Dim _inheritSecurity As Boolean
#End Region

#Region "Public Properties"
        Public Property ProductID() As Integer
            Get
                Return _productID
            End Get
            Set(ByVal Value As Integer)
                _productID = Value
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

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal Value As String)
                _name = Value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return _isActive
            End Get
            Set(ByVal Value As Boolean)
                _isActive = Value
            End Set
        End Property

        Public Property ParentID() As Integer
            Get
                Return _parentID
            End Get
            Set(ByVal Value As Integer)
                _parentID = Value
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

        Public Property InheritSecurity() As Boolean
            Get
                Return _inheritSecurity
            End Get
            Set(ByVal value As Boolean)
                _inheritSecurity = value
            End Set
        End Property
#End Region

    End Class

End Namespace
