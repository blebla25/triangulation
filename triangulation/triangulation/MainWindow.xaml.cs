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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vaja3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<Vector> points = new List<Vector>();
        List<List<Vector>> daljice = new List<List<Vector>>();
        List<List<Vector>> triangulacija = new List<List<Vector>>();
        List<Vector> konveksnaLupina = new List<Vector>();
        List<int> indeksi = new List<int>();
        


        private static readonly Random random = new Random();

        private static double RandomNumberBetween(double minValue, double maxValue)
        {
            var next = random.NextDouble();

            return minValue + (next * (maxValue - minValue));
        }

        private void NarisiTocko(double x, double y)
        {
            int dotSize = 3;
            Ellipse currentDot = new Ellipse();
            currentDot.Stroke = new SolidColorBrush(Colors.Black);
            currentDot.StrokeThickness = 2;
            Canvas.SetZIndex(currentDot, 2);
            currentDot.Height = dotSize;
            currentDot.Width = dotSize;
            currentDot.Fill = new SolidColorBrush(Colors.Black);
            currentDot.Margin = new Thickness(x - 1, y - 1, 0, 0); // Sets the position.


            MyCanvas.Children.Add(currentDot);

        }

        private void NarisiDaljico(Vector p1, Vector p2)
        {
            Line myLine = new Line();

            myLine.Stroke = System.Windows.Media.Brushes.Black;

            myLine.X1 = p1.X + 50;
            myLine.X2 = p2.X + 50;  // 150 too far
            myLine.Y1 = p1.Y + 100;
            myLine.Y2 = p2.Y + 100;

            myLine.StrokeThickness = 1;
            myLine.SnapsToDevicePixels = true;
            MyCanvas.Children.Add(myLine);
        }

        private void Narisi()
        {
            for (int i = 0; i < points.Count; i++)
            {
                NarisiTocko(points[i].X + 50, points[i].Y + 100);
            }
        }
        private void Izprazni()
        {

            MyCanvas.Children.Clear();
            points.Clear();
            daljice.Clear();
            triangulacija.Clear();
            konveksnaLupina.Clear();
            indeksi.Clear();
        }


        private void NarisiDaljice(List<List<Vector>> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                NarisiDaljico(data[i][0], data[i][1]);
            }
        }
        private double truncate(double original)
        {
            double truncated = Math.Truncate(original * 100) / 100;
            return truncated;
        }



        private bool IzračunajPresecisce(Vector point1, Vector point2, Vector point3, Vector point4)
        {
            double D, A, B;
            D = (point2.X - point1.X) * (point4.Y - point3.Y) - (point4.X - point3.X) * (point2.Y - point1.Y);
            A = (point4.X - point3.X) * (point1.Y - point3.Y) - (point1.X - point3.X) * (point4.Y - point3.Y);
            B = (point2.X - point1.X) * (point1.Y - point3.Y) - (point1.X - point3.X) * (point2.Y - point1.Y);
            if (D == A && A == B && B == 0)
            {
                return false; //sovpadata
            }
            else
            {
                double Ua = A / D;
                double Ub = B / D;
                if (D == 0)
                {
                    return false; // vzporedni

                }
                else
                {
                    if (Ua <= 1 && Ua >= 0 && Ub <= 1 && Ub >= 0)
                    {

                        double x = truncate(point1.X + Ua * (point2.X - point1.X));
                        double y = truncate(point1.Y + Ua * (point2.Y - point1.Y));
                        Vector presecisce = new Vector(x, y);
                        if (Ua > 0.9999 || Ub > 0.9999 || Ua < 0.0001 || Ub < 0.0001)
                        {
                            return false; //dotikata
                        }
                        else
                        {
                            return true; //sekata
                        }

                    }
                    else return false; //ne sekata
                }
            }
        }

        private double EvklidskaRazdalja(Vector p1, Vector p2)
        {
            double razdalja = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            return razdalja;
        }

        private List<double> GetDistances()
        {
            List<double> distances = new List<double>();
            for (int i = 0; i < daljice.Count; i++)
            {
                distances.Add(EvklidskaRazdalja(daljice[i][0], daljice[i][1]));
            }
            return distances;
        }

        private void GenerirajNormalno()
        {
            Random rand = new Random();
            for (int i = 0; i < Int32.Parse(StTock.Text); i++)
            {

                //reuse this if you are generating many
                double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = (Math.Sqrt(-2.0 * Math.Log(u1)) *
                             Math.Sin(2.0 * Math.PI * u2)); //random normal(0,1)
                double randNormal =
                   250 + 50 * randStdNormal; //random normal(mean,stdDev^2)
                double u11 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
                double u22 = 1.0 - rand.NextDouble();
                double randStdNormal2 = (Math.Sqrt(-2.0 * Math.Log(u11)) *
                             Math.Sin(2.0 * Math.PI * u22)); //random normal(0,1)
                double randNormal2 =
                   200 + 50 * randStdNormal2; //random normal(mean,stdDev^2)
                points.Add(new Vector(randNormal, randNormal2));
            }
        }
        private void GenerirajFunkcija()
        {
            Izprazni();

            if (PorazdelitevTock.SelectedIndex == 0)
            {
                Random rand = new Random();
                for (int i = 0; i < Int32.Parse(StTock.Text); i++)
                {
                    double x = RandomNumberBetween(0.00, 500.0);
                    double y = RandomNumberBetween(0.00, 550.0);
                    points.Add(new Vector(x, y));
                }
            }
            else if (PorazdelitevTock.SelectedIndex == 1)
            {
                GenerirajNormalno();
            }
            Narisi();
        }

        private void NarediDaljice()
        {
            for(int i = 0; i < points.Count; i++)
            {
                for(int j = i + 1; j < points.Count; j++)
                {
                    List<Vector> tmp = new List<Vector>();
                    tmp.Add(points[i]);
                    tmp.Add(points[j]);
                    daljice.Add(tmp);
                }
            }
        }


        private void MWT()
        {
            for(int i = 0; i < daljice.Count; i++)
            {
                bool status = false;
                for(int j = 0; j < triangulacija.Count; j++)
                {
                    if(IzračunajPresecisce(daljice[i][0], daljice[i][1], triangulacija[j][0], triangulacija[j][1]))
                    {
                        status = true;
                        break;
                    }
                }
                if(!status)
                {
                    triangulacija.Add(daljice[i]);
                }
            }
        }

        private void HamiltonovaTriangulacija()
        {
            
            JarvisovObhod();
            TvoriTrikotnike();
            NarisiTrikotnike(triangulacija);
        }

        private void NarisiTrikotnike(List<List<Vector>> trikotniki)
        {
            for(int i = 0; i < trikotniki.Count; i++)
            {
                NarisiDaljico(trikotniki[i][0], trikotniki[i][1]);
                NarisiDaljico(trikotniki[i][0], trikotniki[i][2]);
                NarisiDaljico(trikotniki[i][1], trikotniki[i][2]);
            }
        }

   
        private void Generiraj(object sender, RoutedEventArgs e)
        {
            GenerirajFunkcija();

        }
        private void Izracunaj(object sender, RoutedEventArgs e)
        {

            if(Algoritem.SelectedIndex == 0)
            {
                NarediDaljice();
                List<double> tmp = GetDistances();
                daljice = sortQueens(daljice, tmp);
                MWT();
                NarisiDaljice(triangulacija);
            }
            else if(Algoritem.SelectedIndex == 1)
            {
                
                HamiltonovaTriangulacija();
                
            }
        
        }



        private int NajdiE()
        {
            int index = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y > points[index].Y) index = i;
            }
            return index;
        }

        private void TvoriTrikotnike()
        {
            int indexA;
            int indexB;
            int indexC;

            indexA = indeksi[0];
            indexB = indexA + 1;
            indexC = 0;

            Vector A = new Vector();
            Vector B = new Vector();
            Vector C = new Vector();

            while (konveksnaLupina.Count > 0)
            {
                List<Vector> trikotnik = new List<Vector>();
                A = konveksnaLupina[indexA];
                B = konveksnaLupina[indexB];
                C = konveksnaLupina[indexC];
                trikotnik.Add(A);
                trikotnik.Add(B);
                trikotnik.Add(C);
                triangulacija.Add(trikotnik);
                
                if (triangulacija.Count >= 2 && PreveriTrikotnik(triangulacija.Count - 1))
                {
                    triangulacija.RemoveAt(triangulacija.Count - 1);
                    trikotnik.Clear();
                    trikotnik.Add(A);
                    trikotnik.Add(B);
                    trikotnik.Add(A);
                    triangulacija.Add(trikotnik);
                    int t = indexA;
                    indexA = indexB;
                    indexB = t;
                    indexC = indexA + 1;
                }
                else
                {
                    int tm = indexB;
                    indexA = indexB;
                    indexB = indexC;
                    indexC = tm + 1;
                }
                if (indexA >= konveksnaLupina.Count || indexB >= konveksnaLupina.Count || indexC >= konveksnaLupina.Count)
                {
                    trikotnik.Clear();
                    trikotnik.Add(A);
                    trikotnik.Add(B);
                    trikotnik.Add(A);
     //               triangulacija.Add(trikotnik);
                    indexC = indexA;
                    if (indexA == indexB) break;
                   
                }
            }
        }

        private bool PreveriTrikotnik(int index)
        {
            bool status = false;
            List<Vector> tmp = triangulacija[index];
            for (int i = 0; i < konveksnaLupina.Count -1; i++)
            {
                List<Vector> tmp2 = new List<Vector>();
                tmp2.Add(konveksnaLupina[i]);
                tmp2.Add(konveksnaLupina[i + 1]);
                if (IzračunajPresecisce(tmp[0], tmp[1], tmp2[0], tmp2[1]))
                {
                    status = true;
                }


                if (IzračunajPresecisce(tmp[1], tmp[2], tmp2[0], tmp2[1]))
                {
                    status = true;
                }


                if (IzračunajPresecisce(tmp[0], tmp[2], tmp2[0], tmp2[1]))
                {
                    status = true;
                }

            }
            return status;
        }

        private void JarvisovObhod()
        {
            konveksnaLupina.Clear();
            List<Vector> S = new List<Vector>();
            S = new List<Vector>(points);
            int EIndex = NajdiE();
            Vector E = points[EIndex];
            Vector tmp = new Vector(E.X + 50, E.Y);
            Vector Xos = tmp - E;
            int index = 0;
            double najmansiKot = 100;
            konveksnaLupina.Add(E);

            for (int i = 0; i < points.Count; i++)
            {
                if (i != EIndex)
                {
                    Vector V2 = points[i] - E;
                    double param = (Xos.X * V2.X + Xos.Y * V2.Y) / (Xos.Length * V2.Length);
                    double kot = Math.Acos(param);
                    if (kot < najmansiKot)
                    {
                        index = i;
                        najmansiKot = kot;
                    }
                }
            }
            Vector test = new Vector(points[index].X, points[index].Y);
            konveksnaLupina.Add(test);
            points.RemoveAt(index);
            S.RemoveAt(index);

          
            while (S.Count > 0)
            {
                index = 0;
                najmansiKot = 100;
         
                Vector A = new Vector(konveksnaLupina[konveksnaLupina.Count - 2].X, konveksnaLupina[konveksnaLupina.Count - 2].Y);
                tmp = new Vector(konveksnaLupina[konveksnaLupina.Count - 1].X, konveksnaLupina[konveksnaLupina.Count - 1].Y);
                Xos = tmp - A;
                for (int i = 0; i < S.Count; i++)
                {

                    Vector V2 = S[i] - tmp;
                    double param = (Xos.X * V2.X + Xos.Y * V2.Y) / (Xos.Length * V2.Length);
                    double kot = Math.Acos(param);
                    if (kot < najmansiKot)
                    {
                        index = i;
                        najmansiKot = kot;
                        
                    }
                }
                test = new Vector(S[index].X, S[index].Y);
                if (test == E)
                {
                   
                    indeksi.Add(konveksnaLupina.Count-1);
                    S.RemoveAt(index);
                    points.RemoveAt(index);
                    if (points.Count == 0) break;
                   

                }
                else
                {
                    konveksnaLupina.Add(test);
                    S.RemoveAt(index);
                    points.RemoveAt(index);
                }
              
            }

            NarisiKonvekcijo(konveksnaLupina);

        }

        private void NarisiKonvekcijo(List<Vector> tocke)
        {
            for (int i = 0; i < tocke.Count -1; i++)
            {
                
                    NarisiDaljico(tocke[i], tocke[i + 1]);
            }
        }


        public List<List<Vector>> sortQueens(List<List<Vector>> statesArray, List<double> heuristics)
        {
            List<List<Vector>> statesListSorted = new List<List<Vector>>();
            List<double> heuristicsCopy = new List<double>();

            statesListSorted.AddRange(statesArray);
            heuristicsCopy.AddRange(heuristics);
            heuristics.Sort();

            List<int> index = new List<int>();
            for (int i = 0; i < heuristicsCopy.Count; i++)
            {
                for (int j = 0; j < heuristics.Count; j++)
                {
                    if (heuristicsCopy[i] == heuristics[j])
                    {
                        if (index.Contains(j))
                            continue;
                        statesListSorted[j] = statesArray[i];
                        index.Add(j);
                        break;
                    }
                }
            }
            return statesListSorted;
        }


    }
}
