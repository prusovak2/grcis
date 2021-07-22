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
  public class SaturationFormula : ModuleFormula
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public SaturationFormula ()
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
    public override string Name => "SaturationFormula";

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

        float targetH = 120f;

        // targetH=<float>
        if (Util.TryParse(p, "targetH", ref targetH))
          targetH = Util.Clamp(targetH, 0, 359);

        Dictionary<string, object> sc = new Dictionary<string, object>();
        sc["targetH"] = targetH;
        sc["tooltip"] =  "tarhetH=<float>..[0,359], the main color to base a transformation on <default=120>";

        return sc;
      };

      // R <-> B channel swap with weights.
      f.pixelTransform0 = (
        in ImageContext ic,
        ref float R,
        ref float G,
        ref float B) =>
      {
        float targetH = 120f;
        Util.TryParse(ic.context, "targetH", ref targetH);

        Arith.RGBtoHSV(R, G, B, out float H, out float S, out float V);

        float Hdistance = Math.Abs(H-targetH);
        float Vdistance = Math.Abs(V - 0.5f);
        const float max_distance = 359;


        S = S *(float)(1.1 *Math.Pow(Math.Cos(Hdistance/ max_distance),2));
        V = V *(float)(1.1 *Math.Pow(Math.Cos(Vdistance / 0.5f), 2));

        /*if(Hdistance > 30)
        {
           float Hshift = (H/12)+15;
           H = targetH + Hshift;
        }*/
        H = (targetH) + (float)((Math.Cos(Hdistance / max_distance) * 30));
        Arith.HSVtoRGB(H, S, V, out R, out G, out B);
        return true;
        
      };

      return f;
    }
  }
}
