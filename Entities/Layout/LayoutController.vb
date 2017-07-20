Imports System.IO

Imports DotNetNuke.Common.Utilities

Namespace Ventrian.FeedbackCenter.Entities.Layout

    Public Class LayoutController

#Region " Public Methods "

        Public Function GetLayout(ByVal type As LayoutType, ByVal moduleID As Integer, ByVal settings As Hashtable) As LayoutInfo

            Dim cacheKey As String = "FeedbackCenter-" & moduleID.ToString() & "-" & type.ToString()
            Dim objLayout As LayoutInfo = CType(DataCache.GetCache(cacheKey), LayoutInfo)

            If (objLayout Is Nothing) Then

                Dim delimStr As String = "[]"
                Dim delimiter As Char() = delimStr.ToCharArray()

                objLayout = New LayoutInfo
                Dim folderPath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\Templates\" & moduleID.ToString())
                Dim filePath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\Templates\" & moduleID.ToString() & "\" & type.ToString().Replace("_", "."))

                If (File.Exists(filePath) = False) Then

                    Dim layout As String = ""

                    Dim filePathDefault As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\Templates\Default\" & type.ToString().Replace("_", "."))
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

        Public Sub UpdateLayout(ByVal type As LayoutType, ByVal moduleID As Integer, ByVal content As String)

            Dim folderPath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\Templates\" & moduleID.ToString())
            Dim filePath As String = HttpContext.Current.Server.MapPath("~\DesktopModules\FeedbackCenter\Templates\" & moduleID.ToString() & "\" & type.ToString().Replace("_", "."))

            If Not (Directory.Exists(folderPath)) Then
                Directory.CreateDirectory(folderPath)
            End If

            File.WriteAllText(filePath, content)

            Dim cacheKey As String = "FeedbackCenter-" & moduleID.ToString() & "-" & type.ToString()
            DataCache.RemoveCache(cacheKey)

        End Sub

#End Region

        Public Sub New()

        End Sub
    End Class

End Namespace
