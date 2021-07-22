using System;
using System.Collections.Generic;
using System.Drawing;
using Utilities;
using MathSupport;

namespace Modules
{
  /// <summary>
  /// ModuleFormulaInternal class - template for internal image filter
  /// defined pixel-by-pixel using lambda functions.
  /// </summary>
  public class FacesTransformationFormula : ModuleFormula
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public FacesTransformationFormula ()
    {
      param = "fast";
    }

    /// <summary>
    /// Author's full name (SurnameFirstname).
    /// </summary>
    public override string Author => "PrusovaKaterina";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "FaceTransfirmationFormula";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip =>
      "fast/slow .. fast/slow bitmap access\r";

    //====================================================
    //--- Formula defined directly in this source file ---

    /// <summary>
    /// Defines implicit formulas if available.
    /// </summary>
    /// <returns>null if formulas should be read from a script.</returns>
    protected override Formula GetFormula ()
    {
      Formula f = new Formula();

      // Text params -> script context.
      // Any global pre-processing is allowed here.
      f.contextCreate = (in Bitmap input, in string param) =>
      {
        if (string.IsNullOrEmpty(param))
          return null;

        Dictionary<string, string> p = Util.ParseKeyValueList(param);

        float coeff = 1.0f;

        // coeff=<float>
        if (Util.TryParse(p, "coeff", ref coeff))
          coeff = Util.Saturate(coeff);

        string type = "green";

        // type=<string>
        if (p.ContainsKey("type"))
        {
          type = p["type"];
        }

        Dictionary<string, object> sc = new Dictionary<string, object>();
        sc["coeff"] = coeff;
        sc["type"] = type;
        sc["tooltip"] = "coeff=<float> .. color shift coefficient (0.0 - no color change, 1.0 - complete color change)\r" +
                        "type...one of {red, green, blue, angry, smurf, ogre} .. type of color transformation to carry out (default=green)";

        return sc;
      };

      // R <-> B channel swap with weights.
      f.pixelTransform0 = (
        in ImageContext ic,
        ref float R,
        ref float G,
        ref float B) =>
      {
        const string red = "red";
        const string blue = "blue";
        const string green = "green";
        const string smurf = "smurf";
        const string ogre = "ogre";
        const string angry = "angry";

        float coeff = 0.9f;
        Util.TryParse(ic.context, "coeff", ref coeff);

        string type = "green";
        Util.TryParse(ic.context, "type", ref type);

        Arith.RGBtoHSV(R, G, B, out float H, out float S, out float V);

        float targetColor = 120;
        if ((type == green) || (type == ogre))
        {
          targetColor = 120;  
        }
        if((type == blue) || (type == smurf))
        {
          targetColor = 240;
        }
        if((type == red) || (type == angry))
        {
          targetColor = 0;
        }

        bool color_face = ((type == ogre) || (type == smurf) || (type == angry));

        //float diffAbsRG = Math.Abs(R-G);
        //float temp = Math.Max(Math.Max(R,G),B) - Math.Min(Math.Min(R,G),B);
        //bool is_face = ((0.0 <= H) && (H<= 50.0 ) && (0.23<= S) && (S <= 0.68) && (R > 40) && /*(G>40) && (B>10) && (R >G) && (R>B) /*&& (diffAbsRG>15)*/ );
        //bool is_face_R1 =((R>95)&&(G>40)&&(B>20)&&(temp>15)&&(diffAbsRG>15)&&(R>G) && (R>B));
        //bool is_face_R2 =((R>220)&&(G>210)&&(B>170)&&(diffAbsRG<=15)&&(B<R)&&(B<G));
        bool is_face = (((0.0 <= H) && (H<= 35.0 ) && (0.18<= S) && (S <= 0.50) && (V> 0.80)) || ((0.0 <= H) && (H<= 25.0 ) && (0.18<= S) && (S <= 0.85) &&(V> 0.11)) );
        
        if ((is_face && color_face)|| (!is_face) && (! color_face))
        {
          float distance = targetColor - H;
          H = H + (distance * coeff);
          Arith.HSVtoRGB(H, S, V, out R,out G, out B);
          return true;
        }
            
        return false;
      };

      // Test create function: sinc(r^2)
      f.pixelCreate = (
        in ImageContext ic,
        out float R,
        out float G,
        out float B) =>
      {
        // [x, y] in {0, 1]
        double x = ic.x / (double)Math.Max(1, ic.width  - 1);
        double y = ic.y / (double)Math.Max(1, ic.height - 1);

        // I need uniform scale (x-scale == y-scale) with origin at the image center.
        if (ic.width > ic.height)
        {
          // Landscape.
          x -= 0.5;
          y = ic.height * (y - 0.5) / ic.width;
        }
        else
        {
          // Portrait.
          x = ic.width * (x - 0.5) / ic.height;
          y -= 0.5;
        }

        // Custom scales.
        float freq = 12.0f;
        Util.TryParse(ic.context, "freq", ref freq);

        x *= freq;
        y *= freq;

        // Periodic function of r^2.
        double rr = x * x + y * y;
        bool odd = ((int)Math.Round(rr) & 1) > 0;

        // Simple color palette (yellow, blue).
        R = odd ? 0.0f : 1.0f;
        G = R;
        B = 1.0f - R;
      };

      return f;
    }
  }
}
