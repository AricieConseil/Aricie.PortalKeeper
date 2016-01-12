Imports System.Web
Imports Aricie.Services
Imports Aricie.Collections
Imports System.Xml.Serialization

Namespace Services

    
    ''' <summary>
    ''' Base class for context
    ''' </summary>
    ''' <typeparam name="TContext"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class ContextBase(Of TContext As {New, IContext(Of TContext)})
        Implements IContext(Of TContext)
        Implements IContextLookup
        Implements IServiceProvider
        Implements IDisposable
        Implements IModuleContext
        Implements IContextOwnerProvider




        Private _DnnContext As DnnContext
        Private _Items As New SerializableDictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        Private _Services As New Dictionary(Of Type, Object)

        Private Shared _GlobalInstance As TContext
        Private _FlowId As String

        ''' <summary>
        ''' Gets or sets the current DnnContext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        Public Property DnnContext() As DnnContext Implements IModuleContext.DnnContext
            Get
                If _DnnContext Is Nothing Then
                    _DnnContext = DnnContext.Current
                End If
                Return _DnnContext
            End Get
            Set(ByVal value As DnnContext)
                _DnnContext = value
            End Set
        End Property

        ''' <summary>
        ''' Retrieve instance of context
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetInstance() As TContext Implements IContext(Of TContext).GetInstance

        ''' <summary>
        ''' Gets singleton instance
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Instance() As TContext
            Get
                Return GlobalInstance.GetInstance
            End Get
        End Property

        Public Shared ReadOnly Property GlobalInstance As TContext
            Get
                If _GlobalInstance Is Nothing Then
                    _GlobalInstance = Activator.CreateInstance(Of TContext)()
                End If
                Return _GlobalInstance
            End Get
        End Property

        ''' <summary>
        ''' Gets singleton instance from HttpContext
        ''' </summary>
        ''' <param name="context"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Instance(ByVal context As HttpContext) As TContext
            Get
                Return DnnContext.Current(context).GetService(Of TContext)()
            End Get
        End Property


        ''' <summary>
        ''' Gets flow identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FlowId() As String
            Get
                If String.IsNullOrEmpty(_FlowId) Then
                    _FlowId = Guid.NewGuid.ToString()
                End If
                Return _FlowId
            End Get
        End Property

        ''' <summary>
        ''' Returns value from inner dictionary
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetVar(ByVal key As String) As Object
            Return Me.Item(key)
        End Function

        ''' <summary>
        ''' Sets value in inner dictionary
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Sub SetVar(ByVal key As String, ByVal value As Object)
            SyncLock Me._Items
                Me._Items(key) = value
            End SyncLock
        End Sub

        ''' <summary>
        ''' Serts multiple values in inner dictionary
        ''' </summary>
        ''' <param name="items"></param>
        ''' <remarks></remarks>
        Public Sub SetVars(ByVal items As IDictionary(Of String, Object))
            SyncLock Me._Items
                For Each objItem As KeyValuePair(Of String, Object) In items
                    Me._Items(objItem.Key) = objItem.Value
                Next
            End SyncLock
        End Sub

        ''' <summary>
        ''' Return value from inner dictionary
        ''' </summary>
        ''' <param name="key"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Item(ByVal key As String) As Object
            Get
                Dim toReturn As Object = Nothing
                SyncLock Me._Items
                    Me._Items.TryGetValue(key, toReturn)
                End SyncLock
                Return toReturn
            End Get
            Set(ByVal value As Object)
                SyncLock Me._Items
                    Me._Items(key) = value
                End SyncLock
            End Set
        End Property

        ''' <summary>
        ''' Returns inner item dictionary
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Items() As System.Collections.Generic.IDictionary(Of String, Object) Implements IContextLookup.Items
            Get
                Return _Items
            End Get
        End Property

        
        Public Property ContextOwner As Object Implements IContextOwnerProvider.ContextOwner
            Get
                Return Me
            End Get
            Set(value As Object)
                'can't change the context owner here
            End Set
        End Property

       

#Region "IServiceProvider"

        ''' <summary>
        ''' Returns inner item from dictionary using key built from parameters
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="params"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetItem(Of T)(ByVal ParamArray params() As String) As T
            Dim key As String = Constants.GetKey(Of T)(params)
            Return DirectCast(Me.Item(key), T)
        End Function

        ''' <summary>
        ''' Sets inner item from dictionary using key built from parameters
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="item"></param>
        ''' <param name="params"></param>
        ''' <remarks></remarks>
        Public Sub SetItem(Of T)(ByVal item As T, ByVal ParamArray params() As String)
            Dim key As String = Constants.GetKey(Of T)(params)
            Me.Item(key) = item
        End Sub

        ''' <summary>
        ''' returns instance of type
        ''' </summary>
        ''' <param name="serviceType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetService(ByVal serviceType As Type) As Object Implements IServiceProvider.GetService
            Dim toReturn As Object = Nothing
            If Not Me._Services.TryGetValue(serviceType, toReturn) Then
                toReturn = CreateService(serviceType)
                Me._Services(serviceType) = toReturn
            End If
            Return toReturn
        End Function

        ''' <summary>
        ''' returns instance of type
        ''' </summary>
        ''' <param name="serviceType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function CreateService(ByVal serviceType As Type) As Object
            Return ReflectionHelper.CreateObject(serviceType)
        End Function

        ''' <summary>
        ''' returns instance of type
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetService(Of T)() As T
            Return DirectCast(Me.GetService(GetType(T)), T)
        End Function

        ''' <summary>
        ''' returns instance of type
        ''' </summary>
        ''' <param name="typeName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetService(ByVal typeName As String) As Object
            Dim objType As Type = ReflectionHelper.CreateType(typeName)
            Return Me.GetService(objType)
        End Function

#End Region



#Region " IDisposable Support "

        Private disposedValue As Boolean = False        ' To detect redundant calls
        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then

                    For Each objItem As Object In Me._Items.Values
                        If TypeOf objItem Is IDisposable Then
                            DirectCast(objItem, IDisposable).Dispose()
                        End If
                    Next

                    For Each objService As Object In Me._Services.Values
                        If TypeOf objService Is IDisposable Then
                            DirectCast(objService, IDisposable).Dispose()
                        End If
                    Next

                End If

            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region





    End Class


    <Obsolete("Use ContextBase instead")> _
    Public Class BaseContext(Of T As IModuleContext)
        Implements IModuleContext
        Implements IContextItems



        Private _Items As New Dictionary(Of String, Object)(StringComparison.InvariantCultureIgnoreCase)


        Private _DnnContext As DnnContext


        Private _FlowId As String = Guid.NewGuid.ToString()

        Public Property DnnContext() As DnnContext Implements IModuleContext.DnnContext
            Get
                Return _DnnContext
            End Get
            Set(ByVal value As DnnContext)
                _DnnContext = value
            End Set
        End Property



        Public Shared ReadOnly Property Instance() As T
            Get

                Return DnnContext.Current().GetService(Of T)()
            End Get
        End Property

        Public Shared ReadOnly Property Instance(ByVal context As HttpContext) As T
            Get

                Return DnnContext.Current(context).GetService(Of T)()

            End Get
        End Property


        Public ReadOnly Property FlowId() As String
            Get
                Return _FlowId
            End Get
        End Property



        Public Property Items() As System.Collections.Generic.Dictionary(Of String, Object) Implements IContextItems.Items
            Get
                Return _Items
            End Get
            Set(ByVal value As System.Collections.Generic.Dictionary(Of String, Object))
                _Items = value
            End Set
        End Property

    End Class

End Namespace