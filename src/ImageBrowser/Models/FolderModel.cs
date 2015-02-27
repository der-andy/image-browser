﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ImageBrowser.Models
{
    public class FolderModel
    {
        public string CurrentFolder { get; private set; }

        public FolderModel(string folder)
        {
            string absolutePath = Application.GetAbsolutePath(folder);
            CurrentFolder = absolutePath;

            CurrentFolder = CurrentFolder == Application.ImagesRoot
                ? ""
                : CurrentFolder.Substring(Application.ImagesRoot.Length + 1);

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