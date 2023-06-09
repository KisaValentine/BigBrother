using System;

namespace BigBrother.Utils
{
    internal class Memory
    {
        public static unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.Character* PtrToCharacterStruct(IntPtr charPtr)
        {
            return (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*)charPtr;
        }
    }
}