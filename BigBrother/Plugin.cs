using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using BigBrother.Windows;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game;
using System.Collections.Generic;
using System;
using Dalamud.Game.ClientState.Objects.Types;

namespace BigBrother
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Big Brother";
        private const string ConfigCommand = "/bb config";
        private const string MonitorCommand = "/bb";
        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public static Configuration Configuration { get; private set; } = null!;
        public static WindowSystem WindowSystem = new("BigBrother");

        public static Dictionary<IntPtr, GameObject> TrackedPlayers = new Dictionary<IntPtr, GameObject>();

        private MonitorWindow monitorWindow;
        private ConfigWindow configWindow;


        [PluginService][RequiredVersion("1.0")] public static ObjectTable Objects { get; private set; } = null!;

        [PluginService][RequiredVersion("1.0")] public static TargetManager TargetManager{ get; private set; } = null!;

        [PluginService][RequiredVersion("1.0")] public static Framework Framework { get; private set; } = null!;



        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(this.PluginInterface);

            monitorWindow = new MonitorWindow();
            configWindow = new ConfigWindow();
            WindowSystem.AddWindow(monitorWindow);
            WindowSystem.AddWindow(configWindow);

            this.CommandManager.AddHandler(ConfigCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens the config window."
            });

            this.CommandManager.AddHandler(MonitorCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens the monitor window."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            
        }

        public void Dispose()
        {
            monitorWindow.Dispose();
            configWindow.Dispose();
            WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(ConfigCommand);
            this.CommandManager.RemoveHandler(MonitorCommand);
        }

        private void OnCommand(string command, string args)
        {
            switch (args) {
               case "":
                    WindowSystem.GetWindow("Monitor")!.IsOpen = true;

                    break;
                case "config":

                    WindowSystem.GetWindow("Configuration Window")!.IsOpen = true;
                    break;
            }
        }

        private void DrawUI()
        {
            WindowSystem.Draw();
        }

        public static void DrawConfigUI()
        {

            WindowSystem.GetWindow("Configuration Window")!.IsOpen = true;

        }
    }
}
