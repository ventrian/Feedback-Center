'
' Feedback Center for DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Scott McCulloch ( smcculloch@iinet.net.au ) ( http://www.smcculloch.net )
'

Imports System
Imports System.Data

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework

Imports Ventrian.FeedbackCenter.Data

Namespace Ventrian.FeedbackCenter.Entities

    Public Class TrackingController

#Region "Public Methods"

        Public Function [Get](ByVal feedbackID As Integer, ByVal userID As Integer) As TrackingInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetTracking(feedbackID, userID), GetType(TrackingInfo)), TrackingInfo)

        End Function

        Public Function List(ByVal feedbackID As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().ListTracking(feedbackID), GetType(TrackingInfo))

        End Function


        Public Function Add(ByVal objTracking As TrackingInfo) As Integer

            Return CType(DataProvider.Instance().AddTracking(objTracking.FeedbackID, objTracking.UserID, objTracking.CreateDate), Integer)

        End Function

        Public Sub Delete(ByVal feedbackID As Integer, ByVal userID As Integer)

            DataProvider.Instance().DeleteTracking(feedbackID, userID)

        End Sub

#End Region

    End Class

End Namespace
