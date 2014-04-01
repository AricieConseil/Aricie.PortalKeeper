using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aricie.DNN.Services;
using DotNetNuke.ComponentModel;
using DotNetNuke.Services.FileSystem;
using FileInfo = DotNetNuke.Services.FileSystem.FileInfo;

namespace Aricie.DNN
{
    public class DNN7ObsoleteDNNProvider : Aricie.DNN.Services.ObsoleteDNNProvider
    {

        public override FolderInfo GetFolderFromPath(int portalId, string path)
        {
            return FolderManager.Instance.GetFolder(portalId, path) as FolderInfo;
        }

        public override void AddFolder(FolderInfo objFolderInfo)
        {
            FolderManager.Instance.AddFolder(FolderMappingController.Instance.GetFolderMapping(objFolderInfo.PortalID,
                objFolderInfo.FolderMappingID), objFolderInfo.FolderPath);

        }

        public override IEnumerable<FileInfo> GetFiles(FolderInfo folderInfo)
        {
            return FolderManager.Instance.GetFiles(folderInfo).OfType<FileInfo>().ToList();
        }


        public override int AddOrUpdateFile(FileInfo objFile, byte[] content, bool contentOnly)
        {
            var objFolder = FolderManager.Instance.GetFolder(objFile.FolderId);
            if (objFolder != null)
            {
                if (content != null && content.Length > 0)
                {
                    using (var objStream = new MemoryStream(content))
                    {
                        if (objFile.FileId > 0)
                        {
                            var toReturn = objFile.FileId;
                            if (!contentOnly)
                            {
                                toReturn = FileManager.Instance.UpdateFile(objFile, objStream).FileId;
                            }
                            //Todo: seems the updatefile does not have the proper call to that function. Should be registered as a bug.
                            FolderProvider folderProvider =
                                FolderProvider.Instance(
                                    ComponentBase<IFolderMappingController, FolderMappingController>.Instance
                                        .GetFolderMapping(objFolder.PortalID, objFolder.FolderMappingID)
                                        .FolderProviderType);
                            folderProvider.UpdateFile(objFolder, objFile.FileName, objStream);
                            return toReturn;
                        }
                        else
                        {
                            return FileManager.Instance.AddFile(objFolder, objFile.FileName, objStream).FileId;
                        }
                    }
                }
                else
                {
                    if (objFile.FileId > 0)
                    {
                        var toReturn = objFile.FileId;
                        if (!contentOnly)
                        {
                            toReturn = FileManager.Instance.UpdateFile(objFile).FileId;
                        }
                        return toReturn;
                    }
                    else
                    {
                        return FileManager.Instance.AddFile(objFolder, objFile.FileName, null).FileId;
                    }
                }
                
            }
            return -1;
        }


        public override byte[] GetFileContent(FileInfo objFileInfo)
        {
            using (Stream objStream =  FileManager.Instance.GetFileContent(objFileInfo))
            {
                return Common.ReadStream(objStream);
            }
        }

        public override void ClearFileContent(FileInfo objFileInfo)
        {
            DatabaseFolderProvider.ClearFileContent(objFileInfo.FileId);
        }

      
    }
}
