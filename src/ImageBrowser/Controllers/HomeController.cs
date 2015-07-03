using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageBrowser.Models;
using ImageResizer;

namespace ImageBrowser.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string path)
        {
            if (!Directory.Exists(Settings.Singleton.ImageRootPath))
            {
                throw new Exception("Root path " + Settings.Singleton.ImageRootPath + " not found! Please check your configuration!");
            }

            string absolutePath = Settings.Singleton.GetAbsolutePath(path);
            if (!Directory.Exists(absolutePath))
            {
                throw new Exception("Path not found: " + path);
            }

            var model = new FolderModel(path);

            return View("Folder", model);
        }

        public ActionResult Image(string path)
        {
            string absolutePath = Settings.Singleton.GetAbsolutePath(path);
            if (!System.IO.File.Exists(absolutePath))
            {
                return HttpNotFound();
            }

            return View(new ImageModel(absolutePath));
        }

        public ActionResult Thumbnail(string path)
        {
            string thumbnailRoot = Server.MapPath("~/App_Data/Thumbnails");
            string absolutePath = Settings.Singleton.GetAbsolutePath(path);

            var fi = new FileInfo(absolutePath);

            if (!fi.Exists)
            {
                return HttpNotFound();
            }

            string hash = fi.LastWriteTimeUtc.Ticks.ToString("x16");

            var thumbnailPath = Path.Combine(thumbnailRoot, path + "_" + Settings.Singleton.ThumbnailSize + "_" + hash);

            if (!System.IO.File.Exists(thumbnailPath))
            {
                var settings = new ResizeSettings
                {
                    Format = "jpg",
                    BackgroundColor = Color.Black,
                    Anchor = ContentAlignment.MiddleCenter,
                    Width = Settings.Singleton.ThumbnailSize,
                    Height = Settings.Singleton.ThumbnailSize,
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
            string absolutePath = Settings.Singleton.GetAbsolutePath(path);

            var di = new DirectoryInfo(absolutePath);

            if (!di.Exists)
            {
                return HttpNotFound();
            }

            string hash = di.LastWriteTimeUtc.Ticks.ToString("x16");

            var thumbnailPath = Path.Combine(thumbnailRoot, path, Settings.Singleton.ThumbnailSize + "_" + hash);

            if (!System.IO.File.Exists(thumbnailPath))
            {
                int miniSize = Settings.Singleton.ThumbnailSize / 2;

                var settings = new ResizeSettings
                {
                    Format = "jpg",
                    BackgroundColor = Color.Black,
                    Anchor = ContentAlignment.MiddleCenter,
                    Width = miniSize,
                    Height = miniSize,
                    Mode = FitMode.Crop
                };

                Directory.CreateDirectory(Path.GetDirectoryName(thumbnailPath));

                string[] firstImages = Directory.EnumerateFiles(absolutePath, "*.jpg", SearchOption.AllDirectories).Take(4).ToArray();
                var resizedImages = new MemoryStream[firstImages.Length];

                using (var destBitmap = new Bitmap(Settings.Singleton.ThumbnailSize, Settings.Singleton.ThumbnailSize, PixelFormat.Format24bppRgb))
                using (Graphics destGraphics = Graphics.FromImage(destBitmap))
                {

                    for (int i = 0; i < firstImages.Length; i++)
                    {
                        resizedImages[i] = new MemoryStream();
                        ImageBuilder.Current.Build(firstImages[i], resizedImages[i], settings);

                        using (var srcBitmap = System.Drawing.Image.FromStream(resizedImages[i]))
                        {
                            destGraphics.DrawImageUnscaled(srcBitmap, (i % 2) * miniSize, (i / 2) * miniSize);
                        }
                    }

                    ImageCodecInfo jpeg = ImageCodecInfo.GetImageEncoders().Single(x => x.MimeType == "image/jpeg");
                    var parameters = new EncoderParameters(1);
                    parameters.Param[0] = new EncoderParameter(Encoder.Quality, 95L);

                    destBitmap.Save(thumbnailPath, jpeg, parameters);
                }
            }

            return File(thumbnailPath, "image/jpeg");
        }

        public ActionResult PlaceHolder(int width, int height)
        {
            using (var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb))
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "image/png");
            }
        }

        public ActionResult Resized(string path, int width, int height)
        {
            string absolutePath = Settings.Singleton.GetAbsolutePath(path);

            var fi = new FileInfo(absolutePath);

            if (!fi.Exists)
            {
                return HttpNotFound();
            }

            var settings = new ResizeSettings
            {
                Format = "jpg",
                BackgroundColor = Color.Black,
                Anchor = ContentAlignment.MiddleCenter,
                Width = width,
                Height = height,
                Mode = FitMode.Stretch
            };

            using (var ms = new MemoryStream())
            {
                ImageBuilder.Current.Build(absolutePath, ms, settings);

                return File(ms.ToArray(), "image/jpeg");
            }
        }

        public ActionResult Raw(string path)
        {
            string absolutePath = Settings.Singleton.GetAbsolutePath(path);

            if (!System.IO.File.Exists(absolutePath))
            {
                return HttpNotFound();
            }

            return File(absolutePath, "image/jpeg");
        }
    }
}