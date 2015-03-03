Imports System.ComponentModel
Imports Aricie.DNN.Services

Namespace Aricie.DNN.Modules.PortalKeeper

    Public Enum ScriptMode
        ScriptFile
        Script
        Command
    End Enum


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

