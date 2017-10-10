using ShadowverseArtOverlay.Framework;

namespace ShadowverseArtOverlay.RemoteMemoryObjects
{
    public class DetailMgr : RemoteMemoryObject
    {
        public DetailPanelControl DetailPanelControl => ReadObject<DetailPanelControl>(Address + 0x50);
    }
}
