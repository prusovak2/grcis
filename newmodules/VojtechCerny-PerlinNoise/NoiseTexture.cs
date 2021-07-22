using MathSupport;
using Rendering;
using System;
using Utilities;

namespace VojtechCerny {
  /// <summary>
  /// (Example usage) A simple texture created from underlying noise.
  /// </summary>
  [Serializable]
  public class NoiseTexture : ITexture {
    INoise noiseGenerator;
    public NoiseTexture (INoise noise) {
      this.noiseGenerator = noise;
    }

    public long Apply (Intersection inter) {
      double noise = noiseGenerator.GetAt(inter.TextureCoord.X, inter.TextureCoord.Y);

      // Scale to 0-1
      noise = (noise + 1) / 2;

      double[] color = { noise, noise, noise };
      Util.ColorCopy(color, inter.SurfaceColor);

      inter.textureApplied = true;

      return (long) RandomStatic.numericRecipes((ulong)(noise * 1000000));
    }
  }
}
