using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

using System.Text;
using System.Diagnostics;

namespace IntelligentScissors
{
    class GraphOperations
    {

        const double INFINITY = 10000000000000000000;

        public static List <  List <Tuple <int, int , double> > > Adj;
        


        private static void SaveConstructedGraph(int nodes, double time)
        {
            using (StreamWriter writer = new StreamWriter("output.txt"))
            {
              
                for (int i = 0; i < nodes; i++)
                {
                    writer.Write(i + "|");
                                  
                    writer.Write("edges:");
                    for (int j = 0; j < Adj[i].Count; j++)
                    {
                        writer.Write("(" + i + "," + Adj[i][j].Item1 + "," + Adj[i][j].Item3 + ")");
                    }
                    writer.WriteLine();
                }
                writer.Write("Graph construction took: " + time + " seconds.");

            }
        }
    

        public static List<List<Tuple <int, int, double>>> ConstructGraph (RGBPixel[,] ImageMatrix)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int imgHeight = ImageOperations.GetHeight(ImageMatrix);
            int imgWidth = ImageOperations.GetWidth(ImageMatrix);

            int nodes = imgHeight * imgWidth;
            Adj = new List< List < Tuple<int, int, double> > >();

            
            for (int i =0; i < nodes; i++)
            {
                Adj.Add(GetAdjacentNodes(i, ImageMatrix));
            }
            sw.Stop();
            SaveConstructedGraph(nodes, sw.Elapsed.TotalSeconds);
            return Adj;
        }

        public static List<Tuple<int, int, double>> GetAdjacentNodes(int nodeIndex, RGBPixel[,] ImageMatrix) {

            int imgWidth = ImageOperations.GetWidth(ImageMatrix);
            int imgHeight = ImageOperations.GetHeight(ImageMatrix);

            List< Tuple<int,int,double> > Adjacents = new List<Tuple<int, int, double>>();

             // col , row
            int X = nodeIndex % imgWidth, Y = nodeIndex / imgWidth;

            if (X <= imgWidth - 2) // right node
            {
                Vector2D Ge = ImageOperations.CalculatePixelEnergies(X, Y, ImageMatrix);
                double Right_G = 1.0 / Ge.X;
                if (double.IsInfinity(Right_G))
                    Right_G = 1E+16;

                Adjacents.Add(Tuple.Create(nodeIndex + 1, nodeIndex, Right_G));

            }
            if (Y <= imgHeight - 2) // bottom node
            {
                Vector2D Ge = ImageOperations.CalculatePixelEnergies(X, Y, ImageMatrix);
                double Bottom_G = 1.0 / Ge.Y;
                if (double.IsInfinity(Bottom_G))
                    Bottom_G = 1E+16;
                int next_node = ((Y + 1) * imgWidth) + (X);


                Adjacents.Add(Tuple.Create(next_node, nodeIndex, Bottom_G)) ;

            }
            if (X >= 1) // left node
            {
                Vector2D Ge = ImageOperations.CalculatePixelEnergies(X - 1, Y, ImageMatrix);
                double Left_G = 1.0 / Ge.X;
                if (double.IsInfinity(Left_G))
                    Left_G = 1E+16;

                Adjacents.Add(Tuple.Create(nodeIndex - 1, nodeIndex, Left_G));
            }
            if (Y >= 1) // top node
            {
                Vector2D Ge = ImageOperations.CalculatePixelEnergies(X, Y - 1, ImageMatrix);
                double Top_G = 1.0 / Ge.Y;
                if (double.IsInfinity(Top_G))
                    Top_G = 1E+16;

                int next_node = ((Y - 1) * imgWidth) + (X);

                Adjacents.Add(Tuple.Create(next_node, nodeIndex, Top_G));
            }

            return Adjacents;
        }

        public static List<int> Dijkstra(int src, RGBPixel[,] ImageMatrix)
        {
            int width = ImageOperations.GetWidth(ImageMatrix);
            int height = ImageOperations.GetHeight(ImageMatrix);
            int nodes = width * height;

            List<double> Distance = new List<double>();
            List<int> prev = new List<int>();
           

            for (int i = 0; i < nodes; i++)
            {
                Distance.Add(INFINITY);
                prev.Add(-1);
            }

            PriorityQueue pq = new PriorityQueue();
            Tuple<int, int, double> source = Tuple.Create(src, -1,  (double)0);
            Distance[src] = 0;
            pq.Push(source);

            while (!pq.IsEmpty())
            {
             
                Tuple<int, int, double> minimum = pq.Top();
                pq.Pop();
                
              
                List< Tuple<int, int, double> > Adjacents = GraphOperations.GetAdjacentNodes(minimum.Item1, ImageMatrix); 

                for (int i = 0; i < Adjacents.Count; i++) 
                {

                    Tuple<int, int, double> edge = Adjacents[i];
                   
                    if (Distance[edge.Item1] > Distance[edge.Item2] + edge.Item3)
                    {
                        double newWeight = Distance[edge.Item2] + edge.Item3; 
                        Tuple<int, int, double> input = Tuple.Create(edge.Item1, edge.Item2, newWeight);
                        pq.Push(input); // log(v)
                        Distance[edge.Item1] = Distance[edge.Item2] + edge.Item3;
                        prev[edge.Item1] = minimum.Item2;
                    }
                }
            }

            return prev;
        }

        public static List<Point> BackTrackPath(List<int> prev, int dest , int imgWidth)
        {
            Stack<int> s = new Stack<int>();

            while (prev[dest] != -1)
            {
                s.Push(dest);
                dest = prev[dest];
            }

            List<Point> path = new List<Point>();

            while (s.Count != 0)
            {
                int nodeIndex = s.Pop();
                int X = nodeIndex % imgWidth, Y = nodeIndex / imgWidth;
                path.Add(new Point(X, Y));
            }

            return path;
        }

    }
}
