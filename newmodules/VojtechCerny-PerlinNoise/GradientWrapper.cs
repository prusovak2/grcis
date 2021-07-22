using OpenTK;

namespace VojtechCerny {
  /// <summary>
  /// A simple wrapper around INoise that turns it into INoiseWithGradient - calculates gradient by sampling close neighborhood.
  /// </summary>
  public class GradientWrapper : INoiseWithGradient {
    protected INoise noise;
    protected double delta;

    public GradientWrapper (INoise noise, double delta = 1e-6) {
      this.noise = noise;
      this.delta = delta;
    }

    public double GetAt (double x, double y) {
      return noise.GetAt(x, y);
    }

    public double GetAt (Vector2d vec) {
      return GetAt(vec.X, vec.Y);
    }

    public double GetAt (double x, double y, out double xD, out double yD) {
      double value = noise.GetAt(x, y);
      xD = (noise.GetAt(x + delta, y) - value) / delta;
      yD = (noise.GetAt(x, y + delta) - value) / delta;
      return value;
    }

    public double GetAt (Vector2d vec, out double xD, out double yD) {
      return GetAt(vec.X, vec.Y, out xD, out yD);
    }
  }

  public class GradientWrapper3D : INoiseWithGradient3D {
    protected INoise3D noise;
    protected double delta;

    public GradientWrapper3D (INoise3D noise, double delta = 1e-6) {
      this.noise = noise;
      this.delta = delta;
    }

    public double GetAt (double x, double y, double z) {
      return noise.GetAt(x, y, z);
    }

    public double GetAt (Vector3d vec) {
      return GetAt(vec.X, vec.Y, vec.Z);
    }

    public double GetAt (double x, double y, double z, out double xD, out double yD, out double zD) {
      double value = noise.GetAt(x, y, z);
      xD = (noise.GetAt(x + delta, y, z) - value) / delta;
      yD = (noise.GetAt(x, y + delta, z) - value) / delta;
      zD = (noise.GetAt(x, y, z + delta) - value) / delta;
      return value;
    }

    public double GetAt (Vector3d vec, out double xD, out double yD, out double zD) {
      return GetAt(vec.X, vec.Y, vec.Z, out xD, out yD, out zD);
    }
  }
}
