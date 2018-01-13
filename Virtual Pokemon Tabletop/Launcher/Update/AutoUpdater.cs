﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Windows;

namespace Launcher.Update
{
    internal class AutoUpdater
    {
        public static ProjectInfo VersioningInfo { get; set; }
        private static Data LatestVersion { get; set; }

        public static void CheckForUpdates(List<ReleaseStream> rs)
        {
            #region Versioning Info

            using (var str = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Launcher.ProjectVariables.json"))
            {
                using (var read = new StreamReader(str))
                {
                    VersioningInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectInfo>(read.ReadToEnd());
                }
            }

            #endregion

            try
            {
                #region Alpha Release

                if (rs.Contains(ReleaseStream.Alpha))
                {
                    var data = Get_UpdateInfo("http://www.virtual-ptu.com/api/Updater/Alpha/Latest");
                    if (Check_Update(data))
                    {
                        UpdateAvaliable(data);
                        return;
                    }
                }

                #endregion

                #region Beta Release

                if (rs.Contains(ReleaseStream.Beta))
                {
                    var data = Get_UpdateInfo("http://www.virtual-ptu.com/api/Updater/Beta/Latest");
                    if (Check_Update(data))
                    {
                        UpdateAvaliable(data);
                        return;
                    }
                }

                #endregion

                #region Master Release

                if (rs.Contains(ReleaseStream.Master))
                {
                    var data = Get_UpdateInfo("http://www.virtual-ptu.com/api/Updater/Master/Latest");
                    if (Check_Update(data))
                        UpdateAvaliable(data);
                }

                #endregion
            }
            catch
            {
            }
        }

        private static bool Check_Update(Data data)
        {
            #region Get Version (Latest)

            var Version_Info = new int[4];
            var i = 0;
            foreach (var id in LatestVersion.Version_ID.Split('.'))
            {
                Version_Info[i] = Convert.ToInt32(id);
                i++;
            }

            #endregion

            var FVI = FileVersionInfo.GetVersionInfo(MainWindow.AssemblyDirectory + @"\Launcher.exe");

            if (FVI.ProductMajorPart < Version_Info[0] ||
                FVI.ProductMajorPart <= Version_Info[0] && FVI.ProductMinorPart < Version_Info[1] ||
                FVI.ProductMajorPart <= Version_Info[0] && FVI.ProductMinorPart <= Version_Info[1] &&
                FVI.ProductBuildPart < Version_Info[2] ||
                FVI.ProductMajorPart <= Version_Info[0] && FVI.ProductMinorPart <= Version_Info[1] &&
                FVI.ProductBuildPart <= Version_Info[2] && FVI.ProductPrivatePart < Version_Info[3])
                return true;
            return false;
        }

        private static Data Get_UpdateInfo(string url)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Data>(new WebClient().DownloadString(url));
        }

        private static void UpdateAvaliable(Data data)
        {
            var mbr = MessageBox.Show(
                "An update for Virtual Pokemon Tabletop was found...\n\nCurrent Version: " + VersioningInfo.Version +
                "\nLatest Version: " + data.Version_Name, "Virtual Pokemon Tabletop Update", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.Yes);

            if (mbr == MessageBoxResult.Yes)
            {
                Directory.CreateDirectory(MainWindow.AssemblyDirectory + "/Updater/");
                var wc = new WebClient();
                wc.DownloadFile("http://vptu.assaultbirdsoftware.me/Updater/UpdateApp/VPTU_Updater.zip",
                    MainWindow.AssemblyDirectory + "/Updater/VPTU_Updater.zip");

                using (var FileStream = new FileStream(MainWindow.AssemblyDirectory + "/Updater/VPTU_Updater.zip",
                    FileMode.Open))
                using (var Archive = new ZipArchive(FileStream, ZipArchiveMode.Read))
                {
                    Archive.ExtractToDirectory(MainWindow.AssemblyDirectory + "/Updater/");
                }
                Process.Start(MainWindow.AssemblyDirectory + "/Updater/Updater.exe", "");
                Process.GetCurrentProcess().Kill();
            }
            else if (mbr == MessageBoxResult.No)
            {
                // Nothing
            }
        }
    }
}