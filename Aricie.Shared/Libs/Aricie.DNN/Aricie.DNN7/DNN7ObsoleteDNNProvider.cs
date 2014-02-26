using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aricie.DNN.Services;
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

        public override int AddOrUpdateFile(FileInfo objFile, byte[] content)
        {
            var objFolder = FolderManager.Instance.GetFolder(objFile.FolderId);
            if (objFolder != null)
            {
                using (var objStream = new MemoryStream(content))
                {
                    if (objFile.FileId > 0)
                    {
                        return FileManager.Instance.UpdateFile( objFile, objStream).FileId;
                    }
                    else
                    {
                        return FileManager.Instance.AddFile(objFolder, objFile.FileName, objStream).FileId;    
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
