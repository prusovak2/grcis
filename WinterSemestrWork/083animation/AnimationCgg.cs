﻿using CircleCanvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using Utilities;
using MathSupport;

namespace _083animation
{
  /// <summary>
  /// Solid color disc.
  /// </summary>
  public class Disc
  {
    public double cx;
    public double cy;
    public double radius;
    public Color color;

    public Disc (double x, double y, double r, Color c)
    {
      cx = x;
      cy = y;
      radius = r;
      color = c;
    }
  }

  public class DiscPair
  {
    public Disc OriginalDisc;
    public Disc TargetDisc;
    public double MiddleX;
    public double MiddleY;
    public int MovementTimeMiddle;

    public DiscPair(Disc original, Disc target, double mx, double my, int mTm)
    {
      this.OriginalDisc = original;
      this.TargetDisc = target;
      this.MiddleX = mx;
      this.MiddleY = my;
      this.MovementTimeMiddle = mTm;
    }
  }

  public class Animation
  {
    /// <summary>
    /// CGG logo colors.
    /// </summary>
    protected static Color[] COLORS =
    {
      Color.FromArgb( 0x71, 0x21, 0x6d ),
      Color.FromArgb( 0xe8, 0x75, 0x05 )
    };

    /// <summary>
    /// Final discs' geometries { cx, cy, radius }.
    /// </summary>
    protected static double[,] DISC_DATA = new double[,]
    {
      {  59.2317,  77.2244, 2.1480 },
      {  29.5167,  69.7424, 4.1070 },
      {  50.0857,  90.1954, 4.4050 },
      {  29.5177,  49.3654, 3.0170 },
      {  38.0227,  87.1904, 3.5160 },
      {  53.8837,  98.0334, 3.5160 },
      {  78.9077,  97.6174, 3.5160 },
      {  36.4977,  72.5804, 1.9350 },
      {  60.7857,  93.7054, 2.1020 },
      {  86.1887,  94.2194, 3.5160 },
      {  40.5107,  79.7254, 2.1480 },
      {  37.2237,  35.1484, 2.1480 },
      {  66.0817, 100.9204, 2.1490 },
      {  70.4577, 101.4664, 1.5330 },
      {  56.2357,  92.1694, 1.5330 },
      {  44.7487,  77.3954, 1.5330 },
      {  54.0487,  84.2384, 1.5320 },
      {  76.9757,  90.9514, 1.5330 },
      {  82.2157,  88.7444, 1.5330 },
      {  67.4197,  88.4774, 1.5320 },
      {  48.5097,  78.6024, 1.2590 },
      {  50.6307,  83.4184, 1.2590 },
      {  51.5867,  77.6684, 1.2600 },
      {  53.6277,  74.7974, 1.1050 },
      {  54.0487,  71.5134, 0.8840 },
      {  53.0627,  69.0624, 0.6160 },
      {  92.6397,  85.9514, 1.0090 },
      {  92.1357,  90.4004, 2.6920 },
      {  97.6217,  83.6214, 3.5170 },
      { 102.7087,  71.6204, 3.5160 },
      { 101.3077,  78.2894, 2.1480 },
      {  96.8637,  77.4394, 1.3270 },
      {  99.2907,  65.3304, 2.1480 },
      {  76.2987,  67.0884, 1.4210 },
      {  83.8597,  64.4754, 1.7710 },
      {  92.1817,  64.6674, 2.5340 },
      {  87.6617,  68.6074, 2.1490 },
      {  96.4247,  69.4334, 2.1470 },
      {  83.3117,  68.7374, 1.3950 },
      {  79.0617,  63.9114, 1.5330 },
      {  92.9847,  73.0534, 1.5320 },
      {  92.0437,  69.1544, 1.2590 },
      {  99.6267,  61.2294, 1.2600 },
      { 105.5807,  61.2294, 1.2590 },
      { 103.2607,  59.8614, 0.6160 },
      {  87.6187,  64.7184, 1.1640 },
      {  96.0087,  62.6084, 0.9250 },
      { 102.5717,  62.1864, 0.9240 },
      {  79.0617,  70.0544, 0.9860 },
      {  88.7847,  86.8944, 0.9870 },
      {  70.8887,  94.6564, 4.1510 },
      { 104.8957,  65.4684, 2.1480 },
      {  62.5257,  36.2424, 1.2590 },
      {  48.6047,  72.8824, 3.4750 },
      {  42.8639,  93.2835, 2.1470 },
      { 105.0609,  57.4165, 1.2600 },
      { 104.3889,  53.7185, 0.9240 },
      {  33.3619,  41.6875, 4.0540 },
      {  38.0979,  66.9685, 2.6320 },
      {  47.2959,  32.9445, 3.4930 },
      {  54.0759,  26.2755, 3.5160 },
      {  59.2599,  83.8605, 2.5450 },
      {  53.2559,  55.6815, 3.5160 },
      {  35.6559,  56.9115, 2.1480 },
      {  41.4969,  74.1405, 1.9350 },
      {  38.6299,  49.7946, 2.1470 },
      {  54.0709,  33.2505, 2.1490 },
      {  60.3729,  23.6845, 2.1480 },
      {  68.1619,  22.9995, 2.1480 },
      {  79.6469,  25.4605, 2.1470 },
      {  73.0149,  39.6136, 1.5330 },
      {  43.2439,  65.3215, 1.5330 },
      {  35.4669,  61.7655, 1.5330 },
      {  41.2919,  30.9945, 1.5320 },
      {  47.6439,  26.8275, 1.5330 },
      {  41.7009,  36.4695, 1.5330 },
      {  43.6149,  88.2946, 1.5330 },
      {  46.5559,  96.9765, 1.5330 },
      {  58.9989, 100.9415, 1.5320 },
      {  43.8209,  83.8495, 1.5320 },
      {  56.9479,  88.0876, 1.5320 },
      {  64.4689,  96.8395, 1.5330 },
      {  60.9139,  89.0446, 1.5330 },
      {  63.9609,  85.3825, 1.5330 },
      {  65.5629,  47.3385, 1.2590 },
      {  41.6339,  40.9125, 1.2590 },
      {  84.5749,  26.4135, 1.2590 },
      {  88.1299,  28.6055, 1.2590 },
      {  68.7079,  30.5215, 1.2590 },
      {  46.5549,  44.6055, 1.2590 },
      {  31.9809,  54.1835, 1.2590 },
      {  91.4069,  31.3415, 1.2590 },
      {  72.6149,  22.7615, 1.2600 },
      {  95.5089,  35.7165, 1.2590 },
      {  43.2449,  70.0016, 1.2590 },
      {  45.9389,  67.7875, 0.8280 },
      {  38.3909,  76.1325, 0.8290 },
      {  33.5079,  64.9095, 1.0120 },
      {  62.4469,  80.0485, 0.9860 },
      {  64.4689,  92.1225, 0.9860 },
      {  74.2449, 100.9415, 0.9850 },
      {  59.3819,  96.9765, 0.9850 },
      {  47.6159,  83.8645, 0.9850 },
      {  51.0689,  67.7256, 0.6160 },
      {  48.5379,  67.1715, 0.6160 },
      {  50.6569,  80.4176, 0.6160 },
      {  52.8739,  81.3165, 0.6160 },
      {  69.8029,  53.9026, 0.6150 },
      {  83.2019,  46.5195, 0.6160 },
      {  82.0219,  44.3315, 0.6160 },
      {  78.1429,  52.3985, 0.6160 },
      {  78.2799,  44.3305, 0.6160 },
      {  57.5629,  46.1085, 0.6160 },
      {  73.4259,  62.7285, 0.6150 },
      {  84.9129,  71.6425, 0.6160 },
      {  90.1479,  72.1605, 0.6160 },
      {  92.2099,  82.8805, 0.6160 },
      {  61.9379,  66.3465, 0.9250 },
      {  46.3099,  81.1435, 0.9240 },
      {  60.6739,  56.9105, 0.9240 },
      {  54.0769,  42.0065, 0.9230 },
      {  94.6879,  48.2965, 0.9240 },
      {  44.6419,  50.8955, 0.9240 },
      {  34.1619,  48.1945, 0.9240 },
      {  35.1529,  52.5565, 0.9250 },
      {  91.8169,  35.9225, 0.6150 },
      {  99.2009,  39.6815, 0.6170 },
      {  77.8699,  34.2125, 0.6160 },
      {  75.2609,  23.9505, 0.6160 },
      {  72.8099,  26.8285, 0.6150 },
      {  84.2959,  37.4935, 0.6160 },
      {  49.1539,  50.3475, 0.6150 },
      {  46.5549,  54.3135, 0.6160 },
      {  51.8889,  61.1505, 0.6160 },
      {  48.6329,  81.4155, 0.6150 },
      {  64.3029,  89.0446, 0.6160 },
      {  70.7589,  88.7715, 0.6160 },
      {  79.0259,  87.1266, 0.6160 },
      {  87.8229,  89.6195, 0.6160 },
      {  62.1439, 102.1725, 0.9860 },
      {  61.7329,  98.7536, 0.9850 },
      {  55.1709,  79.8825, 1.2590 },
      {  32.9449,  79.7035, 4.0080 },
      {  28.7889,  59.8885, 4.4040 },
      {  85.5819,  88.9855, 1.0100 },
      {  97.1309,  73.6945, 1.5330 },
      {  80.6229,  66.8545, 1.1050 },
      {  81.1029,  92.4005, 0.9860 },
      {  95.8939,  65.7575, 0.6160 },
      {  96.2149,  88.6015, 0.9240 },
      {  99.4209,  75.3705, 0.6160 },
      { 104.2069,  75.9176, 0.6170 },
      {  94.2499,  75.8786, 0.9860 },
      {  94.3039,  79.3955, 0.9850 },
      {  81.1029,  78.9485, 0.6150 },
      {  89.3899,  82.7275, 0.8290 },
      {  84.5739,  77.2465, 0.9860 }
    };

    /// <summary>
    /// Number of disc having the 1st color.
    /// </summary>
    protected const int FIRST_COLOR = 54;

    protected static double minX, maxX, minY, maxY;

    protected static double maxR;

    protected static List<Disc> LOGOdiscs;

    protected static List<Disc> CGGdiscs = new List<Disc>();

    protected static List<DiscPair> Pairs = new List<DiscPair>();

    protected static double kxy;

    protected static double dx, dy;

    protected static bool forward = true;

    protected const double SIZE = 0.9;

    protected static void SetViewport (int width, int height)
    {
      double k = (width * SIZE) / (maxX - minX);
      kxy = (height * SIZE) / (maxY - minY);
      if ( k < kxy ) kxy = k;
      dx = 0.5 * (width -  (minX + maxX) * kxy);
      dy = 0.5 * (height - (minY + maxY) * kxy);
    }

    protected static float X (double x)
    {
      return (float)(x * kxy + dx);
    }

    protected static float Y (double y)
    {
      return (float)(y * kxy + dy);
    }

    /// <summary>
    /// Initialize form parameters.
    /// </summary>
    public static void InitParams (out string name, out int wid, out int hei, out double from, out double to, out double fps, out string param, out string tooltip)
    {
      // Author.
      name = "Kateřina Průšová";

      // Single frame.
      wid = 640;
      hei = 480;

      // Animation.
      from = 0.0;
      to   = 7.0;
      fps  = 25.0;

      // Form params.
      param   = "forward";
      tooltip = "forward[=<bool>] ... if true, animation creates the logo";
    }

    /// <summary>
    /// Global initialization. Called before each animation batch
    /// or single-frame computation.
    /// </summary>
    /// <param name="width">Width of future frames in pixels.</param>
    /// <param name="height">Height of future frames in pixels.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    /// <param name="fps">Required fps.</param>
    /// <param name="param">Text parameter field from the form.</param>
    public static void InitAnimation (int width, int height, double start, double end, double fps, string param)
    {
      int wq = width  / 2;
      int hq = height / 2;
      int minq = Math.Min(wq, hq);

      float radius = Math.Min(width / 9, height / 4);
      float centreXC = (width/9) *2;
      float centreY = height / 2;
      float centreXG1 = centreXC + (width/9) *2 + (width/18);
      float centreXG2 = centreXG1 + (width/9) *2 + (width/18);

      //count Disc attributes of CGG discs
      DrawC(centreXC, centreY, radius);
      DrawG(centreXG1, centreY, radius, 0);
      DrawG(centreXG2, centreY, radius, 1);



      // Disc data initialization:
      minX = minY = Double.MaxValue;
      maxX = maxY = maxR = Double.MinValue;
      LOGOdiscs = new List<Disc>();

      //count Disc attributes of LOGO discs
      for (int i = 0; i < DISC_DATA.Length/3; i++)
      {
        double x =  wq + (( DISC_DATA[i, 0] - 65.0) * 0.018 * minq );
        double y =  hq + (( DISC_DATA[i, 1] - 65.0) * 0.018 * minq );
        double r = DISC_DATA[i, 2] * 0.018 * minq;
        LOGOdiscs.Add(new Disc(x, y, r, i < FIRST_COLOR ? COLORS[0] : COLORS[1]));
        if (x - r < minX) minX = x - r;
        if (x + r > maxX) maxX = x + r;
        if (y - r < minY) minY = y - r;
        if (y + r > maxY) maxY = y + r;
        if (r > maxR)     maxR = r;
      }

      SetViewport(width, height);

      RandomJames rnd = new RandomJames();
      //pair LOGO and GCC discs
      int logoLen = Animation.LOGOdiscs.Count;
      int cggLen = Animation.CGGdiscs.Count;
      int minLen = Math.Min(logoLen, cggLen);
      for (int i = 0; i < minLen; i++)
      {
        double middleX = rnd.RandomDouble(0, width);
        double middleY = rnd.RandomDouble(0, height);
        int movemetnTimeMiddle = rnd.RandomInteger(4,6);
        DiscPair pair = new DiscPair(CGGdiscs[i], LOGOdiscs[i], middleX, middleY, movemetnTimeMiddle);
        Animation.Pairs.Add(pair);
      }
      if(cggLen < logoLen)
      {
        //start missing
        for (int i = cggLen; i < logoLen; i++)
        {
          Disc targetDisc = LOGOdiscs[i];
          double startRadius = 0;
          Color startColor = targetDisc.color;
          double startX = rnd.RandomDouble(0, width);  //change to be around the center of the canvas?
          double startY = rnd.RandomDouble(0, height); //change to be around the center of the canvas?

          Disc artificialStart = new Disc(startX, startY, startRadius, startColor);

          double middleX = rnd.RandomDouble(0, width);
          double middleY = rnd.RandomDouble(0, height);
          int movemetnTimeMiddle = rnd.RandomInteger(4,6);

          DiscPair pair = new DiscPair(artificialStart, targetDisc, middleX, middleY, movemetnTimeMiddle);
          Animation.Pairs.Add(pair);
        }
      }
      if(logoLen < cggLen)
      {
        //end missing
        for (int i = logoLen; i < cggLen; i++)
        {
          Disc startDisc = CGGdiscs[i];
          double targetRadius = 0;
          Color targetColor = startDisc.color;
          double targetX = rnd.RandomDouble(0, width);  //change to be around center / not around center? 
          double targetY = rnd.RandomDouble(0, height); //change to be around center / not around center?

          Disc artifitialTarget = new Disc(targetX, targetY, targetRadius, targetColor);

          double middleX = rnd.RandomDouble(0, width);
          double middleY = rnd.RandomDouble(0, height);
          int movemetnTimeMiddle = rnd.RandomInteger(4,6);

          DiscPair pair = new DiscPair(startDisc, artifitialTarget, middleX, middleY, movemetnTimeMiddle);
          Animation.Pairs.Add(pair);
        }
      }

      // Parse parameters.
      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // forward[=<bool>]
        Util.TryParse(p, "forward", ref forward);
      }

      /*
      Disc original = new Disc(width/3, 2*(height/3), 10, COLORS[0]);
      Disc target = new Disc(2*(width/3), 2*(height/3), 25, COLORS[1]);
      Animation.TestDiscPair = new DiscPair(original, target, width/2, height/3);
      */
      


      // !!!}}
    }

    public static DiscPair TestDiscPair;

    /// <summary>
    /// Draw single animation frame.
    /// </summary>
    /// <param name="c">Canvas to draw to.</param>
    /// <param name="time">Current time in seconds.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void DrawFrame (Canvas c, double time, double start, double end, string param)
    {
      // !!!{{ TODO: put your frame-rendering code here

      c.Clear(Color.Black);
      c.SetAntiAlias(true);

      foreach (DiscPair pair in Pairs)
      {
        MoveDisc(c, pair, time, start, end);
      }


      //TEST MOVEMENT AND COLOR CHANGING
      /*c.SetColor(Color.White);
      c.FillDisc(2*(c.Width/3), 2*(c.Height/3), 5);
      c.SetColor(Color.Yellow);
      c.FillDisc(c.Width/2, c.Height / 3, 5);
      c.SetColor(Color.Red);
      c.FillDisc(c.Width / 3, 2*(c.Height / 3), 5);
      MoveDisc(c, TestDiscPair, time, start, end);*/

      /*foreach(Disc d in CGGdiscs)
      {
        c.SetColor(d.color);
        c.FillDisc((float)d.cx, (float)d.cy, (float)d.radius);
      }*/

      //LOGO APPEARS
      /*double tim = (time - start) / (end - start);
      if (!forward)
        tim = 1.0 - tim;
      float radius = (float)(maxR * tim);

      foreach (Disc d in discs)
      {
        c.SetColor(d.color);
        float r = (float)(Math.Min(radius, d.radius) * kxy);
        c.FillDisc(X(d.cx), Y(d.cy), r);
      }*/

      // !!!}}
    }
    public static void DrawG (float centreX, float centreY, float letterRadius, int upperColor)
    {
      int up_color = upperColor % 2;
      int low_color = (upperColor+1) %2;
      float x;
      float y;
      float base_radius = (float)letterRadius/3;
      float curr_radius = 0;
      float pos = (float)Math.PI/10;

      float Gstart_radius = base_radius * ( 1f/3f);
      float radius_interval_unit = base_radius - Gstart_radius;

      float horizontal_pos = pos;
      float horizontal_smallest_radius = Gstart_radius/(1.5f);
      float horizontal_radius_interval_unit = Gstart_radius - horizontal_smallest_radius;
      float horizontal_progress = letterRadius;
      float horizontal_y = (float)Math.Sin(horizontal_pos) * letterRadius + centreY;
      float horizontal_x = (float)Math.Cos(horizontal_pos) * letterRadius + centreX;
      float curr_horizontal_radius = Gstart_radius;

      //c.SetColor(COLORS[low_color]);
      while (horizontal_progress >= 0)
      {
        float prev_horizontal_radius = curr_horizontal_radius;
        curr_horizontal_radius = horizontal_smallest_radius + (float)Math.Pow((horizontal_progress / letterRadius), 2) * horizontal_radius_interval_unit;
        //c.FillDisc(horizontal_x, horizontal_y, curr_horizontal_radius);
        Animation.CGGdiscs.Add(new Disc(horizontal_x, horizontal_y, curr_horizontal_radius, COLORS[low_color]));
        horizontal_x -= (prev_horizontal_radius + curr_horizontal_radius);
        horizontal_progress -= ((prev_horizontal_radius + curr_horizontal_radius));
      }
      //c.SetColor(COLORS[low_color]);
      while (pos <= (Math.PI))
      {
        x = (float)Math.Cos(pos) * letterRadius + centreX;
        y = (float)Math.Sin(pos) * letterRadius + centreY;

        curr_radius = Gstart_radius + (float)Math.Pow(Math.Sin((pos) / 2), 2) * radius_interval_unit;
        //c.FillDisc((float)x, (float)y, curr_radius);
        Animation.CGGdiscs.Add(new Disc(x, y, curr_radius, COLORS[low_color]));
        double alpha = 2* Math.Asin(curr_radius/letterRadius);
        pos += (float)alpha;

      }

      x = (float)Math.Cos(Math.PI) * letterRadius + centreX;
      y = (float)Math.Sin(Math.PI) * letterRadius + centreY;
      //c.SetColor(COLORS[upperColor]);
      //c.FillDisc((float)x, (float)y, curr_radius);
      Animation.CGGdiscs.Add(new Disc(x, y, curr_radius, COLORS[up_color]));
      pos = (float)((Math.PI * 2) - Math.PI / 6);

      while (pos >= (Math.PI))
      {
        x = (float)Math.Cos(pos) * letterRadius + centreX;
        y = (float)Math.Sin(pos) * letterRadius + centreY;

        curr_radius = (float)Math.Pow(Math.Sin((pos) / 2), 2) * base_radius;
        //c.FillDisc((float)x, (float)y, curr_radius);
        Animation.CGGdiscs.Add(new Disc(x, y, curr_radius, COLORS[up_color]));
        pos -= (float)((Math.PI / 4) * Math.Pow(Math.Sin((pos) / 2), 2));
      }
    }

    public static void DrawC (float centreX, float centreY, float letterRadius)
    {
      //c.SetColor(COLORS[0]);
      float x;
      float y;
      float base_radius = (float)letterRadius/3;
      float curr_radius = 0;
      float pos = (float)Math.PI/6;
      while (pos <= (Math.PI))
      {
        x = (float)Math.Cos(pos) * letterRadius + centreX;
        y = (float)Math.Sin(pos) * letterRadius + centreY;

        curr_radius = (float)Math.Pow(Math.Sin((pos) / 2), 2) * base_radius;
        //c.FillDisc((float)x, (float)y, curr_radius);
        Animation.CGGdiscs.Add(new Disc(x, y, curr_radius, COLORS[0]));
        pos += (float)((Math.PI / 4) * Math.Pow(Math.Sin((pos) / 2), 2));
      }
      x = (float)Math.Cos(Math.PI) * letterRadius + centreX;
      y = (float)Math.Sin(Math.PI) * letterRadius + centreY;
      //c.SetColor(COLORS[1]);
      //c.FillDisc((float)x, (float)y, curr_radius);
      Animation.CGGdiscs.Add(new Disc(x, y, curr_radius, COLORS[1]));
      pos = (float)((Math.PI * 2) - Math.PI / 6);
      while (pos >= (Math.PI))
      {
        x = (float)Math.Cos(pos) * letterRadius + centreX;
        y = (float)Math.Sin(pos) * letterRadius + centreY;

        curr_radius = (float)Math.Pow(Math.Sin((pos) / 2), 2) * base_radius;
        //c.FillDisc((float)x, (float)y, curr_radius);
        Animation.CGGdiscs.Add(new Disc(x, y, curr_radius, COLORS[1]));
        pos -= (float)((Math.PI / 4) * Math.Pow(Math.Sin((pos) / 2), 2));
      }
    }

    public static void MoveDisc(Canvas c, DiscPair discPair, double time, double start, double end)
    {
      double tenthOfTime = (end - start) / 10d;

      //if(time < tenthOfTime)
      if(time < 2*tenthOfTime )
      {
        //draw CGG for the first time period
        c.SetColor(discPair.OriginalDisc.color);
        c.FillDisc((float)discPair.OriginalDisc.cx, (float)discPair.OriginalDisc.cy, (float)discPair.OriginalDisc.radius);
      }
      // else if(time < 9 * tenthOfTime )
      else if (time < 8 * tenthOfTime )
      {
        //move dots

        //double startOfMovingPeriod = tenthOfTime;
        double startOfMovingPeriod = 2 * tenthOfTime;
        //double endOfMovingPeriod = 9 * tenthOfTime;
        double endOfMovingPeriod = 8 * tenthOfTime;
        double middleOfMovingPeriod = 5 * tenthOfTime;

        double originalH = 0, originalS = 0, originalV = 0, targetH = 0, targetS = 0, targetV = 0;
        bool changeColor = (discPair.OriginalDisc.color != discPair.TargetDisc.color);
        if (changeColor)
        {
          Arith.ColorToHSV(discPair.OriginalDisc.color, out originalH, out originalS, out originalV);
          Arith.ColorToHSV(discPair.TargetDisc.color, out targetH, out targetS, out targetV);
        }

        //(time - start) / (end - start)
        double wholeMovingTimeCoeff = (time - startOfMovingPeriod) / (endOfMovingPeriod - startOfMovingPeriod);
        //radius changes linearly the whole moving period
        double r = discPair.OriginalDisc.radius + ((discPair.TargetDisc.radius - discPair.OriginalDisc.radius) * wholeMovingTimeCoeff );
        if (time < 5 * tenthOfTime)
        {
          //move dots to random middle location
          double firstPeriodTimeCoeff = (time - startOfMovingPeriod) / (middleOfMovingPeriod - startOfMovingPeriod);
          double x = discPair.OriginalDisc.cx + ((discPair.MiddleX - discPair.OriginalDisc.cx) * firstPeriodTimeCoeff );
          double y = discPair.OriginalDisc.cy + ((discPair.MiddleY - discPair.OriginalDisc.cy) * firstPeriodTimeCoeff );

          if (changeColor)
          {
            double curS = originalS - ( originalS * firstPeriodTimeCoeff);
            double curV = originalV + (1 - originalV ) * firstPeriodTimeCoeff;
            Color curCol = Arith.HSVToColor(originalH, curS, curV);
            c.SetColor(curCol);
          }
          else
          {
            c.SetColor(discPair.OriginalDisc.color);
          }
          c.FillDisc((float)x, (float)y, (float)r);
        }
        else
        {
          //move dots to form LOGO
          double secondPeriodTimeCoeff = (time - middleOfMovingPeriod) / (endOfMovingPeriod - middleOfMovingPeriod);
          double x = discPair.MiddleX + ((discPair.TargetDisc.cx - discPair.MiddleX) * secondPeriodTimeCoeff );
          double y = discPair.MiddleY + ((discPair.TargetDisc.cy - discPair.MiddleY) * secondPeriodTimeCoeff );

          if (changeColor)
          {
            double curS = (targetS *  secondPeriodTimeCoeff);
            double curV = 1 + ((targetV - 1) *  secondPeriodTimeCoeff);
            Color curCol = Arith.HSVToColor(targetH, curS, curV);
            //Console.WriteLine($"time: {time}, timeCoeff: {secondPeriodTimeCoeff}, curS: {curS}");
            c.SetColor(curCol);
          }
          else
          {
            c.SetColor(discPair.OriginalDisc.color);
          }

          c.FillDisc((float)x, (float)y, (float)r);
        }
      }
      else
      {
        //last tenth of time - draw LOGO
        c.SetColor(discPair.TargetDisc.color);
        c.FillDisc((float)discPair.TargetDisc.cx, (float)discPair.TargetDisc.cy, (float)discPair.TargetDisc.radius);
      }
    }
  }
}
