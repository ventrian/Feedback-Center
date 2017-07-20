'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Namespace Ventrian.FeedbackCenter.Entities

    Public Class CrumbInfo

#Region "Private Members"

        Dim _caption As String
        Dim _url As String

#End Region

#Region "Public Properties"

        Public Property Caption() As String
            Get
                Return _caption
            End Get
            Set(ByVal Value As String)
                _caption = Value
            End Set
        End Property

        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal Value As String)
                _url = Value
            End Set
        End Property

#End Region

    End Class

End Namespace
