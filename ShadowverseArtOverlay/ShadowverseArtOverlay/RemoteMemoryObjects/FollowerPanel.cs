using ShadowverseArtOverlay.Framework;

namespace ShadowverseArtOverlay.RemoteMemoryObjects
{
    public class FollowerPanel : RemoteMemoryObject
    {
        public NameLabel NameLabel => ReadObject<NameLabel>(Address + 0xC);
    }
}
