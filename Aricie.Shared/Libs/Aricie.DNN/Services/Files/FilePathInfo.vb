Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.ComponentModel
Imports System.Web.Configuration
Imports System.Security.Cryptography
Imports System.ComponentModel
Imports DotNetNuke.UI.WebControls
Imports Aricie.DNN.Services.Flee
Imports Aricie.DNN.Entities
Imports System.Xml.Serialization
Imports System.IO
Imports System.Xml
Imports System.Text
Imports Aricie.DNN.UI.WebControls
Imports Aricie.DNN.UI.WebControls.EditControls

Namespace Services.Files
    <Serializable()>
    Public Class FolderPathInfo
        Inherits PathInfo

    End Class

    <Serializable()>
    Public Class FilePathInfo
        Inherits PathInfo

        <SortOrder(0)> _
        Public Property ChooseDnnFile As Boolean

        <ConditionalVisible("ChooseDnnFile", False, True)> _
        Public Property DnnFile As New ControlUrlInfo(UrlControlMode.File Or UrlControlMode.Database Or UrlControlMode.Secure Or UrlControlMode.Upload)

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        Public Overrides Property PathMode As FilePathMode
            Get
                Return MyBase.PathMode
            End Get
            Set(value As FilePathMode)
                MyBase.PathMode = value
            End Set
        End Property

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        <Selector(GetType(PortalSelector), "PortalName", "PortalID", False, False, "", "", False, False)> _
        <Editor(GetType(SelectorEditControl), GetType(EditControl))> _
        <ConditionalVisible("PathMode", False, True, FilePathMode.AdminPath)>
         Public Overrides Property PortalId As Integer
            Get
                Return MyBase.PortalId
            End Get
            Set(value As Integer)
                MyBase.PortalId = value
            End Set
        End Property

        <ConditionalVisible("ChooseDnnFile", True, True)> _
        Public Overrides Property Path As SimpleOrExpression(Of String)
            Get
                Return MyBase.Path
            End Get
            Set(value As SimpleOrExpression(Of String))
                MyBase.Path = value
            End Set
        End Property








    End Class
End Namespace