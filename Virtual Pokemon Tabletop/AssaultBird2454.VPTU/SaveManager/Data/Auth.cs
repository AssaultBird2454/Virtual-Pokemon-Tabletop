using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AssaultBird2454.VPTU.SaveManager.Data
{
    public class Auth
    {
        SaveManager Manager;
        public Auth(SaveManager _Manager)
        {
            Manager = _Manager;
        }

        #region User
        public List<Authentication_Manager.Data.User> Users_List()
        {
            List<Authentication_Manager.Data.User> list = new List<Authentication_Manager.Data.User>();
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Auth_Users", Manager.Connection))
            {
                using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.HasRows)
                        return list;

                    while (dr.Read())
                    {
                        Authentication_Manager.Data.User user = new Authentication_Manager.Data.User();

                        user.UserID = dr.GetString(0);
                        user.IC_Name = dr.GetString(1);
                        user.Name = dr.GetString(2);
                        user.isGM = dr.GetBoolean(3);
                        string Color = dr.GetString(4);
                        user.UserColor = (Color)ColorConverter.ConvertFromString(Color);

                        list.Add(user);
                    }

                    return list;
                }
            }
        }
        public Authentication_Manager.Data.User Users_GetByID(string ID)
        {
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Auth_Users WHERE `User_ID`=\"" + ID + "\"", Manager.Connection))
            {
                using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.HasRows)
                        return null;

                    while (dr.Read())
                    {
                        Authentication_Manager.Data.User user = new Authentication_Manager.Data.User();

                        user.UserID = dr.GetString(0);
                        user.IC_Name = dr.GetString(1);
                        user.Name = dr.GetString(2);
                        user.isGM = dr.GetBoolean(3);
                        user.UserColor = (Color)ColorConverter.ConvertFromString(dr.GetString(4));

                        return user;
                    }
                    return null;
                }
            }
        }
        public Authentication_Manager.Data.User Users_GetByKey(string Key)
        {
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Auth_Users WHERE `User_ID`=(SELECT `User_ID` FROM Auth_Identities WHERE Key=\"" + Key + "\");", Manager.Connection))
            {
                using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.HasRows)
                        return null;

                    while (dr.Read())
                    {
                        Authentication_Manager.Data.User user = new Authentication_Manager.Data.User();

                        user.UserID = dr.GetString(0);
                        user.IC_Name = dr.GetString(1);
                        user.Name = dr.GetString(2);
                        user.isGM = dr.GetBoolean(3);
                        user.UserColor = (Color)ColorConverter.ConvertFromString(dr.GetString(4));

                        return user;
                    }
                    return null;
                }
            }
        }
        public void Users_Update(Authentication_Manager.Data.User User)
        {
            if (!Users_HasUser(User.UserID))
            {
                Users_Add(User);
                return;
            }
            new System.Data.SQLite.SQLiteCommand("UPDATE Auth_Users SET `User_CharacterName`=\"" + User.IC_Name + "\", `User_UserName`=\"" + User.Name + "\", `User_IsGM`=\"" + User.isGM + "\", `User_Color`=\"" + User.UserColor.ToString() + "\" WHERE `User_ID`=\"" + User.UserID + "\";", Manager.Connection).ExecuteNonQuery();
        }
        public void Users_Add(Authentication_Manager.Data.User User)
        {
            if (Users_HasUser(User.UserID))
            {
                Users_Update(User);
                return;
            }
            new System.Data.SQLite.SQLiteCommand("INSERT INTO Auth_Users (`User_ID`, `User_CharacterName`, `User_UserName`, `User_IsGM`, `User_Color`) VALUES (\"" + User.UserID + "\", \"" + User.IC_Name + "\", \"" + User.Name + "\", \"" + Convert.ToInt32(User.isGM) + "\", \"" + User.UserColor.ToString() + "\");", Manager.Connection).ExecuteNonQuery();
        }
        public void Users_Remove(Authentication_Manager.Data.User User)
        {
            new System.Data.SQLite.SQLiteCommand("DELETE FROM Auth_Users WHERE `User_ID`=\"" + User.UserID + "\";", Manager.Connection).ExecuteNonQuery();
            new System.Data.SQLite.SQLiteCommand("DELETE FROM Auth_Identities WHERE `User_ID`=\"" + User.UserID + "\";", Manager.Connection).ExecuteNonQuery();
        }
        public bool Users_HasUser(string ID)
        {
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT COUNT(User_ID) as Count FROM Auth_Users WHERE `User_ID`=\"" + ID + "\";", Manager.Connection))
            using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
            {
                if (!dr.HasRows)
                    return false;

                dr.Read();
                if (dr.GetInt32(0) >= 1)
                    return true;
                return false;
            }
        }
        #endregion

        #region Identity
        public string Identity_GetKey(string UserID)
        {
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT `Key` FROM Auth_Identities WHERE `User_ID` = \"" + UserID + "\";", Manager.Connection))
            {
                using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.HasRows)
                        return Identity_GenerateKey(UserID);

                    dr.Read();
                    return dr.GetString(0);
                }
            }
        }
        public Authentication_Manager.Data.Identity Identity_GetIdentity(string Key)
        {
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Auth_Users WHERE `User_ID`=(SELECT `User_ID` FROM Auth_Identities WHERE Key=\"" + Key + "\");", Manager.Connection))
            {
                using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.HasRows)
                        return null;

                    while (dr.Read())
                    {
                        Authentication_Manager.Data.Identity ID = new Authentication_Manager.Data.Identity();

                        ID.UserID = dr.GetString(1);
                        ID.Key = dr.GetString(0);

                        return ID;
                    }
                    return null;
                }
            }
        }
        public string Identity_GenerateKey(string UserID)
        {
            string Key = RNG.Generators.RSG.GenerateString(32);

            if (Users_HasKey(UserID))
                new System.Data.SQLite.SQLiteCommand("DELETE FROM Auth_Identities WHERE `User_ID`=\"" + UserID + "\";", Manager.Connection).ExecuteNonQuery();
            new System.Data.SQLite.SQLiteCommand("INSERT INTO `Auth_Identities` (`Key`, `User_ID`) VALUES (\"" + Key + "\", \"" + UserID + "\");", Manager.Connection).ExecuteNonQuery();

            return Key;
        }
        public bool Users_HasKey(string ID)
        {
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT COUNT(User_ID) as Count FROM Auth_Identities WHERE `User_ID`=\"" + ID + "\";", Manager.Connection))
            using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
            {
                if (!dr.HasRows)
                    return false;

                dr.Read();
                if (dr.GetInt32(0) >= 1)
                    return true;
                return false;
            }
        }
        #endregion
    }
}
