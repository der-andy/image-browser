using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ImageBrowser.Models
{
    public class FolderModel
    {
        public string CurrentFolder { get; private set; }

        public string DisplayName
        {
            get { return CurrentFolder.Replace(Path.DirectorySeparatorChar + "", " > ").Or("Root folder"); }
        }

        public string Description
        {
            get
            {
                var items = new List<string>();

                if (Folders.Length == 1)
                {
                    items.Add("1 subfolder");
                }
                else if (Folders.Length > 1)
                {
                    items.Add(Folders.Length + " subfolders");
                }

                if (Files.Length == 1)
                {
                    items.Add("1 image");
                }
                else if (Files.Length > 1)
                {
                    items.Add(Files.Length + " images");
                }

                return string.Join(", ", items).Or("Empty folder");
            }
        }

        public FolderModel(string folder)
        {
            string absolutePath = Application.GetAbsolutePath(folder);

            CurrentFolder = absolutePath == Application.ImagesRoot
                ? ""
                : absolutePath.Substring(Application.ImagesRoot.Length + 1);

            Files = Directory.EnumerateFiles(absolutePath, "*.jpg", SearchOption.TopDirectoryOnly)
                .Select(fullpath => new Item(fullpath))
                .Where(i => !i.ShortName.StartsWith("."))
                .OrderBy(i => i.ShortName)
                .ToArray();

            Folders = Directory.EnumerateDirectories(absolutePath)
                .Select(fullpath => new Item(fullpath))
                .Where(i => !i.ShortName.StartsWith("."))
                .OrderBy(i => i.ShortName)
                .ToArray();
        }

        public Item[] Files { get; private set; }
        public Item[] Folders { get; private set; }
    }
}