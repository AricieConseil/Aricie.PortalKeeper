Imports Aricie.DNN.UI.Attributes
Imports Aricie.DNN.Services.Flee
Imports System.Globalization
Imports System.Threading

Namespace Services.Filtering
    
    Public Class ComparerInfo

        Private _Comparer As StringComparer

        Public Property Comparison As StringComparison = StringComparison.OrdinalIgnoreCase

        <ConditionalVisible("Comparison", False, True, StringComparison.CurrentCulture, StringComparison.CurrentCultureIgnoreCase)> _
        Public Property CultureMode As CultureInfoMode = CultureInfoMode.Current

        <ConditionalVisible("Comparison", False, True, StringComparison.CurrentCulture, StringComparison.CurrentCultureIgnoreCase)> _
        <ConditionalVisible("CultureMode", False, True, CultureInfoMode.Custom)> _
        Public Property CustomCultureLocale() As String = CultureInfo.CurrentCulture.Name

        Public Function GetComparer() As StringComparer
            If _Comparer Is Nothing Then
                Select Case Me.Comparison
                    Case StringComparison.OrdinalIgnoreCase
                        _Comparer = StringComparer.OrdinalIgnoreCase
                    Case StringComparison.Ordinal
                        _Comparer = StringComparer.Ordinal
                    Case StringComparison.InvariantCulture
                        _Comparer = StringComparer.InvariantCulture
                    Case StringComparison.InvariantCultureIgnoreCase
                        _Comparer = StringComparer.InvariantCultureIgnoreCase
                    Case Else
                        Dim objCulture As CultureInfo = Nothing
                        Select Case Me.CultureMode
                            Case CultureInfoMode.Invariant
                                objCulture = CultureInfo.InvariantCulture
                            Case CultureInfoMode.Current
                                objCulture = CultureInfo.CurrentCulture ' DirectCast(Thread.CurrentThread.CurrentCulture.Clone, CultureInfo)
                            Case CultureInfoMode.CurrentUI
                                objCulture = CultureInfo.CurrentUICulture  'DirectCast(Thread.CurrentThread.CurrentUICulture.Clone, CultureInfo)
                            Case CultureInfoMode.Custom
                                objCulture = New CultureInfo(CustomCultureLocale)
                        End Select
                        _Comparer = StringComparer.Create(objCulture, Me.Comparison = StringComparison.CurrentCultureIgnoreCase)
                End Select
            End If
            Return _Comparer
        End Function


    End Class
End NameSpace