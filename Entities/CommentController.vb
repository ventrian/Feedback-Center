Imports System
Imports System.Data

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework

Imports Ventrian.FeedbackCenter.Data

Namespace Ventrian.FeedbackCenter.Entities

    Public Class CommentController

#Region "Public Methods"

        Public Function [Get](ByVal commentID As Integer) As CommentInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetComment(commentID), GetType(CommentInfo)), CommentInfo)

        End Function

        Public Function List(ByVal moduleID As Integer, ByVal feedbackID As Integer, ByVal isApproved As Boolean) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().ListComment(moduleID, feedbackID, isApproved), GetType(CommentInfo))

        End Function

        Public Function Add(ByVal objComment As CommentInfo) As Integer

            Return CType(DataProvider.Instance().AddComment(objComment.FeedbackID, objComment.UserID, objComment.CreateDate, objComment.Comment, objComment.AnonymousName, objComment.AnonymousEmail, objComment.AnonymousUrl, objComment.IsApproved, objComment.FileAttachment), Integer)

        End Function

        Public Sub Update(ByVal objComment As CommentInfo)

            DataProvider.Instance().UpdateComment(objComment.CommentID, objComment.FeedbackID, objComment.UserID, objComment.CreateDate, objComment.Comment, objComment.AnonymousName, objComment.AnonymousEmail, objComment.AnonymousUrl, objComment.IsApproved, objComment.FileAttachment)

        End Sub


        Public Sub Delete(ByVal commentID As Integer)

            DataProvider.Instance().DeleteComment(commentID)

        End Sub
#End Region

    End Class

End Namespace
