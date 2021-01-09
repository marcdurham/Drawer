using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Viewport3D myViewport3D;
        PerspectiveCamera myPCamera = new PerspectiveCamera();

        public MainWindow()
        {
            InitializeComponent();
            // Declare scene objects.
            ////myViewport3D = new Viewport3D();
            var myModel3DGroup = new Model3DGroup();
            var myGeometryModel = new GeometryModel3D();
            var myModelVisual3D = new ModelVisual3D();
            // Defines the camera used to view the 3D object. In order to view the 3D object,
            // the camera must be positioned and pointed such that the object is within view
            // of the camera.
            //var myPCamera = new PerspectiveCamera();

            // Specify where in the 3D scene the camera is.
            myPCamera.Position = new Point3D(0, 0, 2);

            // Specify the direction that the camera is pointing.
            myPCamera.LookDirection = new Vector3D(0, 0, -1);

            // Define camera's horizontal field of view in degrees.
            myPCamera.FieldOfView = 60;

            // Asign the camera to the viewport
            myViewport3D.Camera = myPCamera;
            // Define the lights cast in the scene. Without light, the 3D object cannot
            // be seen. Note: to illuminate an object from additional directions, create
            // additional lights.
            var myDirectionalLight = new DirectionalLight();
            myDirectionalLight.Color = Colors.White;
            myDirectionalLight.Direction = new Vector3D(-0.61, -0.5, -0.61);

            myModel3DGroup.Children.Add(myDirectionalLight);

            // The geometry specifes the shape of the 3D plane. In this sample, a flat sheet
            // is created.
            var myMeshGeometry3D = new MeshGeometry3D();

            // Create a collection of normal vectors for the MeshGeometry3D.
            var myNormalCollection = new Vector3DCollection();
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myNormalCollection.Add(new Vector3D(0, 0, 1));
            myMeshGeometry3D.Normals = myNormalCollection;

            // Create a collection of vertex positions for the MeshGeometry3D.
            var myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(new Point3D(-0.5, -0.5, 0.5));
            myPositionCollection.Add(new Point3D(0.5, -0.5, 0.5));
            myPositionCollection.Add(new Point3D(0.5, 0.5, 0.5));
            myPositionCollection.Add(new Point3D(0.5, 0.5, 0.5));
            myPositionCollection.Add(new Point3D(-0.5, 0.5, 0.5));
            myPositionCollection.Add(new Point3D(-0.5, -0.5, 0.5));
            myMeshGeometry3D.Positions = myPositionCollection;

            // Create a collection of texture coordinates for the MeshGeometry3D.
            var myTextureCoordinatesCollection = new PointCollection();
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myTextureCoordinatesCollection.Add(new Point(1, 0));
            myTextureCoordinatesCollection.Add(new Point(1, 1));
            myTextureCoordinatesCollection.Add(new Point(1, 1));
            myTextureCoordinatesCollection.Add(new Point(0, 1));
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myMeshGeometry3D.TextureCoordinates = myTextureCoordinatesCollection;

            // Create a collection of triangle indices for the MeshGeometry3D.
            var myTriangleIndicesCollection = new Int32Collection();
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(5);
            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

            // Apply the mesh to the geometry model.
            myGeometryModel.Geometry = myMeshGeometry3D;

            // The material specifies the material applied to the 3D object. In this sample a
            // linear gradient covers the surface of the 3D object.

            // Create a horizontal linear gradient with four stops.
            var myHorizontalGradient = new LinearGradientBrush();
            myHorizontalGradient.StartPoint = new Point(0, 0.5);
            myHorizontalGradient.EndPoint = new Point(1, 0.5);
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));

            // Define material and apply to the mesh geometries.
            var myMaterial = new DiffuseMaterial(myHorizontalGradient);
            myGeometryModel.Material = myMaterial;

            // Apply a transform to the object. In this sample, a rotation transform is applied,
            // rendering the 3D object rotated.
            var myRotateTransform3D = new RotateTransform3D();
            var myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = new Vector3D(0, 3, 0);
            rotation = 40;
            //myAxisAngleRotation3d.Angle = 40;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;
            myGeometryModel.Transform = myRotateTransform3D;

           

            // Add the geometry model to the model group.
            myModel3DGroup.Children.Add(myGeometryModel);

            // Add the group of models to the ModelVisual3d.
            myModelVisual3D.Content = myModel3DGroup;


            //
            myViewport3D.Children.Add(myModelVisual3D);

            // Apply the viewport to the page so it will be rendered.
            //this.Content = myViewport3D;
            //this.MouseDown += HitTest;
            myViewport3D.MouseDown += HitTest;
        }

        public void HitTest(object sender, MouseButtonEventArgs args)
        {
            Point mouseposition = args.GetPosition(myViewport3D);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D
            VisualTreeHelper.HitTest(myViewport3D, null, HTResult, pointparams);
        }

        private void myViewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseposition = e.GetPosition(myViewport3D);
            statusButton.Content = $"M:{mouseposition.X},{mouseposition.Y}";
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D
            VisualTreeHelper.HitTest(myViewport3D, null, MouseMoveResult, pointparams);
        }

        public HitTestResultBehavior HTResult(HitTestResult rawresult)
        {
            MessageBox.Show(rawresult.ToString());
            var rayResult = rawresult as RayHitTestResult;
            
            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;

                if (rayMeshResult != null)
                {
                    var hitgeo = rayMeshResult.ModelHit as GeometryModel3D;

                   // UpdateResultInfo(rayMeshResult);
                   // UpdateMaterial(hitgeo, (side1GeometryModel3D.Material as MaterialGroup));
                }
            }

            return HitTestResultBehavior.Continue;
        }

        private int rotation = 0;
        public HitTestResultBehavior MouseMoveResult(HitTestResult rawresult)
        {
            //MessageBox.Show("MouseMove:" + rawresult.ToString());
            var rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;

                if (rayMeshResult != null)
                {
                    var hitgeo = rayMeshResult.ModelHit as GeometryModel3D;
                    statusButton2.Content = $"3D:{rayMeshResult.PointHit.X:F4},{rayMeshResult.PointHit.Y:F4},{rayMeshResult.PointHit.Z:F4}";


                    // UpdateResultInfo(rayMeshResult);
                    // UpdateMaterial(hitgeo, (side1GeometryModel3D.Material as MaterialGroup));
                }
            }

            return HitTestResultBehavior.Continue;
        }

        private void zoomOut_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.Position = new Point3D(0, 0, myPCamera.Position.Z + 1);
        }

        private void zoomIn_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.Position = new Point3D(0, 0, myPCamera.Position.Z - 1);
        }


        //// Add a cylinder.
        //private void AddCylinder(
        //    MeshGeometry3D mesh,
        //    Point3D end_point, 
        //    Vector3D axis, 
        //    double radius, 
        //    int num_sides)
        //{
        //    // Get two vectors perpendicular to the axis.
        //    Vector3D v1;
        //    if ((axis.Z < -0.01) || (axis.Z > 0.01))
        //        v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
        //    else
        //        v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
        //    Vector3D v2 = Vector3D.CrossProduct(v1, axis);

        //    // Make the vectors have length radius.
        //    v1 *= (radius / v1.Length);
        //    v2 *= (radius / v2.Length);

        //    // Make the top end cap.
        //    double theta = 0;
        //    double dtheta = 2 * Math.PI / num_sides;
        //    for (int i = 0; i < num_sides; i++)
        //    {
        //        Point3D p1 = end_point +
        //            Math.Cos(theta) * v1 +
        //            Math.Sin(theta) * v2;
        //        theta += dtheta;
        //        Point3D p2 = end_point +
        //            Math.Cos(theta) * v1 +
        //            Math.Sin(theta) * v2;
        //        AddTriangle(mesh, end_point, p1, p2);
        //    }

        //    // Make the bottom end cap.
        //    Point3D end_point2 = end_point + axis;
        //    theta = 0;
        //    for (int i = 0; i < num_sides; i++)
        //    {
        //        Point3D p1 = end_point2 +
        //            Math.Cos(theta) * v1 +
        //            Math.Sin(theta) * v2;
        //        theta += dtheta;
        //        Point3D p2 = end_point2 +
        //            Math.Cos(theta) * v1 +
        //            Math.Sin(theta) * v2;
        //        AddTriangle(mesh, end_point2, p2, p1);
        //    }

        //    // Make the sides.
        //    theta = 0;
        //    for (int i = 0; i < num_sides; i++)
        //    {
        //        Point3D p1 = end_point +
        //            Math.Cos(theta) * v1 +
        //            Math.Sin(theta) * v2;
        //        theta += dtheta;
        //        Point3D p2 = end_point +
        //            Math.Cos(theta) * v1 +
        //            Math.Sin(theta) * v2;

        //        Point3D p3 = p1 + axis;
        //        Point3D p4 = p2 + axis;

        //        AddTriangle(mesh, p1, p3, p2);
        //        AddTriangle(mesh, p2, p3, p4);
        //    }
        //}
    }
}
