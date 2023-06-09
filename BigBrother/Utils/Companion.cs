using System;

namespace BigBrother.Utils
{
    internal class Companion
    {
        public static unsafe uint GetCompanionOwnerID(IntPtr companion)
        {
            return Memory.PtrToCharacterStruct(companion)->CompanionOwnerID;
        }
    }
}