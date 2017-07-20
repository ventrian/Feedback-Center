
Namespace Ventrian.FeedbackCenter.Entities

    Public Class Constants

#Region " Constants "


        Public Const ENABLE_CAPTCHA_SETTING As String = "EnableCaptcha"
        Public Const ENABLE_RSS_SETTING As String = "EnableRSS"
        Public Const ENABLE_STATISTICS_SETTING As String = "EnableStatistics"
        Public Const COMMENT_ATTACHMENT_SETTING As String = "CommentAttachments"
        Public Const PERMISSION_APPROVE_SETTING As String = "PermissionApprove"
        Public Const PERMISSION_AUTO_APPROVE_SETTING As String = "PermissionAutoApprove"
        Public Const PERMISSION_COMMENT_SETTING As String = "PermissionComment"
        Public Const PERMISSION_SUBMIT_SETTING As String = "PermissionSubmit"
        Public Const PERMISSION_VOTE_SETTING As String = "PermissionVote"
        Public Const PERMISSION_CATEGORY_VIEW_SETTING As String = "PermissionCategoryView"
        Public Const PERMISSION_CATEGORY_SUBMIT_SETTING As String = "PermissionCategorySubmit"

        Public Const ACTIVE_SOCIAL_SUBMISSION_SETTING As String = "ActiveSocialSubmission"
        Public Const ACTIVE_SOCIAL_SUBSCRIBE_SETTING As String = "ActiveSocialSubscribe"
        Public Const ACTIVE_SOCIAL_VOTE_SETTING As String = "ActiveSocialVote"
        Public Const ACTIVE_SOCIAL_COMMENT_SETTING As String = "ActiveSocialComment"

        Public Const FEEDBACK_LATEST_MODULE_ID As String = "LatestModuleID"
        Public Const FEEDBACK_LATEST_TAB_ID As String = "LatestTabID"

        Public Const FEEDBACK_LATEST_HTML_HEADER As String = "LatestHeader"
        Public Const FEEDBACK_LATEST_HTML_BODY As String = "LatestBody"
        Public Const FEEDBACK_LATEST_HTML_FOOTER As String = "LatestFooter"
        Public Const FEEDBACK_LATEST_HTML_NO_FEEDBACK As String = "LatestNoFeedback"

        Public Const FEEDBACK_LATEST_MAX_COUNT As String = "LatestMaxCount"
        Public Const FEEDBACK_LATEST_USER_ID_FILTER As String = "LatestUserIDFilter"
        Public Const FEEDBACK_LATEST_USER_ID_PARAM As String = "LatestUserIDParam"

        Public Const FEEDBACK_LATEST_HTML_HEADER_DEFAULT As String = "<div class=""Normal"">"
        Public Const FEEDBACK_LATEST_HTML_BODY_DEFAULT As String = "<a href=""[LINK]"">[TITLE]</a><br />"
        Public Const FEEDBACK_LATEST_HTML_FOOTER_DEFAULT As String = "</div>"
        Public Const FEEDBACK_LATEST_HTML_NO_FEEDBACK_DEFAULT As String = "No feedback currently exists."

        Public Const FEEDBACK_LATEST_MAX_COUNT_DEFAULT As Integer = 10
        Public Const FEEDBACK_LATEST_USER_ID_FILTER_DEFAULT As Boolean = False
        Public Const FEEDBACK_LATEST_USER_ID_PARAM_DEFAULT As String = ""

        Public Const DEFAULT_ENABLE_CAPTCHA As Boolean = False
        Public Const DEFAULT_ENABLE_RSS As Boolean = True
        Public Const DEFAULT_ENABLE_STATISTICS As Boolean = True
        Public Const DEFAULT_COMMENT_ATTACHMENTS As Boolean = False

#End Region

    End Class

End Namespace
