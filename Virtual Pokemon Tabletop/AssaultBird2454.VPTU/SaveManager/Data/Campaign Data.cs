using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultBird2454.VPTU.SaveManager.Data
{
    public class Campaign_Data
    {
        SaveManager Manager;
        public Campaign_Data(SaveManager _Manager)
        {
            Manager = _Manager;
        }

        public string Campaign_Name
        {
            get
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT `value` FROM Campaign_Settings WHERE `key` = \"Campaign_Name\";", Manager.Connection))
                {
                    using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                            return "";

                        dr.Read();
                        return dr.GetString(0);
                    }
                }
            }
            set
            {
                if (new System.Data.SQLite.SQLiteCommand("UPDATE Campaign_Settings SET `value`=\"" + value + "\" WHERE `key`=\"Campaign_Name\";", Manager.Connection).ExecuteNonQuery() == 0)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Campaign_Settings (`key`, `value`) VALUES (\"Campaign_Name\", \"" + value + "\");", Manager.Connection).ExecuteNonQuery();
                }
            }
        }
        public string Campaign_Desc
        {
            get
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT `value` FROM Campaign_Settings WHERE `key` = \"Campaign_Desc\";", Manager.Connection))
                {
                    using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                            return "";

                        dr.Read();
                        return dr.GetString(0);
                    }
                }
            }
            set
            {
                if (new System.Data.SQLite.SQLiteCommand("UPDATE Campaign_Settings SET `value`=\"" + value + "\" WHERE `key`=\"Campaign_Desc\";", Manager.Connection).ExecuteNonQuery() == 0)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Campaign_Settings (`key`, `value`) VALUES (\"Campaign_Desc\", \"" + value + "\");", Manager.Connection).ExecuteNonQuery();
                }
            }
        }
        public string Campaign_GM_Name
        {
            get
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT `value` FROM Campaign_Settings WHERE `key` = \"Campaign_GM_Name\";", Manager.Connection))
                {
                    using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                            return "";

                        dr.Read();
                        return dr.GetString(0);
                    }
                }
            }
            set
            {
                if (new System.Data.SQLite.SQLiteCommand("UPDATE Campaign_Settings SET `value`=\"" + value + "\" WHERE `key`=\"Campaign_GM_Name\";", Manager.Connection).ExecuteNonQuery() == 0)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Campaign_Settings (`key`, `value`) VALUES (\"Campaign_GM_Name\", \"" + value + "\");", Manager.Connection).ExecuteNonQuery();
                }
            }
        }

        public string Server_Address
        {
            get
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT `value` FROM Campaign_Settings WHERE `key` = \"Server_Address\";", Manager.Connection))
                {
                    using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                            return "127.0.0.1";

                        dr.Read();
                        return dr.GetString(0);
                    }
                }
            }
            set
            {
                if (new System.Data.SQLite.SQLiteCommand("UPDATE Campaign_Settings SET `value`=\"" + value + "\" WHERE `key`=\"Server_Address\";", Manager.Connection).ExecuteNonQuery() == 0)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Campaign_Settings (`key`, `value`) VALUES (\"Server_Address\", \"" + value + "\");", Manager.Connection).ExecuteNonQuery();
                }
            }
        }
        public int Server_Port
        {
            get
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT `value` FROM Campaign_Settings WHERE `key` = \"Server_Port\";", Manager.Connection))
                {
                    using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                            return 25444;

                        dr.Read();
                        return Convert.ToInt32(dr.GetString(0));
                    }
                }
            }
            set
            {
                if (new System.Data.SQLite.SQLiteCommand("UPDATE Campaign_Settings SET `value`=\"" + value + "\" WHERE `key`=\"Server_Port\";", Manager.Connection).ExecuteNonQuery() == 0)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Campaign_Settings (`key`, `value`) VALUES (\"Server_Port\", \"" + value + "\");", Manager.Connection).ExecuteNonQuery();
                }
            }
        }
    }

    public class Campaign_Settings
    {
        SaveManager Manager;
        public Campaign_Settings(SaveManager _Manager)
        {
            Manager = _Manager;
        }

        public void InitNullObjects()
        {
            // Check for Null and Set to new object
        }

        // Settings and variables
    }

    public class Server_Settings
    {
        SaveManager Manager;

        public Server_Settings(SaveManager _Manager)
        {
            Manager = _Manager;
        }

        public void InitNullObjects()
        {
            // Check for Null and Set to new object
        }

        // Settings and variables
    }
}
