using OpenTK;
using System;
using System.Collections.Generic;

namespace VojtechCerny {
  /// <summary>
  /// 2D and 3D Perlin noise, based off of this video https://www.youtube.com/watch?v=iW4nFygKAjw and Perlin's improved noise implementation https://mrl.nyu.edu/~perlin/noise/.
  /// 2D variant features analytical derivatives from Inigo Quilez's blog https://www.iquilezles.org/www/articles/gradientnoise/gradientnoise.htm
  /// </summary>
  public class PerlinNoise : INoiseWithGradient, INoise3D {
    /// <summary>
    /// A single octave of this noise.
    /// </summary>
    public class Octave : INoiseWithGradient, INoise3D {
      double frequency;
      double amplitude;
      protected double xOffset;
      protected double yOffset;
      protected double zOffset;

      static readonly int[] p = { 151, 160, 137, 91, 90, 15, 131, 13, 201,
        95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37,
        240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
        94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56,
        87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139,
        48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133,
        230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25,
        63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
        196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
        64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255,
        82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
        223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
        101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79,
        113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242,
        193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249,
        14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204,
        176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222,
        114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
      };
      // perm is just twice repeated p, this is done to not deal with index overflows in noise calculation
      static readonly int[] perm = new int[512];

      static Octave() {
        for (int i = 0; i < 256; ++i) {
          perm[i] = perm[i + 256] = p[i];
        }
      }

      public Octave(double frequency, double amplitude, double xOffset = 0.0, double yOffset = 0.0, double zOffset = 0.0) {
        this.frequency = frequency;
        this.amplitude = amplitude;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.zOffset = zOffset;
      }

      #region 2D
      public double GetAt (double x, double y) {
        return GetAt(x, y, out _, out _, false);
      }
      public double GetAt (Vector2d vec) {
        return GetAt(vec.X, vec.Y);
      }

      public double GetAt (double x, double y, out double xD, out double yD) {
        return GetAt(x, y, out xD, out yD, true);
      }

      public double GetAt (Vector2d vec, out double xD, out double yD) {
        return GetAt(vec.X, vec.Y, out xD, out yD);
      }

      protected double GetAt (double x, double y, out double xD, out double yD, bool calculateDerivatives) {
        // Offset and rescale coordinates
        x += xOffset;
        y += yOffset;
        x *= frequency;
        y *= frequency;

        // Get square coordinates (mod 256)
        int squareX = (int) Math.Floor(x) & 255;
        int squareY = (int) Math.Floor(y) & 255;

        // Hashes of corners of this square
        int hash1 = Hash(squareX, squareY);
        int hash2 = Hash(squareX + 1, squareY);
        int hash3 = Hash(squareX, squareY + 1);
        int hash4 = Hash(squareX + 1, squareY + 1);

        // Floating-point coordinates wrt square
        double xf = x - Math.Floor(x);
        double yf = y - Math.Floor(y);

        // Dot products with gradients from each corner (step 1 of Perlin noise)
        double d1 = Grad(hash1, xf, yf);
        double d2 = Grad(hash2, xf - 1, yf);
        double d3 = Grad(hash3, xf, yf - 1);
        double d4 = Grad(hash4, xf - 1, yf - 1);

        // Perform bilinear interpolation with smoothstep
        double u = Fade(xf);
        double v = Fade(yf);

        double xInterpolation1 = Lerp(u, d1, d2);
        double xInterpolation2 = Lerp(u, d3, d4);
        double yInterpolation = Lerp(v, xInterpolation1, xInterpolation2);

        if (calculateDerivatives) {
          // Interpolate the derivatives
          double ud = FadeDeriv(xf);
          double vd = FadeDeriv(yf);

          Vector2d g1 = GradVector(hash1);
          Vector2d g2 = GradVector(hash2);
          Vector2d g3 = GradVector(hash3);
          Vector2d g4 = GradVector(hash4);

          Vector2d xInterp1D = Lerp(u, g1, g2);
          Vector2d xInterp2D = Lerp(u, g3, g4);
          Vector2d yInterpD = Lerp(v, xInterp1D, xInterp2D);

          yInterpD.X += ud * (v * (d1 - d2 - d3 + d4) + d2 - d1);
          yInterpD.Y += vd * (u * (d1 - d2 - d3 + d4) + d3 - d1);

          xD = yInterpD.X;
          yD = yInterpD.Y;
        } else {
          xD = 0.0;
          yD = 0.0;
        }

        return yInterpolation * amplitude;
      }

      /// <summary>
      /// Utility method that computes dot-product of given coordinates with a random diagonal vector (based on hash).
      /// </summary>
      protected static double Grad (int hash, double x, double y) {
        switch (hash & 3) {
          case 0: return x + y;
          case 1: return -x + y;
          case 2: return x - y;
          case 3: return -x - y;
          // Never happens
          default: return 0;
        }
      }

      int Hash (int squareX, int squareY) {
        return perm[perm[squareX] + squareY];
      }

      protected static Vector2d GradVector (int hash) {
        switch (hash & 3) {
          case 0:
            return new Vector2d(1.0, 1.0);
          case 1:
            return new Vector2d(-1.0, 1.0);
          case 2:
            return new Vector2d(1.0, -1.0);
          case 3:
            return new Vector2d(-1.0, -1.0);
          // Never happens
          default:
            return Vector2d.Zero;
        }
      }

      #endregion

      #region 3D
      public double GetAt (double x, double y, double z) {
        // Offset and rescale coordinates
        x += xOffset;
        y += yOffset;
        z += zOffset;
        x *= frequency;
        y *= frequency;
        z *= frequency;

        // Get cube coordinates (mod 256)
        int cubeX = (int) Math.Floor(x) & 255;
        int cubeY = (int) Math.Floor(y) & 255;
        int cubeZ = (int) Math.Floor(z) & 255;

        // Hashes of corners of this square
        int hash1 = Hash(cubeX, cubeY, cubeZ);
        int hash2 = Hash(cubeX + 1, cubeY, cubeZ);
        int hash3 = Hash(cubeX, cubeY + 1, cubeZ);
        int hash4 = Hash(cubeX + 1, cubeY + 1, cubeZ);
        int hash5 = Hash(cubeX, cubeY, cubeZ + 1);
        int hash6 = Hash(cubeX + 1, cubeY, cubeZ + 1);
        int hash7 = Hash(cubeX, cubeY + 1, cubeZ + 1);
        int hash8 = Hash(cubeX + 1, cubeY + 1, cubeZ + 1);

        // Floating-point coordinates wrt cube
        double xf = x - Math.Floor(x);
        double yf = y - Math.Floor(y);
        double zf = z - Math.Floor(z);

        // Dot products with gradients from each corner (step 1 of Perlin noise)
        double d1 = Grad(hash1, xf, yf, zf);
        double d2 = Grad(hash2, xf - 1, yf, zf);
        double d3 = Grad(hash3, xf, yf - 1, zf);
        double d4 = Grad(hash4, xf - 1, yf - 1, zf);
        double d5 = Grad(hash5, xf, yf, zf - 1);
        double d6 = Grad(hash6, xf - 1, yf, zf - 1);
        double d7 = Grad(hash7, xf, yf - 1, zf - 1);
        double d8 = Grad(hash8, xf - 1, yf - 1, zf - 1);

        // Perform trilinear interpolation with smoothstep
        double u = Fade(xf);
        double v = Fade(yf);
        double w = Fade(zf);

        double xInterpolation1 = Lerp(u, d1, d2);
        double xInterpolation2 = Lerp(u, d3, d4);
        double xInterpolation3 = Lerp(u, d5, d6);
        double xInterpolation4 = Lerp(u, d7, d8);
        double yInterpolation1 = Lerp(v, xInterpolation1, xInterpolation2);
        double yInterpolation2 = Lerp(v, xInterpolation3, xInterpolation4);
        double zInterpolation = Lerp(w, yInterpolation1, yInterpolation2);

        return zInterpolation * amplitude;
      }

      public double GetAt (Vector3d vec) {
        return GetAt(vec.X, vec.Y, vec.Z);
      }

      int Hash(int cubeX, int cubeY, int cubeZ) {
        return perm[perm[perm[cubeX] + cubeY] + cubeZ];
      }

      /// <summary>
      /// Utility method that computes dot-product of given coordinates with a random diagonal vector (based on hash).
      /// Gives the same result as Perlin's original bit-tricks notation, but more readable (and shouldn't be slower).
      /// </summary>
      protected static double Grad (int hash, double x, double y, double z) {
        switch (hash & 15) {
          case 0: return x + y;
          case 1: return -x + y;
          case 2: return x - y;
          case 3: return -x - y;
          case 4: return x + z;
          case 5: return -x + z;
          case 6: return x - z;
          case 7: return -x - z;
          case 8: return y + z;
          case 9: return -y + z;
          case 10: return y - z;
          case 11: return -y - z;
          case 12: return x + y;
          case 13: return -y + z;
          case 14: return -x + y;
          case 15: return -y - z;
          // Never happens
          default: return 0;
        }
      }
      #endregion


      /// <summary>
      /// Fade function - smootherstep, expects a number to be in range between 0.0 and 1.0.
      /// </summary>
      protected static double Fade (double x) { 
        return x * x * x * (x * (x * 6 - 15) + 10);
      }

      /// <summary>
      /// Derivation of fade function (smootherstep), number still expected between 0.0 and 1.0.
      /// </summary>
      protected static double FadeDeriv (double x) {
        return 30 * x * x * (x * (x - 2) + 1);
      }

      protected static double Lerp (double x, double edge0, double edge1) {
        return edge0 + x * (edge1 - edge0);
      }

      protected static Vector2d Lerp (double x, Vector2d edge0, Vector2d edge1) {
        return edge0 + x * (edge1 - edge0);
      }
    }

    bool isNormalized;
    double amplitudeSum = 0.0;
    List<Octave> octaves = new List<Octave>();

    /// <summary>
    /// Constructs the Perlin Noise.
    /// </summary>
    /// <param name="numOctaves">Number of octaves to use internally. Larger number increases the amount of small details.</param>
    /// <param name="frequencyMultiplier">How much is the frequency (of the gradient grid) multiplied with each octave</param>
    /// <param name="amplitudeMultiplier">How much is the amplitude (of output) multiplied with each octave.</param>
    /// <param name="baseFrequency">Frequency of first octave</param>
    /// <param name="baseAmplitude">Amplitude of first octave</param>
    /// <param name="isNormalized">If true, the noise will be scaled to the <-1, 1> interval</param>
    /// <param name="seed"/>Seed for randomness. If null, a time-dependent seed will be used.</param>
    public PerlinNoise (int numOctaves,
                        double frequencyMultiplier,
                        double amplitudeMultiplier,
                        double baseFrequency = 1.0,
                        double baseAmplitude = 1.0,
                        bool isNormalized = true,
                        int? seed = null) {

      Random random = seed == null ? new Random() : new Random((int) seed);

      double amplitude = baseAmplitude;
      double frequency = baseFrequency;

      for (int i = 0; i < numOctaves; ++i) {
        octaves.Add(new Octave(frequency, amplitude, random.NextDouble() * 1_000_000, random.NextDouble() * 1_000_000));
        amplitudeSum += amplitude;
        
        amplitude *= amplitudeMultiplier;
        frequency *= frequencyMultiplier;
      }

      this.isNormalized = isNormalized;
    }

    public double GetAt (double x, double y) {
      double noise = 0;
      foreach (var octave in octaves) {
        noise += octave.GetAt(x, y);
      }

      if (isNormalized) noise /= amplitudeSum;
      return noise;
    }

    public double GetAt (Vector2d vec) {
      return GetAt(vec.X, vec.Y);
    }

    public double GetAt (double x, double y, out double xD, out double yD) {
      double noise = 0;
      xD = 0.0;
      yD = 0.0;

      foreach (var octave in octaves) {
        double singleXd, singleYd;
        noise += octave.GetAt(x, y, out singleXd, out singleYd);
        xD += singleXd;
        yD += singleYd;
      }

      if (isNormalized) {
        noise /= amplitudeSum;
        xD /= amplitudeSum;
        yD /= amplitudeSum;
      }
      return noise;
    }

    public double GetAt (Vector2d vec, out double xD, out double yD) {
      return GetAt(vec.X, vec.Y, out xD, out yD);
    }

    public double GetAt (double x, double y, double z) {
      double noise = 0;
      foreach (var octave in octaves) {
        noise += octave.GetAt(x, y, z);
      }

      if (isNormalized) noise /= amplitudeSum;
      return noise;
    }

    public double GetAt (Vector3d vec) {
      return GetAt(vec.X, vec.Y, vec.Z);
    }
  }
}
