'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System
Imports System.Data

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework

Imports Ventrian.FeedbackCenter.Data

Namespace Ventrian.FeedbackCenter.Entities

    Public Class VoteController

#Region "Public Methods"

        Public Function [Get](ByVal feedbackID As Integer, ByVal userID As Integer) As VoteInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetVote(feedbackID, userID), GetType(VoteInfo)), VoteInfo)

        End Function

        Public Function Add(ByVal objVote As VoteInfo) As Integer

            Return CType(DataProvider.Instance().AddVote(objVote.FeedbackID, objVote.UserID, objVote.CreateDate, objVote.IsPositive), Integer)

        End Function

#End Region

    End Class

End Namespace
