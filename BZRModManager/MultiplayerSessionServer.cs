using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager
{
    class MultiplayerSessionServer
    {
        private const string BZ98R = "bigboat:battlezone_98_redux";
        private const string BZCC = "bigboat:battlezone_combat_commander";

        public static MultiplayerGamelistData GetMpGamesBZ98R() { return GetMpGames(BZ98R); }
        public static MultiplayerGamelistData GetMpGamesBZCC() { return GetMpGames(BZCC); }
        public static MultiplayerGamelistData GetMpGames(string GameID)
        {
            WebClient client = new WebClient();
            string GameListJson = client.DownloadString($"http://multiplayersessionlist.iondriver.com/api/1.0/sessions?game={GameID}");
            JObject data = JObject.Parse(GameListJson);

            if (data["DataCache"] != null && data["Heroes"] != null)
            {
                JObject p1 = data["Heroes"].Value<JObject>();
                JObject p2 = data["DataCache"].Value<JObject>();
                RichDataCheck(ref p1, ref p2);
            }
            if (data["Sessions"] != null)
            {
                var SessionArray = data["Sessions"].Value<JArray>();
                for (var i = 0; i < SessionArray.Count; i++)
                {
                    if (data["SessionDefault"] != null)
                    {
                        JObject Session = (JObject)(data["SessionDefault"].Value<JObject>().DeepClone());
                        Session.Merge((JObject)(SessionArray[i]));
                        SessionArray[i] = Session;
                    }

                    if (data["DataCache"] != null)
                    {
                        JObject p1 = (JObject)(SessionArray[i]);
                        JObject p2 = (JObject)(data["DataCache"]);
                        RichDataCheck(ref p1, ref p2);
                    }
                }
            }

            //JsonSerializer tmpSer = new JsonSerializer();
            //tmpSer.Converters.Add(new MultiplayerGamelistDataConverter());
            return data.ToObject<MultiplayerGamelistData>();// (tmpSer);
        }

        private static void RichDataCheck(ref JObject data, ref JObject rich)
        {
            var keys = data.Properties().Select(dr => dr.Name).ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                if (rich[keys[i]] != null && (data[keys[i]].Type == JTokenType.Object || data[keys[i]].Type == JTokenType.Array) && data[keys[i]] != null)
                {
                    JToken dat = data[keys[i]];
                    bool wasNotArray = false;
                    if (dat.Type != JTokenType.Array)
                    {
                        dat = new JArray() { dat };
                        wasNotArray = true;
                    }
                    JArray datArr = (JArray)dat;
                    if (datArr.Count > 0 && datArr[0].Type == JTokenType.Object)
                        for (var j = 0; j < datArr.Count; j++)
                        {
                            JObject p1 = (JObject)(datArr[j]);
                            JObject p2 = (JObject)(rich[keys[i]]);
                            RichDataCheck(ref p1, ref p2);
                            datArr[j] = p1;
                        }
                    data[keys[i]] = wasNotArray ? datArr[0] : datArr;
                }
            }
            if (data["ID"] != null && data["ID"].Type == JTokenType.String && rich[data["ID"].Value<string>()] != null && rich[data["ID"].Value<string>()].Type == JTokenType.Object)
            {
                var keys2 = ((JObject)(rich[data["ID"].Value<string>()])).Properties().Select(dr => dr.Name).ToList();
                for (var i = 0; i < keys2.Count; i++)
                {
                    if (data[keys2[i]] == null)
                    {
                        data[keys2[i]] = rich[data["ID"].Value<string>()][keys2[i]];
                    }
                }
            }
            //console.log('-------------');
        }
    }

    public class MultiplayerGamelistData
    {
        public MultiplayerGamelistData_Metadata Metadata { get; set; }
        public Dictionary<string, MultiplayerGamelistData_Hero> Heroes { get; set; }
        public Dictionary<string, MultiplayerGamelistData_Mod> Mods { get; set; }
        public List<MultiplayerGamelistData_Session> Sessions { get; set; }

        public bool? EndpointExpired { get; set; }
    }

    public class MultiplayerGamelistData_Metadata
    {
        public Dictionary<string, MultiplayerGamelistData_Metadata_ListServer> ListServer { get; set; }
    }
    public class MultiplayerGamelistData_Metadata_ListServer
    {
        public string Status { get; set; }
        public bool? Success { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class MultiplayerGamelistData_Hero
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }

    public class MultiplayerGamelistData_Mod
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Url { get; set; }
    }

    public class MultiplayerGamelistData_Session
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Address { get; set; }
        public List<MultiplayerGamelistData_Session_PlayerTypes> PlayerTypes { get; set; }
        public Dictionary<string, int> PlayerCount { get; set; }
        public MultiplayerGamelistData_Session_Level Level { get; set; }
        public MultiplayerGamelistData_Session_Status Status { get; set; }
        public List<MultiplayerGamelistData_Session_Player> Players { get; set; }
        public MultiplayerGamelistData_Session_Game Game { get; set; }
        public MultiplayerGamelistData_Session_Time Time { get; set; }
        public MultiplayerGamelistData_Session_Attributes Attributes { get; set; }
    }

    public class MultiplayerGamelistData_Session_PlayerTypes
    {
        public List<string> Types { get; set; }
        public int? Max { get; set; }
    }

    public class MultiplayerGamelistData_Session_Level
    {
        public string ID { get; set; }
        public string MapFile { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string CRC32 { get; set; }
        public string GameType { get; set; }
        public string GameMode { get; set; }
        public string Mod { get; set; }
        public MultiplayerGamelistData_Session_Level_Attributes Attributes { get; set; }
    }

    public class MultiplayerGamelistData_Session_Level_Attributes
    {
        public int? TimeLimit { get; set; }
        public int? KillLimit { get; set; }
        public int? Lives { get; set; }
        public bool? Satellite { get; set; }
        public bool? Barracks { get; set; }
        public bool? Sniper { get; set; }
        public bool? Splinter { get; set; }
        public string Respawn { get; set; }
    }
    
    public class MultiplayerGamelistData_Session_Status
    {
        public bool? IsLocked { get; set; }
        public bool? HasPassword { get; set; }
        public string State { get; set; }
    }

    public class MultiplayerGamelistData_Session_Player
    {
        public string Name { get; set; }
        public Dictionary<string, MultiplayerGamelistData_Session_Player_ID> IDs { get; set; }
        public MultiplayerGamelistData_Session_Player_Team Team { get; set; }
        public MultiplayerGamelistData_Session_Player_Hero Hero { get; set; }
        public MultiplayerGamelistData_Session_Player_Stats Stats { get; set; }
        public Dictionary<string, JToken> Attributes { get; set; }
    }

    public class MultiplayerGamelistData_Session_Player_ID
    {
        public JToken ID { get; set; }
        public string Raw { get; set; }
        public string AvatarUrl { get; set; }
        public string ProfileUrl { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
    }

    public class MultiplayerGamelistData_Session_Player_Team
    {
        public bool Leader { get; set; }
        public int? ID { get; set; }
    }

    public class MultiplayerGamelistData_Session_Player_Hero
    {
        public string ID { get; set; }
    }

    public class MultiplayerGamelistData_Session_Player_Stats
    {
        public int? Kills { get; set; }
        public int? Deaths { get; set; }
        public int? Score { get; set; }
    }

    public class MultiplayerGamelistData_Session_Game
    {
        public string Version { get; set; }
        public string Mod { get; set; }
        public List<string> Mods { get; set; }
        public string ModHash { get; set; }
    }

    public class MultiplayerGamelistData_Session_Time
    {
        public int? Seconds { get; set; }
        public int? Resolution { get; set; }
        public bool? Max { get; set; }
        public string Context { get; set; }
    }

    public class MultiplayerGamelistData_Session_Attributes
    {
        public bool? SyncJoin { get; set; }
        public int? TPS { get; set; }
        public int? MaxPing { get; set; }
        public string ListServer { get; set; }
    }
}
