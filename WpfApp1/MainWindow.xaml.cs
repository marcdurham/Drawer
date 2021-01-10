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
        DrawMode DrawMode;
        CubeCollection cubes = new CubeCollection();
        PipeSegmentCollection pipeSegments = new PipeSegmentCollection();
        Stack<Point3D> clicks = new Stack<Point3D>();
        //Viewport3D myViewport3D;
        OrthographicCamera myPCamera = new OrthographicCamera();
        ModelVisual3D paper;
        Point3D mouseMiddleDownPoint;

        public MainWindow()
        {
            InitializeComponent();

            // Specify where in the 3D scene the camera is.
            myPCamera.Position = new Point3D(0, 0, 2);

            // Specify the direction that the camera is pointing.
            myPCamera.LookDirection = new Vector3D(0, 0, -1);

            // Define camera's horizontal field of view in degrees.
            //myPCamera.FieldOfView = 60;

            myViewport3D.Camera = myPCamera;
            paper = PaperBuilder.GetPaper();
            

            //
            myViewport3D.Children.Add(paper);

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
            if (args.LeftButton == MouseButtonState.Pressed)
            {
                VisualTreeHelper.HitTest(myViewport3D, null, HTResult, pointparams);
            }
        }

        private void myViewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseposition = e.GetPosition(myViewport3D);
            


            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);


            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            var pointparams = new PointHitTestParameters(mouseposition);
            
            var rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            if(DrawMode == DrawMode.PipeStart)
            {
                //double xDiff = myPCamera.Position.X
                //myPCamera.Position = new Point3D(testpoint3D.X, testpoint3D.Y, myPCamera.Position.Z);

                
                //floater.Visual.Transform = new 
                //myViewport3D.Children.Add(floater.Visual);
            }

            statusButton.Content = $"M:{mouseposition.X},{mouseposition.Y}/{rayparams.Origin.X:F2},{rayparams.Origin.Y:F2}";
            
            //test for a result in the Viewport3D
            VisualTreeHelper.HitTest(myViewport3D, null, MouseMoveResult, pointparams);
           
        }
        private Cube floater;
        public HitTestResultBehavior HTResult(HitTestResult rawresult)
        {
            //MessageBox.Show(rawresult.ToString());
            var rayResult = rawresult as RayHitTestResult;
            
            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;

                if (rayMeshResult != null)
                {
                    var hitgeo = rayMeshResult.ModelHit as GeometryModel3D;
                    
                    clicks.Push(rayMeshResult.PointHit);

                    if (DrawMode == DrawMode.Create)
                    {
                        DrawThing(rayMeshResult.PointHit, Brushes.Red);
                    }
                    else if(DrawMode == DrawMode.Select)
                    {
                        if (cubes.Remove(rayMeshResult.VisualHit))
                        {
                            myViewport3D.Children.Remove(rayMeshResult.VisualHit);
                        }

                        DrawThing(rayMeshResult.PointHit, Brushes.Blue);
                    }
                    else if(DrawMode == DrawMode.Delete)
                    {
                        if (cubes.Remove(rayMeshResult.VisualHit))
                        {
                            myViewport3D.Children.Remove(rayMeshResult.VisualHit);
                        }
                        if (pipeSegments.Remove(rayMeshResult.VisualHit))
                        {
                            myViewport3D.Children.Remove(rayMeshResult.VisualHit);
                        }

                    }
                    else if (DrawMode == DrawMode.PipeStart)
                    {
                        startPipePoint = rayResult.PointHit;
                        DrawMode = DrawMode.PipeContinue;
                        modeLabel.Content = "PipeContinue";
                    }
                    else if (DrawMode == DrawMode.PipeContinue)
                    {
                        //DrawThing(startPipePoint, Brushes.Green);
                        //DrawThing(rayResult.PointHit, Brushes.Yellow);
                        DrawPipeSegment(startPipePoint, rayResult.PointHit, Brushes.Blue);
                        startPipePoint = rayResult.PointHit;
                    }
                    // UpdateResultInfo(rayMeshResult);
                    // UpdateMaterial(hitgeo, (side1GeometryModel3D.Material as MaterialGroup));
                }
            }

            return HitTestResultBehavior.Continue;
        }

        public HitTestResultBehavior MiddleButtonHTResult(HitTestResult rawresult)
        {
            var rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                var rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;

                if (rayMeshResult != null)
                {
                    //var hitgeo = rayMeshResult.ModelHit as GeometryModel3D;

                    if (DrawMode == DrawMode.Panning)
                    {
                        mouseMiddleDownPoint = rayMeshResult.PointHit;
                        statusMouseMiddleDownPointLabel.Content = $"MMDP:{rayMeshResult.PointHit.X:F4},{rayMeshResult.PointHit.Y:F4}";
                    }
                }
            }

            return HitTestResultBehavior.Continue;
        }

        void DrawPipeSegment(Point3D start, Point3D end, Brush brush)
        {
            var myModel3DGroup = new Model3DGroup();
            var myGeometryModel = new GeometryModel3D();
            var myModelVisual3D = new ModelVisual3D();

            // The geometry specifes the shape of the 3D plane. In this sample, a flat sheet
            // is created.
            var myMeshGeometry3D = new MeshGeometry3D();

            var myDirectionalLight = new DirectionalLight();
            myDirectionalLight.Color = Colors.White;
            myDirectionalLight.Direction = new Vector3D(-0.61, -0.5, -0.61);

            myModel3DGroup.Children.Add(myDirectionalLight);
            var myAmbientLight = new AmbientLight(Colors.White);
            myModel3DGroup.Children.Add(myAmbientLight);
            // Create a collection of vertex positions for the MeshGeometry3D.
            var myPositionCollection = new Point3DCollection();

            double width = 0.005;
            double halfWidth = width / 2;
            //myPositionCollection.Add(new Point3D(start.X, start.Y, 0.5));

            double xd = start.X - end.X;
            double yd = start.Y - end.Y;
            double ratio = halfWidth / Math.Sqrt(Math.Pow(xd, 2) + Math.Pow(yd, 2));
            double xOffset = ratio * Math.Abs(yd);
            double yOffset = ratio * Math.Abs(xd);
            double realRatio = xd / yd;


            if (realRatio > 0)
            {
                myPositionCollection.Add(new Point3D(start.X + xOffset, start.Y - yOffset, 0.5));
                myPositionCollection.Add(new Point3D(start.X - xOffset, start.Y + yOffset, 0.5));
                myPositionCollection.Add(new Point3D(end.X + xOffset, end.Y - yOffset, 0.5));
                myPositionCollection.Add(new Point3D(end.X - xOffset, end.Y + yOffset, 0.5));
            }
            else
            {
                myPositionCollection.Add(new Point3D(start.X - xOffset, start.Y - yOffset, 0.5));
                myPositionCollection.Add(new Point3D(start.X + xOffset, start.Y + yOffset, 0.5));
                myPositionCollection.Add(new Point3D(end.X - xOffset, end.Y - yOffset, 0.5));
                myPositionCollection.Add(new Point3D(end.X + xOffset, end.Y + yOffset, 0.5));
            }

            //myPositionCollection.Add(new Point3D(end.X, end.Y, 0.5));
            myMeshGeometry3D.Positions = myPositionCollection;

            // Create a collection of triangle indices for the MeshGeometry3D.
            var myTriangleIndicesCollection = new Int32Collection();
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);

            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);

            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

            // Apply the mesh to the geometry model.
            myGeometryModel.Geometry = myMeshGeometry3D;

            // The material specifies the material applied to the 3D object. In this sample a

            // Define material and apply to the mesh geometries.
            //var myMaterial = new DiffuseMaterial(myHorizontalGradient);
            var myMaterial = new DiffuseMaterial(brush);
            
            //var myMaterial = new EmissiveMaterial(brush);
            myGeometryModel.Material = myMaterial;
            myGeometryModel.BackMaterial = myMaterial;

            // Add the geometry model to the model group.
            myModel3DGroup.Children.Add(myGeometryModel);

            // Add the group of models to the ModelVisual3d.
            myModelVisual3D.Content = myModel3DGroup;
            

            //
            myViewport3D.Children.Add(myModelVisual3D);

            var segment = new PipeSegment(myModelVisual3D);
            pipeSegments.Add(segment);
        }



        private int rotation = 0;
        private Point3D startPipePoint;
        public HitTestResultBehavior MouseMoveResult(HitTestResult rawresult)
        {
            //MessageBox.Show("MouseMove:" + rawresult.ToString());
            var rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                var rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;

                if (rayMeshResult != null)
                {
                    var hitgeo = rayMeshResult.ModelHit as GeometryModel3D;
                    statusButton2.Content = $"3D:{rayMeshResult.PointHit.X:F4},{rayMeshResult.PointHit.Y:F4},{rayMeshResult.PointHit.Z:F4}";

                    //if()
                   

                    if (DrawMode == DrawMode.Create)
                    { 
                        Cursor = Cursors.Cross;
                    }
                    else if (DrawMode == DrawMode.Delete)
                    {
                        Cursor = Cursors.Arrow;
                    }
                    
                    if (DrawMode == DrawMode.Select)
                    {
                        Cursor = Cursors.Arrow;
                        foreach(var segment in pipeSegments.Map)
                        {
                         //   segment.Value.Visual.Mat
                         
                        }
                        if(pipeSegments.Contains(rayMeshResult.VisualHit))
                        {
                            hitgeo.Material = new DiffuseMaterial(Brushes.AliceBlue);
                            hitgeo.BackMaterial = new DiffuseMaterial(Brushes.AliceBlue);

                        }
                    }
                    else
                    {
                        if (pipeSegments.Contains(rayMeshResult.VisualHit))
                        {
                            hitgeo.Material = new DiffuseMaterial(Brushes.Blue);
                            hitgeo.BackMaterial = new DiffuseMaterial(Brushes.Blue);

                        }
                    }

                    if(DrawMode == DrawMode.Panning)
                    {
                        double xo = mouseMiddleDownPoint.X - rayResult.PointHit.X;
                        double yo = mouseMiddleDownPoint.Y - rayResult.PointHit.Y;
                        panOffsetLabel.Content = $"Offset:{xo:F3},{yo:F3}";
                        myPCamera.Position = new Point3D(myPCamera.Position.X + xo, myPCamera.Position.Y + yo, myPCamera.Position.Z);
                    }
                    

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


        private Cube DrawThing(Point3D point, Brush brush)
        {
            var myModel3DGroup = new Model3DGroup();
            var myGeometryModel = new GeometryModel3D();
            var myModelVisual3D = new ModelVisual3D();

            // The geometry specifes the shape of the 3D plane. In this sample, a flat sheet
            // is created.
            var myMeshGeometry3D = new MeshGeometry3D();

            var myDirectionalLight = new DirectionalLight();
            myDirectionalLight.Color = Colors.White;
            myDirectionalLight.Direction = new Vector3D(-0.61, -0.5, -0.61);

            myModel3DGroup.Children.Add(myDirectionalLight);

            // Create a collection of vertex positions for the MeshGeometry3D.
            var myPositionCollection = new Point3DCollection();
            var center = new Point3D(point.X, point.Y, 0.5);

            double width = 0.05;
            myPositionCollection.Add(new Point3D(center.X + width, center.Y + width, center.Z + width));
            myPositionCollection.Add(new Point3D(center.X + width, center.Y - width, center.Z + width));
            myPositionCollection.Add(new Point3D(center.X - width, center.Y + width, center.Z + width));
            myPositionCollection.Add(new Point3D(center.X - width, center.Y - width, center.Z + width));

            myPositionCollection.Add(new Point3D(center.X + width, center.Y + width, center.Z - width));
            myPositionCollection.Add(new Point3D(center.X + width, center.Y - width, center.Z - width));
            myPositionCollection.Add(new Point3D(center.X - width, center.Y + width, center.Z - width));
            myPositionCollection.Add(new Point3D(center.X - width, center.Y - width, center.Z - width));
            myMeshGeometry3D.Positions = myPositionCollection;

            // Create a collection of triangle indices for the MeshGeometry3D.
            var myTriangleIndicesCollection = new Int32Collection();
            // top
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(1);

            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);

            // bottom
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(6);
            myTriangleIndicesCollection.Add(5);

            myTriangleIndicesCollection.Add(5);
            myTriangleIndicesCollection.Add(6);
            myTriangleIndicesCollection.Add(7);

            // left
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(5);

            myTriangleIndicesCollection.Add(5);
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(1);

            // right
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(6);

            myTriangleIndicesCollection.Add(6);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(7);

            // front
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(7);

            myTriangleIndicesCollection.Add(7);
            myTriangleIndicesCollection.Add(5);
            myTriangleIndicesCollection.Add(1);

            // back
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(4);

            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(6);

            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

            // Apply the mesh to the geometry model.
            myGeometryModel.Geometry = myMeshGeometry3D;

            // The material specifies the material applied to the 3D object. In this sample a

            // Define material and apply to the mesh geometries.
            //var myMaterial = new DiffuseMaterial(myHorizontalGradient);
            var myMaterial = new DiffuseMaterial(brush);
            myGeometryModel.Material = myMaterial;

            // Add the geometry model to the model group.
            myModel3DGroup.Children.Add(myGeometryModel);

            // Add the group of models to the ModelVisual3d.
            myModelVisual3D.Content = myModel3DGroup;


            //
            myViewport3D.Children.Add(myModelVisual3D);

            var cube = new Cube(myModelVisual3D);
            cubes.Add(cube);
            return cube;
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            var myModel3DGroup = new Model3DGroup();
            var myGeometryModel = new GeometryModel3D();
            var myModelVisual3D = new ModelVisual3D();

            // The geometry specifes the shape of the 3D plane. In this sample, a flat sheet
            // is created.
            var myMeshGeometry3D = new MeshGeometry3D();

            var myDirectionalLight = new DirectionalLight();
            myDirectionalLight.Color = Colors.White;
            myDirectionalLight.Direction = new Vector3D(-0.61, -0.5, -0.61);

            myModel3DGroup.Children.Add(myDirectionalLight);


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
            myPositionCollection.Add(new Point3D(-1.5, -1.6, 0.25));
            myPositionCollection.Add(new Point3D(-1.6, -1.5, 0.25));
            myPositionCollection.Add(new Point3D(1.5, 1.6, 0.75));

            myPositionCollection.Add(new Point3D(1.5, 1.6, 0.75));
            myPositionCollection.Add(new Point3D(1.6, 1.5, 0.75));
            myPositionCollection.Add(new Point3D(-1.6, -1.5, 0.25));
            myMeshGeometry3D.Positions = myPositionCollection;

            // Create a collection of texture coordinates for the MeshGeometry3D.
            var myTextureCoordinatesCollection = new PointCollection();
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myTextureCoordinatesCollection.Add(new Point(3, 0));
            myTextureCoordinatesCollection.Add(new Point(3, 3));
            myTextureCoordinatesCollection.Add(new Point(3, 3));
            myTextureCoordinatesCollection.Add(new Point(0, 3));
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

            // Define material and apply to the mesh geometries.
            //var myMaterial = new DiffuseMaterial(myHorizontalGradient);
            var myMaterial = new DiffuseMaterial(Brushes.Red);
            myGeometryModel.Material = myMaterial;

          // Add the geometry model to the model group.
            myModel3DGroup.Children.Add(myGeometryModel);

            // Add the group of models to the ModelVisual3d.
            myModelVisual3D.Content = myModel3DGroup;


            //
            myViewport3D.Children.Add(myModelVisual3D);
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.Position = new Point3D(myPCamera.Position.X, myPCamera.Position.Y + 1, myPCamera.Position.Z);
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.Position = new Point3D(myPCamera.Position.X, myPCamera.Position.Y - 1, myPCamera.Position.Z);
        }

        private void tiltUpButton_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.LookDirection = new Vector3D(myPCamera.LookDirection.X, myPCamera.LookDirection.Y + 0.05, myPCamera.LookDirection.Z);
        }

        private void tiltDownButton_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.LookDirection = new Vector3D(myPCamera.LookDirection.X, myPCamera.LookDirection.Y - 0.05, myPCamera.LookDirection.Z);
            
        }

        private void panLeft_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.Position = new Point3D(myPCamera.Position.X - 1, myPCamera.Position.Y, myPCamera.Position.Z);
        }

        private void panRight_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.Position = new Point3D(myPCamera.Position.X + 1, myPCamera.Position.Y, myPCamera.Position.Z);
        }

        private void tiltLeft_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.LookDirection = new Vector3D(myPCamera.LookDirection.X + 0.05, myPCamera.LookDirection.Y, myPCamera.LookDirection.Z);
        }

        private void tiltRight_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.LookDirection = new Vector3D(myPCamera.LookDirection.X - 0.05, myPCamera.LookDirection.Y, myPCamera.LookDirection.Z);
        }

        private void resetCamera_Click(object sender, RoutedEventArgs e)
        {
            myPCamera.Position = new Point3D(0, 0, 2);
            myPCamera.LookDirection = new Vector3D(0, 0, -2);
        }

        private void deleteLast_Click(object sender, RoutedEventArgs e)
        {
            //if(cubes.Count == 0)
            //{
            //    return;
            //}

            //myViewport3D.Children.Remove(cubes.Last());
            //cubes.Remove(cubes.Last());
        }

        private void modeCreate_Click(object sender, RoutedEventArgs e)
        {
            DrawMode = DrawMode.Create;
            modeLabel.Content = "Create";
            Cursor = Cursors.Cross;
        }

        private void modeSelect_Click(object sender, RoutedEventArgs e)
        {
            DrawMode = DrawMode.Select;
            modeLabel.Content = "Select";
            Cursor = Cursors.Arrow;
        }

        private void modeDelete_Click(object sender, RoutedEventArgs e)
        {
            DrawMode = DrawMode.Delete;
            modeLabel.Content = "Delete";
            Cursor = Cursors.Arrow;
        }

        private void border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                DrawMode = DrawMode.Panning;
                modeLabel.Content = "Panning";
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Point mouseposition = e.GetPosition(myViewport3D);
                Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
                Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
                var pointparams = new PointHitTestParameters(mouseposition);
                //var rayparams = new RayHitTestParameters(testpoint3D, testdirection);

                VisualTreeHelper.HitTest(myViewport3D, null, MiddleButtonHTResult, pointparams);
            }
        }

        private void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DrawMode == DrawMode.Panning && e.MiddleButton == MouseButtonState.Released)
            {
                // TODO: Keep track of the previous mode?
                DrawMode = DrawMode.Create;
                modeLabel.Content = "Create";

                mouseMiddleDownPoint = new Point3D();
                statusMouseMiddleDownPointLabel.Content = $"MMDP:<None>";
            }
        }

        private void drawPipeButton_Click(object sender, RoutedEventArgs e)
        {
            DrawMode = DrawMode.PipeStart;
            modeLabel.Content = "PipeStart";
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
