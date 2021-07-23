using OpenTK;

namespace VojtechCerny {
  public interface INoise {
    double GetAt (double x, double y);
    double GetAt (Vector2d vec);
  }

  public interface INoise3D {
    double GetAt (double x, double y, double z);
    double GetAt (Vector3d vec);
  }
}
