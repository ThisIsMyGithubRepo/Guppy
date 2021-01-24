using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Guppy.OutputItems;
using System.Windows.Media.Media3D;
using WPFSurfacePlot3D;
using HelixToolkit.Wpf;

namespace Guppy
{
	/// <summary>
	/// Interaction logic for MeshView.xaml
	/// </summary>
	public partial class MeshView : Window
	{
		private pr_G29T_MeshMap _mm;
		private SurfacePlotModel viewModel;

		public MeshView(pr_G29T_MeshMap mm)
		{
			InitializeComponent();
			_mm = mm;

			// Initialize surface plot objects
			viewModel = new SurfacePlotModel();

			surfacePlotView.DataContext = viewModel;
			//surfacePlotView.SurfaceBrush = BrushHelper.CreateGradientBrush(Colors.Red, Colors.Green, Colors.Blue);

			viewModel.PlotData(_mm.MeshValues);
			viewModel.ShowMiniCoordinates = true ;
			viewModel.ShowSurfaceMesh = false;

			//UpdateMesh3DView();
		}



		//private void UpdateMesh3DView()
		//{
		//	int sizeX = _mm.MeshValues.GetLength(0);
		//	int sizeY = _mm.MeshValues.GetLength(1);

		//	MeshGeometry3D buildPlate = new MeshGeometry3D();
		//	for (int y = 0; y < sizeY; y++)
		//	{
		//		for (int x = 0; x < sizeX; x++)
		//		{
  //                  buildPlate.Positions.Add(new Point3D(x, y,  _mm.MeshValues[x, y]));
		//		}
		//	}

  //          int idx;
            
		//	//Build bottom row triangles
		//	for (int y = 0; y < (sizeY-1); y++)
		//	{
		//		for (int x = 0; x < (sizeX-1); x++)
		//		{

  //                  // 2
  //                  // 1 .
  //                  // 0 . .
  //                  //   0 1 2 3 4 5
  //                  // Triangle is points 0, 1, 6

  //                  idx = y * sizeX + x; // e.g. at (0,0) this is 0. At (0,1) = 6 using triangle in comment.

  //                  buildPlate.TriangleIndices.Add(idx); //bottom left of triangle
		//			buildPlate.TriangleIndices.Add(idx + 1); //bottom right of triangle
		//			buildPlate.TriangleIndices.Add(idx + sizeX); //top left of triangle
		//		}
		//	}

  //          //Build top row triangles
  //          for (int y = 1; y < sizeY; y++)
  //          {
  //              for (int x = 1; x < sizeX; x++)
  //              {

  //                  // 2
  //                  // 1 ..
  //                  // 0  .
  //                  //   0 1 2 3 4 5
  //                  // Triangle is points 7, 6, 1

  //                  idx = y * sizeX + x; // e.g. at (0,0) this is 0. At (0,1) = 6 using triangle in comment.

  //                  buildPlate.TriangleIndices.Add(idx); //bottom left of triangle
  //                  buildPlate.TriangleIndices.Add(idx - 1); //bottom right of triangle
  //                  buildPlate.TriangleIndices.Add(idx - sizeX); //top left of triangle
  //              }
  //          }

  //          Model3DGroup myModel3DGroup = new Model3DGroup();
  //          GeometryModel3D myGeometryModel = new GeometryModel3D();
  //          ModelVisual3D myModelVisual3D = new ModelVisual3D();
  //          // Defines the camera used to view the 3D object. In order to view the 3D object,
  //          // the camera must be positioned and pointed such that the object is within view
  //          // of the camera.
  //          PerspectiveCamera myPCamera = new PerspectiveCamera();

  //          // Specify where in the 3D scene the camera is.
  //          myPCamera.Position = new Point3D(8, -20, 9);

  //          // Specify the direction that the camera is pointing.
  //          myPCamera.LookDirection = new Vector3D(0, 1, -0.25);

  //          // Define camera's horizontal field of view in degrees.
  //          myPCamera.FieldOfView = 60;

  //          // Asign the camera to the viewport
  //          mesh3dViewport.Camera = myPCamera;

  //          // Define the lights cast in the scene. Without light, the 3D object cannot
  //          // be seen. Note: to illuminate an object from additional directions, create
  //          // additional lights.
  //          AmbientLight myAmbientLight = new AmbientLight(Colors.White);
  //          myModel3DGroup.Children.Add(myAmbientLight);

  //          // The geometry specifes the shape of the 3D plane. In this sample, a flat sheet
  //          // is created.
  //          MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();

  //          // Create a collection of vertex positions for the MeshGeometry3D.
  //          Point3DCollection myPositionCollection = new Point3DCollection();

  //          myPositionCollection.Add(new Point3D(0, 0, 0));
  //          myPositionCollection.Add(new Point3D(1, 0, 0));
  //          myPositionCollection.Add(new Point3D(2, 0, 0));
  //          myPositionCollection.Add(new Point3D(3, 0, 0));
  //          myPositionCollection.Add(new Point3D(0, 1, 0));
  //          myPositionCollection.Add(new Point3D(1, 1, 0));
  //          myPositionCollection.Add(new Point3D(2, 1, 0));
  //          myPositionCollection.Add(new Point3D(3, 1, 0));

  //          myMeshGeometry3D.Positions = myPositionCollection;

  //          // Create a collection of triangle indices for the MeshGeometry3D.
  //          Int32Collection myTriangleIndicesCollection = new Int32Collection();
  //          myTriangleIndicesCollection.Add(0);
  //          myTriangleIndicesCollection.Add(1);
  //          myTriangleIndicesCollection.Add(4);
  //          myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

  //          // Apply the mesh to the geometry model.
  //          myGeometryModel.Geometry = buildPlate; //myMeshGeometry3D;

  //          // The material specifies the material applied to the 3D object. In this sample a
  //          // linear gradient covers the surface of the 3D object.

  //          // Create a horizontal linear gradient with four stops.
  //          SolidColorBrush frontBrush = new SolidColorBrush(Colors.Black);
  //          frontBrush.Opacity = 0.2;
  //          SolidColorBrush backBrush = new SolidColorBrush(Colors.Blue);
  //          backBrush.Opacity = 0.2;

  //          // Define material and apply to the mesh geometries.
  //          DiffuseMaterial frontMaterial = new DiffuseMaterial(frontBrush);
  //          DiffuseMaterial backMaterial = new DiffuseMaterial(backBrush);
  //          myGeometryModel.Material = frontMaterial;
  //          myGeometryModel.BackMaterial = backMaterial;

  //          // Apply a transform to the object. In this sample, a rotation transform is applied,
  //          // rendering the 3D object rotated.
  //          RotateTransform3D myRotateTransform3D = new RotateTransform3D();
  //          AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
  //          myAxisAngleRotation3d.Axis = new Vector3D(0, 0, 0);
  //          myAxisAngleRotation3d.Angle = 0;
  //          myRotateTransform3D.Rotation = myAxisAngleRotation3d;
  //          myGeometryModel.Transform = myRotateTransform3D;

  //          // Add the geometry model to the model group.
  //          myModel3DGroup.Children.Add(myGeometryModel);

  //          // Add the group of models to the ModelVisual3d.
  //          myModelVisual3D.Content = myModel3DGroup;

  //          //
  //          mesh3dViewport.Children.Clear();
  //          mesh3dViewport.Children.Add(myModelVisual3D);

  //      }

    }
}
