using OpenTK;

namespace VojtechCerny {
  public interface INoiseWithGradient : INoise {
    double GetAt (double x, double y, out double xD, out double yD);

    double GetAt (Vector2d vec, out double xD, out double yD);
  }

  public interface INoiseWithGradient3D : INoise3D {
    double GetAt (double x, double y, double z, out double xD, out double yD, out double zD);

    double GetAt (Vector3d vec, out double xD, out double yD, out double zD);
  }
}
