Imports System.Security.Cryptography
Imports Aricie.DNN.UI.Attributes
Imports System.ComponentModel
Imports Aricie.DNN.UI.WebControls.EditControls
Imports DotNetNuke.UI.WebControls

Namespace Security.Cryptography
    ''' <summary>
    ''' Represents the standard parameters for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
    ''' </summary>
    <Serializable> _
    Public Class RSAParametersInfo

        Public Sub New()

        End Sub

        Public Sub New(objParameters As RSAParameters)
            Me._UnderlyingParameters = objParameters
        End Sub

        Public Function ToRSAParameters() As RSAParameters
            Return _UnderlyingParameters
        End Function


        Private _UnderlyingParameters As New RSAParameters

        ''' <summary>
        ''' Represents the Exponent parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property Exponent As String
            Get
                If _UnderlyingParameters.Exponent IsNot Nothing AndAlso _UnderlyingParameters.Exponent.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.Exponent)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.Exponent = Convert.FromBase64String(value)
            End Set
        End Property


        ''' <summary>
        ''' Represents the Modulus parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property Modulus As String
            Get
                If _UnderlyingParameters.Modulus IsNot Nothing AndAlso _UnderlyingParameters.Modulus.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.Modulus)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.Modulus = Convert.FromBase64String(value)
            End Set
        End Property

        ''' <summary>
        ''' Represents the P parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property P As String
            Get
                If _UnderlyingParameters.P IsNot Nothing AndAlso _UnderlyingParameters.P.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.P)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.P = Convert.FromBase64String(value)
            End Set
        End Property

        ''' <summary>
        ''' Represents the Q parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property Q As String
            Get
                If _UnderlyingParameters.Q IsNot Nothing AndAlso _UnderlyingParameters.Q.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.Q)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.Q = Convert.FromBase64String(value)
            End Set
        End Property

        ''' <summary>
        ''' Represents the DP parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property DP As String
            Get
                If _UnderlyingParameters.DP IsNot Nothing AndAlso _UnderlyingParameters.DP.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.DP)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.DP = Convert.FromBase64String(value)
            End Set
        End Property

        ''' <summary>
        ''' Represents the DQ parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property DQ As String
            Get
                If _UnderlyingParameters.DQ IsNot Nothing AndAlso _UnderlyingParameters.DQ.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.DQ)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.DQ = Convert.FromBase64String(value)
            End Set
        End Property

        ''' <summary>
        ''' Represents the InverseQ parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property InverseQ As String
            Get
                If _UnderlyingParameters.InverseQ IsNot Nothing AndAlso _UnderlyingParameters.InverseQ.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.InverseQ)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.InverseQ = Convert.FromBase64String(value)
            End Set
        End Property

        ''' <summary>
        ''' Represents the D parameter for the <see cref="T:System.Security.Cryptography.RSA"/> algorithm.
        ''' </summary>
        <Width(500)> _
     <LineCount(10)> _
  <Editor(GetType(WriteAndReadCustomTextEditControl), GetType(EditControl))> _
        Public Property D As String
            Get
                If _UnderlyingParameters.D IsNot Nothing AndAlso _UnderlyingParameters.D.Length > 0 Then
                    Return Convert.ToBase64String(_UnderlyingParameters.D)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                _UnderlyingParameters.D = Convert.FromBase64String(value)
            End Set
        End Property

    End Class
End Namespace