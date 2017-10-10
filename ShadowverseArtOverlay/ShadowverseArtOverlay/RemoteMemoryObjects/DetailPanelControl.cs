using ShadowverseArtOverlay.Framework;

namespace ShadowverseArtOverlay.RemoteMemoryObjects
{
    /// <summary>
    /// The Detail Panel Control
    /// </summary>
    public class DetailPanelControl : RemoteMemoryObject
    {
        public FollowerPanel FollowerPanel => ReadObject<FollowerPanel>(Address + 0xB0);
        public FollowerEvoPanel FollowerEvoPanel => ReadObject<FollowerEvoPanel>(Address + 0xB4);
        public NonFollowerPanel NonFollowerPanel => ReadObject<NonFollowerPanel>(Address + 0xB8);
    }
}
