using System;
using System.Collections.Generic;
using System.Diagnostics;
using MathSupport;
using OpenTK;
using Utilities;
using Rendering;
using Rendering.JanMatejka;
using VojtechCerny;

namespace Rendering
{
  namespace KaterinaPrusova
  {
    public class OrangePeelBumpTexture : ITexture
    {

      private PerlinNew PerlinNoise = new PerlinNew();
      private double Freq; // 4 to 14, best 9
      private double Ampl;  // 0.001 to 0.006 best 0.004
      private double Struc; // 0.4 to 1.2 best 0.8

      private Vector3d StartColor = new Vector3d(1d, 200/255d, 0);
      private Vector3d EndColor = new Vector3d(204/255d, 0, 0);
      private Vector3d Diff => EndColor - StartColor;

      public OrangePeelBumpTexture(int seed = 73, double freq = 8, double amplitude =  0.006, double struc = 1.4)
      {
        this.PerlinNoise = new PerlinNew(seed);
        this.Freq = freq;
        this.Ampl = amplitude;
        this.Struc = struc;
      }

      public long Apply (Intersection inter)
      {

        inter.SurfaceColor = new double[] { 1.0, 0.45, 0 };


        return MakeBumps(inter);
      }

      /// <summary>
      /// Modifies the surface normal so that the orange seems bumpy
      /// </summary>
      /// <param name="inter"></param>
      /// <returns></returns>
      private long MakeBumps (Intersection inter)
      {
        double Xlocal = inter.CoordLocal.X;
        double Ylocal = inter.CoordLocal.Y;
        double Zlocal = inter.CoordLocal.Z;
        double radius = Math.Sqrt(Xlocal*Xlocal + Ylocal*Ylocal + Zlocal*Zlocal);
        double[] normal = new double[3];
        normal[0] = Xlocal / radius;
        normal[1] = Ylocal / radius;
        normal[2] = -1 * Math.Sqrt(1 - normal[0] * normal[0] - normal[1] * normal[1]);

        // approximate the gradient of the noise function at [x, y, z]
        double epsilon = 0.00000000001;
        double f0 = NoiseWithFrequency(normal[0], normal[1], normal[2], this.Freq);
        double fx = NoiseWithFrequency(normal[0]+epsilon, normal[1], normal[2], this.Freq);
        double fy = NoiseWithFrequency(normal[0], normal[1]+epsilon, normal[2], this.Freq);
        double fz = NoiseWithFrequency(normal[0], normal[1], normal[2]+ epsilon, this.Freq);

        double gradX = (fx - f0) / epsilon;
        double gradY = (fy - f0) / epsilon;
        double gradZ = (fz - f0) / epsilon;

        // Modify the surface normal by subtracting the noise gradient
        // Suggested by Ken Perlin https://developer.download.nvidia.com/books/HTML/gpugems/gpugems_ch05.html
        // Even thought Blinn claims it to be incorrect http://elibrary.lt/resursai/Leidiniai/Litfund/Lithfund_leidiniai/IT/Texturing.and.Modeling.-.A.Procedural.Approach.3rd.edition.eBook-LRN.pdf page 171
        // and suggests other approach https://www.semanticscholar.org/paper/Simulation-of-wrinkled-surfaces-Blinn/83fb6d2721cd26be4964882ce929ff8c98ebb688
        // Which did not work for me in case of 3D texture 
        normal[0] -= gradX;
        normal[1] -= gradY;
        normal[2] -= gradZ;
        Normalize(normal);

        inter.NormalLocal = new Vector3d(normal[0], normal[1], normal[2]);
        inter.Normal = Vector3d.TransformVector(inter.NormalLocal, inter.LocalToWorld).Normalized();

        inter.textureApplied = true;
        return (long)RandomStatic.numericRecipes((ulong)(f0 * 1000000));
      }

      /// <summary>
      /// Allows to influence the frequency of the noise 
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <param name="z"></param>
      /// <param name="freq"></param>
      /// <returns></returns>
      private double NoiseWithFrequency(double x, double y, double z, double freq)
      {
        double d = this.Struc;
        double x1, y1, z1;
        x1 = d * x - d * z;
        z1 = d * x + d * z;
        y1 = d * x1 + d * y;
        x1 = d * x1 - d * y;
        return this.Ampl * PerlinNoise.OctavePerlin(freq * x1 + 100, freq * y1, freq * z1, 2, 2.5);
      }

      private static void Normalize (double[] v)
      {
        double norm = Math.Sqrt(v[0]*v[0] + v[1]*v[1] + v[2]*v[2]);
        v[0] /= norm;
        v[1] /= norm;
        v[2] /= norm;
      }

      /// <summary>
      /// Implementation of Perlin improved noise function https://developer.download.nvidia.com/books/HTML/gpugems/gpugems_ch05.html
      /// Taken from here https://gist.github.com/Flafla2/1a0b9ebef678bbce3215 and slightly adjusted
      /// </summary>
      public class PerlinNew
      {
        private Random random;

        public PerlinNew (int seed = 73)
        {
          this.random = new Random(seed);
          Shuffle(this.permutation);
        }

        public double OctavePerlin (double x, double y, double z, int octaves, double persistence)
        {
          double total = 0;
          double frequency = 1;
          double amplitude = 1;
          for (int i = 0; i < octaves; i++)
          {
            total += perlin(x * frequency, y * frequency, z * frequency) * amplitude;

            amplitude *= persistence;
            frequency *= 2;
          }
          return total;
        }

        /// <summary>
        /// Get scalar noise value at point [<paramref name="x"/>,<paramref name="y"/>, <paramref name="z"/>] in 3D space
        /// </summary>
        /// <returns></returns>
        public double perlin (double x, double y, double z)
        {
          int xi = ((int)(Math.Floor(x) % 256) + 256) % 256;
          int yi = ((int)(Math.Floor(y) % 256) + 256) % 256;
          int zi = ((int)(Math.Floor(z) % 256) + 256) % 256;

          double xf = x - (int)x;
          if (x < 0)
            xf = 1 + xf;
          double yf = y - (int)y;
          if (y < 0)
            yf = 1 + yf;
          double zf = z - (int)z;
          if (z < 0)
            zf = 1 + zf;


          double u = fade(xf);
          double v = fade(yf);
          double w = fade(zf);

          int a  = permutation[xi]  +yi;                // This here is Perlin's hash function.  We take our x value (remember,
          int aa = permutation[a]   +zi;                // between 0 and 255) and get a random value (from our p[] array above) between
          int ab = permutation[a+1] +zi;                // 0 and 255.  We then add y to it and plug that into p[], and add z to that.
          int b  = permutation[xi+1]+yi;                // Then, we get another random value by adding 1 to that and putting it into p[]
          int ba = permutation[b]   +zi;                // and add z to it.  We do the whole thing over again starting with x+1.  Later
          int bb = permutation[b+1] +zi;                // we plug aa, ab, ba, and bb back into p[] along with their +1's to get another set.
                                                        // in the end we have 8 values between 0 and 255 - one for each vertex on the unit cube.
                                                        // These are all interpolated together using u, v, and w below.

          double x1, x2, y1, y2;
          x1 = lerp(grad(permutation[aa], xf, yf, zf),      // This is where the "magic" happens.  We calculate a new set of p[] values and use that to get
                grad(permutation[ba], xf - 1, yf, zf),      // our final gradient values.  Then, we interpolate between those gradients with the u value to get
                u);                   // 4 x-values.  Next, we interpolate between the 4 x-values with v to get 2 y-values.  Finally,
          x2 = lerp(grad(permutation[ab], xf, yf - 1, zf),      // we interpolate between the y-values to get a z-value.
                grad(permutation[bb], xf - 1, yf - 1, zf),
                u);                   // When calculating the p[] values, remember that above, p[a+1] expands to p[xi]+yi+1 -- so you are
          y1 = lerp(x1, x2, v);               // essentially adding 1 to yi.  Likewise, p[ab+1] expands to p[p[xi]+yi+1]+zi+1] -- so you are adding
                                              // to zi.  The other 3 parameters are your possible return values (see grad()), which are actually
          x1 = lerp(grad(permutation[aa + 1], xf, yf, zf - 1),    // the vectors from the edges of the unit cube to the point in the unit cube itself.
                grad(permutation[ba + 1], xf - 1, yf, zf - 1),
                u);
          x2 = lerp(grad(permutation[ab + 1], xf, yf - 1, zf - 1),
                      grad(permutation[bb + 1], xf - 1, yf - 1, zf - 1),
                      u);
          y2 = lerp(x1, x2, v);

          return (lerp(y1, y2, w) + 1) / 2;           // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
        }

        private void Shuffle<T> (IList<T> toShuffle)
        {
          int n = toShuffle.Count;
          while (n > 1)
          {
            n--;
            int k = random.Next(n + 1);
            T value = toShuffle[k];
            toShuffle[k] = toShuffle[n];
            toShuffle[n] = value;
          }
        }

        private static double grad (int hash, double x, double y, double z)
        {
          int h = hash & 15;                  // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
          double u = h < 8 /* 0b1000 */ ? x : y;        // If the most signifigant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.

          double v;                     // In Ken Perlin's original implementation this was another conditional operator (?:).  I
                                        // expanded it for readability.

          if (h < 4 /* 0b0100 */)               // If the first and second signifigant bits are 0 set v = y
            v = y;
          else if (h == 12 /* 0b1100 */ || h == 14 /* 0b1110*/)// If the first and second signifigant bits are 1 set v = x
            v = x;
          else                        // If the first and second signifigant bits are not equal (0/1, 1/0) set v = z
            v = z;

          return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v); // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
        }

        private static double fade (double t)
        {
          // Fade function as defined by Ken Perlin.  This eases coordinate values
          // so that they will "ease" towards integral values.  This ends up smoothing
          // the final output.
          return t * t * t * (t * (t * 6 - 15) + 10);     // 6t^5 - 15t^4 + 10t^3
        }

        private static double lerp (double a, double b, double x)
        {
          return a + x * (b - a);
        }

        /// <summary>
        /// Hash lookup table as defined by Ken Perlin.  
        /// This is a randomly arranged array of all numbers from 0-255 inclusive.
        /// The array is doubled to avoid using division (%) when indexing in the Hash function
        /// </summary>
        private int[] permutation = { 151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,

        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
      };
      }
    }
  }
}
