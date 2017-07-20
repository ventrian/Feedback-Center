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

    Public Class FeedbackController

#Region " Public Methods "

        Public Function [Get](ByVal feedbackID As Integer) As FeedbackInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetFeedback(feedbackID), GetType(FeedbackInfo)), FeedbackInfo)

        End Function

        Public Function List(ByVal moduleId As Integer, ByVal productID As Integer, ByVal isClosed As Boolean, ByVal keywords As String, ByVal sortType As SortType, ByVal sortDirection As SortDirection, ByVal maxCount As Integer, ByVal userID As Integer, ByVal trackingID As Integer, ByVal isApproved As Boolean, ByVal objProducts As List(Of Integer)) As ArrayList

            Dim products As String = ""
            If (objProducts IsNot Nothing) Then
                For Each product As Integer In objProducts
                    If (products = "") Then
                        products = product.ToString()
                    Else
                        products = products & "," & product.ToString()
                    End If
                Next
            End If

            Return CBO.FillCollection(DataProvider.Instance().ListFeedback(moduleId, productID, isClosed, keywords, CType(sortType, Integer), CType(sortDirection, Integer), maxCount, userID, trackingID, isApproved, products), GetType(FeedbackInfo))

        End Function

        Public Function Add(ByVal objFeedback As FeedbackInfo) As Integer

            Return CType(DataProvider.Instance().AddFeedback(objFeedback.ModuleID, objFeedback.TypeID, objFeedback.ProductID, objFeedback.UserID, objFeedback.IsClosed, objFeedback.IsResolved, objFeedback.CreateDate, objFeedback.Title, objFeedback.Details, objFeedback.VoteTotal, objFeedback.VoteTotalNegative, objFeedback.ClosedDate, objFeedback.AnonymousName, objFeedback.AnonymousEmail, objFeedback.AnonymousUrl, objFeedback.IsApproved), Integer)

        End Function

        Public Sub Update(ByVal objFeedback As FeedbackInfo)

            DataProvider.Instance().UpdateFeedback(objFeedback.FeedbackID, objFeedback.ModuleID, objFeedback.TypeID, objFeedback.ProductID, objFeedback.UserID, objFeedback.IsClosed, objFeedback.IsResolved, objFeedback.CreateDate, objFeedback.Title, objFeedback.Details, objFeedback.VoteTotal, objFeedback.VoteTotalNegative, objFeedback.ClosedDate, objFeedback.AnonymousName, objFeedback.AnonymousEmail, objFeedback.AnonymousUrl, objFeedback.IsApproved)

        End Sub

        Public Sub Delete(ByVal feedbackID As Integer)

            DataProvider.Instance().DeleteFeedback(feedbackID)

        End Sub

#End Region

    End Class

End Namespace
