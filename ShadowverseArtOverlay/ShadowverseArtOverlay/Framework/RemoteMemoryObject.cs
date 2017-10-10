﻿namespace ShadowverseArtOverlay.Framework
{
    public abstract class RemoteMemoryObject
    {
        public int Address { get; protected set; }
        protected Shadowverse Game { get; set; }
        public Memory M { get; set; }


        public T ReadObjectAt<T>(int offset) where T : RemoteMemoryObject, new()
        {
            return ReadObject<T>(Address + offset);
        }

        public T ReadObject<T>(int addressPointer) where T : RemoteMemoryObject, new()
        {
            var t = new T { M = M, Address = M.ReadInt(addressPointer), Game = Game };
            return t;
        }

        public T GetObjectAt<T>(int offset) where T : RemoteMemoryObject, new()
        {
            return GetObject<T>(Address + offset);
        }

        public T GetObject<T>(int address) where T : RemoteMemoryObject, new()
        {
            var t = new T { M = M, Address = address, Game = Game };
            return t;
        }

        public T AsObject<T>() where T : RemoteMemoryObject, new()
        {
            var t = new T { M = M, Address = Address, Game = Game };
            return t;
        }
    }
}
