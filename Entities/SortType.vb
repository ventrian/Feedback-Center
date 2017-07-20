'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System
Imports System.Configuration
Imports System.Data

Imports DotNetNuke.Common.Utilities

Namespace Ventrian.FeedbackCenter

    Public Enum SortType

        Popularity = 1
        CreateDate = 2
        ClosedDate = 3
        LastCommentDate = 4

    End Enum

End Namespace
