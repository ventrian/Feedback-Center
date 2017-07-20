'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System
Imports System.Data

Imports DotNetNuke.Common.Utilities

Imports Ventrian.FeedbackCenter.Data

Namespace Ventrian.FeedbackCenter.Entities

    Public Class TypeController

#Region "Public Methods"

        Public Function List() As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().ListType(), GetType(TypeInfo))

        End Function

#End Region

    End Class

End Namespace
