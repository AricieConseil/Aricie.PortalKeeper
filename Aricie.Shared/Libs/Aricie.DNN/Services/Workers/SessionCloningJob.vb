Imports System.Web.SessionState
Imports System.Xml
Imports Aricie.Services

Namespace Services.Workers
    Public Structure SessionCloningJob(Of T)

        Public Sub New(objSession As HttpSessionState, objToClone As T, strKey As String)
            Session = objSession
            CloneObject = objToClone
            Key = strKey
        End Sub

        Private Key As String
        Private Session As HttpSessionState
        Private CloneObject As T

        Public Sub CloneInSession()
            Session(Key) = ReflectionHelper.CloneObject(Of T)(CloneObject)
            If Not _SerializationWarmedUp Then
                _SerializationWarmedUp = True
                Dim dumbXml As XmlDocument = ReflectionHelper.Serialize(CloneObject)
            End If
        End Sub

        Public Sub Enqueue()
            _SessionCloningQueue.EnqueueTask(Me)
        End Sub

        Private Shared _SerializationWarmedUp As Boolean

        Private Shared _SessionCloningQueue As New TaskQueue(Of SessionCloningJob(Of T))(New Action(Of SessionCloningJob(Of T))( _
            Sub(objCloningBag As SessionCloningJob(Of T)) objCloningBag.CloneInSession()) _
                                                                                         , New TaskQueueInfo(5, True, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero))

    End Structure
End NameSpace