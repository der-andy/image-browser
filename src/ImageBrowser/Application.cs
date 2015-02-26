using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ImageBrowser
{
    public static class Application
    {
        public static string ImagesRoot
        {
            get { return ConfigurationManager.AppSettings["ImagesRoot"]; }
        }

        public static int ThumbnailSize
        {
            get { return int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]); }
        }

        public static string GetAbsolutePath(string imagePath)
        {
            string absolutePath = Path.GetFullPath(Path.Combine(ImagesRoot, imagePath ?? ""));

            if (!(absolutePath + Path.DirectorySeparatorChar).StartsWith(ImagesRoot + Path.DirectorySeparatorChar))
            {
                throw new ArgumentException("Path must be within the configured root path!");
            }

            return absolutePath;
        }
    }
}