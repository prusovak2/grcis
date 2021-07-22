// Text params -> script context.
// Any global pre-processing is allowed here.
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

enum Color
{
  Red,
  Green,
  Blue
}
struct Color_prep
{
  public float main;
  public float additional;
}

formula.contextCreate = (in Bitmap input, in string param) =>
{
  if (string.IsNullOrEmpty(param))
    return null;

  Dictionary<string, string> p = Util.ParseKeyValueList(param);

  Color color = Color.Blue;
  int col_index=0;
  if (Util.TryParse(p, "color", ref col_index)){
        int col = col_index % 3;
        switch (col)
        {
          case 0:
          {
              color = Color.Red;
              break;
          }
          case 1:
          {
              color = Color.Green;
              break;
          }
          default:
              color = Color.Blue;
              break;
        }
  }
  int iter_count = 10000;
  bool smooth = true;
  Color_prep[] palette = new Color_prep[iter_count];
  Util.TryParse(p, "smooth", ref smooth);
  if (!smooth)
  {
    for (int i = 0; i < iter_count; i++)
    {

      float main = (float)(i / (i + 8f));
      float additional = (float)i / 256f;
      palette[i].main = main;
      palette[i].additional = additional;
    }
  }
  Dictionary<string, object> sc = new Dictionary<string, object>();
  sc["color"] = color;
  sc["smooth"] = smooth;
  sc["palette"] = palette;
  sc["iter_count"] = iter_count;
  sc["tooltip"] = "color=<int> .. color%3=x, x=0 => red, x=1 => green, x=2 => blue, (default=blue)\r" +
                  "smooth=<bool> .. nicer picture, bit more demanding computation (default=true)";

  return sc;
};

// R <-> B channel swap with weights.
formula.pixelTransform0 = (
  in ImageContext ic,
  ref float R,
  ref float G,
  ref float B) =>
{
  float coeff = 0.0f;
  Util.TryParse(ic.context, "coeff", ref coeff);

  float r = Util.Saturate(R * (1.0f - coeff) + B * coeff);
  float b = Util.Saturate(R * coeff          + B * (1.0f - coeff));
  R = r;
  B = b;

  // Output color was modified.
  return true;
};

// Test create function: sinc(r^2)
formula.pixelCreate = (
  in ImageContext ic,
  out float R,
  out float G,
  out float B) =>
{
  int iter_count = 0;
  int maxiter = 10000;
  Util.TryParse(ic.context, "iter_count", ref maxiter);
  double escape_radius = 40d;

  double Re0 = (ic.x - ic.width / 2.0) * 4.0 / ic.width;
  double Imag0 = (ic.y - ic.height / 2.0) * 4.0 / ic.width;
  double Re = 0;
  double Imag = 0;
  double modulus = 0;
  bool in_set = true;
  while (true) {
    //Z = Z * Z + C;
    double Re_temp = (Re * Re) - (Imag * Imag) + Re0;  //Re^2
    Imag = (2 * Re * Imag) + Imag0; //Imag^2
    Re = Re_temp;
    iter_count++;
    modulus = (Re * Re) + (Imag * Imag); // |Z|^2
    if (modulus > escape_radius)
    {
      //modulus is too large, pixel is not a member of Mandelbrot set
      in_set = false;
      break;
    }
    if (iter_count > maxiter)
    {
      //max iteration count reached, escape radius was not exceeded, pixel is likely to belong to Mandelbrot set
      in_set = true;
      break;
    }
  }
  if (in_set)
  {
    //color pixels in set black
    R = B = G = 0;
  }
  else
  {
    bool smooth = (bool)ic.context["smooth"];
    float main;
    float additional;
    float halfmain;
    if (!smooth)
    {
      Color_prep[] palette = (Color_prep[])ic.context["palette"];
      additional = palette[iter_count - 1].additional;
      main = palette[iter_count - 1].main;
      halfmain = main;
    }
    else
    {
      // n + 1 - log (log  |Z(n)|) / log 2
      double col = iter_count - Math.Log(Math.Log(Math.Sqrt(modulus))) / Math.Log(2);
      main = (float)(col / (col + 8f));
      additional = (float)col / 256f;
      halfmain = (float)col / 2*256f;
    }
    

    Color color = (Color)ic.context["color"]; 
    switch (color)
    {
      case Color.Red:
        {
          G = B = main;
          R = additional;
          break;
        }
      case Color.Green:
        {
          R = B = additional;
          G = main;
          break;
        }
      default:
        {
            R = G = additional;
            B = main;
            break;
        } 
    }
  }
};

