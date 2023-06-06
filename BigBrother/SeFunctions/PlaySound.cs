using BigBrother.Utils;
using Dalamud.Game;

//Huge thanks to Ottermandias. https://github.com/Ottermandias/GatherBuddy/blob/ddd280a6a642a47daed2cb4f5b5900b80f4bdc50/GatherBuddy/Alarms/Sounds.cs#L3-L23
namespace BigBrother.SeFunctions
{

    public delegate ulong PlaySoundDelegate(int id, ulong unk1, ulong unk2);
    public sealed class PlaySound : SeFunctionBase<PlaySoundDelegate>
    {

        public PlaySound(SigScanner sigScanner)
            : base(sigScanner, "E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??")
        { }

        public void Play(Sounds id)
            => Invoke((int)id, 0ul, 0ul);
    }
}
