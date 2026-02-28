using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MinecraftLauncher.Core.Interfaces;

namespace MinecraftLauncher.Core.Managers
{
    /// <summary>
    /// Manages fetching and rendering Minecraft player skins.
    /// </summary>
    public class SkinRenderer : ISkinRenderer
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _cacheDirectory;

        public SkinRenderer(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _cacheDirectory = Path.Combine(appDataPath, "MinecraftLauncher", "cache", "skins");
            
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        /// <summary>
        /// Fetches a player's skin from Mojang API with fallback to crafatar.com.
        /// </summary>
        public async Task<Bitmap> FetchSkinAsync(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            // Check cache first
            var cachedSkin = await GetCachedSkinAsync(username);
            if (cachedSkin != null)
            {
                return cachedSkin;
            }

            Bitmap? skin = null;

            // Try Mojang API first
            try
            {
                skin = await FetchFromMojangAsync(username, cancellationToken);
            }
            catch
            {
                // Fallback to crafatar
                try
                {
                    skin = await FetchFromCrafatarAsync(username, cancellationToken);
                }
                catch
                {
                    // Return default Steve skin
                    skin = CreateDefaultSteveSkin();
                }
            }

            // Cache the skin
            if (skin != null)
            {
                await CacheSkinAsync(username, skin);
            }

            return skin ?? CreateDefaultSteveSkin();
        }

        /// <summary>
        /// Fetches skin from Mojang's session server.
        /// </summary>
        private async Task<Bitmap> FetchFromMojangAsync(string username, CancellationToken cancellationToken)
        {
            // First, get UUID from username
            var uuidUrl = $"https://api.mojang.com/users/profiles/minecraft/{username}";
            var uuidJson = await _httpClientService.GetStringAsync(uuidUrl, cancellationToken);
            var uuid = ExtractUuidFromJson(uuidJson);

            // Then get skin URL from profile
            var profileUrl = $"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}";
            var profileJson = await _httpClientService.GetStringAsync(profileUrl, cancellationToken);
            var skinUrl = ExtractSkinUrlFromProfile(profileJson);

            // Download the skin texture
            var skinBytes = await _httpClientService.GetByteArrayAsync(skinUrl, cancellationToken);
            using var ms = new MemoryStream(skinBytes);
            return new Bitmap(ms);
        }

        /// <summary>
        /// Fetches skin from crafatar.com as fallback.
        /// </summary>
        private async Task<Bitmap> FetchFromCrafatarAsync(string username, CancellationToken cancellationToken)
        {
            var url = $"https://crafatar.com/skins/{username}";
            var skinBytes = await _httpClientService.GetByteArrayAsync(url, cancellationToken);
            using var ms = new MemoryStream(skinBytes);
            return new Bitmap(ms);
        }

        /// <summary>
        /// Extracts UUID from Mojang API JSON response.
        /// </summary>
        private string ExtractUuidFromJson(string json)
        {
            // Simple JSON parsing for UUID
            var idIndex = json.IndexOf("\"id\"");
            if (idIndex == -1) throw new InvalidOperationException("UUID not found in response");
            
            var startQuote = json.IndexOf("\"", idIndex + 5);
            var endQuote = json.IndexOf("\"", startQuote + 1);
            
            return json.Substring(startQuote + 1, endQuote - startQuote - 1);
        }

        /// <summary>
        /// Extracts skin URL from Mojang profile JSON.
        /// </summary>
        private string ExtractSkinUrlFromProfile(string json)
        {
            // Decode base64 textures property
            var texturesIndex = json.IndexOf("\"value\"");
            if (texturesIndex == -1) throw new InvalidOperationException("Textures not found in profile");
            
            var startQuote = json.IndexOf("\"", texturesIndex + 8);
            var endQuote = json.IndexOf("\"", startQuote + 1);
            var base64 = json.Substring(startQuote + 1, endQuote - startQuote - 1);
            
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            
            // Extract URL from decoded JSON
            var urlIndex = decoded.IndexOf("\"url\"");
            if (urlIndex == -1) throw new InvalidOperationException("Skin URL not found");
            
            var urlStart = decoded.IndexOf("\"", urlIndex + 6);
            var urlEnd = decoded.IndexOf("\"", urlStart + 1);
            
            return decoded.Substring(urlStart + 1, urlEnd - urlStart - 1);
        }

        /// <summary>
        /// Creates a default Steve skin.
        /// </summary>
        private Bitmap CreateDefaultSteveSkin()
        {
            // Create a simple default Steve skin (64x64)
            var skin = new Bitmap(64, 64);
            using var g = Graphics.FromImage(skin);
            
            // Fill with skin color
            g.Clear(Color.FromArgb(255, 220, 177, 153));
            
            // Draw simple features
            using var brush = new SolidBrush(Color.FromArgb(255, 51, 25, 0));
            g.FillRectangle(brush, 8, 8, 8, 8); // Head
            
            return skin;
        }

        /// <summary>
        /// Renders a 3D player model from a skin texture using GDI+.
        /// </summary>
        public Bitmap RenderSkin3D(Bitmap skinTexture, float rotationX, float rotationY)
        {
            if (skinTexture == null)
            {
                throw new ArgumentNullException(nameof(skinTexture));
            }

            // Detect skin model type (Steve vs Alex)
            bool isAlexModel = IsAlexModel(skinTexture);

            // Create output bitmap
            var output = new Bitmap(400, 600);
            using var g = Graphics.FromImage(output);
            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            // Apply rotation transformations
            var centerX = output.Width / 2f;
            var centerY = output.Height / 2f;
            
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(rotationY);
            g.TranslateTransform(-centerX, -centerY);

            // Render head
            RenderCube(g, skinTexture, 8, 8, 8, 8, 150, 100, 80);

            // Render body
            RenderCube(g, skinTexture, 20, 20, 8, 12, 150, 200, 60);

            // Render arms
            int armWidth = isAlexModel ? 3 : 4;
            RenderCube(g, skinTexture, 44, 20, armWidth, 12, 90, 200, 50); // Right arm
            RenderCube(g, skinTexture, 36, 52, armWidth, 12, 210, 200, 50); // Left arm

            // Render legs
            RenderCube(g, skinTexture, 4, 20, 4, 12, 130, 320, 50); // Right leg
            RenderCube(g, skinTexture, 20, 52, 4, 12, 170, 320, 50); // Left leg

            return output;
        }

        /// <summary>
        /// Detects if the skin is an Alex model (3px arms) or Steve model (4px arms).
        /// </summary>
        private bool IsAlexModel(Bitmap skin)
        {
            // Alex skins have transparent pixels in the arm area
            // Check pixel at (50, 16) - should be transparent for Alex
            if (skin.Width >= 64 && skin.Height >= 64)
            {
                var pixel = skin.GetPixel(50, 16);
                return pixel.A < 128; // Transparent or semi-transparent
            }
            
            return false; // Default to Steve
        }

        /// <summary>
        /// Renders a simple cube representation of a body part.
        /// </summary>
        private void RenderCube(Graphics g, Bitmap skin, int texX, int texY, int texWidth, int texHeight, 
            int x, int y, int size)
        {
            // Extract texture region
            var rect = new Rectangle(texX, texY, texWidth, texHeight);
            
            try
            {
                // Draw front face
                var destRect = new Rectangle(x, y, size, size);
                g.DrawImage(skin, destRect, rect, GraphicsUnit.Pixel);
                
                // Draw simple shading for depth
                using var shadeBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
                g.FillRectangle(shadeBrush, x + size - 10, y, 10, size);
            }
            catch
            {
                // If texture extraction fails, draw a colored rectangle
                using var brush = new SolidBrush(Color.Gray);
                g.FillRectangle(brush, x, y, size, size);
            }
        }

        /// <summary>
        /// Gets a cached skin for a username.
        /// </summary>
        public async Task<Bitmap?> GetCachedSkinAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            var cachePath = Path.Combine(_cacheDirectory, $"{username}.png");
            
            if (!File.Exists(cachePath))
            {
                return null;
            }

            try
            {
                // Read from file asynchronously
                var bytes = await File.ReadAllBytesAsync(cachePath);
                using var ms = new MemoryStream(bytes);
                return new Bitmap(ms);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Caches a skin for a username.
        /// </summary>
        public async Task CacheSkinAsync(string username, Bitmap skin)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            if (skin == null)
            {
                throw new ArgumentNullException(nameof(skin));
            }

            var cachePath = Path.Combine(_cacheDirectory, $"{username}.png");
            
            try
            {
                // Save to memory stream first
                using var ms = new MemoryStream();
                skin.Save(ms, ImageFormat.Png);
                var bytes = ms.ToArray();
                
                // Write to file asynchronously
                await File.WriteAllBytesAsync(cachePath, bytes);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - caching is not critical
                Console.WriteLine($"Failed to cache skin for {username}: {ex.Message}");
            }
        }
    }
}
