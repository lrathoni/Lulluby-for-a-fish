
#if UNITY_EDITOR

using System.IO;
using UnityEditor.Experimental.AssetImporters;

namespace MptUnity.Utility
{
    // taken from:
    // https://forum.unity.com/threads/loading-a-file-with-a-custom-extension-as-a-textasset.625078/#post-5839426
    [ScriptedImporter(1, "mptm")]
    public class MptmImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            byte[] data = File.ReadAllBytes(ctx.assetPath);
            // add the bytes extension.
            string textPath = Path.ChangeExtension(ctx.assetPath, "mptm.bytes");
            
            File.WriteAllBytes(textPath, data);
        }
    }
}

#endif