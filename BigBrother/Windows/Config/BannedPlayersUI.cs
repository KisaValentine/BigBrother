using BigBrother.Utils;
using ImGuiNET;

namespace BigBrother.Windows
{
    internal partial class ConfigWindow
    {
        private byte[] banbuffer = new byte[256];
        private byte[] banreason = new byte[256];
        private void DrawBannedPlayersUI()
        {
            using var raii = new ImGuiRaii();

            if (!raii.Begin(() => ImGui.BeginTabItem("Banned List"), ImGui.EndTabItem))
            {
                return;
            }
            ImGui.Text("Add player to the banned list");
            ImGui.InputText("##input", banbuffer, (uint)banbuffer.Length);
            ImGui.InputText("##reason", banreason, (uint)banreason.Length);
            ImGui.SameLine();
            if (ImGui.Button("+"))
            {
                var banned = new Banned(System.Text.Encoding.UTF8.GetString(banbuffer, 0, banbuffer.Length), System.Text.Encoding.UTF8.GetString(banreason, 0, banreason.Length), ++Plugin.Configuration.bannedPlayersNumber);
                Plugin.Configuration.banPlayers.Add(banned);
                Plugin.Configuration.Save();
                for (int i = 0; i < banbuffer.Length; i++)
                {
                    banbuffer[i] = 0;
                }
            }
            ImGui.Separator();

            raii.Begin(() => ImGui.BeginTable("##BannedPlayersTable", 3), ImGui.EndTable);

            ImGui.TableSetupColumn("##Delete", ImGuiTableColumnFlags.WidthFixed, 30);
            ImGui.TableSetupColumn("Player name", ImGuiTableColumnFlags.WidthStretch, ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X);
            ImGui.TableSetupColumn("Reason", ImGuiTableColumnFlags.WidthStretch, ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X);
            ImGui.TableHeadersRow();

            for (var i = 0; i < Plugin.Configuration.banPlayers.Count; i++)
            {
                var banned = (Banned)Plugin.Configuration.banPlayers[i];
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Button($"DEL##{i}"))
                {
                    Plugin.Configuration.banPlayers.RemoveAt(i--);
                    Plugin.Configuration.bannedPlayersNumber -= 1;
                    Plugin.Configuration.Save();
                    continue;
                }
                ImGui.TableNextColumn();
                ImGui.Text(banned.name);
                ImGui.TableNextColumn();
                ImGui.Text(banned.reason);
            }
        }
    }
}