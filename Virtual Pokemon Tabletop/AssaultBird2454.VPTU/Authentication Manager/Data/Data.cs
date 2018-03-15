using Newtonsoft.Json;
using System.Collections.Generic;

namespace AssaultBird2454.VPTU.Authentication_Manager.Data
{
    public class User
    {
        public User(bool Init = false)
        {
            if (Init)
            {
                UserID = RNG.Generators.RSG.GenerateString(5);
                Name = "";
                IC_Name = "";

                UserColor = new System.Windows.Media.Color() { R = 0, G = 178, B = 214, A = 255 };
            }
        }

        public string UserID { get; set; }
        public string Name { get; set; }
        public string IC_Name { get; set; }
        public bool isGM { get; set; }
        public System.Windows.Media.Color UserColor { get; set; }
    }

    public class PermissionData
    {
        public PermissionData(bool Init = false)
        {
            if (Init)
            {
                Groups = new List<string>();
                Permissions = new List<Permissions_Manager.Data.Permission_Node>();
            }
        }

        public string UserID { get; set; }
        [JsonIgnore]
        public string Group_String
        {
            get
            {
                int i = 0;
                string s = "";
                foreach (string gro in Groups)
                {
                    if (i != 0)
                        s = s + ", ";
                    s = s + gro.ToString();
                    i++;
                }

                return s;
            }
        }
        public List<string> Groups { get; set; }
        public List<Permissions_Manager.Data.Permission_Node> Permissions { get; set; }
    }

    public class Group
    {
        public string GroupID { get; set; }
        public string Name { get; set; }

        public List<Permissions_Manager.Data.Permission_Node> Permissions { get; set; }
    }

    public class Identity
    {
        public string UserID { get; set; }
        /// <summary>
        /// A 32char long random string that is unique to a player
        /// </summary>
        public string Key { get; set; }
    }

    public class ClientIdentity
    {
        public string AuthKey { get; set; }
        public string Campaign_Name { get; set; }
        public string ICN { get; set; }
        public string Server_Address { get; set; }
        public int Server_Port { get; set; }
    }
}
