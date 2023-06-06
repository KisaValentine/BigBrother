using Dalamud.Game.ClientState.Objects.Types;

namespace BigBrother.Utils
{
    public class Banned
    {
        public string name;
        public string reason;
        public int id;


        public Banned(string name, string reason, int id)
        {
            this.name = name;
            this.reason = reason;
            this.id = id;
        }

        public static unsafe bool IsBanned(Character a)
        {
            return (Memory.PtrToCharacterStruct(a.Address)->DrawData.Flags2 & 1) == 1;
        }
    }
}
