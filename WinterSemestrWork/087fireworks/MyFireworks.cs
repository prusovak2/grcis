using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _087fireworks
{
  public static class IntExtensions
  {
    /// <summary>
    /// modulo that behaves reasonable for negative values 
    /// </summary>
    /// <param name="dividend"></param>
    /// <param name="divisor"></param>
    /// <returns></returns>
    public static int Mod(this int dividend, int divisor)
    {
      return (dividend % divisor + divisor) % divisor;
    }
  }

  public static class RandomJamesExtensions
  {
    public static bool RandomBool(this RandomJames rnd)
    {
      double randDouble = rnd.RandomDouble(0,1);
      return randDouble < 0.5;
    }
    public static bool RandomBool(this RandomJames rnd, double probability)
    {
      if ((probability > 1) || (probability < 0))
      {
        throw new ArgumentException("Probability must be a number from interval [0,1].");
      }
      double randDouble = rnd.RandomDouble(0,1);
      return randDouble < probability;
    }
  }

  /// <summary>
  /// Base color of particles emitted by a launcher
  /// </summary>
  public enum BaseColor
  {
    Red,
    Green,
    Blue
  }

  /// <summary>
  /// various shades of particle's colors
  /// </summary>
  public enum ColType
  {
    LigteningBase,
    DarkeningBase,
    Darkening1,
    Darkening1Darker,
    Dark1Darker,   // DirectionalyDirectionalyDoubleExploding
    Dark1,   // createTripleExploding
    Darkest1,
    Light1,  //DirectionalyExploding
    Darkening2,
    WhiteToLight,
    OrangeToBlack,
    DarkeningGray,
    WhiteToGray,
    White,
    Yellow
  }

  public struct ColorRange
  {
    public Vector3 start;
    public Vector3 end;
    public bool isColorChanging;

    public ColorRange(Vector3 start, Vector3? end = null)
    {
      this.start = start;
      if(end is null)
      {
        this.isColorChanging = false;
        this.end = default;
      }
      else
      {
        this.isColorChanging = true;
        this.end = end.Value;
      }
    }
  }

  /// <summary>
  /// Stores colors
  /// </summary>
  public static class ColorPalette
  {
    public static Dictionary<BaseColor, Dictionary<ColType, ColorRange>> Ranges = new Dictionary<BaseColor, Dictionary<ColType, ColorRange>>();

    public static Dictionary<ColType, ColorRange> ColIndifferentRanges = new Dictionary<ColType, ColorRange>();

    static ColorPalette ()
    {
      //red shades
      Dictionary<ColType, ColorRange> redRanges = new Dictionary<ColType, ColorRange>();
      redRanges.Add(ColType.LigteningBase, new ColorRange(new Vector3(1, 0, 0), new Vector3(1, 1, 1)));
      redRanges.Add(ColType.DarkeningBase, new ColorRange(new Vector3(1, 0, 0), new Vector3(0, 0, 0)));
      redRanges.Add(ColType.Darkening1, new ColorRange(new Vector3(1, 0, 127 / 255f), new Vector3(51 / 255f, 0, 25 / 255f)));
      redRanges.Add(ColType.Darkening1Darker, new ColorRange(new Vector3(153/255f, 0, 76 / 255f), new Vector3(51 / 255f, 0, 25 / 255f)));
      redRanges.Add(ColType.Dark1Darker, new ColorRange(new Vector3(51 / 255f, 0, 25 / 255f)));
      redRanges.Add(ColType.Dark1, new ColorRange(new Vector3(153 / 255f, 0, 76 / 255f)));
      redRanges.Add(ColType.Light1, new ColorRange(new Vector3(255 / 255f, 102/255f, 178 / 255f)));
      redRanges.Add(ColType.Darkening2, new ColorRange(new Vector3(1, 25/255f ,  25/ 255f), new Vector3(102 / 255f, 0, 0)));
      redRanges.Add(ColType.WhiteToLight, new ColorRange(new Vector3(1, 1, 1), new Vector3(1, 170/255f, 210/255f)));
      redRanges.Add(ColType.Darkest1, new ColorRange(new Vector3(51 / 255f, 0, 102/255f), new Vector3(51/255f, 0, 25/25f)));

      //green shades
      Dictionary<ColType, ColorRange> greenRanges = new Dictionary<ColType, ColorRange>();
      greenRanges.Add(ColType.LigteningBase, new ColorRange(new Vector3(70 / 255f, 220 / 255f, 30 / 255f), new Vector3(1, 1, 1)));
      greenRanges.Add(ColType.DarkeningBase, new ColorRange(new Vector3(70 / 255f, 220 / 255f, 30 / 255f), new Vector3(0, 0, 0)));
      greenRanges.Add(ColType.Darkening1, new ColorRange(new Vector3(51 / 255f, 1, 153 / 255f), new Vector3(0, 51 / 255f, 25 / 255f)));
      greenRanges.Add(ColType.Darkening1Darker, new ColorRange(new Vector3(0, 204/255f, 102 / 255f), new Vector3(0, 51 / 255f, 25 / 255f)));
      greenRanges.Add(ColType.Dark1Darker, new ColorRange(new Vector3(0, 51 / 255f, 25 / 255f)));
      greenRanges.Add(ColType.Dark1, new ColorRange(new Vector3(0, 153 / 255f, 30 / 255f)));
      greenRanges.Add(ColType.Light1, new ColorRange(new Vector3(0, 1, 128 / 255f)));
      greenRanges.Add(ColType.Darkening2, new ColorRange(new Vector3(1, 1, 0), new Vector3(25 / 255f, 51/255f, 0)));
      greenRanges.Add(ColType.WhiteToLight, new ColorRange(new Vector3(1, 1, 1), new Vector3(153/255f, 1, 153 / 255f)));
      greenRanges.Add(ColType.Darkest1, new ColorRange(new Vector3(255/255, 255/255f, 51/255f)/*, new Vector3(0,51/255f,25/255f)*/));

      //blue shades
      Dictionary<ColType, ColorRange> blueRanges = new Dictionary<ColType, ColorRange>();
      blueRanges.Add(ColType.LigteningBase, new ColorRange(new Vector3(0, 0, 1), new Vector3(1, 1, 1)));
      blueRanges.Add(ColType.DarkeningBase, new ColorRange(new Vector3(0, 0, 1), new Vector3(0, 0, 0)));
      blueRanges.Add(ColType.Darkening1, new ColorRange(new Vector3(51 / 255f, 1, 1), new Vector3(0, 51 / 255f, 51 / 255f)));
      blueRanges.Add(ColType.Darkening1Darker, new ColorRange(new Vector3(0, 204/255f, 204/255f), new Vector3(0, 51 / 255f, 51 / 255f)));
      blueRanges.Add(ColType.Dark1Darker, new ColorRange(new Vector3(0, 51 / 255f, 51 / 255f)));
      blueRanges.Add(ColType.Dark1, new ColorRange(new Vector3(0, 102 / 255f, 204 / 255f)));
      blueRanges.Add(ColType.Light1, new ColorRange(new Vector3(51/255f, 1, 1)));
      blueRanges.Add(ColType.Darkening2, new ColorRange(new Vector3(51/255f, 51 / 255f, 1), new Vector3(51 / 255f, 0, 102/255f)));
      blueRanges.Add(ColType.WhiteToLight, new ColorRange(new Vector3(1, 1, 1), new Vector3(153/255f, 1, 1)));
      blueRanges.Add(ColType.Darkest1, new ColorRange(new Vector3(0, 0, 204/255f), new Vector3(0, 0, 51/255f)));

      Ranges.Add(BaseColor.Red, redRanges);
      Ranges.Add(BaseColor.Green, greenRanges);
      Ranges.Add(BaseColor.Blue, blueRanges);

      //Indifferent
      ColIndifferentRanges.Add(ColType.OrangeToBlack, new ColorRange(new Vector3(1, 85 / 255f, 0), new Vector3(0, 0, 0)));
      ColIndifferentRanges.Add(ColType.DarkeningGray, new ColorRange(new Vector3(100 / 255f, 100 / 255f, 100 / 255f), new Vector3(32 / 255f, 32 / 255f, 32 / 255f)));
      ColIndifferentRanges.Add(ColType.WhiteToGray, new ColorRange(new Vector3(1, 1, 1), new Vector3(96 / 255f, 96 / 255f, 96 / 255f)));
      ColIndifferentRanges.Add(ColType.White, new ColorRange(new Vector3(1, 1, 1)));
      ColIndifferentRanges.Add(ColType.Yellow, new ColorRange(new Vector3(1, 1, 0)));
    }
  }

  /// <summary>
  /// Creates various types of particles
  /// </summary>
  public static class ParticleCreator
  {
    public delegate Particle CreateParticlesDelegate (Launcher launcher, Fireworks fw, double time);

    public static RandomJames rnd = new RandomJames();
    /// <summary>
    /// current mode
    /// </summary>
    public static int ModeIndex = 16;
    /// <summary>
    /// list of all modes of explosions
    /// </summary>
    public static List<CreateParticlesDelegate> Modes  = new List<CreateParticlesDelegate>();

    static ParticleCreator ()
    {
      Modes.Add(new CreateParticlesDelegate(CreateSimpleParticle));
      Modes.Add(new CreateParticlesDelegate(CreateSimpleExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateTripleExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateDirectionalyDirectionalyDoubleExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateDirectionalyExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateSimpleDoubleExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateStrongerExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateStrongerDoubleExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateStripeDoubleExplodingParticle));
      Modes.Add(new CreateParticlesDelegate(CreateDirectionalDoubleExplodingParticle));
      //combined modes
      Modes.Add(new CreateParticlesDelegate(TripeOrStripe));
      Modes.Add(new CreateParticlesDelegate(StrongerDoubleOrDirectionalDirectional));
      Modes.Add(new CreateParticlesDelegate(StrongerOrTriple));
      Modes.Add(new CreateParticlesDelegate(StrongerDoubleOrDouble));
      Modes.Add(new CreateParticlesDelegate(StrongerDoubleOrStripe));
      Modes.Add(new CreateParticlesDelegate(StrongerOrDirectionalDirectional));
      Modes.Add(new CreateParticlesDelegate(DirectionalDirectionalOrStripe));
    }
    /// <summary>
    /// Create particle according to current mode
    /// </summary>
    /// <param name="launcher"></param>
    /// <param name="fw"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static Particle CreateParticle (Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.5)
      {
        return CreateSimpleParticle(launcher, fw, time);
      }
      else
      {
        return Modes[ModeIndex](launcher, fw, time);
      }
    }
    /// <summary>
    /// to avoid setting this values in every Create.. method
    /// </summary>
    /// <param name="particle"></param>
    /// <param name="launcher"></param>
    /// <param name="fw"></param>
    /// <param name="time"></param>
    /// <param name="col"></param>
    /// <param name="size"></param>
    /// <param name="age"></param>
    /// <param name="timeTillExplosion"></param>
    public static void InitParticle (this Particle particle, Launcher launcher, Fireworks fw, double time, ColorRange col, double? size = null , double? age = null,  double? timeTillExplosion = null)
    {
      Vector3d dir = Geometry.RandomDirectionNormal(rnd, launcher.aim, fw.variance);

      double actualAge = (age ?? rnd.RandomDouble(4.0, 6.0));

      particle.position = launcher.position;
      particle.velocity = dir * rnd.RandomDouble(15, 20);
      particle.up = launcher.up;
      particle.color = col.start;
      particle.startColor = col.start;
      particle.endColor = col.end;
      particle.isColorChanging = col.isColorChanging;
      particle.colorRange = (Vector3d)(particle.endColor - particle.startColor);
      particle.size = size ?? rnd.RandomDouble(0.2, 4.0);
      particle.simTime = time;
      particle.maxAge = time + actualAge;
      particle.TimeOfBirth = time;
      particle.baseColor = launcher.particleBaseColor;
      if(particle is ExplodingParticle explodingParticle)
      {
        explodingParticle.TimeOfExplosion = time + (timeTillExplosion ?? rnd.RandomDouble(0.2, 0.5)) * actualAge;
      }
    }

    public static Particle CreateSimpleParticle(Launcher launcher, Fireworks fw, double time)
    {
      Particle newParticle = new Particle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.Darkening1]);
      return newParticle;
    }

    public static Particle CreateSimpleExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      SimpleExplodingParticle newParticle = new SimpleExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.ColIndifferentRanges[ColType.DarkeningGray]);
      return newParticle;
    }

    public static Particle CreateTripleExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      TripleExplodingParticle newParticle = new TripleExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.Dark1Darker], timeTillExplosion: 0.2);
      return newParticle;
    }

    public static Particle CreateDirectionalyDirectionalyDoubleExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      DirectionalyDirectionalyDoubleExplodingParticle newParticle = new DirectionalyDirectionalyDoubleExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.Darkening1Darker], timeTillExplosion: 0.2);
      return newParticle;
    }

    public static Particle CreateDirectionalyExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      DirectionalyExplodingParticle newParticle = new DirectionalyExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.Darkening1Darker], timeTillExplosion: 0.2);
      return newParticle;
    }

    public static Particle CreateSimpleDoubleExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      SimpleDoubleExplodingParticle newParticle = new SimpleDoubleExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.Dark1Darker]);
      return newParticle;
    }

    public static Particle CreateStrongerExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      StrongerExplodingParticle newParticle = new StrongerExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.Dark1Darker]);
      return newParticle;
    }

    public static Particle CreateStrongerDoubleExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      StrongerDoubleExplodingParticle newParticle = new StrongerDoubleExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.ColIndifferentRanges[ColType.DarkeningGray]);
      return newParticle;
    }

    public static Particle CreateStripeDoubleExplodingParticle (Launcher launcher, Fireworks fw, double time)
    {
      StripeDoubleExplodingParticle newParticle = new StripeDoubleExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.DarkeningBase], timeTillExplosion: 0.2);
      return newParticle;
    }

    public static Particle CreateDirectionalDoubleExplodingParticle(Launcher launcher, Fireworks fw, double time)
    {
      DirectionalyDoubleExplodingParticle newParticle = new DirectionalyDoubleExplodingParticle();
      newParticle.InitParticle(launcher, fw, time, ColorPalette.Ranges[launcher.particleBaseColor][ColType.Darkening2], timeTillExplosion: 0.2);
      return newParticle;
    }
    // Combined modes

    public static Particle TripeOrStripe(Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.6)
      {
        return CreateTripleExplodingParticle(launcher, fw, time);
      }
      else
      {
        return CreateStripeDoubleExplodingParticle(launcher, fw, time);
      }
    }

    public static Particle StrongerDoubleOrDirectionalDirectional (Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.5)
      {
        return CreateStrongerDoubleExplodingParticle(launcher, fw, time);
      }
      else
      {
        return CreateDirectionalyDirectionalyDoubleExplodingParticle(launcher, fw, time);
      }
    }

    public static Particle StrongerOrTriple (Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.6)
      {
        return CreateStrongerExplodingParticle(launcher, fw, time);
      }
      else
      {
        return CreateTripleExplodingParticle(launcher, fw, time);
      }
    }

    public static Particle StrongerDoubleOrDouble (Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.5)
      {
        return CreateStrongerDoubleExplodingParticle(launcher, fw, time);
      }
      else
      {
        return CreateSimpleDoubleExplodingParticle(launcher, fw, time);
      }
    }

    public static Particle StrongerDoubleOrStripe (Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.6)
      {
        return CreateStrongerDoubleExplodingParticle(launcher, fw, time);
      }
      else
      {
        return CreateStripeDoubleExplodingParticle(launcher, fw, time);
      }
    }

    public static Particle StrongerOrDirectionalDirectional (Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.5)
      {
        return CreateStrongerExplodingParticle(launcher, fw, time);
      }
      else
      {
        return CreateDirectionalyDirectionalyDoubleExplodingParticle(launcher, fw, time);
      }
    }

    public static Particle DirectionalDirectionalOrStripe(Launcher launcher, Fireworks fw, double time)
    {
      double rd = rnd.RandomDouble(0,1);
      if (rd < 0.5)
      {
        return CreateDirectionalyDirectionalyDoubleExplodingParticle(launcher, fw, time);
      }
      else
      {
        return CreateStripeDoubleExplodingParticle(launcher, fw, time);
      }
    }

  }

  /// <summary>
  /// Defines types of explosions
  /// </summary>
  public static class Explosions
  {
    static Explosions ()
    {
      Vector3d right = new Vector3d(1,0,0);
      Vector3d left = new Vector3d(-1,0,0);
      Vector3d up = new Vector3d(0,1,0);
      Vector3d down = new Vector3d(0,-1,0);
      Vector3d front = new Vector3d(0,0,1);
      Vector3d back  = new Vector3d(0,0,-1);
      Aims = new Vector3d[6];
      Aims[0] = right;
      Aims[1] = left;
      Aims[2] = up;
      Aims[3] = down;
      Aims[4] = front;
      Aims[5] = back;
    }
    /// <summary>
    /// to allow to emit particles in every direction
    /// </summary>
    public static readonly Vector3d[] Aims;

    public static RandomJames rnd = new RandomJames();

    public static void SimpleExplosion (Particle particle, Fireworks fw, double time, int explosionCoeff = 1, ColorRange? col = null)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        ColorRange colorToUse;
        if (col.HasValue)
        {
          colorToUse = col.Value;
        }
        else
        {
          double rd = rnd.RandomDouble(0,1);
          if (rd < 0.7)
          {
            colorToUse = ColorPalette.Ranges[particle.baseColor][ColType.Darkening1Darker];
          }
          else
          {
            colorToUse = ColorPalette.Ranges[particle.baseColor][ColType.Darkest1];
          }
        }
        int aimIndex = rnd.RandomInteger(0,5);
        Vector3d aim = Aims[aimIndex];
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, aim, fw.variance) * explosionCoeff;
        Particle p = new Particle(particle.position, dir + particle.velocity, particle.up,
                             colorToUse,
                            rnd.RandomDouble(0.2, 2.0), time, rnd.RandomDouble(2.0, 3.0), particle.baseColor);
        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }

    public static void SimpleDoubleExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        int aimIndex = rnd.RandomInteger(0,5);
        Vector3d aim = Aims[aimIndex];
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, aim, fw.variance) * 2;
        Particle p = new SimpleExplodingParticle(particle.position, dir + particle.velocity, particle.up,
                            ColorPalette.ColIndifferentRanges[ColType.OrangeToBlack],
                            rnd.RandomDouble(0.2, 2.0), time,  rnd.RandomDouble(1, 1.2), particle.baseColor);

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }

    public static void StrongerDoubleExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        int aimIndex = rnd.RandomInteger(0,5);
        Vector3d aim = Aims[aimIndex];
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, aim, fw.variance) * 2;
        Particle p = new StrongerExplodingParticle(particle.position, dir + particle.velocity, particle.up,
                            ColorPalette.ColIndifferentRanges[ColType.White],
                            rnd.RandomDouble(0.2, 2.0), time, rnd.RandomDouble(1, 1.2), particle.baseColor); 

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }

    public static void DirectionalExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        double rd = rnd.RandomDouble(0,1);
        ColorRange colorToUse;
        if(rd < 0.6)
        {
          colorToUse = ColorPalette.Ranges[particle.baseColor][ColType.Darkening1];
        }
        else if(rd< 0.9)
        {
          colorToUse = ColorPalette.Ranges[particle.baseColor][ColType.Darkest1];
        }
        else
        {
          colorToUse = ColorPalette.Ranges[particle.baseColor][ColType.WhiteToLight];
        }
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, particle.velocity, 0.3) * 5;
        Particle p = new Particle(particle.position, dir + particle.velocity, particle.up,
                            colorToUse,
                            rnd.RandomDouble(0.2, 3.0), time, rnd.RandomDouble(1, 1.2), particle.baseColor);

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }

    public static void DirectionalDoubleExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        int aimIndex = rnd.RandomInteger(0,5);
        Vector3d aim = Aims[aimIndex];
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, aim, fw.variance) * 2;
        Particle p = new DirectionalyExplodingParticle(particle.position, dir + particle.velocity, particle.up,
                            ColorPalette.Ranges[particle.baseColor][ColType.Darkening2],
                            rnd.RandomDouble(0.2, 2.0), time, rnd.RandomDouble(1, 1.2), particle.baseColor);

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }

    public static void DirectionalDirectionalDoubleExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, particle.velocity, 0.3) * 5;
        Particle p = new DirectionalyExplodingParticle(particle.position, dir + particle.velocity, particle.up,
                            ColorPalette.Ranges[particle.baseColor][ColType.Dark1Darker],
                            rnd.RandomDouble(0.2, 2.0), time, rnd.RandomDouble(1, 1.2), particle.baseColor, 0.2);

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }

    public static void TripleExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 7;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        int aimIndex = rnd.RandomInteger(0,5);
        Vector3d aim = Aims[aimIndex];
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, aim, fw.variance) * 2;
        Particle p = new DirectionalyDirectionalyDoubleExplodingParticle(particle.position, dir + particle.velocity, particle.up,
                            ColorPalette.Ranges[particle.baseColor][ColType.Dark1],
                            rnd.RandomDouble(0.2, 2.0), time, rnd.RandomDouble(1, 1.2), particle.baseColor, 0.2);

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }
    public static void StripeExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, particle.velocity, 0.1) * probability;
        Particle p = new Particle(particle.position, dir + particle.velocity, particle.up,
                            ColorPalette.Ranges[particle.baseColor][ColType.WhiteToLight],
                            rnd.RandomDouble(0.2, 2.0), time, rnd.RandomDouble(1, 1.2), particle.baseColor);

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }

    public static void StripeDoubleExplosion (Particle particle, Fireworks fw, double time)
    {
      double probability = 10;
      while (probability > 1.0 ||
             rnd.UniformNumber() < probability)
      {
        int aimIndex = rnd.RandomInteger(0,5);
        Vector3d aim = Aims[aimIndex];
        Vector3d dir = Geometry.RandomDirectionNormal(Explosions.rnd, aim, fw.variance) * 2;
        Particle p = new StripeExplodingParticle(particle.position, dir + particle.velocity, particle.up,
                            ColorPalette.ColIndifferentRanges[ColType.Yellow],
                            rnd.RandomDouble(0.2, 2.0), time, rnd.RandomDouble(1, 1.2), particle.baseColor, timeTillExplosion: 0.2);

        fw.AddParticle(p);
        particle.maxAge = time;
        probability -= 1;
      }
    }
  }

  /// <summary>
  /// Rocket/particle launcher.
  /// Primary purpose: generate rockets/particles.
  /// If rendered, usually by triangles.
  /// </summary>
  public class Launcher : DefaultRenderObject
  {
    /// <summary>
    /// Particle source position.
    /// </summary>
    public Vector3d position;

    /// <summary>
    /// Particle source aim (initial direction of the particles).
    /// </summary>
    public Vector3d aim;

    /// <summary>
    /// Particle source up vector (initial up vector of the particles).
    /// </summary>
    public Vector3d up;

    /// <summary>
    /// colors of particles emitted by this launcher are based on this color
    /// </summary>
    public BaseColor particleBaseColor;

    /// <summary>
    /// Number of particles generated in every second.
    /// </summary>
    public double frequency;

    /// <summary>
    /// Last simulated time in seconds.
    /// </summary>
    public double simTime;

    /// <summary>
    /// Is this launcher currently emitting particles
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Global constant launcher color.
    /// </summary>
    static Vector3 color = new Vector3(1.0f, 0.4f, 0.2f);
    
    static Dictionary<BaseColor, Vector3> LauchColors = new Dictionary<BaseColor, Vector3>();

    /// <summary>
    /// Color to be used when displaying this launcher
    /// </summary>
    public Vector3 LauncherColor => LauchColors[particleBaseColor];

    /// <summary>
    /// Shared random generator. Should be Locked if used in multi-thread environment.
    /// </summary>
    static RandomJames rnd = new RandomJames();

    static Launcher ()
    {
      LauchColors.Add(BaseColor.Red, new Vector3(1, 0, 0));
      LauchColors.Add(BaseColor.Green, new Vector3(0, 204/255f, 0));
      LauchColors.Add(BaseColor.Blue, new Vector3(0 , 128 / 255f, 1));
    }

    public Launcher (double freq, Vector3d? pos = null, Vector3d? _aim = null, Vector3d? _up = null, BaseColor? baseColor = null)
    {
      position = pos ?? Vector3d.Zero;
      aim = _aim ?? new Vector3d(0.1, 1.0, -0.1);
      aim.Normalize();
      up = _up ?? new Vector3d(0.5, 0.0, 0.5);
      up.Normalize();
      frequency = freq;      // number of emitted particles per second
      simTime = 0.0;
      particleBaseColor = baseColor ?? BaseColor.Red;
    }

    /// <summary>
    /// Simulate object to the given time.
    /// </summary>
    /// <param name="time">Required target time.</param>
    /// <param name="fw">Simulation context.</param>
    /// <returns>False in case of expiry.</returns>
    public bool Simulate (double time, Fireworks fw)
    {
      if (time <= simTime)
        return true;

      double dt = time - simTime;
      // generate new particles for the [simTime-time] interval:
      if (this.IsActive)
      {
        double probability = dt * frequency;
        while (probability > 1.0 ||
               rnd.UniformNumber() < probability)
        {
          // the most important line of all
          Particle newParticle = ParticleCreator.CreateParticle(this, fw, time);
          fw.AddParticle(newParticle);
          probability -= 1.0;
        }
      }
      simTime = time;
      return true;
    }

    #region Launcher Rendering
    // --- rendering ---

    public override uint Triangles => 2;

    public override uint TriVertices => 4;

    /// <summary>
    /// Triangles: returns vertex-array size (if ptr is null) or fills vertex array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">Index number in the global vertex array.</param>
    /// <param name="stride">Vertex size (stride) in bytes.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size/line-width attribute?</param>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    public override unsafe int TriangleVertices (ref float* ptr, ref uint origin, out int stride, bool txt, bool col, bool normal, bool ptsize)
    {
      int total = base.TriangleVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);
      if (ptr == null)
        return total;

      Vector3d n = Vector3d.Cross(aim, up).Normalized();
      Vector3 myColor = LauncherColor;

      // 1. center
      if (txt)
        Fill(ref ptr, 0.0f, 0.5f);
      if (col)
        //Fill(ref ptr, ref color);
        Fill(ref ptr, ref myColor);
      if (normal)
        Fill(ref ptr, ref n);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, ref position);

      // 2. aim
      if (txt)
        Fill(ref ptr, 1.0f, 0.5f);
      if (col)
        //Fill(ref ptr, ref color);
        Fill(ref ptr, ref myColor);
      if (normal)
        Fill(ref ptr, ref n);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, position + 0.2 * aim);

      // 3. up
      if (txt)
        Fill(ref ptr, 0.0f, 0.25f);
      if (col)
        //Fill(ref ptr, ref color);
        Fill(ref ptr, ref myColor);
      if (normal)
        Fill(ref ptr, ref n);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, position + 0.05 * up);

      // 4. down
      if (txt)
        Fill(ref ptr, 0.0f, 0.75f);
      if (col)
        //Fill(ref ptr, ref color);
        Fill(ref ptr, ref myColor);
      if (normal)
        Fill(ref ptr, ref n);
      if (ptsize)
        *ptr++ = 1.0f;
      Fill(ref ptr, position - 0.05 * up);

      return total;
    }

    /// <summary>
    /// Triangles: returns index-array size (if ptr is null) or fills index array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">First index to use.</param>
    /// <returns>Data size of the index-set (in bytes).</returns>
    public override unsafe int TriangleIndices (ref uint* ptr, uint origin)
    {
      if (ptr != null)
      {
        *ptr++ = origin;
        *ptr++ = origin + 1;
        *ptr++ = origin + 2;

        *ptr++ = origin;
        *ptr++ = origin + 3;
        *ptr++ = origin + 1;
      }

      return 6 * sizeof(uint);
    }
  }
  #endregion

  /// <summary>
  /// Fireworks particle - active (rocket) or passive (glowing particle) element
  /// of the simulation. Rendered usually by a GL_POINT primitive.
  /// </summary>
  public class Particle : DefaultRenderObject
  {
    /// <summary>
    /// Current particle position.
    /// </summary>
    public Vector3d position;

    /// <summary>
    /// Current particle velocity.
    /// </summary>
    public Vector3d velocity;

    /// <summary>
    /// Current particle up vector.
    /// </summary>
    public Vector3d up;

    /// <summary>
    /// Particle color.
    /// </summary>
    public Vector3 color;

    /// <summary>
    /// Particle size in pixels.
    /// </summary>
    public double size;

    /// <summary>
    /// Time of death.
    /// </summary>
    public double maxAge;

    /// <summary>
    /// Last simulated time in seconds.
    /// </summary>
    public double simTime;

    //Fields I added

    /// <summary>
    /// Time when this particle was created
    /// </summary>
    public double TimeOfBirth;

    public bool isColorChanging = false;

    public Vector3 startColor;

    public Vector3 endColor;

    /// <summary>
    /// vector from start color to end color to allow the particle to change color linearly in time
    /// stored to avoid computing this value in each Simulate call 
    /// </summary>
    public Vector3d colorRange;

    /// <summary>
    /// given by a launcher that emitted this particle
    /// </summary>
    public BaseColor baseColor;

    /// <summary>
    /// Shared random generator
    /// </summary>
    static protected RandomJames rnd = new RandomJames();

    public Particle () { }

    public Particle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange colRange, double siz, double time, double age, BaseColor baseColor)
    {
      position = pos;
      velocity = vel;
      up = _up;
      color = colRange.start;
      startColor = colRange.start;
      endColor = colRange.end;
      isColorChanging = colRange.isColorChanging;
      this.colorRange = (Vector3d)(endColor - startColor);
      size = siz;
      simTime = time;
      maxAge = time + age;
      TimeOfBirth = time;
      this.baseColor = baseColor;
    }

    /// <summary>
    /// Simulate object to the given time.
    /// </summary>
    /// <param name="time">Required target time.</param>
    /// <param name="fw">Simulation context.</param>
    /// <returns>False in case of expiry.</returns>
    public virtual bool Simulate (double time, Fireworks fw)
    {
      //how particles act
      if (time <= simTime)
        return true;

      if (time > maxAge)
        return false;

      bool retval = SimulateMovement(time, fw);
      if (fw.particleDynamic)
      {
        DoLinearColorStep(time);
      }
      simTime = time;

      return retval;
    }

    public bool SimulateMovement(double time, Fireworks fw)
    {
      // fly the particle:
      double dt = time - simTime;
      double k1 = 0.008;
      double k2 = 0.03;
      double normSquare = Math.Pow(velocity.X, 2) + Math.Pow(velocity.Y, 2) + Math.Pow(velocity.Z, 2);
      double aceleration = k1*Math.Sqrt(normSquare) + k2 * normSquare;

      velocity -= new Vector3d(0, 9.81, 0) * dt;
      velocity -= Vector3d.Clamp(velocity, new Vector3d(0, 0, 0), new Vector3d(0.5, 0.5, 0.5) * aceleration * dt);
      position += dt * (velocity);

      if (fw.particleDynamic)
      {
        velocity += dt * -0.05 * up;
        double extinction = Math.Pow(0.82, dt); //base extinction on particle age
        size *= extinction;
        color *= (float)extinction;
      }
      return true;
    }

    /// <summary>
    /// changes color of this particle linearly in time
    /// </summary>
    /// <param name="time"></param>
    public virtual void DoLinearColorStep (double time)
    {
      if (isColorChanging)
      {
        LinearColorStep(time, maxAge);
      }
    }

    /// <summary>
    /// maxAge or timeOfExplosion is to be used as endTime depending on a particular particle type
    /// </summary>
    /// <param name="time"></param>
    /// <param name="endTime"></param>
    protected void LinearColorStep (double time, double endTime)
    {
      // (time-start)/(end-start)
      double timeCoef = (time - TimeOfBirth) / (endTime-TimeOfBirth);
      Vector3 colorStep = (Vector3)(colorRange * timeCoef);
      this.color = this.startColor + colorStep;
    }

    #region Particle Rendering
    //--- rendering ---

    public override uint Points => 1;

    /// <summary>
    /// Points: returns vertex-array size (if ptr is null) or fills vertex array.
    /// </summary>
    /// <param name="ptr">Data pointer (null for determining buffer size).</param>
    /// <param name="origin">Index number in the global vertex array.</param>
    /// <param name="stride">Vertex size (stride) in bytes.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size/line-width attribute?</param>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    public override unsafe int PointVertices (ref float* ptr, ref uint origin, out int stride, bool txt, bool col, bool normal, bool ptsize)
    {
      int total = base.PointVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);
      if (ptr == null)
        return total;

      if (txt)
        Fill(ref ptr, color.Xy);
      if (col)
        Fill(ref ptr, ref color);
      if (normal)
        Fill(ref ptr, ref up);
      if (ptsize)
        *ptr++ = (float)size;
      Fill(ref ptr, ref position);

      return total;
    }
  }
  #endregion

  public abstract class ExplodingParticle : Particle
  {
    /// <summary>
    /// Time when the particle will explode if it is exploding particle
    /// </summary>
    public double TimeOfExplosion;

    public ExplodingParticle () { }

    public ExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor)
    {
      TimeOfExplosion = time + (timeTillExplosion ?? rnd.RandomDouble(0.2, 0.5)) * age;
    }

    public sealed override bool Simulate (double time, Fireworks fw)
    {
      //how particles act
      if (time <= simTime)
        return true;

      if (time > maxAge)
        return false;

      bool retval = SimulateMovement(time, fw);
      if (fw.particleDynamic)
      {
        DoLinearColorStep(time);
      }
      this.Explode(fw, time);

      simTime = time;
      return retval;
    }

    /// <summary>
    /// changes color of this particle linearly in time
    /// </summary>
    /// <param name="time"></param>
    public sealed override void DoLinearColorStep (double time)
    {
      if (isColorChanging)
      {
        LinearColorStep(time, TimeOfExplosion);
      }
    }

    public abstract void Explode (Fireworks fw, double time);

  }

  public class SimpleExplodingParticle : ExplodingParticle
  {
    public SimpleExplodingParticle () { }

    public SimpleExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.SimpleExplosion(this, fw, time, 1);
      }
    }
  }

  public class StrongerExplodingParticle : ExplodingParticle
  {
    public StrongerExplodingParticle () { }

    public StrongerExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
   : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        int explCoef = rnd.RandomInteger(2, 8);
        Explosions.SimpleExplosion(this, fw, time, explCoef, ColorPalette.Ranges[this.baseColor][ColType.Darkening2]);
      }
    }
  }

  public class SimpleDoubleExplodingParticle : ExplodingParticle
  {
    public SimpleDoubleExplodingParticle () { }

    public SimpleDoubleExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.SimpleDoubleExplosion(this, fw, time);
      }
    }
  }

  public class StrongerDoubleExplodingParticle : ExplodingParticle
  {
    public StrongerDoubleExplodingParticle () { }

    public StrongerDoubleExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.StrongerDoubleExplosion(this, fw, time);
      }
    }
  }

  public class DirectionalyExplodingParticle : ExplodingParticle
  {
    public DirectionalyExplodingParticle () { }

    public DirectionalyExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.DirectionalExplosion(this, fw, time);
      }
    }
  }

  public class DirectionalyDoubleExplodingParticle : ExplodingParticle
  {
    public DirectionalyDoubleExplodingParticle () { }

    public DirectionalyDoubleExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.DirectionalDoubleExplosion(this, fw, time);
      }
    }
  }
  /// <summary>
  /// only to be used as a second generation of particles in triple explosion
  /// </summary>
  public class DirectionalyDirectionalyDoubleExplodingParticle : ExplodingParticle
  {
    public DirectionalyDirectionalyDoubleExplodingParticle () { }

    public DirectionalyDirectionalyDoubleExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.DirectionalDirectionalDoubleExplosion(this, fw, time);
      }
    }
  }

  public class TripleExplodingParticle : ExplodingParticle
  {
    public TripleExplodingParticle () { }

    public TripleExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.TripleExplosion(this, fw, time);
      }
    }
  }
  /// <summary>
  /// to be only used as a second generation particle in StripeDoubleExplosion
  /// </summary>
  public class StripeExplodingParticle : ExplodingParticle
  {
    public StripeExplodingParticle () { }

    public StripeExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.StripeExplosion(this, fw, time);
      }
    }
  }

  public class StripeDoubleExplodingParticle : ExplodingParticle
  {
    public StripeDoubleExplodingParticle() { }

    public StripeDoubleExplodingParticle (Vector3d pos, Vector3d vel, Vector3d _up, ColorRange col, double siz, double time, double age, BaseColor baseColor, double? timeTillExplosion = null)
      : base(pos, vel, _up, col, siz, time, age, baseColor, timeTillExplosion) { }

    public override void Explode (Fireworks fw, double time)
    {
      if (simTime <= TimeOfExplosion && TimeOfExplosion <= time)
      {
        Explosions.StripeDoubleExplosion(this, fw, time);
      }
    }
  }

  /// <summary>
  /// Fireworks instance.
  /// Global framework for the simulation.
  /// </summary>
  public class Fireworks
  {
    /// <summary>
    /// Set of active particles.
    /// </summary>
    List<Particle> particles;

    /// <summary>
    /// New particles (to be added at the end of current simulation frame).
    /// </summary>
    List<Particle> newParticles;

    /// <summary>
    /// Expired particle indices (to be removed at the end of current simulation frame).
    /// </summary>
    List<int> expiredParticles;

    /// <summary>
    /// Set of active launchers.
    /// </summary>
    List<Launcher> launchers;

    public int Launchers => launchers.Count;

    public int Particles => particles.Count;

    /// <summary>
    /// Maximum number of particles in the simulation.
    /// </summary>
    int maxParticles;

    /// <summary>
    /// Particle emitting frequency for the launchers.
    /// </summary>
    double freq;

    /// <summary>
    /// Dynamic particle behavior.
    /// </summary>
    public bool particleDynamic;

    /// <summary>
    /// Variance for particle generation (direction).
    /// </summary>
    public double variance;

    /// <summary>
    /// This limit is used for render-buffer allocation.
    /// </summary>
    public int MaxParticles => maxParticles;

    /// <summary>
    /// This limit is used for render-buffer allocation.
    /// </summary>
    public int MaxLaunchers => 20;

    public Particle GetParticle (int i)
    {
      if (i < particles.Count)
        return particles[i];

      return null;
    }

    public Launcher GetLauncher (int i)
    {
      if (i < launchers.Count)
        return launchers[i];

      return null;
    }

    public int ticks = 1;

    public CoordinateAxes axes;

    public bool HasAxes => ticks > 0 &&
                           axes != null;

    /// <summary>
    /// Lock-protected simulation state.
    /// Pause-related stuff could be stored/handled elsewhere.
    /// </summary>
    public bool Running
    {
      get;
      set;
    }

    /// <summary>
    /// Number of simulated frames so far.
    /// </summary>
    public int Frames { get; private set; }

    /// <summary>
    /// Current sim-world time.
    /// </summary>
    public double Time { get; private set; }

    /// <summary>
    /// Significant change of simulation parameters .. need to reallocate buffers.
    /// </summary>
    public bool Dirty
    {
      get;
      set;
    }

    /// <summary>
    /// Slow motion coefficient.
    /// </summary>
    public static double slow = 0.25;

    public Fireworks (int maxPart = 1000)
    {
      maxParticles = maxPart;
      freq = 10.0;
      particles = new List<Particle>(maxParticles);
      newParticles = new List<Particle>();
      expiredParticles = new List<int>();
      launchers = new List<Launcher>();
      Frames = 0;
      Time = 0.0;
      Running = true;
      Dirty = false;
      particleDynamic = false;
      variance = 0.1;
      axes = new CoordinateAxes(1.0f, ticks, ticks, ticks);
    }

    /// <summary>
    /// [Re-]initialize the simulation system.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    public void Reset (string param)
    {
      // input params:
      Update(param);

      // initialization job itself:
      particles.Clear();
      launchers.Clear();

      Launcher l = new Launcher(freq, new Vector3d(-2.0, -5.0, 0.0), null, new Vector3d(-0.5, 0.0, -0.5), baseColor:BaseColor.Red);
      AddLauncher(l);
      l = new Launcher(freq, new Vector3d(-1.0, -5.0, 0.0), baseColor:BaseColor.Blue);
      AddLauncher(l);
      l = new Launcher(freq, new Vector3d(0.0, -5.0, 0.0), null, new Vector3d(-0.5, 0.0, -0.5), baseColor:BaseColor.Green);
      AddLauncher(l);
      l = new Launcher(freq, new Vector3d(1.0, -5.0, 0.0), null, new Vector3d(-0.5, 0.0, -0.5), baseColor: BaseColor.Blue);
      AddLauncher(l);
      l = new Launcher(freq, new Vector3d(2.0, -5.0, 0.0), null, new Vector3d(-0.5, 0.0, -0.5), baseColor: BaseColor.Red);
      AddLauncher(l);

      Frames = 0;
      Time = 0.0f;
      Running = true;
    }

    /// <summary>
    /// Update simulation parameters.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    public void Update (string param)
    {
      // input params:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if (p.Count == 0)
        return;

      // launchers: frequency
      if (Util.TryParse(p, "freq", ref freq))
      {
        if (freq < 1.0)
          freq = 4.0;
        if (freq > 15)
          freq = 15;
        foreach (var l in launchers)
          l.frequency = freq;
      }

      // launchers: variance
      if (Util.TryParse(p, "variance", ref variance))
      {
        if (variance < 0.0)
          variance = 0.0;
      }

      // global: maxParticles
      if (Util.TryParse(p, "max", ref maxParticles))
      {
        if (maxParticles < 10)
          maxParticles = 1000;
        Dirty = true;
      }

      // global: ticks
      if (Util.TryParse(p, "ticks", ref ticks))
      {
        if (ticks < 0)
          ticks = 0;

        axes = new CoordinateAxes(1.0f, ticks, ticks, ticks);
        Dirty = true;
      }

      // global: slow-motion coeff
      if (!Util.TryParse(p, "slow", ref slow) ||
          slow < 1.0e-4)
        slow = 0.25;

      // global: screencast
      bool recent = false;
      if (Util.TryParse(p, "screencast", ref recent) &&
          (Form1.screencast != null) != recent)
        Form1.StartStopScreencast(recent);

      // particles: dynamic behavior
      bool dyn = false;
      if (Util.TryParse(p, "dynamic", ref dyn))
        particleDynamic = dyn;
    }

    public void AddLauncher (Launcher la)
    {
      if (launchers.Count < MaxLaunchers)
        launchers.Add(la);
    }

    public void AddParticle (Particle p)
    {
      if (particles.Count + newParticles.Count - expiredParticles.Count < maxParticles)
        newParticles.Add(p);
    }

    static IComparer<int> ReverseComparer = new ReverseComparer<int>();

    /// <summary>
    /// Do one step of simulation.
    /// </summary>
    /// <param name="time">Required target time.</param>
    public void Simulate (double time)
    {
      if (!Running)
        return;

      Frames++;

      // clean the work table:
      newParticles.Clear();
      expiredParticles.Clear();

      int i;
      bool oddFrame = (Frames & 1) > 0;

      // simulate launchers:
      if (oddFrame)
        for (i = 0; i < launchers.Count; i++)
          launchers[i].Simulate(time, this);
      else
        for (i = launchers.Count; --i >= 0;)
          launchers[i].Simulate(time, this);

      // simulate particles:
      if (oddFrame)
      {
        for (i = 0; i < particles.Count; i++)
          if (!particles[i].Simulate(time, this))
            expiredParticles.Add(i);
      }
      else
        for (i = particles.Count; --i >= 0;)
          if (!particles[i].Simulate(time, this))
            expiredParticles.Add(i);

      // remove expired particles:
      expiredParticles.Sort(ReverseComparer);
      foreach (int j in expiredParticles)
        particles.RemoveAt(j);

      // add new particles:
      foreach (var p in newParticles)
        particles.Add(p);

      Time = time;
    }

    /// <summary>
    /// Prepares (fills) all the triangle-related data into the provided vertex buffer and index buffer.
    /// </summary>
    /// <returns>Number of used indices (to draw).</returns>
    public unsafe int FillTriangleData (ref float* ptr, ref uint* iptr, out int stride, bool txt, bool col, bool normal, bool ptsize)
    {
      uint* bakIptr = iptr;
      stride = 0;
      uint origin = 0;

      foreach (var l in launchers)
      {
        uint bakOrigin = origin;
        l.TriangleVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);
        l.TriangleIndices(ref iptr, bakOrigin);
      }

      foreach (var p in particles)
      {
        uint bakOrigin = origin;
        p.TriangleVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);
        p.TriangleIndices(ref iptr, bakOrigin);
      }

      return (int)(iptr - bakIptr);
    }

    /// <summary>
    /// Prepares (fills) all the line-related data into the provided vertex buffer and index buffer.
    /// </summary>
    /// <returns>Number of used indices (to draw).</returns>
    public unsafe int FillLineData (ref float* ptr, ref uint* iptr, out int stride, bool txt, bool col, bool normal, bool ptsize)
    {
      uint* bakIptr = iptr;
      stride = 0;
      uint origin = 0;

      foreach (var l in launchers)
      {
        uint bakOrigin = origin;
        l.LineVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);
        l.LineIndices(ref iptr, bakOrigin);
      }

      foreach (var p in particles)
      {
        uint bakOrigin = origin;
        p.LineVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);
        p.LineIndices(ref iptr, bakOrigin);
      }

      if (HasAxes)
      {
        uint bakOrigin = origin;
        axes.LineVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);
        axes.LineIndices(ref iptr, bakOrigin);
      }

      return (int)(iptr - bakIptr);
    }

    /// <summary>
    /// Prepares (fills) all the point-related data into the provided vertex buffer.
    /// </summary>
    /// <returns>Number of point-sprites (to draw).</returns>
    public unsafe int FillPointData (ref float* ptr, out int stride, bool txt, bool col, bool normal, bool ptsize)
    {
      stride = 0;
      uint origin = 0;

      foreach (var l in launchers)
        l.PointVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);

      foreach (var p in particles)
        p.PointVertices(ref ptr, ref origin, out stride, txt, col, normal, ptsize);

      return (int)origin;
    }

    /// <summary>
    /// Handles mouse-button push.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseButtonDown (MouseEventArgs e)
    {
      return false;
    }

    /// <summary>
    /// Handles mouse-button release.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseButtonUp (MouseEventArgs e)
    {
      return false;
    }

    /// <summary>
    /// Handles mouse move.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MousePointerMove (MouseEventArgs e)
    {
      return false;
    }

    /// <summary>
    /// Handles keyboard key press.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool KeyHandle (KeyEventArgs e)
    {
      // handling key
      switch (e.KeyCode)
      {
        case Keys.F:
            this.launchers[0].IsActive = !this.launchers[0].IsActive;
          break;
        case Keys.G:
          this.launchers[1].IsActive = !this.launchers[1].IsActive;
          break;
        case Keys.H:
          this.launchers[2].IsActive = !this.launchers[2].IsActive;
          break;
        case Keys.J:
          this.launchers[3].IsActive = !this.launchers[3].IsActive;
          break;
        case Keys.K:
          this.launchers[4].IsActive = !this.launchers[4].IsActive;
          break;
        case Keys.C:
          this.launchers[0].particleBaseColor = (BaseColor)(((int)this.launchers[0].particleBaseColor + 1) % 3);
          break;
        case Keys.V:
          this.launchers[1].particleBaseColor = (BaseColor)(((int)this.launchers[1].particleBaseColor + 1) % 3);
          break;
        case Keys.B:
          this.launchers[2].particleBaseColor = (BaseColor)(((int)this.launchers[2].particleBaseColor + 1) % 3);
          break;
        case Keys.N:
          this.launchers[3].particleBaseColor = (BaseColor)(((int)this.launchers[3].particleBaseColor + 1) % 3);
          break;
        case Keys.M:
          this.launchers[4].particleBaseColor = (BaseColor)(((int)this.launchers[4].particleBaseColor + 1) % 3);
          break;
        case Keys.Right:
          ParticleCreator.ModeIndex = (ParticleCreator.ModeIndex + 1).Mod(ParticleCreator.Modes.Count);
          break;
        case Keys.Left:
          ParticleCreator.ModeIndex = (ParticleCreator.ModeIndex - 1).Mod(ParticleCreator.Modes.Count);
          break;
        default:
          return false;
      }
      return true;
    }
  }

  public partial class Form1
  {
    /// <summary>
    /// Form-data initialization.
    /// </summary>
    static void InitParams (out string param, out string tooltip, out string name, out MouseButtons trackballButton, out Vector3 center, out float diameter,
                            out bool useTexture, out bool globalColor, out bool useNormals, out bool usePtSize)
    {
      param           = "freq=3.0,max=60000,slow=0.25,dynamic=1,variance=0.1,ticks=0";
      tooltip         = "freq,max,slow,dynamic,variance,ticks,screencast";
      trackballButton = MouseButtons.Left;
      center          = new Vector3(0.0f, 1.0f, 0.0f);
      diameter        = 17.0f;
      useTexture      = false;
      globalColor     = false;
      useNormals      = false;
      usePtSize       = true;

      name = "Kateřina Průšová";
    }

    /// <summary>
    /// Set real-world coordinates of the camera/light source.
    /// </summary>
    void SetLightEye (Vector3 center, float diameter)
    {
      diameter += diameter;
      lightPosition = center + new Vector3(-2.0f * diameter, diameter, diameter);
    }

    /// <summary>
    /// Can we use shaders?
    /// </summary>
    bool canShaders = false;

    /// <summary>
    /// Are we currently using shaders?
    /// </summary>
    bool useShaders = false;

    uint[] VBOid = null;  // vertex array VBO (colors, normals, coords), index array VBO
    int[] VBOlen = null;  // currently allocated lengths of VBOs

    /// <summary>
    /// Simulation fireworks.
    /// </summary>
    Fireworks fw;

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo>();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    GlProgram activeProgram = null;

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long primitiveCounter = 0L;
    double lastFps = 0.0;
    double lastPps = 0.0;

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    void Application_Idle (object sender, EventArgs e)
    {
      while (glControl1.IsIdle)
      {
        glControl1.MakeCurrent();
        Simulate();
        Render(true);

        long now = DateTime.Now.Ticks;
        if (now - lastFpsTime > 5000000)      // more than 0.5 sec
        {
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter * 1.0e7 / (now - lastFpsTime));
          lastPps = 0.5 * lastPps + 0.5 * (primitiveCounter * 1.0e7 / (now - lastFpsTime));
          lastFpsTime = now;
          frameCounter = 0;
          primitiveCounter = 0L;

          if (lastPps < 5.0e5)
            labelFps.Text = string.Format(CultureInfo.InvariantCulture, "Fps: {0:f1}, Pps: {1:f1}k",
                                          lastFps, lastPps * 1.0e-3);
          else
            labelFps.Text = string.Format(CultureInfo.InvariantCulture, "Fps: {0:f1}, Pps: {1:f1}m",
                                          lastFps, lastPps * 1.0e-6);

          if (fw != null)
            labelStat.Text = string.Format(CultureInfo.InvariantCulture, "time: {0:f1}s, fr: {1}{2}, laun: {3}, part: {4}",
                                           fw.Time, fw.Frames,
                                           (screencast != null) ? (" (" + screencast.Queue + ')') : "",
                                           fw.Launchers, fw.Particles);
        }
      }
    }

    /// <summary>
    /// OpenGL init code.
    /// </summary>
    void InitOpenGL ()
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // general OpenGL:
      glControl1.VSync = true;
      GL.ClearColor(Color.FromArgb(14, 20, 40));    // darker "navy blue"
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.VertexProgramPointSize);
      GL.ShadeModel(ShadingModel.Flat);

      // VBO init:
      VBOid = new uint[2];           // one big buffer for vertex data, another buffer for tri/line indices
      GL.GenBuffers(2, VBOid);
      GlInfo.LogError("VBO init");
      VBOlen = new int[2];           // zeroes..

      // shaders:
      canShaders = SetupShaders();

      // texture:
      GenerateTexture();
    }

    bool SetupShaders ()
    {
      activeProgram = null;

      foreach (var programInfo in programs.Values)
        if (programInfo.Setup())
          activeProgram = programInfo.program;

      if (activeProgram == null)
        return false;

      GlProgramInfo defInfo;
      if (programs.TryGetValue("default", out defInfo) &&
          defInfo.program != null)
        activeProgram = defInfo.program;

      return true;
    }

    // generated texture:
    const int TEX_SIZE = 128;
    const int TEX_CHECKER_SIZE = 8;
    static Vector3 colWhite = new Vector3(0.85f, 0.75f, 0.15f);
    static Vector3 colBlack = new Vector3(0.15f, 0.15f, 0.60f);
    static Vector3 colShade = new Vector3(0.15f, 0.15f, 0.15f);

    /// <summary>
    /// Texture handle
    /// </summary>
    int texName = 0;

    /// <summary>
    /// Generate the texture.
    /// </summary>
    void GenerateTexture ()
    {
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
      texName = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, texName);

      Vector3[] data = new Vector3[ TEX_SIZE * TEX_SIZE ];
      for (int y = 0; y < TEX_SIZE; y++)
        for (int x = 0; x < TEX_SIZE; x++)
        {
          int i = y * TEX_SIZE + x;
          bool odd = ((x / TEX_CHECKER_SIZE + y / TEX_CHECKER_SIZE) & 1) > 0;
          data[i] = odd ? colBlack : colWhite;
          // add some fancy shading on the edges:
          if ((x % TEX_CHECKER_SIZE) == 0 || (y % TEX_CHECKER_SIZE) == 0)
            data[i] += colShade;
          if (((x + 1) % TEX_CHECKER_SIZE) == 0 || ((y + 1) % TEX_CHECKER_SIZE) == 0)
            data[i] -= colShade;
        }

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEX_SIZE, TEX_SIZE, 0, PixelFormat.Rgb, PixelType.Float, data);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);

      GlInfo.LogError("create-texture");
    }

    static int Align (int address)
    {
      return ((address + 15) & -16);
    }

    /// <summary>
    /// Reset VBO buffer's size.
    /// Forces InitDataBuffers() call next time buffers will be needed..
    /// </summary>
    void ResetDataBuffers ()
    {
      VBOlen[0] = VBOlen[1] = 0;
    }

    /// <summary>
    /// Initialize VBO buffers.
    /// Determine maximum buffer sizes and allocate VBO objects.
    /// Vertex buffer (max 6 batches):
    /// <list type=">">
    /// <item>launchers - triangles</item>
    /// <item>launchers - lines</item>
    /// <item>launchers - points</item>
    /// <item>particles - triangles</item>
    /// <item>particles - lines</item>
    /// <item>particles - points</item>
    /// </list>
    /// Index buffer (max 4 batches):
    /// <list type=">">
    /// <item>launchers - triangles</item>
    /// <item>launchers - lines</item>
    /// <item>particles - triangles</item>
    /// <item>particles - lines</item>
    /// </list>
    /// </summary>
    unsafe void InitDataBuffers ()
    {
      Particle p;
      Launcher l;
      if (fw == null ||
          (p = fw.GetParticle(0)) == null ||
          (l = fw.GetLauncher(0)) == null)
        return;

      fw.Dirty = false;

      // init data buffers for current simulation state (current number of launchers + max number of particles):
      // triangles: determine maximum stride, maximum vertices and indices
      float* ptr = null;
      uint* iptr = null;
      uint origin = 0;
      int stride;

      // vertex-buffer size:
      int maxVB;
      maxVB = Align(fw.MaxParticles * p.TriangleVertices(ref ptr, ref origin, out stride, true, true, true, true) +
                                    fw.MaxLaunchers * l.TriangleVertices(ref ptr, ref origin, out stride, true, true, true, true));
      maxVB = Math.Max(maxVB, Align(fw.MaxParticles * p.LineVertices(ref ptr, ref origin, out stride, true, true, true, true) +
                                    fw.MaxLaunchers * l.LineVertices(ref ptr, ref origin, out stride, true, true, true, true) +
                                    (fw.HasAxes ? fw.axes.LineVertices(ref ptr, ref origin, out stride, true, true, true, true) : 0)));
      maxVB = Math.Max(maxVB, Align(fw.MaxParticles * p.PointVertices(ref ptr, ref origin, out stride, true, true, true, true) +
                                    fw.MaxLaunchers * l.PointVertices(ref ptr, ref origin, out stride, true, true, true, true)));
      // maxVB contains maximal vertex-buffer size for all batches

      // index-buffer size:
      int maxIB;
      maxIB = Align(fw.MaxParticles * p.TriangleIndices(ref iptr, 0) +
                                    fw.MaxLaunchers * l.TriangleIndices(ref iptr, 0));
      maxIB = Math.Max(maxIB, Align(fw.MaxParticles * p.LineIndices(ref iptr, 0) +
                                    fw.MaxLaunchers * l.LineIndices(ref iptr, 0) +
                                    (fw.HasAxes ? fw.axes.LineIndices(ref iptr, 0) : 0)));
      // maxIB contains maximal index-buffer size for all launchers

      VBOlen[0] = maxVB;
      VBOlen[1] = maxIB;

      // Vertex buffer in VBO[ 0 ]:
      GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)VBOlen[0], IntPtr.Zero, BufferUsageHint.DynamicDraw);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GlInfo.LogError("allocate vertex-buffer");

      // Index buffer in VBO[ 1 ]:
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[1]);
      GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)VBOlen[1], IntPtr.Zero, BufferUsageHint.DynamicDraw);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
      GlInfo.LogError("allocate index-buffer");
    }

    // appearance:
    Vector3 globalAmbient = new Vector3(  0.2f,  0.2f,  0.2f);
    Vector3 matAmbient    = new Vector3(  1.0f,  0.8f,  0.3f);
    Vector3 matDiffuse    = new Vector3(  1.0f,  0.8f,  0.3f);
    Vector3 matSpecular   = new Vector3(  0.8f,  0.8f,  0.8f);
    float matShininess    = 100.0f;
    Vector3 whiteLight    = new Vector3(  1.0f,  1.0f,  1.0f);
    Vector3 lightPosition = new Vector3(-20.0f, 10.0f, 10.0f);

    // attribute/vertex arrays:
    private void SetVertexAttrib (bool on)
    {
      if (activeProgram != null)
        if (on)
          activeProgram.EnableVertexAttribArrays();
        else
          activeProgram.DisableVertexAttribArrays();
    }

    void InitShaderRepository ()
    {
      programs.Clear();
      GlProgramInfo pi;

      // default program:
      pi = new GlProgramInfo("default", new GlShaderInfo[]
      {
        new GlShaderInfo(ShaderType.VertexShader, "vertex.glsl", "087fireworks"),
        new GlShaderInfo(ShaderType.FragmentShader, "fragment.glsl", "087fireworks")
      });
      programs[pi.name] = pi;

      // put more programs here:
      // pi = new GlProgramInfo( ..
      //   ..
      // programs[ pi.name ] = pi;
    }

    /// <summary>
    /// Simulation time of the last checkpoint in system ticks (100ns units)
    /// </summary>
    long ticksLast = DateTime.Now.Ticks;

    /// <summary>
    /// Simulation time of the last checkpoint in seconds.
    /// </summary>
    double timeLast = 0.0;

    /// <summary>
    /// Prime simulation init.
    /// </summary>
    private void InitSimulation ()
    {
      fw = new Fireworks();
      ResetSimulation();
    }

    /// <summary>
    /// [Re-]initialize the simulation.
    /// </summary>
    private void ResetSimulation ()
    {
      Snapshots.ResetFrameNumber();
      if (fw != null)
        lock (fw)
        {
          ResetDataBuffers();
          fw.Reset(textParam.Text);
          ticksLast = DateTime.Now.Ticks;
          timeLast = 0.0;
        }
    }

    /// <summary>
    /// Pause / restart simulation.
    /// </summary>
    private void PauseRestartSimulation ()
    {
      if (fw != null)
        lock (fw)
          fw.Running = !fw.Running;
    }

    /// <summary>
    /// Update Simulation parameters.
    /// </summary>
    private void UpdateSimulation ()
    {
      if (fw != null)
        lock (fw)
          fw.Update(textParam.Text);
    }

    /// <summary>
    /// Simulate one frame.
    /// </summary>
    private void Simulate ()
    {
      if (fw != null)
        lock (fw)
        {
          long nowTicks = DateTime.Now.Ticks;
          if (nowTicks > ticksLast)
          {
            if (fw.Running)
            {
              double timeScale = checkSlow.Checked ? Fireworks.slow : 1.0;
              timeLast += (nowTicks - ticksLast) * timeScale * 1.0e-7;
              fw.Simulate(timeLast);
            }
            ticksLast = nowTicks;
          }
        }
    }

    /// <summary>
    /// Render one frame.
    /// </summary>
    private void Render (bool snapshot = false)
    {
      if (!loaded)
        return;

      frameCounter++;
      useShaders = canShaders &&
                   activeProgram != null;

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.ShadeModel(ShadingModel.Smooth);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.Disable(EnableCap.CullFace);

      tb.GLsetCamera();
      RenderScene();

      if (snapshot &&
          screencast != null &&
          fw != null &&
          fw.Running)
        screencast.SaveScreenshotAsync(glControl1);

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    void RenderScene ()
    {
      if (useShaders &&
          fw != null)
      {
        if ((VBOlen[0] == 0 &&
             VBOlen[1] == 0) ||
            fw.Dirty)
          InitDataBuffers();

        if (VBOlen[0] > 0 ||
            VBOlen[1] > 0)
        {
          // Scene rendering from VBOs:
          SetVertexAttrib(true);

          // using GLSL shaders:
          GL.UseProgram(activeProgram.Id);

          // uniforms:
          Matrix4 modelView  = tb.ModelView;
          Matrix4 projection = tb.Projection;
          Vector3 eye        = tb.Eye;
          GL.UniformMatrix4(activeProgram.GetUniform("matrixModelView"), false, ref modelView);
          GL.UniformMatrix4(activeProgram.GetUniform("matrixProjection"), false, ref projection);

          GL.Uniform3(activeProgram.GetUniform("globalAmbient"), ref globalAmbient);
          GL.Uniform3(activeProgram.GetUniform("lightColor"), ref whiteLight);
          GL.Uniform3(activeProgram.GetUniform("lightPosition"), ref lightPosition);
          GL.Uniform3(activeProgram.GetUniform("eyePosition"), ref eye);
          GL.Uniform3(activeProgram.GetUniform("Ka"), ref matAmbient);
          GL.Uniform3(activeProgram.GetUniform("Kd"), ref matDiffuse);
          GL.Uniform3(activeProgram.GetUniform("Ks"), ref matSpecular);
          GL.Uniform1(activeProgram.GetUniform("shininess"), matShininess);

          // color handling:
          bool useColors = !checkGlobalColor.Checked;
          GL.Uniform1(activeProgram.GetUniform("globalColor"), useColors ? 0 : 1);

          // use varying normals?
          bool useNormals = checkNormals.Checked;
          GL.Uniform1(activeProgram.GetUniform("useNormal"), useNormals ? 1 : 0);

          bool usePointSize = checkPointSize.Checked;
          if (usePointSize)
            GL.Enable(EnableCap.VertexProgramPointSize);
          else
            GL.Disable(EnableCap.VertexProgramPointSize);
          GL.Uniform1(activeProgram.GetUniform("sizeMul"), usePointSize ? 1.0f : 0.0f);
          usePointSize = true;
          GlInfo.LogError("set-uniforms");

          // texture handling:
          bool useTexture = checkTexture.Checked;
          GL.Uniform1(activeProgram.GetUniform("useTexture"), useTexture ? 1 : 0);
          GL.Uniform1(activeProgram.GetUniform("texSurface"), 0);
          if (useTexture)
          {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texName);
          }
          GlInfo.LogError("set-texture");

          // [txt] [colors] [normals] [ptsize] vertices
          GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
          GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[1]);
          int stride;       // stride for vertex arrays
          int indices;      // number of indices for index arrays

          //-------------------------
          // draw all triangles:

          IntPtr vertexPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
          IntPtr indexPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
          unsafe
          {
            float* ptr = (float*)vertexPtr.ToPointer();
            uint* iptr = (uint*)indexPtr.ToPointer();
            indices = fw.FillTriangleData(ref ptr, ref iptr, out stride, useTexture, useColors, useNormals, usePointSize);
          }
          GL.UnmapBuffer(BufferTarget.ArrayBuffer);
          GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
          IntPtr p = IntPtr.Zero;

          if (indices > 0)
          {
            if (activeProgram.HasAttribute("texCoords"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("texCoords"), 2, VertexAttribPointerType.Float, false, stride, p);
            if (useTexture)
              p += Vector2.SizeInBytes;

            if (activeProgram.HasAttribute("color"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride, p);
            if (useColors)
              p += Vector3.SizeInBytes;

            if (activeProgram.HasAttribute("normal"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("normal"), 3, VertexAttribPointerType.Float, false, stride, p);
            if (useNormals)
              p += Vector3.SizeInBytes;

            if (activeProgram.HasAttribute("ptSize"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("ptSize"), 1, VertexAttribPointerType.Float, false, stride, p);
            if (usePointSize)
              p += sizeof(float);

            GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride, p);
            GlInfo.LogError("triangles-set-attrib-pointers");

            // engage!
            GL.DrawElements(PrimitiveType.Triangles, indices, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GlInfo.LogError("triangles-draw-elements");

            primitiveCounter += indices / 3;
          }

          //-------------------------
          // draw all lines:

          vertexPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
          indexPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
          unsafe
          {
            float* ptr = (float*)vertexPtr.ToPointer();
            uint* iptr = (uint*)indexPtr.ToPointer();
            indices = fw.FillLineData(ref ptr, ref iptr, out stride, useTexture, useColors, useNormals, usePointSize);
          }
          GL.UnmapBuffer(BufferTarget.ArrayBuffer);
          GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);

          if (indices > 0)
          {
            p = IntPtr.Zero;

            if (activeProgram.HasAttribute("texCoords"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("texCoords"), 2, VertexAttribPointerType.Float, false, stride, p);
            if (useTexture)
              p += Vector2.SizeInBytes;

            if (activeProgram.HasAttribute("color"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride, p);
            if (useColors)
              p += Vector3.SizeInBytes;

            if (activeProgram.HasAttribute("normal"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("normal"), 3, VertexAttribPointerType.Float, false, stride, p);
            if (useNormals)
              p += Vector3.SizeInBytes;

            if (activeProgram.HasAttribute("ptSize"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("ptSize"), 1, VertexAttribPointerType.Float, false, stride, p);
            if (usePointSize)
              p += sizeof(float);

            GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride, p);
            GlInfo.LogError("lines-set-attrib-pointers");

            // engage!
            GL.DrawElements(PrimitiveType.Lines, indices, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GlInfo.LogError("lines-draw-elements");

            primitiveCounter += indices / 2;
          }

          //-------------------------
          // draw all points:

          GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

          vertexPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
          unsafe
          {
            float* ptr = (float*)vertexPtr.ToPointer();
            indices = fw.FillPointData(ref ptr, out stride, useTexture, useColors, useNormals, usePointSize);
          }
          GL.UnmapBuffer(BufferTarget.ArrayBuffer);

          if (indices > 0)
          {
            p = IntPtr.Zero;

            if (activeProgram.HasAttribute("texCoords"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("texCoords"), 2, VertexAttribPointerType.Float, false, stride, p);
            if (useTexture)
              p += Vector2.SizeInBytes;

            if (activeProgram.HasAttribute("color"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride, p);
            if (useColors)
              p += Vector3.SizeInBytes;

            if (activeProgram.HasAttribute("normal"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("normal"), 3, VertexAttribPointerType.Float, false, stride, p);
            if (useNormals)
              p += Vector3.SizeInBytes;

            if (activeProgram.HasAttribute("ptSize"))
              GL.VertexAttribPointer(activeProgram.GetAttribute("ptSize"), 1, VertexAttribPointerType.Float, false, stride, p);
            if (usePointSize)
              p += sizeof(float);

            GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride, p);
            GlInfo.LogError("points-set-attrib-pointers");

            // engage!
            GL.DrawArrays(PrimitiveType.Points, 0, indices);
            GlInfo.LogError("points-draw-arrays");

            primitiveCounter += indices;
          }

          GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
          GL.UseProgram(0);

          return;
        }
      }

      // default: draw trivial cube..

      GL.Begin(PrimitiveType.Quads);

      GL.Color3(0.0f, 1.0f, 0.0f);          // Set The Color To Green
      GL.Vertex3(1.0f, 1.0f, -1.0f);        // Top Right Of The Quad (Top)
      GL.Vertex3(-1.0f, 1.0f, -1.0f);       // Top Left Of The Quad (Top)
      GL.Vertex3(-1.0f, 1.0f, 1.0f);        // Bottom Left Of The Quad (Top)
      GL.Vertex3(1.0f, 1.0f, 1.0f);         // Bottom Right Of The Quad (Top)

      GL.Color3(1.0f, 0.5f, 0.0f);          // Set The Color To Orange
      GL.Vertex3(1.0f, -1.0f, 1.0f);        // Top Right Of The Quad (Bottom)
      GL.Vertex3(-1.0f, -1.0f, 1.0f);       // Top Left Of The Quad (Bottom)
      GL.Vertex3(-1.0f, -1.0f, -1.0f);      // Bottom Left Of The Quad (Bottom)
      GL.Vertex3(1.0f, -1.0f, -1.0f);       // Bottom Right Of The Quad (Bottom)

      GL.Color3(1.0f, 0.0f, 0.0f);          // Set The Color To Red
      GL.Vertex3(1.0f, 1.0f, 1.0f);         // Top Right Of The Quad (Front)
      GL.Vertex3(-1.0f, 1.0f, 1.0f);        // Top Left Of The Quad (Front)
      GL.Vertex3(-1.0f, -1.0f, 1.0f);       // Bottom Left Of The Quad (Front)
      GL.Vertex3(1.0f, -1.0f, 1.0f);        // Bottom Right Of The Quad (Front)

      GL.Color3(1.0f, 1.0f, 0.0f);          // Set The Color To Yellow
      GL.Vertex3(1.0f, -1.0f, -1.0f);       // Bottom Left Of The Quad (Back)
      GL.Vertex3(-1.0f, -1.0f, -1.0f);      // Bottom Right Of The Quad (Back)
      GL.Vertex3(-1.0f, 1.0f, -1.0f);       // Top Right Of The Quad (Back)
      GL.Vertex3(1.0f, 1.0f, -1.0f);        // Top Left Of The Quad (Back)

      GL.Color3(0.0f, 0.0f, 1.0f);          // Set The Color To Blue
      GL.Vertex3(-1.0f, 1.0f, 1.0f);        // Top Right Of The Quad (Left)
      GL.Vertex3(-1.0f, 1.0f, -1.0f);       // Top Left Of The Quad (Left)
      GL.Vertex3(-1.0f, -1.0f, -1.0f);      // Bottom Left Of The Quad (Left)
      GL.Vertex3(-1.0f, -1.0f, 1.0f);       // Bottom Right Of The Quad (Left)

      GL.Color3(1.0f, 0.0f, 1.0f);          // Set The Color To Violet
      GL.Vertex3(1.0f, 1.0f, -1.0f);        // Top Right Of The Quad (Right)
      GL.Vertex3(1.0f, 1.0f, 1.0f);         // Top Left Of The Quad (Right)
      GL.Vertex3(1.0f, -1.0f, 1.0f);        // Bottom Left Of The Quad (Right)
      GL.Vertex3(1.0f, -1.0f, -1.0f);       // Bottom Right Of The Quad (Right)

      GL.End();

      primitiveCounter += 12;
    }
  }
}
