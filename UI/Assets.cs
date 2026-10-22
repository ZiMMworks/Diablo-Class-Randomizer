using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Media;
using System.Reflection;

namespace DiabloClassRandomizer.UI
{
    // Loads the Diablo font, the sounds and the results backgrounds, which are embedded inside the .exe.
    // (The logos, banners and select-game background come from Properties\Resources.)
    public static class Assets
    {
        private const string ResourcePrefix = "DiabloClassRandomizer.Resources.";
        private const string DiabloFontName = "Diablo";

        private static readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();
        private static readonly Dictionary<string, SoundPlayer> _soundCache = new Dictionary<string, SoundPlayer>();
        private static readonly List<PrivateFontCollection> _fontCollections = new List<PrivateFontCollection>();
        private static FontFamily _diabloFont;

        // The "Diablo" font. Ships with the app so it works on every PC, not just ones that have it installed.
        public static FontFamily DiabloFont
        {
            get { return _diabloFont ?? (_diabloFont = LoadFont("Fonts/diabloheavy.ttf")); }
        }

        // The first style the font supports (the Diablo font only comes in one weight).
        public static FontStyle StyleFor(FontFamily family)
        {
            return family.IsStyleAvailable(FontStyle.Regular) ? FontStyle.Regular : FontStyle.Bold;
        }

        // Returns an embedded image from Resources\Images, e.g. Assets.GetImage("result_diablo4.png").
        public static Image GetImage(string fileName)
        {
            Image image;
            if (_imageCache.TryGetValue(fileName, out image))
            {
                return image;
            }

            try
            {
                using (Stream stream = OpenResource("Images/" + fileName))
                using (Image temp = Image.FromStream(stream))
                {
                    // Copy into a Bitmap so the stream can be closed safely.
                    image = new Bitmap(temp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not load image: " + ex.Message);
                image = null;
            }

            _imageCache[fileName] = image;
            return image;
        }

        // Plays an embedded .wav from Resources\Audio; errors are ignored.
        public static void PlaySound(string fileName)
        {
            try
            {
                SoundPlayer player;
                if (!_soundCache.TryGetValue(fileName, out player))
                {
                    var buffer = new MemoryStream();
                    using (Stream stream = OpenResource("Audio/" + fileName))
                    {
                        stream.CopyTo(buffer);
                    }

                    buffer.Position = 0;
                    player = new SoundPlayer(buffer);
                    player.Load();   // load once so the sound starts instantly next time
                    _soundCache[fileName] = player;
                }

                player.Stream.Position = 0;
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }

        private static FontFamily LoadFont(string path)
        {
            // If the font is installed on this PC, use it directly.
            using (var installed = new InstalledFontCollection())
            {
                foreach (FontFamily family in installed.Families)
                {
                    if (family.Name == DiabloFontName)
                    {
                        return new FontFamily(DiabloFontName);
                    }
                }
            }

            try
            {
                // GDI+ loads fonts most reliably from a file, so write the embedded copy to %TEMP% first.
                string fontFolder = Path.Combine(Path.GetTempPath(), "DiabloClassRandomizer");
                Directory.CreateDirectory(fontFolder);
                string fontFile = Path.Combine(fontFolder, Path.GetFileName(path));

                try
                {
                    using (Stream stream = OpenResource(path))
                    using (FileStream file = File.Create(fontFile))
                    {
                        stream.CopyTo(file);
                    }
                }
                catch (IOException)
                {
                    // Already in use by another instance; the file is identical.
                }

                var collection = new PrivateFontCollection();
                collection.AddFontFile(fontFile);
                _fontCollections.Add(collection);   // keep it alive for the life of the app
                return collection.Families[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Font load failed, using fallback: " + ex.Message);
                return new FontFamily("Times New Roman");
            }
        }

        private static Stream OpenResource(string path)
        {
            string resourceName = ResourcePrefix + path.Replace('/', '.').Replace('\\', '.');
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException("Embedded resource not found: " + resourceName);
            }

            return stream;
        }
    }
}
