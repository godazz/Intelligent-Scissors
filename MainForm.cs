using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace IntelligentScissors
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Initialize();
        }
        void Initialize()
        {
            AnchorPoints = new List<Point>();
            fullPath = new List<Point>();
        }

        Stopwatch sw;
        Stopwatch execuationWatch;
        List<Point> AnchorPoints;
        RGBPixel[,] ImageMatrix;
        List<int> prev;
        List<Point> path;
        List<Point> fullPath;
        Point[] drawFullPath;
        Point[] drawPath;
        int clickedNode = -1;
        double elapsedTime = -1; // time to backtrack path

 
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GraphOperations.ConstructGraph(ImageMatrix);
            Console.WriteLine("Graph Constructed Sucessfully");
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {

            if (pictureBox1.Image != null)
            {
                
                int imgWidth = ImageOperations.GetWidth(ImageMatrix);
                int nodeIndex = (e.Y * imgWidth) + (e.X);
                clickedNode = nodeIndex;

                AnchorPoints.Add(e.Location);
                if (AnchorPoints.Count == 1)
                {
                    execuationWatch = Stopwatch.StartNew();
                    execuationWatch.Start();
                }

                prev = GraphOperations.Dijkstra(nodeIndex, ImageMatrix);
                if (AnchorPoints.Count > 1)
                {
                    savePath(path , imgWidth , elapsedTime);

                    for (int i = 0; i < path.Count; i++)
                    {
                        fullPath.Add(path[i]);
                    }
                        
                }
                pictureBox1.Refresh();
            }
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            if (pictureBox1.Image != null )
            {
                for (int i = 0; i < AnchorPoints.Count; i++)
                {
                    
                    g.FillEllipse(Brushes.Yellow, new Rectangle(
                        new Point(AnchorPoints[i].X, AnchorPoints[i].Y),
                        new Size(5,5)));
                }

                if (path != null && path.Count > 5)
                {
                    Pen pen = new Pen(Brushes.Aqua, 1);
                    drawPath = path.ToArray();
                    g.DrawCurve(pen, drawPath);
                }

                if (fullPath!= null && fullPath.Count > 5)
                {
                    Pen pen = new Pen(Brushes.Red, 1);
                    drawFullPath = fullPath.ToArray();
                    g.DrawCurve(pen, drawFullPath);
                }


            }

        }
        
        public void updatePath(MouseEventArgs e)
        {
            var g = pictureBox1.CreateGraphics();
            if (pictureBox1!= null && clickedNode != -1) 
            {
                sw = Stopwatch.StartNew();
                int imgWidth = ImageOperations.GetWidth(ImageMatrix);
                int nodeIndex = (e.Y * imgWidth) + (e.X);
                sw.Start();
                path = GraphOperations.BackTrackPath(prev, nodeIndex, imgWidth);
                sw.Stop();
                elapsedTime = sw.Elapsed.TotalSeconds;
                pictureBox1.Refresh();
            }

        }

        private void pictureBox1_MoveMouse (Object sender, MouseEventArgs e)
        {
            updatePath(e);
            textBox_X.Text = e.X.ToString();
            textBox_Y.Text = e.Y.ToString();
            if (pictureBox1.Image != null)
            {
             
                int imgWidth = ImageOperations.GetWidth(ImageMatrix);
                int nodeIndex = (e.Y * imgWidth) + (e.X);
                textBox_idx.Text = nodeIndex.ToString();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
           if (pictureBox1.Image != null)
           {
                if (AnchorPoints.Count > 1)
                {
                    clickedNode = -1;
                   int imgWidth = ImageOperations.GetWidth(ImageMatrix);
                   int nodeIndex = (AnchorPoints[0].Y * imgWidth) + AnchorPoints[0].X;
                   path = GraphOperations.BackTrackPath(prev, nodeIndex, imgWidth);
                    for (int i = 0; i < path.Count; i++)
                    {
                        fullPath.Add(path[i]);
                    }
                    pictureBox1.Refresh();

                }
                execuationWatch.Stop();
                textBox1.Text = execuationWatch.Elapsed.TotalSeconds.ToString();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AnchorPoints.Clear();
            prev.Clear();
            path.Clear();
            fullPath.Clear();
            clickedNode = -1;
            drawPath = null;
            drawFullPath = null;
            pictureBox1.Refresh();
        }

        private void savePath(List<Point> path , int imgWidth , double time)
        {
            using (System.IO.StreamWriter writer = new StreamWriter("path.txt"))
            {
                int srcNodeIndex = (path[0].Y * imgWidth) + path[0].X;
                int destNodeIndex = (path[path.Count - 1].Y * imgWidth) + path[path.Count - 1].X;
                string x = string.Format("The shortest path from node {0} at ({1}, {2}) to Node {3} at ({4}, {5})" , srcNodeIndex, path[0].X , path[0].Y , destNodeIndex, path[path.Count - 1].X, path[path.Count - 1].Y);
                writer.Write(x);
      
                for (int i = 0; i < path.Count; i++)
                {
                    int nodeIndex = (path[i].Y * imgWidth) + path[i].X;
                    string y = string.Format("({0},X = {1}, Y = {2})", nodeIndex, path[i].X, path[i].Y);
                    writer.WriteLine(y);
                }

                writer.Write("Path construction took: " + time + " seconds.");

            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

    }
}