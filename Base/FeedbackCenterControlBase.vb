Imports DotNetNuke.Common.Utilities

Namespace Ventrian.FeedbackCenter

    Public Class FeedbackCenterControlBase
        Inherits System.Web.UI.UserControl

#Region " Private Members "

        Private _feedbackID As Integer = Null.NullInteger

#End Region

#Region " Private Methods "

        Public Property FeedbackID() As Integer
            Get
                Return _feedbackID
            End Get
            Set(ByVal value As Integer)
                _feedbackID = value
            End Set
        End Property

        Protected ReadOnly Property FeedbackCenterBase() As FeedbackCenterBase
            Get
                Dim objParent As Control = Parent

                While objParent IsNot Nothing
                    If (TypeOf objParent Is FeedbackCenterBase) Then
                        Return CType(objParent, FeedbackCenterBase)
                    Else
                        objParent = objParent.Parent
                    End If
                End While

                Return Nothing
            End Get
        End Property

#End Region

    End Class

End Namespace
