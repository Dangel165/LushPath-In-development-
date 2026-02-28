using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MinecraftLauncher.UI
{
    /// <summary>
    /// Helper class for managing application icons
    /// </summary>
    public static class IconHelper
    {
        private static Icon _defaultIcon;
        private const string DefaultIconFileName = "launcher-icon.ico";

        /// <summary>
        /// Gets the default application icon
        /// </summary>
        /// <returns>Default icon or null if not found</returns>
        public static Icon GetDefaultIcon()
        {
            if (_defaultIcon == null)
            {
                string iconPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    DefaultIconFileName
                );

                if (File.Exists(iconPath))
                {
                    try
                    {
                        _defaultIcon = new Icon(iconPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load default icon: {ex.Message}");
                    }
                }
            }

            return _defaultIcon;
        }

        /// <summary>
        /// Sets the icon for a form using the default application icon
        /// </summary>
        /// <param name="form">Form to set icon for</param>
        public static void SetFormIcon(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var icon = GetDefaultIcon();
            if (icon != null)
            {
                form.Icon = icon;
            }
        }

        /// <summary>
        /// Loads a custom icon from a file path
        /// </summary>
        /// <param name="path">Path to icon file</param>
        /// <returns>Loaded icon or default icon if loading fails</returns>
        public static Icon LoadCustomIcon(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return GetDefaultIcon();
            }

            try
            {
                return new Icon(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load custom icon from {path}: {ex.Message}");
                return GetDefaultIcon();
            }
        }

        /// <summary>
        /// Validates if a file is a valid icon file
        /// </summary>
        /// <param name="path">Path to icon file</param>
        /// <returns>True if valid icon file</returns>
        public static bool IsValidIconFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return false;

            try
            {
                using (var icon = new Icon(path))
                {
                    return icon != null;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the icon file path for the application
        /// </summary>
        /// <returns>Full path to icon file</returns>
        public static string GetIconPath()
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                DefaultIconFileName
            );
        }
    }
}
