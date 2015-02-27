using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IniParser;
using IniParser.Model;

namespace ImageBrowser
{
    public class PicasaReader
    {
        private readonly IniData _data;
        private readonly Dictionary<string, string> _names;

        public PicasaReader(string picasaIniFile)
        {
            var parser = new FileIniDataParser();
            _data = parser.ReadFile(picasaIniFile, Encoding.UTF8);

            _names = _data.Sections.ContainsSection("Contacts2")
                ? _data.Sections["Contacts2"].ToDictionary(kd => kd.KeyName, kd => kd.Value)
                : new Dictionary<string, string>();
        }

        public IEnumerable<Tuple<RectangleF, string>> GetFaces(string imageFileName)
        {
            imageFileName = Path.GetFileName(imageFileName);

            if (!_data.Sections.ContainsSection(imageFileName))
            {
                yield break;
            }

            KeyDataCollection section = _data.Sections[imageFileName];

            string facesString = section["faces"];
            if (string.IsNullOrEmpty(facesString))
            {
                yield break;
            }

            foreach (string face in facesString.Split(';'))
            {
                Match m = Regex.Match(face, @"^rect64\((?<rect>[0-9a-f]+)\),(?<face>[0-9a-f]+)$");
                if (!m.Success)
                {
                    continue;
                }

                if (!_names.ContainsKey(m.Groups["face"].Value))
                {
                    continue;
                }

                string name = _names[m.Groups["face"].Value].Split(';').First();

                string rect = m.Groups["rect"].Value.PadLeft(16, '0');

                float left = int.Parse(rect.Substring(0, 4), NumberStyles.HexNumber)/65536.0f;
                float top = int.Parse(rect.Substring(4, 4), NumberStyles.HexNumber)/65536.0f;
                float right = int.Parse(rect.Substring(8, 4), NumberStyles.HexNumber)/65536.0f;
                float bottom = int.Parse(rect.Substring(12, 4), NumberStyles.HexNumber)/65536.0f;

                yield return Tuple.Create(new RectangleF(left, top, right - left, bottom - top), name);
            }
        }
    }
}