using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aricie.DNN.Services;
using DotNetNuke.ComponentModel;
using DotNetNuke.Services.FileSystem;
using FileInfo = DotNetNuke.Services.FileSystem.FileInfo;
using System.Collections;

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

        public override IEnumerable<FileInfo> GetFiles(FolderInfo objfolderInfo)
        {
            return FolderManager.Instance.GetFiles(objfolderInfo).OfType<FileInfo>().ToList();
        }

        public override FileInfo GetFile(FolderInfo objfolderInfo, string filename)
        {
            return (FileInfo) FileManager.Instance.GetFile(objfolderInfo, filename);
        }

        public override DateTime GetFileLastModificationDate(FileInfo objFile)
        {
             if (objFile != null) 
             {
                 return objFile.LastModificationTime;   
             }
             return DateTime.MinValue;
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


        public override Boolean IsIndexingAllowedForModule(DotNetNuke.Entities.Modules.ModuleInfo objModule)
        {
            DotNetNuke.Entities.Tabs.TabController objTabController = new DotNetNuke.Entities.Tabs.TabController();
            Hashtable tabSettings = objTabController.GetTabSettings(objModule.TabID);
            if (!tabSettings.Contains("AllowIndex") || tabSettings["AllowIndex"].ToString().ToLowerInvariant() == "true")
            {
                // Check if indexing is disabled for the current module
                DotNetNuke.Entities.Modules.ModuleController objModuleController = new DotNetNuke.Entities.Modules.ModuleController();
                Hashtable tabModuleSettings = objModuleController.GetTabModuleSettings(objModule.TabModuleID);
                return (!tabModuleSettings.Contains("AllowIndex") || tabModuleSettings["AllowIndex"].ToString().ToLowerInvariant() == "true");
            }
            else
            {
                return false;
            }
        }

    }
}



