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

        public ActionResult FolderThumbnail(string path)
        {
            string thumbnailRoot = Server.MapPath("~/App_Data/Thumbnails");
            string absolutePath = Application.GetAbsolutePath(path);

            var di = new DirectoryInfo(absolutePath);

            if (!di.Exists)
            {
                return HttpNotFound();
            }

            string hash = di.LastWriteTimeUtc.Ticks.ToString("x16");

            var thumbnailPath = Path.Combine(thumbnailRoot, path, Application.ThumbnailSize + "_" + hash);

            if (!System.IO.File.Exists(thumbnailPath))
            {
                // TODO: This needs to be adjusted to return a thumbnail with more than the first image of the folger

                var settings = new ResizeSettings
                {
                    Format = "jpg",
                    BackgroundColor = Color.Black,
                    Anchor = ContentAlignment.MiddleCenter,
                    Width = Application.ThumbnailSize,
                    Height = Application.ThumbnailSize,
                    Mode = FitMode.Crop
                };

                string firstImage = Directory.EnumerateFiles(absolutePath, "*.jpg", SearchOption.AllDirectories).FirstOrDefault();
                if (!System.IO.File.Exists(firstImage))
                {
                    return HttpNotFound();
                }

                Directory.CreateDirectory(Path.GetDirectoryName(thumbnailPath));

                using (var fs = System.IO.File.OpenWrite(thumbnailPath))
                {
                    ImageBuilder.Current.Build(firstImage, fs, settings);
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