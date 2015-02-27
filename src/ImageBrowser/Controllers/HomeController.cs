using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageBrowser.Models;
using ImageResizer;

namespace ImageBrowser.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string path)
        {
            string absolutePath = Application.GetAbsolutePath(path);
            if (!Directory.Exists(absolutePath))
            {
                return HttpNotFound();
            }

            var model = new FolderModel(path);

            return View("Folder", model);
        }

        public ActionResult Image(string path)
        {
            string absolutePath = Application.GetAbsolutePath(path);
            if (!System.IO.File.Exists(absolutePath))
            {
                return HttpNotFound();
            }

            return View(new ImageModel(absolutePath));
        }

        public ActionResult Thumbnail(string path)
        {
            string thumbnailRoot = Server.MapPath("~/App_Data/Thumbnails");
            string absolutePath = Application.GetAbsolutePath(path);

            var fi = new FileInfo(absolutePath);

            if (!fi.Exists)
            {
                return HttpNotFound();
            }
            
            string hash = fi.LastWriteTimeUtc.Ticks.ToString("x16");

            var thumbnailPath = Path.Combine(thumbnailRoot, path + "_" + Application.ThumbnailSize + "_" + hash);

            if (!System.IO.File.Exists(thumbnailPath))
            {
                var settings = new ResizeSettings
                {
                    Format = "jpg",
                    BackgroundColor = Color.Black,
                    Anchor = ContentAlignment.MiddleCenter,
                    Width = Application.ThumbnailSize,
                    Height = Application.ThumbnailSize,
                    Mode = FitMode.Crop
                };

                Directory.CreateDirectory(Path.GetDirectoryName(thumbnailPath));

                using (var fs = System.IO.File.OpenWrite(thumbnailPath))
                {
                    ImageBuilder.Current.Build(absolutePath, fs, settings);
                }
            }

            return File(thumbnailPath, "image/jpeg");
        }

        public ActionResult Raw(string path)
        {
            string absolutePath = Application.GetAbsolutePath(path);

            if (!System.IO.File.Exists(absolutePath))
            {
                return HttpNotFound();
            }

            return File(absolutePath, "image/jpeg");
        }
    }
}