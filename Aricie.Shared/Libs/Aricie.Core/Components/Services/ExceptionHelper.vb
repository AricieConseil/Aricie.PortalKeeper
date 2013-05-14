Imports Aricie.Providers

Namespace Services

    Public Module ExceptionHelper

        Public Sub LogException(ex As Exception)
            SystemServiceProvider.Instance().LogException(ex)
        End Sub


    End Module

End Namespace

