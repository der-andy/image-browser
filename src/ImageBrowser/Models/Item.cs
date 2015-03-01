using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageBrowser.Models
{
    public class Item
    {
        public Item(string fullName)
        {
            FullName = fullName;
            RelativeName = fullName.Substring(Settings.Singleton.ImageRootPath.Length + 1);
            ShortName = Path.GetFileName(fullName);
            ShortNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
        }

        /// <summary>
        ///     Returns the absolute server path to the file or folder.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        ///     Returns the file or folder path relative to the <see cref="Settings.ImageRootPath"/>.
        /// </summary>
        public string RelativeName { get; private set; }

        /// <summary>
        ///     Returns the file or folder name.
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        ///     Returns the file or folder name without extension.
        /// </summary>
        public string ShortNameWithoutExtension { get; private set; }
    }
}