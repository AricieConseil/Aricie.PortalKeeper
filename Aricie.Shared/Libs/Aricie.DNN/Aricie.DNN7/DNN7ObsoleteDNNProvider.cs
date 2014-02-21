using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            
        }

        public override byte[] GetFileContent(FileInfo objFileInfo)
        {
            using (Stream objStream =  FileManager.Instance.GetFileContent(objFileInfo))
            {
                return Common.ReadStream(objStream);
            }
        }

        public override void UpdateFileContent(FileInfo objFileInfo, byte[] content)
        {
            DatabaseFolderProvider.UpdateFileContent(objFileInfo.FileId, content);
        }

        public override void ClearFileContent(FileInfo objFileInfo)
        {
            DatabaseFolderProvider.ClearFileContent(objFileInfo.FileId);
        }

        public override IEnumerable<FileInfo> GetFiles(FolderInfo folderInfo)
        {
            return FolderManager.Instance.GetFiles(folderInfo).OfType<FileInfo>().ToList();
        }
    }
}
