using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml;
using Autodesk.DesignScript.Interfaces;
using Dynamo.Controls;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Logging;
using Dynamo.Selection;
using Dynamo.ViewModels;
using Dynamo.Wpf.Properties;
using Dynamo.Visualization;
using DynamoUtilities;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using Color = SharpDX.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using GeometryModel3D = HelixToolkit.Wpf.SharpDX.GeometryModel3D;
using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
using Model3D = HelixToolkit.Wpf.SharpDX.Model3D;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
using TextInfo = HelixToolkit.Wpf.SharpDX.TextInfo;
using Newtonsoft.Json;

namespace Dynamo.Wpf.ViewModels.Watch3D
{
    public class CameraData
    { 
        // Default camera position data. These values have been rounded
        // to the nearest whole value.
        // eyeX="-16.9655136013663" eyeY="24.341577725171" eyeZ="50.6494323150915" 
        // lookX="12.4441040333119" lookY="-13.0110656299395" lookZ="-58.5449065206009" 
        // upX="-0.0812375695793365" upY="0.920504853452448" upZ="0.3821927158638" />

        private readonly Vector3D defaultCameraLookDirection = new Vector3D(12, -13, -58);
        private readonly Point3D defaultCameraPosition = new Point3D(-17, 24, 50);
        private readonly Vector3D defaultCameraUpDirection = new Vector3D(0, 1, 0);
        private const double defaultNearPlaneDistance = 0.1;
        private const double defaultFarPlaneDistance = 10000000;

        [JsonIgnore]
        public Point3D EyePosition { get { return new Point3D(EyeX, EyeY, EyeZ); } }
        [JsonIgnore]
        public Vector3D UpDirection { get { return new Vector3D(UpX, UpY, UpZ); } }
        [JsonIgnore]
        public Vector3D LookDirection { get { return new Vector3D(LookX, LookY, LookZ); } }
        [JsonIgnore]
        public double NearPlaneDistance { get; set; }
        [JsonIgnore]
        public double FarPlaneDistance { get; set; }

        // JSON camera data
        public string Name { get; set; }
        public double EyeX { get; set; }
        public double EyeY { get; set; }
        public double EyeZ { get; set; }
        public double LookX { get; set; }
        public double LookY { get; set; }
        public double LookZ { get; set; }
        public double UpX { get; set; }
        public double UpY { get; set; }
        public double UpZ { get; set; }

        public CameraData()
        {
            NearPlaneDistance = defaultNearPlaneDistance;
            FarPlaneDistance = defaultFarPlaneDistance;

            Name = "Default Camera";
            EyeX = defaultCameraPosition.X;
            EyeY = defaultCameraPosition.Y;
            EyeZ = defaultCameraPosition.Z;
            LookX = defaultCameraLookDirection.X;
            LookY = defaultCameraLookDirection.Y;
            LookZ = defaultCameraLookDirection.Z;
            UpX = defaultCameraUpDirection.X;
            UpY = defaultCameraUpDirection.Y;
            UpZ = defaultCameraUpDirection.Z;
        }
        
        public override bool Equals(object obj)
        {
            var other = obj as CameraData;
            return obj is CameraData && this.Name == other.Name
                   && Math.Abs(this.EyeX - other.EyeX) < 0.0001
                   && Math.Abs(this.EyeY - other.EyeY) < 0.0001
                   && Math.Abs(this.EyeZ - other.EyeZ) < 0.0001
                   && Math.Abs(this.LookX - other.LookX) < 0.0001
                   && Math.Abs(this.LookY - other.LookY) < 0.0001
                   && Math.Abs(this.LookZ - other.LookZ) < 0.0001
                   && Math.Abs(this.UpX - other.UpX) < 0.0001
                   && Math.Abs(this.UpY - other.UpY) < 0.0001
                   && Math.Abs(this.UpZ - other.UpZ) < 0.0001
                   && Math.Abs(this.NearPlaneDistance - other.NearPlaneDistance) < 0.0001
                   && Math.Abs(this.FarPlaneDistance - other.FarPlaneDistance) < 0.0001;
        }


    }
    
    internal static class CameraExtensions
    {
        public static CameraData ToCameraData(this PerspectiveCamera camera, string name)
        {
            var camData = new CameraData
            {
                NearPlaneDistance = camera.NearPlaneDistance,
                FarPlaneDistance = camera.FarPlaneDistance,

                Name = name,
                EyeX = camera.Position.X,
                EyeY = camera.Position.Y,
                EyeZ = camera.Position.Z,
                LookX = camera.LookDirection.X,
                LookY = camera.LookDirection.Y,
                LookZ = camera.LookDirection.Z,
                UpX = camera.UpDirection.X,
                UpY = camera.UpDirection.Y,
                UpZ = camera.UpDirection.Z
            };

            return camData;
        }
    }

    internal static class BoundingBoxExtensions
    {
        /// <summary>
        /// Convert a <see cref="BoundingBox"/> to a <see cref="Rect3D"/>
        /// </summary>
        /// <param name="bounds">The <see cref="BoundingBox"/> to be converted.</param>
        /// <returns>A <see cref="Rect3D"/> object.</returns>
        internal static Rect3D ToRect3D(this BoundingBox bounds)
        {
            var min = bounds.Minimum;
            var max = bounds.Maximum;
            var size = new Size3D((max.X - min.X), (max.Y - min.Y), (max.Z - min.Z));
            return new Rect3D(min.ToPoint3D(), size);
        }

        /// <summary>
        /// If a <see cref="GeometryModel3D"/> has more than one point, then
        /// return its bounds, otherwise, return a bounding
        /// box surrounding the point of the supplied size.
        /// 
        /// This extension method is to correct for the Helix toolkit's GeometryModel3D.Bounds
        /// property which does not update correctly as new geometry is added to the GeometryModel3D.
        /// </summary>
        /// <param name="geom">A <see cref="GeometryModel3D"/> object.</param>
        /// <returns>A <see cref="BoundingBox"/> object encapsulating the geometry.</returns>
        internal static BoundingBox Bounds(this GeometryModel3D geom, float defaultBoundsSize = 5.0f)
        {
            if (geom.Geometry.Positions.Count == 0)
            {
                return new BoundingBox();
            }

            if (geom.Geometry.Positions.Count > 1)
            {
                return BoundingBox.FromPoints(geom.Geometry.Positions.ToArray());
            }

            var pos = geom.Geometry.Positions.First();
            var min = pos + new Vector3(-defaultBoundsSize, -defaultBoundsSize, -defaultBoundsSize);
            var max = pos + new Vector3(defaultBoundsSize, defaultBoundsSize, defaultBoundsSize);
            return new BoundingBox(min, max);
        }

        public static Vector3 Center(this BoundingBox bounds)
        {
            return (bounds.Maximum + bounds.Minimum)/2;
        }

    }

    internal static class Vector3Extensions
    {
        internal static double DistanceToPlane(this Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
        {
            return Vector3.Dot(planeNormal, (point - planeOrigin));
        }
    }

    internal static class DoubleExtensions
    {
        internal static bool AlmostEqualTo(this double a, double b, double tolerance)
        {
            return Math.Abs(a - b) < tolerance;
        }
    }
}
