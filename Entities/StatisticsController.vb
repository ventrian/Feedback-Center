Imports System
Imports System.Data

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework

Imports Ventrian.FeedbackCenter.Data

Namespace Ventrian.FeedbackCenter.Entities

    Public Class StatisticsController

#Region "Public Methods"

        Public Function [Get](ByVal moduleID As Integer, ByVal period As Integer) As StatisticsInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetStatistics(moduleID, period), GetType(StatisticsInfo)), StatisticsInfo)

        End Function

#End Region

    End Class

End Namespace
