﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public class ProjectInfo
    {
        public string Version { get; set; }
        public string Compile_Commit { get; set; }
    }

    public static class VersionInfo
    {
        public static ProjectInfo VersioningInfo
        {
            get
            {
                using (Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Launcher.ProjectVariables.json"))
                {
                    using (StreamReader read = new StreamReader(str))
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectInfo>(read.ReadToEnd());
                    }
                }
            }
        }
    }
}
