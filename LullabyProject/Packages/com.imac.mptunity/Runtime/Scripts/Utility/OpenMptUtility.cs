
using MptUnity.Audio;

namespace MptUnity.Utility
{
    public static class OpenMptUtility
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The module extension on success, null on failure</returns>
        public static OpenMpt.ModuleExt LoadModuleExt(byte[] data)
        {
            try
            {
                var moduleExt = new OpenMpt.ModuleExt(data);
                // set render parameters
                // Volume ramping to avoid clicking.
                moduleExt.GetModule().SetRenderParam(
                    OpenMpt.Module.RenderParam.eRenderVolumeRampingStrength,
                    MusicConfig.c_renderVolumeRampingStrength
                );
                return moduleExt;
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }
    
        public static string GetModuleExtMessage(OpenMpt.ModuleExt moduleExt)
        {
            return moduleExt.GetModule().GetMetadata(OpenMpt.Module.c_keyMessage);
        }
        
        public static string GetModuleExtAuthor(OpenMpt.ModuleExt moduleExt)
        {
            return moduleExt.GetModule().GetMetadata(OpenMpt.Module.c_keyAuthor);
        }
        
        public static string GetModuleExtTitle(OpenMpt.ModuleExt moduleExt)
        {
            return moduleExt.GetModule().GetMetadata(OpenMpt.Module.c_keyTitle);
        }

    }
}