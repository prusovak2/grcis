using MathSupport;
using OpenTK;
using Rendering;
using System;

namespace VojtechCerny {
  /// <summary>
  /// (Example usage) A bump-map created from underlying noise.
  /// </summary>
  [Serializable]
  public class NoiseBumpMap : ITexture {
    INoiseWithGradient noiseGenerator;

    double scale;

    /// <summary>
    /// Initializes a new instance of NoiseBumpMap.
    /// </summary>
    /// <param name="noiseGenerator">Underlying noise generator.</param>
    /// <param name="scale">Additional scaling of the noise. Should be rather small (probably less than 0.1) if the noise is normalized to <-1, 1>.</param>
    /// <param name="delta">Delta used for sampling the gradient (only if the noise doesn't implement gradient computation itself).</param>
    public NoiseBumpMap (INoise noiseGenerator, double scale = 0.05, double delta = 0.01) {
      if (noiseGenerator is INoiseWithGradient) {
        this.noiseGenerator = (INoiseWithGradient) noiseGenerator;
      } else {
        this.noiseGenerator = new GradientWrapper(noiseGenerator, delta);
      }
      this.scale = scale;
    }

    public long Apply (Intersection inter) {
      double xGrad, yGrad;

      double noise = noiseGenerator.GetAt(inter.TextureCoord.X, inter.TextureCoord.Y, out xGrad, out yGrad);

      Vector3d localNormal = Vector3d.TransformVector(inter.Normal, inter.WorldToLocal);
      Geometry.GetTangents(ref localNormal, out inter.TangentU, out inter.TangentV);

      Vector3d tu = Vector3d.TransformVector(inter.TangentU, inter.LocalToWorld).Normalized();
      Vector3d tv = Vector3d.TransformVector(inter.TangentV, inter.LocalToWorld).Normalized();

      inter.Normal += xGrad * scale * tu +
                      yGrad * scale * tv;
      inter.Normal.Normalize();

      return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));
    }
  }
}
