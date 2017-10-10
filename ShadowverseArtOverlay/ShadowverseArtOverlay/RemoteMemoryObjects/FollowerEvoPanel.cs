using ShadowverseArtOverlay.Framework;

namespace ShadowverseArtOverlay.RemoteMemoryObjects
{
    public class FollowerEvoPanel : RemoteMemoryObject
    {
        public NameLabel NameLabel => ReadObject<NameLabel>(Address + 0xC);
    }
}
