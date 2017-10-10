using ShadowverseArtOverlay.Framework;

namespace ShadowverseArtOverlay.RemoteMemoryObjects
{
    public class NonFollowerPanel : RemoteMemoryObject
    {
        public NameLabel NameLabel => ReadObject<NameLabel>(Address + 0xC);
    }
}
