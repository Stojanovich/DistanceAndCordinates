using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DistancePlugin
{
    public class DistancePlugin : RocketPlugin<DistancePluginConfiguration>
    {
        public static DistancePlugin Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;
            Say(null, "Distance Plugin loaded successfully!");
        }

        protected override void Unload()
        {
            Say(null, "Distance Plugin unloaded!");
        }

        public void Say(UnturnedPlayer player, string message)
        {
            if (string.IsNullOrEmpty(Configuration.Instance.MessageIcon))
            {
                if (player == null)
                    UnturnedChat.Say(message, UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.yellow));
                else
                    UnturnedChat.Say(player, message, UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.yellow));
            }
            else
            {
                ChatManager.serverSendMessage(message,
                    UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.yellow),
                    null,
                    player?.Player.channel.owner,
                    EChatMode.SAY,
                    Configuration.Instance.MessageIcon,
                    true);
            }
        }
    }

    public class DistancePluginConfiguration : IRocketPluginConfiguration
    {
        public string MessageColor { get; set; }
        public string MessageIcon { get; set; }

        public void LoadDefaults()
        {
            MessageColor = "yellow";
            MessageIcon = "https://i.imgur.com/JZjQEHV.png";
        }
    }

    public class DistanceCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "distance";
        public string Help => "Shows distance to where you're looking";
        public string Syntax => "/distance";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = caller as UnturnedPlayer;
            if (player == null) return;

            RaycastInfo ray = DamageTool.raycast(new Ray(player.Player.look.aim.position, player.Player.look.aim.forward), 1000f, RayMasks.DAMAGE_CLIENT);
            if (ray != null)
            {
                float distance = Vector3.Distance(player.Position, ray.point);
                DistancePlugin.Instance.Say(player, $"Distance: {distance:F2} meters");
            }
            else
            {
                DistancePlugin.Instance.Say(player, "No object in range!");
            }
        }
    }

    public class CoordinatesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "coordinates";
        public string Help => "Shows your current coordinates";
        public string Syntax => "/coordinates";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = caller as UnturnedPlayer;
            if (player == null) return;

            Vector3 position = player.Position;
            DistancePlugin.Instance.Say(player, $"Your coordinates: X: {position.x:F2}, Y: {position.y:F2}, Z: {position.z:F2}");
        }
    }
}