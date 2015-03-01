using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ImageBrowser
{
    public class Settings
    {
        private static Settings _singleton;

        public static Settings Singleton
        {
            get
            {
                return _singleton ?? (_singleton = new Settings());
            }
        }

        public static void Refresh()
        {
            _singleton = null;
        }

        public Settings()
        {
            string filename = HttpContext.Current.Server.MapPath("~/App_Data/Config.json");
            if (!File.Exists(filename))
            {
                IsEmpty = true;
                return;
            }

            string json = File.ReadAllText(filename, Encoding.UTF8);

            JsonConvert.PopulateObject(json, this);
        }

        public void Save()
        {
            string filename = HttpContext.Current.Server.MapPath("~/App_Data/Config.json");

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            try
            {
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save settings file! Please make sure that the app has write access to ~/App_Data", ex);
            }
        }

        public string GetAbsolutePath(string imagePath)
        {
            string absolutePath = Path.GetFullPath(Path.Combine(ImageRootPath, imagePath ?? ""));

            if (!(absolutePath + Path.DirectorySeparatorChar).StartsWith(ImageRootPath + Path.DirectorySeparatorChar))
            {
                throw new ArgumentException("Path must be within the configured root path!");
            }

            return absolutePath;
        }

        [JsonIgnore]
        public bool IsEmpty { get; private set; }

        public string ImageRootPath { get; set; }

        public int ThumbnailSize { get; set; }
    }
}