Imports System.IO

Imports DotNetNuke.Common.Utilities

Namespace Ventrian.FeedbackCenter.Entities.Emails

    Public Class EmailController

#Region " Public Methods "

        Public Function GetLayout(ByVal type As EmailType, ByVal moduleID As Integer, ByVal settings As Hashtable) As EmailInfo

            Dim cacheKey As String = "FeedbackCenter-Email-" & moduleID.ToString() & "-" & type.ToString()
            Dim objLayout As EmailInfo = CType(DataCache.GetCache(cacheKey), EmailInfo)

            If (objLayout Is Nothing) Then

                Dim delimStr As String = "[]"
                Dim delimiter As Char() = delimStr.ToCharArray()

                objLayout = New EmailInfo

                Dim folderPath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\TemplatesEmail\" & moduleID.ToString())
                Dim filePath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\TemplatesEmail\" & moduleID.ToString() & "\" & type.ToString().Replace("_", "."))

                If (File.Exists(filePath) = False) Then

                    Dim layout As String = ""

                    Dim filePathDefault As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\TemplatesEmail\Default\" & type.ToString().Replace("_", "."))
                    If (File.Exists(filePathDefault)) Then
                        layout = File.ReadAllText(filePathDefault)
                    End If

                    If Not (Directory.Exists(folderPath)) Then
                        Directory.CreateDirectory(folderPath)
                    End If

                    File.WriteAllText(filePath, layout)

                End If

                objLayout.Template = File.ReadAllText(filePath)
                objLayout.Tokens = objLayout.Template.Split(delimiter)

                DataCache.SetCache(cacheKey, objLayout, New CacheDependency(filePath))

            End If

            Return objLayout

        End Function

        Public Sub UpdateLayout(ByVal type As EmailType, ByVal moduleID As Integer, ByVal content As String)

            Dim folderPath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\TemplatesEmail\" & moduleID.ToString())
            Dim filePath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\TemplatesEmail\" & moduleID.ToString() & "\" & type.ToString().Replace("_", "."))

            If Not (Directory.Exists(folderPath)) Then
                Directory.CreateDirectory(folderPath)
            End If

            File.WriteAllText(filePath, content)

            Dim cacheKey As String = "FeedbackCenter-Email-" & moduleID.ToString() & "-" & type.ToString()
            DataCache.RemoveCache(cacheKey)

        End Sub

#End Region

    End Class

End Namespace
