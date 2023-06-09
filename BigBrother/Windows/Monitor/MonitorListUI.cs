﻿using BigBrother.Utils;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BigBrother.Windows
{
    internal partial class MonitorWindow
    {
        private void DrawMonitoringListUI()
        {
            if (Plugin.TrackedPlayers is null) return;

            using var raii = new ImGuiRaii();
            var height = ImGui.GetContentRegionAvail().Y;
            height -= ImGui.GetStyle().ItemSpacing.Y;
            var width = ImGui.GetContentRegionAvail().X;

            if (!raii.Begin(() => ImGui.BeginListBox("##MonitorList", new Vector2(width, height)), ImGui.EndListBox))
            {
                return;
            }
            if (Plugin.Configuration.TrackPeople)
            {
                foreach (KeyValuePair<IntPtr, GameObject> entry in Plugin.TrackedPlayers)
                {
                    AddEntry(entry.Value);
                }
            }
        }

        public static bool IsCharacterIgnored(string name)
        =>
            name switch
            {
                "" => false,
                _ => Plugin.Configuration.ignorePlayers.FirstOrDefault(player => player.name.Contains(name)) != null,
            };

        public static bool IsCharacterBanned(string name)
    =>
        name switch
        {
            "" => false,
            _ => Plugin.Configuration.banPlayers.FirstOrDefault(banned => banned.name.Contains(name)) != null,
        };

        private void AddEntry(GameObject? obj, ImGuiSelectableFlags flags = ImGuiSelectableFlags.None)
        {
            if (obj == null) return;
            using var raii = new ImGuiRaii();

            var status = "";
            ImGui.BeginGroup();

                if (Plugin.Configuration.MonitorMinions && obj.ObjectKind is ObjectKind.Companion)
            {
                status += $"{obj.YalmDistanceX} - {obj.YalmDistanceZ} | ";
                status += "M ";
            }

            if (Plugin.Configuration.MonitorWeapons && obj.ObjectKind is ObjectKind.Player)
            {
                status += $"{obj.YalmDistanceX} - {obj.YalmDistanceZ} | ";
                status += "W ";
            } 

            if (Plugin.Configuration.MonitorBanned && IsCharacterBanned(obj.Name.TextValue))
            {
                status += $"{obj.YalmDistanceX} - {obj.YalmDistanceZ} | ";
                status += "B ";
            }

            if (IsCharacterBanned(obj.Name.TextValue) != false)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, obj.ObjectKind == ObjectKind.Player ? _red : _red); 
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, obj.ObjectKind == ObjectKind.Player ? _white : _turquoise);
            }


            ImGui.Selectable(obj.Name.TextValue, false, flags);

            var windowWidth = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
            ImGui.SameLine(windowWidth - ImGui.CalcTextSize(status).X);
            ImGui.TextUnformatted(status);
            ImGui.PopStyleColor();

            ImGui.EndGroup();

            //Thanks https://git.anna.lgbt/ascclemens/PeepingTom/src/branch/main/Peeping%20Tom/PluginUi.cs#L498
            var hovered = ImGui.IsItemHovered(ImGuiHoveredFlags.RectOnly);
            var leftClick = hovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left);

            if (leftClick)
            {
                if (obj.ObjectKind == ObjectKind.Companion)
                {
                    var ownerID = Utils.Companion.GetCompanionOwnerID(obj.Address);
                    obj = Plugin.Objects.SearchById(ownerID);
                }
                Plugin.TargetManager.Target = obj;

            }
        }

        
        private void BuildMonitoringList()
        {
            foreach (var obj in Plugin.Objects)
            {
                if (obj is null) continue;
                if (Plugin.TrackedPlayers.ContainsKey(obj.Address)) continue;
                if (IsCharacterIgnored(obj.Name.TextValue)) continue;
                if (IsCharacterBanned(obj.Name.TextValue)) continue;
                if (Maths.CalculateEuclideanDistance(obj.YalmDistanceX, obj.YalmDistanceZ) > Plugin.Configuration.MonitorRange) continue;
                if (!(obj.ObjectKind == ObjectKind.Player && !Player.IsWeaponHidden((Character)obj)) && !(obj.ObjectKind == ObjectKind.Companion)) continue;
                if (obj.Name.TextValue == "") continue;


                if (Plugin.Configuration.PlaySounds && Plugin.WindowSystem.GetWindow("Monitor")!.IsOpen)
                {
                    if(obj.ObjectKind == ObjectKind.Player)  _sounds.Play(Plugin.Configuration.SoundPlayer);
                    if(obj.ObjectKind == ObjectKind.Companion) _sounds.Play(Plugin.Configuration.SoundMinion);
                }


                PluginLog.Information($"Adding {obj.Name.TextValue}");
                Plugin.TrackedPlayers.Add(obj.Address, obj);
            }
        }

        private static void CleanMonitoringList()
        {
            foreach (KeyValuePair<IntPtr, GameObject> entry in Plugin.TrackedPlayers)
            {
                bool valid = false;
                var gameObject = entry.Value;
                foreach (var obj in Plugin.Objects)
                {
                    double difference = Math.Abs(Plugin.Configuration.MonitorRange + 1);
                     if (IsStillValidTrack(obj.ObjectKind, obj, gameObject) &&
                         (Maths.CalculateEuclideanDistance(obj.YalmDistanceX, obj.YalmDistanceZ) < difference))
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                {
                    PluginLog.Information($"Removing {gameObject.Name.TextValue}");
                    Plugin.TrackedPlayers.Remove(entry.Key);

                }
                
            }
        }

        private static bool IsStillValidTrack(ObjectKind objKind, GameObject gameObject1, GameObject gameObject2)
        =>
            objKind switch
            {

                ObjectKind.Companion => gameObject1.Address == gameObject2.Address,
                ObjectKind.Player => (gameObject1.Address == gameObject2.Address) && !Player.IsWeaponHidden((Character)gameObject2) && !IsCharacterIgnored(gameObject2.Name.TextValue),
                _ => false,
            };
    }
}