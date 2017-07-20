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

    Public Class ProductController

#Region "Public Methods"
        Public Function [Get](ByVal productID As Integer) As ProductInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetProduct(productID), GetType(ProductInfo)), ProductInfo)

        End Function

        Public Function List(ByVal moduleId As Integer, ByVal showActiveOnly As Boolean, ByVal parentID As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().ListProduct(moduleId, showActiveOnly, parentID), GetType(ProductInfo))

        End Function

        Public Function ListAll(ByVal moduleId As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().ListProductAll(moduleId), GetType(ProductInfo))

        End Function

        Public Function Add(ByVal objProduct As ProductInfo) As Integer

            Return CType(DataProvider.Instance().AddProduct(objProduct.ModuleID, objProduct.Name, objProduct.IsActive, objProduct.ParentID, objProduct.Email, objProduct.InheritSecurity), Integer)

        End Function

        Public Sub Update(ByVal objProduct As ProductInfo)

            DataProvider.Instance().UpdateProduct(objProduct.ProductID, objProduct.ModuleID, objProduct.Name, objProduct.IsActive, objProduct.ParentID, objProduct.Email, objProduct.InheritSecurity)

        End Sub

        Public Sub Delete(ByVal productID As Integer)

            DataProvider.Instance().DeleteProduct(productID)

        End Sub
#End Region

    End Class

End Namespace
