using System;
using System.Collections.Generic;
using System.Diagnostics;
using MathSupport;
using OpenTK;
using Utilities;
using Rendering;
using VojtechCerny;

namespace Rendering
{
  namespace KaterinaPrusova
  {
    public class OrangePeelBumpTexture : ITexture
    {
      //private PerlinNoise Perlin = new PerlinNoise(42);
      private PerlinNoise Perlin = new PerlinNoise(6, 3.0, 0.5, 3, seed: 42, baseAmplitude: 0.5);

      private GradientWrapper3D PerlinWrap = new GradientWrapper3D(new PerlinNoise(6, 8.0, 0.5, 3));

      private Vector3d StartColor = new Vector3d(1d, 200/255d, 0);
      private Vector3d EndColor = new Vector3d(204/255d, 0, 0);
      private Vector3d Diff => EndColor - StartColor;
      public long Apply (Intersection inter)
      {
        /*double noise = Perlin.OctavesNoise(inter.CoordObject.X, inter.CoordObject.Y, inter.CoordObject.Z, octaves:4, perstistence:0.2);
        Console.WriteLine(noise);
        inter.SurfaceColor = Vector3dToRGB(StartColor + Diff* noise);
        return 1;*/
        //double noise = Perlin.GetAt(inter.CoordObject.X, inter.CoordObject.Y, inter.CoordObject.Z);

        // Scale to 0-1
        /* noise = (noise + 1) / 2;

         double[] color = { noise, noise, noise };
         Util.ColorCopy(color, inter.SurfaceColor);

         inter.textureApplied = true;

         Geometry.GetTangents(ref localNormal, out inter.TangentU, out inter.TangentV);

         return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));*/
        //Vector3d localNormal = Vector3d.TransformVector(inter.Normal, inter.WorldToLocal);

        //Geometry.GetTangents(ref localNormal, out inter.TangentU, out inter.TangentV);
        //Geometry.GetTangents(ref localNormal, out inter.TangentU, out inter.TangentV);

        //double noise = Perlin.GetAt(inter.CoordObject.X, inter.CoordObject.Y, inter.CoordObject.Z, out xGrad, out yGrad, out zGrad);
        //Vector3d gradientLocal = new Vector3d(xGrad, yGrad, zGrad);
        /*Vector3d normalLocal = inter.NormalLocal;
        Vector3d normalObject = Vector3d.TransformVector(normalLocal, inter.LocalToObject);
        Geometry.GetTangents(ref normalObject, out Vector3d tangentUObject, out Vector3d tangentVObject);
        tangentUObject = tangentUObject.Normalized();
        tangentVObject = tangentVObject.Normalized();

        double noise = NoiseWithTangentGradient(inter.CoordObject.X, inter.CoordObject.Y, inter.CoordObject.Z, tangentUObject, tangentVObject, out double gradU, out double gradV);

        Vector3d D = (gradU * Vector3d.Cross(normalObject, tangentVObject) - gradV * Vector3d.Cross(normalObject, tangentUObject) / normalObject.Length);
        Vector3d modifiedNomalObject = normalObject + D;

        inter.NormalLocal = Vector3d.Transform(modifiedNomalObject, Matrix4d.Invert(inter.LocalToObject)).Normalized();
        inter.Normal = Vector3d.Transform(inter.NormalLocal, inter.LocalToWorld).Normalized();*/

        /*double xGrad, yGrad;

        Vector3d localNormal = Vector3d.TransformVector(inter.Normal, inter.WorldToLocal);
        Geometry.GetTangents(ref localNormal, out inter.TangentU, out inter.TangentV);

        //double noise = NoiseWithTangentGradient(inter.CoordLocal.X, inter.CoordLocal.Y, inter.CoordLocal.Z, inter.TangentU, inter.TangentV,out xGrad, out yGrad);
        double noise = PerlinWrap.GetAt(inter.CoordLocal.X, inter.CoordLocal.Y, inter.CoordLocal.Z, out xGrad, out yGrad, out double zGrad);

        Vector3d tu = Vector3d.TransformVector(inter.TangentU, inter.LocalToWorld).Normalized();
        Vector3d tv = Vector3d.TransformVector(inter.TangentV, inter.LocalToWorld).Normalized();

        inter.Normal += zGrad * 0.1 * tu +
                        yGrad * 0.1 * tv;
        inter.Normal.Normalize();*/

        return StackExchangeIdea(inter);
      }

      public long StackExchangeIdea(Intersection inter)
      {

        double Xlocal = inter.CoordLocal.X;
        double Ylocal = inter.CoordLocal.Y;
        double Zlocal = inter.CoordLocal.Z;
        Vector3d pointLocal = inter.CoordLocal;

        double radius = Math.Sqrt(Xlocal*Xlocal + Ylocal*Ylocal + Zlocal*Zlocal);
        double modulationDepthS = 1d;
        double noise = PerlinWrap.GetAt(Xlocal, Ylocal, Zlocal, out double gradX, out double gradY, out double gradZ);
        Vector3d gradientLocal = new Vector3d(gradX, gradY, gradZ) / (radius + (modulationDepthS * noise));
        Vector3d gradientProjectionH = gradientLocal - (Vector3d.Dot(gradientLocal, pointLocal) * pointLocal);
        Vector3d normalLocal = pointLocal -modulationDepthS * gradientProjectionH;
        inter.NormalLocal = normalLocal.Normalized();
        inter.Normal = Vector3d.Transform(inter.NormalLocal, inter.LocalToWorld).Normalized();
        inter.textureApplied = true;

        return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));
      }

      public long SomeIdea(Intersection inter)
      {
        double scale = 0.1d;

        double Xlocal = inter.CoordLocal.X;
        double Ylocal = inter.CoordLocal.Y;
        double Zlocal = inter.CoordLocal.Z;
        double radius = Math.Sqrt(Xlocal*Xlocal + Ylocal*Ylocal + Zlocal*Zlocal);
        double phi = Math.Atan2(Ylocal, Xlocal) + Math.PI/2d;
        double theta;
        if (Zlocal <= 0)
        {
          theta = -Math.Acos(Zlocal / radius);
        }
        else
        {
          theta = Math.Acos(Zlocal / radius);
        }

        double noise = PerlinWrap.GetAt(phi, theta, radius, out double gPhi, out double gTheta, out double gRad);
        Vector3d D = (scale *gPhi * Vector3d.Cross(inter.NormalLocal, inter.TangentV) - (scale * gTheta * Vector3d.Cross(inter.NormalLocal, inter.TangentU)) / inter.NormalLocal.Length);
        Vector3d normalLocalNew = (inter.NormalLocal * scale * gRad* (-1d)) + D;
        inter.NormalLocal = normalLocalNew.Normalized();
        inter.Normal = Vector3d.TransformVector(normalLocalNew, inter.LocalToWorld).Normalized();
        inter.textureApplied = true;
        return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));
      }

      public long Sperical(Intersection inter)
      {
        double scale = 1d;

        double Xlocal = inter.CoordLocal.X;
        double Ylocal = inter.CoordLocal.Y;
        double Zlocal = inter.CoordLocal.Z;
        double radius = Math.Sqrt(Xlocal*Xlocal + Ylocal*Ylocal + Zlocal*Zlocal);
        double phi = Math.Atan2(Ylocal, Xlocal) + Math.PI/2d;
        double theta;
        if (Zlocal <= 0)
        {
          theta = -Math.Acos(Zlocal / radius);
        }
        else
        {
          theta = Math.Acos(Zlocal / radius);
        }

        //double noise = Perlin.GetAt(phi, theta, out double gradPhi, out double gradTheta);
        double noise = SinusNoiseWithGradient(phi, theta, out double gradPhi, out double gradTheta);

        Geometry.GetTangents(ref inter.NormalLocal, out inter.TangentU, out inter.TangentV);

        Vector3d D = (scale * gradPhi * Vector3d.Cross(inter.NormalLocal, inter.TangentV) - (scale * gradTheta * Vector3d.Cross(inter.NormalLocal, inter.TangentU)) / inter.NormalLocal.Length);
        Vector3d normalLocalNew = (inter.NormalLocal + D).Normalized();
        inter.NormalLocal = normalLocalNew;
        inter.Normal = Vector3d.TransformVector(normalLocalNew, inter.LocalToWorld).Normalized();

        //Console.WriteLine(radius);
        //return 42;
        return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));
      }

      public long FailedShiningOrangeXYZderivatesAcordingUV(Intersection inter)
      {
        double scale = 0.1d;
        Vector3d localNormal = Vector3d.TransformVector(inter.Normal, inter.WorldToLocal);
        Geometry.GetTangents(ref localNormal, out inter.TangentU, out inter.TangentV);

        //Geometry.GetTangents(ref inter.NormalLocal, out inter.TangentU, out inter.TangentV);
        double noise = NoiseWithTangentGradient(inter.CoordLocal.X, inter.CoordLocal.Y, inter.CoordLocal.Z, inter.TangentU, inter.TangentV, out double gradU, out double gradV);
        Vector3d D = (scale *gradU * Vector3d.Cross(inter.NormalLocal, inter.TangentV) - (scale * gradV * Vector3d.Cross(inter.NormalLocal, inter.TangentU)) / inter.NormalLocal.Length);
        Vector3d normalLocalNew = inter.NormalLocal + D;
        inter.NormalLocal = normalLocalNew.Normalized();
        inter.Normal = Vector3d.TransformVector(normalLocalNew, inter.LocalToWorld).Normalized();
        inter.textureApplied = true;
        return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));
      }

      public long Cernys2DMapping(Intersection inter)
      {
        double xGrad, yGrad;

        double noise = Perlin.GetAt(inter.TextureCoord.X, inter.TextureCoord.Y, out xGrad, out yGrad);

        Vector3d localNormal = Vector3d.TransformVector(inter.Normal, inter.WorldToLocal);
        Geometry.GetTangents(ref localNormal, out inter.TangentU, out inter.TangentV);

        Vector3d tu = Vector3d.TransformVector(inter.TangentU, inter.LocalToWorld).Normalized();
        Vector3d tv = Vector3d.TransformVector(inter.TangentV, inter.LocalToWorld).Normalized();

        inter.Normal += xGrad * 0.1 * tu +
                        yGrad * 0.1 * tv;
        inter.Normal.Normalize();

        return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));
      }

      public long WorkingBestForNoReason(Intersection inter)
      {

        double Xlocal = inter.CoordLocal.X;
        double Ylocal = inter.CoordLocal.Y;
        double Zlocal = inter.CoordLocal.Z;

        double scale = 0.001d;

        Vector3d normalLocal = inter.NormalLocal;
        Geometry.GetTangents(ref normalLocal, out Vector3d tangentULocal, out Vector3d tangentVLocal);
        Matrix4d LocalToTangent  = BuildCoordSysTransform(tangentULocal,tangentVLocal, normalLocal, Xlocal, Ylocal, Zlocal);
        Vector3d newXYZ = Vector3d.TransformPosition(new Vector3d(Xlocal, Ylocal,Zlocal), LocalToTangent);
        //Console.WriteLine(LocalToTangent);
        Console.WriteLine(newXYZ);
        double noise = Perlin.GetAt(newXYZ.X, newXYZ.Y, out double gradU, out double gradV);
        //double noise = Perlin.GetAt(inter.CoordLocal.X, inter.CoordLocal.Y, out double gradU, out double gradV);
        Vector3d D = (scale *gradU * Vector3d.Cross(normalLocal, tangentVLocal) - (scale * gradV * Vector3d.Cross(normalLocal, tangentULocal)) / normalLocal.Length);
        Vector3d normalTangent = (normalLocal + D).Normalized();
        inter.NormalLocal = Vector3d.Transform(normalTangent, Matrix4d.Invert(LocalToTangent)).Normalized();
        inter.Normal = Vector3d.Transform(inter.NormalLocal, inter.LocalToWorld).Normalized();
        Geometry.GetTangents(ref inter.NormalLocal, out inter.TangentU, out inter.TangentV);
        inter.textureApplied = true;
        return (long)RandomStatic.numericRecipes((ulong)(noise * 1000000));
      }

      public Matrix4d BuildCoordSysTransform (Vector3d tangentU, Vector3d tangentV, Vector3d normal, double origX, double origY, double origZ)
      {
        return Matrix4d.Invert(Matrix4d.Transpose(new Matrix4d(new Vector4d(tangentU, origX),
                                                new Vector4d(tangentV, origY),
                                                new Vector4d(normal, origZ),
                                                new Vector4d(0, 0, 0, 1))));

        /*return new Matrix4d(new Vector4d(tangentU, 0),
                              new Vector4d(tangentV, 0),
                              new Vector4d(normal, 0),
                              new Vector4d(origX, origY, origZ, 1));*/
      }

      public double NoiseWithTangentGradient (double x, double y, double z, Vector3d tangentU, Vector3d tangentV, out double uD, out double vD, double delta = 1e-6)
      {
        double value = Perlin.GetAt(x, y, z);

        uD = ((Perlin.GetAt(x + delta * tangentU.X, y + delta * tangentU.Y, z + delta * tangentU.Z)- value) / delta);
        vD = ((Perlin.GetAt(x + delta * tangentV.X, y + delta * tangentV.Y, z + delta * tangentV.Z)- value) / delta);
        return value;
      }


      public double SinusNoiseWithGradient (double x, double y, out double xD, out double yD, double delta = 1e-6)
      {
        double value = Math.Sin(Perlin.GetAt(x, y));

        xD = (Math.Sin((Perlin.GetAt(x + delta, y) - value)) / delta);
        yD = (Math.Sin((Perlin.GetAt(x, y + delta)) - value) / delta);
        return value;
      }

      private double[] Vector3dToRGB (Vector3d col)
      {
        return new double[] { col.X, col.Y, col.Z };
      }

    }
  }
}
