Imports System.Xml.Serialization
Imports System.ComponentModel

Namespace Aricie.DNN.Modules.PortalKeeper
    Public Class SimpleRuleEngine
        Inherits RuleEngineSettings(Of SimpleEngineEvent)



        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Mode As RuleEngineMode
            Get
                Return RuleEngineMode.Actions
            End Get
            Set(value As RuleEngineMode)
                'do nothing
            End Set
        End Property

        <XmlIgnore()> _
        <Browsable(False)> _
        Public Overrides Property Rules As List(Of KeeperRule(Of SimpleEngineEvent))
            Get
                Return Nothing
            End Get
            Set(value As List(Of KeeperRule(Of SimpleEngineEvent)))
                'do nothing
            End Set
        End Property


    End Class
End NameSpace