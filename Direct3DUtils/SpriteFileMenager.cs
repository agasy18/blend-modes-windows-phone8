using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Direct3DUtils
{
    
    public partial class Sprite
    {
        public bool EnableImageStoring { get; set; }

        private async void StoreImage(WriteableBitmap bmp, SpriteTextureType spriteTextureType)
        {
            if (!EnableImageStoring)
                return;
            var folderDesc = await FolderOfType(spriteTextureType);
            IRandomAccessStream stream = null;
            DeleteFilesUntilIndex(folderDesc.folder, folderDesc.lastIndex);
            try
            {
                folderDesc.lastIndex++;
                var file = await folderDesc.folder.CreateFileAsync(folderDesc.lastIndex.ToString());
                stream = await file.OpenAsync(FileAccessMode.ReadWrite);

                if (stream.CanWrite)
                {
                    await bmp.WriteBinAsync(stream);
                    await stream.FlushAsync();
                    stream.Dispose();
                    stream = null;
                    return;
                }


            }
            catch
            {

            }
        }



        private async void DeleteFilesUntilIndex(StorageFolder storageFolder, int index)
        {
            try
            {
                var listFile = await storageFolder.GetFilesAsync();
                foreach (var item in listFile)
                {
                    try
                    {
                        var name = int.Parse(item.Name);
                        if (name <= index)
                        {
                            await item.DeleteAsync();
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }



        


        private void ClearCurrentStorage()
        {
           

            
            ClearStorage(SpriteTextureType.Blend);
            ClearStorage(SpriteTextureType.Main);
            ClearStorage(SpriteTextureType.Mask);
        }
        private async void ClearStorage(SpriteTextureType type)
        {
            this.Log("Want to delete",type);
            try
            {
                var t = await FolderOfType(type);
                await t.folder.DeleteAsync();
                folders.Remove(type);
                this.Log("Deleted", type);
            }
            catch(Exception e)
            {
                e.Log("Cant remove",type,this);
            }
        }

        class FolderDesc
        {
            public StorageFolder folder;
            public int lastIndex;
        }

        Dictionary<SpriteTextureType, FolderDesc> folders = new Dictionary<SpriteTextureType, FolderDesc>(3);

        private async Task<FolderDesc> FolderOfType(SpriteTextureType type)
        {
            FolderDesc res;
            if (folders.TryGetValue(type, out res))
                return res;

            var rootThis = await GetThisRoot();
            var typeFolder = await rootThis.CreateFolderAsync(type.ToString(), CreationCollisionOption.OpenIfExists);

            res = new FolderDesc()
            {
                folder = typeFolder,
                lastIndex = 0
            };
            folders.Add(type, res);
            return res;
        }


        StorageFolder thisRoot;

        private async Task<StorageFolder> GetThisRoot()
        {
            if (thisRoot != null)
            {
                return thisRoot;
            }
            var root = await DXMenager.GetRootStorage();
            thisRoot = await root.CreateFolderAsync(id.ToString(), CreationCollisionOption.OpenIfExists);
            return thisRoot;
        }

        internal async Task RestoreTextures()
        {
            this.Log("RestoreTextures");            
            if (!EnableImageStoring)
            {
                return;
            }
            await RestoreTexture(SpriteTextureType.Main);
            await RestoreTexture(SpriteTextureType.Blend);
            await RestoreTexture(SpriteTextureType.Mask);
        }

        public async Task<WriteableBitmap> GetTexture(SpriteTextureType spriteTextureType)
        {
            try
            {
                var folderDesc = await FolderOfType(spriteTextureType);
                int tIndex = folderDesc.lastIndex;
                var file = await folderDesc.folder.GetFileAsync(tIndex.ToString());
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    if (stream.CanRead)
                    {
                        if (stream.Size == 0)
                        {
                            return null;
                        }
                        var bmp = await WritableBitmapBinSave.ReadBinAsync(stream);
                        this.Log("GetTexture ended", spriteTextureType);
                        return bmp;
                    }
                }
            }
            catch (Exception e)
            {
                e.DebugDesc().Log("GetTexture", this);
            }
            return null;
        }


        private async Task RestoreTexture(SpriteTextureType spriteTextureType)
        {
            try
            {
                this.Log("RestoreTexture started", spriteTextureType);
                SetTexture(await GetTexture(spriteTextureType), spriteTextureType);
                this.Log("RestoreTexture ended", spriteTextureType);
            }
            catch { }
                    return;
         
        }

        public override string ToString()
        {
            return "Sprite("+id+")";
        }
    }
    

   

}