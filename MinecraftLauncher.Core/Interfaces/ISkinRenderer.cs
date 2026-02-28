using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftLauncher.Core.Interfaces
{
    /// <summary>
    /// Interface for fetching and rendering Minecraft player skins.
    /// </summary>
    public interface ISkinRenderer
    {
        /// <summary>
        /// Fetches a player's skin from Mojang API with fallback to crafatar.com.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The skin texture as a Bitmap.</returns>
        Task<Bitmap> FetchSkinAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Renders a 3D player model from a skin texture.
        /// </summary>
        /// <param name="skinTexture">The skin texture bitmap.</param>
        /// <param name="rotationX">Rotation around X axis in degrees.</param>
        /// <param name="rotationY">Rotation around Y axis in degrees.</param>
        /// <returns>Rendered 3D skin as a Bitmap.</returns>
        Bitmap RenderSkin3D(Bitmap skinTexture, float rotationX, float rotationY);

        /// <summary>
        /// Gets a cached skin for a username.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <returns>The cached skin bitmap, or null if not cached.</returns>
        Task<Bitmap?> GetCachedSkinAsync(string username);

        /// <summary>
        /// Caches a skin for a username.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <param name="skin">The skin bitmap to cache.</param>
        Task CacheSkinAsync(string username, Bitmap skin);
    }
}
