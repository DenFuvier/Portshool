using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Portshool
{
    public static class PortfolioFileHelper
    {
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };

        public static void LoadPersonPhoto(PictureBox pictureBox, string surname, string name, string patronymic)
        {
            string photoPath = FindPersonPhoto(surname, name, patronymic);

            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }

            if (!string.IsNullOrEmpty(photoPath))
            {
                pictureBox.Image = LoadImageWithoutLock(photoPath);
            }
        }

        public static string FindPersonPhoto(string surname, string name, string patronymic)
        {
            string[] searchDirectories =
            {
                GetPortfolioDirectory("Photos"),
                GetPortfolioDirectory("Documents")
            };

            if (!Directory.Exists(searchDirectories[0]))
            {
                Directory.CreateDirectory(searchDirectories[0]);
            }

            string[] fileNameKeys =
            {
                BuildPersonFileName(surname, name, patronymic),
                BuildPersonFileName(surname, name, null),
                BuildPersonFileName(surname, null, null)
            };

            foreach (string fileNameKey in fileNameKeys.Where(key => !string.IsNullOrWhiteSpace(key)))
            {
                foreach (string directory in searchDirectories.Where(Directory.Exists))
                {
                    foreach (string extension in ImageExtensions)
                    {
                        string path = Path.Combine(directory, fileNameKey + extension);

                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
            }

            return null;
        }

        public static string GetPortfolioDirectory(string sectionName)
        {
            string root = FindPortfolioRoot();
            return Path.Combine(root, "PortfolioFiles", sectionName);
        }

        private static string FindPortfolioRoot()
        {
            DirectoryInfo current = new DirectoryInfo(Application.StartupPath);
            string portfolioRoot = null;
            string projectRoot = null;

            while (current != null)
            {
                if (Directory.Exists(Path.Combine(current.FullName, "PortfolioFiles")))
                {
                    portfolioRoot = current.FullName;
                }

                if (projectRoot == null && File.Exists(Path.Combine(current.FullName, "Portshool.csproj")))
                {
                    projectRoot = current.FullName;
                }

                current = current.Parent;
            }

            if (portfolioRoot != null)
            {
                return portfolioRoot;
            }

            if (projectRoot != null)
            {
                return projectRoot;
            }

            return Application.StartupPath;
        }

        private static string BuildPersonFileName(string surname, string name, string patronymic)
        {
            string[] parts = { surname, name, patronymic };
            return string.Join("_", parts
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Select(CleanFileNamePart));
        }

        private static string CleanFileNamePart(string value)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string cleaned = new string(value.Trim()
                .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
                .ToArray());

            return cleaned.Replace(' ', '_');
        }

        private static Image LoadImageWithoutLock(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return Image.FromStream(stream);
            }
        }
    }
}
