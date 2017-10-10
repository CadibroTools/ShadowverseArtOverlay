using ShadowverseArtOverlay.Framework;

namespace ShadowverseArtOverlay.RemoteMemoryObjects
{
    /// <summary>
    /// Name Label for a selected card
    /// </summary>
    public class NameLabel : RemoteMemoryObject
    {
        /// <summary>
        /// Name of the selected card
        /// </summary>
        public string Text => M.ReadStringU(M.ReadInt(Address + 0x11C) + 0xC);
        
        /// <summary>
        /// mActiveTTF field, specifies whether the card is actively selected
        /// </summary>
        public bool Active => M.ReadInt(Address + 0x124) > 0;
    }
}
