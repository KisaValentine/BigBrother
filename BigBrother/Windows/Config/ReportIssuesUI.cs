using Dalamud.Utility;
using ImGuiNET;

namespace BigBrother.Windows
{
    internal partial class ConfigWindow
    {
        private static void DrawReportIssuesUI()
        {
            using var raii = new ImGuiRaii();
            if (!raii.Begin(() => ImGui.BeginTabItem("Report Issues"), ImGui.EndTabItem))
            {
                return;
            }
            if (ImGui.Button("Big Brother Issues"))
            {
                Util.OpenLink("https://github.com/KisaValentine/BigBrother/issues");
            }
        }
    }
}
