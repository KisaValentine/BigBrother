using System;
using System.Numerics;
using BigBrother.SeFunctions;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BigBrother.Windows
{

    internal partial class ConfigWindow : Window, IDisposable
    {
        private int monitorRange;
        private PlaySound sounds;


        public ConfigWindow() : base(
            "Configuration Window", ImGuiWindowFlags.MenuBar)
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(375, 330),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            monitorRange = Plugin.Configuration.MonitorRange;
            sounds = new PlaySound(new SigScanner());
        }

        public void Dispose()
        {
            Plugin.Configuration.MonitorRange = monitorRange;
            Plugin.Configuration.Save();
        }

        public override void Draw()
        {
            using var raii = new ImGuiRaii();
            if (!raii.Begin(() => ImGui.BeginTabBar("##tabBar"), ImGui.EndTabBar))
                return;

            DrawConfigUI();
            DrawIgnorePlayerUI();
            DrawBannedPlayersUI();
            DrawReportIssuesUI();
        }
    }
}
