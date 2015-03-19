Imports System.ComponentModel
Imports Aricie.DNN.Services
Imports Aricie.DNN.Entities
Imports Aricie.DNN.UI.Attributes

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum ScriptMode
        ScriptFile
        Script
        Command
    End Enum

    <Serializable()> _
    Public Class ExtendedHtmlScraps
        Inherits HtmlPageScrapsInfo(Of ExtendedHtmlScrap)

        <ExtendedCategory("Advanced")> _
        Public Property Python As New EnabledFeature(Of IronPython)

    End Class

    <Serializable()> _
    Public Class ExtendedHtmlScrap
        Inherits HtmlPageScrapInfo

        <ExtendedCategory("Custom")> _
        Public Property Python As New EnabledFeature(Of IronPython)

    End Class

    <Serializable()> _
    Public Class IronPython
        Inherits ScriptBase

        Private _ScriptHost As IronPythonScriptHost

        <Browsable(False)> _
        Public ReadOnly Property ScriptHost As IronPythonScriptHost
            Get
                If _ScriptHost Is Nothing Then
                    SyncLock Me
                        _ScriptHost = New IronPythonScriptHost()
                    End SyncLock
                End If
                Return _ScriptHost
            End Get
        End Property


        Public Overrides Function Run(ByVal input As String, lookup As IContextLookup) As String
            ScriptHost.ClearOutput()
            For Each objVar As KeyValuePair(Of String, Object) In lookup.Items
                ScriptHost.RegisterVariable(objVar.Key, objVar.Value)
            Next
            ScriptHost.Execute(input)
            Return ScriptHost.GetOutput()
        End Function


    End Class

End Namespace

