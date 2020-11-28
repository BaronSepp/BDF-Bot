using System.Collections.Generic;
using Newtonsoft.Json;

namespace BDF.Lib.Entities
{
    public class McQuery
    {
        [JsonProperty("online")]
        public bool Online { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("debug")]
        public Debug Debug { get; set; }

        [JsonProperty("motd")]
        public Motd Motd { get; set; }

        [JsonProperty("players")]
        public Players Players { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("protocol")]
        public int Protocol { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("software")]
        public string Software { get; set; }

        [JsonProperty("map")]
        public string Map { get; set; }

        [JsonProperty("plugins")]
        public Plugins Plugins { get; set; }

        [JsonProperty("mods")]
        public Mods Mods { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
    }

    public class Debug
    {
        [JsonProperty("ping")]
        public bool Ping { get; set; }

        [JsonProperty("query")]
        public bool Query { get; set; }

        [JsonProperty("srv")]
        public bool Srv { get; set; }

        [JsonProperty("querymismatch")]
        public bool Querymismatch { get; set; }

        [JsonProperty("ipinsrv")]
        public bool Ipinsrv { get; set; }

        [JsonProperty("cnameinsrv")]
        public bool Cnameinsrv { get; set; }

        [JsonProperty("animatedmotd")]
        public bool Animatedmotd { get; set; }

        [JsonProperty("cachetime")]
        public int Cachetime { get; set; }
    }

    public class Motd
    {
        [JsonProperty("raw")]
        public IList<string> Raw { get; set; }

        [JsonProperty("clean")]
        public IList<string> Clean { get; set; }

        [JsonProperty("html")]
        public IList<string> Html { get; set; }
    }

    public class Uuid
    {
        [JsonProperty("Spirit55555")]
        public string Spirit55555 { get; set; }

        [JsonProperty("sarsum33")]
        public string Sarsum33 { get; set; }
    }

    public class Players
    {

        [JsonProperty("online")]
        public int Online { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonProperty("list")]
        public IList<string> List { get; set; }

        [JsonProperty("uuid")]
        public Uuid Uuid { get; set; }
    }

    public class Plugins
    {
        [JsonProperty("names")]
        public IList<string> Names { get; set; }

        [JsonProperty("raw")]
        public IList<string> Raw { get; set; }
    }

    public class Mods
    {
        [JsonProperty("names")]
        public IList<string> Names { get; set; }

        [JsonProperty("raw")]
        public IList<string> Raw { get; set; }
    }

    public class Info
    {
        [JsonProperty("raw")]
        public IList<string> Raw { get; set; }

        [JsonProperty("clean")]
        public IList<string> Clean { get; set; }

        [JsonProperty("html")]
        public IList<string> Html { get; set; }
    }
}