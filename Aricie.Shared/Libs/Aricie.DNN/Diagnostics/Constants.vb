Namespace Diagnostics

    ''' <summary>
    ''' Constants for the Diagnostics namespace
    ''' </summary>
    ''' <remarks>Contains many custom debug types</remarks>
    Public Module Constants

        Public PageFlipCategory As String = "Aricie.PageFlip - Debug"

#Region "Aricie.PageFlip - Debug"

        Public PF_State As String = "Step"

        Public PF_Import As String = "Import"
        Public PF_ImportFailed As String = "Import failed"
        Public PF_ImportSuccess As String = "Import Success"

        Public PF_FileFolder As String = "File Folder"
        Public PF_FileName As String = "File Name"

        Public PF_PDFImport As String = "PDF Import"
        Public PF_ZIPImport As String = "ZIP Import"

        Public PF_FilesDeleted As String = "Files Deleted"
        Public PF_FilesDeletedFail As String = "Files Deletion failed"

        Public PF_FolderCreated As String = "Folder Created"
        Public PF_FolderCreationFail As String = "Folder Creation failed"

        Public PF_FlipbookGenerating As String = "Generating FlipBook"
        Public PF_FlipbookGenerationFailed As String = "FlipBook Generation Failed"
        Public PF_FlipbookGenerated As String = "FlipBook Generated"

        Public PF_PpmGenerating As String = "Generating PPM"
        Public PF_PpmGenerated As String = "PPM Generated"
        Public PF_PpmGenerationFailed As String = "PPM Generation Failed"

        Public PF_JpgGeneration As String = "Generating JPG"
        Public PF_JpgGenerated As String = "JPG Generated"
        Public PF_JpgGenerationFailed As String = "JPG Generation Failed"

        Public PF_Error As String = "Error"

#End Region

    End Module

End Namespace