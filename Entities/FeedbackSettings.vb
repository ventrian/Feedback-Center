
Namespace Ventrian.FeedbackCenter.Entities

    Public Class FeedbackSettings

#Region " Private Members "

        Private _settings As Hashtable

#End Region

#Region " Constructors "

        Public Sub New(ByVal settings As Hashtable)
            _settings = settings
        End Sub

#End Region

#Region " Public Properties "

        Public ReadOnly Property EnableCaptcha() As Boolean
            Get
                If (_settings.Contains(Constants.ENABLE_CAPTCHA_SETTING)) Then
                    Return Convert.ToBoolean(_settings(Constants.ENABLE_CAPTCHA_SETTING).ToString())
                Else
                    Return Constants.DEFAULT_ENABLE_CAPTCHA
                End If
            End Get
        End Property

        Public ReadOnly Property EnableRSS() As Boolean
            Get
                If (_settings.Contains(Constants.ENABLE_RSS_SETTING)) Then
                    Return Convert.ToBoolean(_settings(Constants.ENABLE_RSS_SETTING).ToString())
                Else
                    Return Constants.DEFAULT_ENABLE_RSS
                End If
            End Get
        End Property

        Public ReadOnly Property EnableStatistics() As Boolean
            Get
                If (_settings.Contains(Constants.ENABLE_STATISTICS_SETTING)) Then
                    Return Convert.ToBoolean(_settings(Constants.ENABLE_STATISTICS_SETTING).ToString())
                Else
                    Return Constants.DEFAULT_ENABLE_STATISTICS
                End If
            End Get
        End Property

        Public ReadOnly Property CommentAttachments() As Boolean
            Get
                If (_settings.Contains(Constants.COMMENT_ATTACHMENT_SETTING)) Then
                    Return Convert.ToBoolean(_settings(Constants.COMMENT_ATTACHMENT_SETTING).ToString())
                Else
                    Return Constants.DEFAULT_COMMENT_ATTACHMENTS
                End If
            End Get
        End Property

        Public ReadOnly Property ActiveSocialSubmissionKey() As String
            Get
                If (_settings.Contains(Constants.ACTIVE_SOCIAL_SUBMISSION_SETTING)) Then
                    Return _settings(Constants.ACTIVE_SOCIAL_SUBMISSION_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property ActiveSocialSubscribeKey() As String
            Get
                If (_settings.Contains(Constants.ACTIVE_SOCIAL_SUBSCRIBE_SETTING)) Then
                    Return _settings(Constants.ACTIVE_SOCIAL_SUBSCRIBE_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property ActiveSocialVoteKey() As String
            Get
                If (_settings.Contains(Constants.ACTIVE_SOCIAL_VOTE_SETTING)) Then
                    Return _settings(Constants.ACTIVE_SOCIAL_VOTE_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property ActiveSocialCommentKey() As String
            Get
                If (_settings.Contains(Constants.ACTIVE_SOCIAL_COMMENT_SETTING)) Then
                    Return _settings(Constants.ACTIVE_SOCIAL_COMMENT_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property PermissionApprove() As String
            Get
                If (_settings.Contains(Constants.PERMISSION_APPROVE_SETTING)) Then
                    Return _settings(Constants.PERMISSION_APPROVE_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property PermissionAutoApprove() As String
            Get
                If (_settings.Contains(Constants.PERMISSION_AUTO_APPROVE_SETTING)) Then
                    Return _settings(Constants.PERMISSION_AUTO_APPROVE_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property PermissionComment() As String
            Get
                If (_settings.Contains(Constants.PERMISSION_COMMENT_SETTING)) Then
                    Return _settings(Constants.PERMISSION_COMMENT_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property PermissionSubmit() As String
            Get
                If (_settings.Contains(Constants.PERMISSION_SUBMIT_SETTING)) Then
                    Return _settings(Constants.PERMISSION_SUBMIT_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property PermissionVote() As String
            Get
                If (_settings.Contains(Constants.PERMISSION_VOTE_SETTING)) Then
                    Return _settings(Constants.PERMISSION_VOTE_SETTING).ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

#End Region

    End Class

End Namespace
