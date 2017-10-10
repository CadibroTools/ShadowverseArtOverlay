using ShadowverseArtOverlay.Framework;

namespace ShadowverseArtOverlay.RemoteMemoryObjects
{
    /// <summary>
    /// Battle Manager Singleton
    /// </summary>
    public class SingleBattleMgr : RemoteMemoryObject
    {
        public DetailMgr DetailManager => ReadObject<DetailMgr>(Address + 0x38);
    }
}
