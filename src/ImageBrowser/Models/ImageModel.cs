using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using ExifLib;

namespace ImageBrowser.Models
{
    public class ImageModel
    {
        private readonly ExifReader _exif;

        public ImageModel(string absolutePath)
        {
            string absolutePath1 = absolutePath;

            try
            {
                _exif = new ExifReader(absolutePath1);
            }
            catch (ExifLibException)
            {
                _exif = null;
            }

            Item = new Item(absolutePath);

            string picasaIni = Path.Combine(Path.GetDirectoryName(absolutePath1), ".picasa.ini");
            if (File.Exists(picasaIni))
            {
                var p = new PicasaReader(picasaIni);
                Faces = p.GetFaces(Path.GetFileName(absolutePath1)).ToArray();
            }
            else
            {
                Faces = new Tuple<RectangleF, string>[0];
            }

            using (FileStream fs = File.OpenRead(absolutePath))
            {
                var decoder = new JpegBitmapDecoder(fs, BitmapCreateOptions.None, BitmapCacheOption.None);
                var metadata = decoder.Frames[0].Metadata as BitmapMetadata;
                if (metadata != null && metadata.Keywords != null)
                {
                    Keywords = metadata.Keywords.ToArray();
                }
                else
                {
                    Keywords = new string[0];
                }

                Width = decoder.Frames[0].PixelWidth;
                Height = decoder.Frames[0].PixelHeight;
            }
        }

        public Tuple<RectangleF, string>[] Faces { get; private set; }

        public Item Item { get; private set; }

        public string[] Keywords { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public IEnumerable<ExifTags> GetAvailableExifTags()
        {
            object foo;
            return Enum.GetValues(typeof (ExifTags)).Cast<ExifTags>().Where(tag => _exif.GetTagValue(tag, out foo));
        }

        public T GetExifTag<T>(ExifTags tag)
        {
            T value;
            if (_exif == null || !_exif.GetTagValue(tag, out value))
            {
                return default(T);
            }

            return value;
        }

        public string GetExifData(ExifTags tag)
        {
            object value;
            if (_exif == null || !_exif.GetTagValue(tag, out value))
            {
                return null;
            }

            switch (tag)
            {
                case ExifTags.Make:
                case ExifTags.Model:
                case ExifTags.Software:
                    return (string) value;

                case ExifTags.ISOSpeedRatings:
                    return string.Format("ISO-{0}", value);

                case ExifTags.FNumber:
                    return string.Format(CultureInfo.InvariantCulture, "F/{0:N1}", value);

                case ExifTags.FocalLength:
                    return string.Format("{0} mm", value);

                case ExifTags.ExposureTime:
                    var v = (double) value;
                    if (v < 1)
                    {
                        return string.Format(CultureInfo.InvariantCulture, "1/{0} s", 1 / v);
                    }
                    else
                    {
                        return string.Format(CultureInfo.InvariantCulture, "{0} s", v);
                    }

                case ExifTags.MakerNote:
                case ExifTags.UserComment:
                case ExifTags.FlashpixVersion:
                case ExifTags.ComponentsConfiguration:
                case ExifTags.ExifVersion:
                    return string.Join(" ", ((byte[]) value).Select(b => b.ToString("x02")));

                case ExifTags.DateTime:
                case ExifTags.DateTimeDigitized:
                case ExifTags.DateTimeOriginal:
                    DateTime dt;
                    _exif.GetTagValue(tag, out dt);
                    return string.Format("{0:ddd, yyyy-MMM-dd HH:mm:ss}", dt);

                default:
                    return value.ToString();
            }
        }
    }
}