using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipX.Models
{
    public class SettingsModel
    {
        public GeneralSettings GeneralSettings { get; set; }
        public DiscordSettings DiscordSettings { get; set; }
        public TwitchSettings TwitchSettings { get; set; }
    }

    public class GeneralSettings
    {
        public int DelayTimeAsMs { get; set; }
        public string RedisEndpoint { get; set; }
    }

    public class DiscordSettings
    {
        public string ApplicationId { get; set; }
        public string PublicKey { get; set; }
        public string SecretToken { get; set; }
        public string PermissionNumber { get; set; }
        public ulong ChannelId { get; set; }


    }

    public class TwitchSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ChannelName { get; set; }
    }
}
