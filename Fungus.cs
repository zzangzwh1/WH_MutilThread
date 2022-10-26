using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GDIDrawer;
using System.Threading;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Lab3_WonhyukCho
{
    public class Fungus
    {
        public static ConcurrentDictionary<Point, Color> dicFung = new ConcurrentDictionary<Point, Color>();
        public static int count = 0;
        private int backtrack = 100_000;
        //current location
        private Point fLocation;
        //color fungus
        private Color fClr;
        public static CDrawer fGdi;
        //fungus thread
        private Thread fThread;
        private static Random rnd = new Random();

        public Point NextPoint
        {
            get
            {
                Point pt;
                lock (rnd)
                {
                    pt = new Point(rnd.Next(0, fGdi.ScaledWidth), rnd.Next(0, fGdi.ScaledHeight));
                }
                return pt;

            }
        }
        public Fungus(CDrawer gdi, Color c)
        {
            fGdi = gdi;
            fClr = c;
            //fLocation = 
            do
            {
                fLocation = NextPoint;
            } while (dicFung.ContainsKey(fLocation));

            //add new fungus into the collection
            dicFung[fLocation] = fClr;
            count++;
            fThread = new Thread(StartFungusThread);
            fThread.IsBackground = true;
            fThread.Start();

        }

        public static IEnumerable<Point> Shuffle(IEnumerable<Point> iP)
        {
            List<Point> temp = iP.ToList();
            for (int i = 0; i < temp.Count(); ++i)
            {
                int j = rnd.Next(i, temp.Count());
                Point p = temp[i];
                temp[i] = temp[j];
                temp[j] = p;

            }
            return temp;
        }
        public static int Substitute(int num)
        {
            lock (rnd)
            {
                return rnd.Next(0, num);
            }
        }
        public void StartFungusThread()
        {
            /* If the dictionary indicates the CDrawer is full job done, reset the CDrawer background to Black
             and clear the dictionary -starting from scratch.*/
            while (true)
            {

                if (dicFung.Count >= fGdi.ScaledWidth * fGdi.ScaledHeight)
                {
                    fGdi.BBColour = Color.Black;
                    dicFung.Clear();
                }
                List<Point> adj = GetAdj(fLocation).Where(p => p.X > 0 && p.X < fGdi.ScaledWidth && p.Y > 0 && p.Y < fGdi.ScaledHeight).ToList();

              /*  adj.RemoveAll(p => (p.X >= fGdi.ScaledWidth || p.X < 0) || (p.Y >= fGdi.ScaledHeight || p.Y < 0));
                adj.RemoveAll(p => dicFung.ContainsKey(p) && dicFung[p] != fClr);*/
                adj.RemoveAll(p => dicFung.ContainsKey(p) && dicFung[p] != fClr && dicFung[p] != fClr);

                // create lists for visited location and unvisited location

                List<Point> visited;
                List<Point> notVisited;
                lock (dicFung)
                {
                    visited = Shuffle(adj.Where(p => dicFung.ContainsKey(p))).ToList();
                    notVisited = Shuffle(adj.Where(p => !dicFung.ContainsKey(p))).ToList();

                }

              


                Thread.Sleep(0);
                /*   foreach (Point p in adj)
                   {
                       if (dicFung.ContainsKey(p) && dicFung[p] != fClr)
                           Trace.WriteLine($"Fungus[{fClr.Name}] : {p} - candidate spot taken ...");

                       else if (dicFung.ContainsKey(p) && dicFung[p].Equals(fClr))
                           backtrack--;
                       else
                       {
                           dicFung.TryAdd(p, fClr);
                           fLocation = p;
                           fGdi.SetBBScaledPixel(fLocation.X, fLocation.Y, fClr);
                           break;
                       }
                       fLocation = p;
                   }



                   if (backtrack < 1)
                   {
                       count--;
                       Trace.WriteLine($"Fungus[{fClr.Name}] : terminated, too much backtracking...");
                       Thread.CurrentThread.Abort();
                   }
   */





                ConcurrentQueue<Point> adjCQ = new ConcurrentQueue<Point>(notVisited.Concat(visited));

                while (adjCQ.Any())
                {
                    adjCQ.TryDequeue(out Point temp);
                    if (dicFung.ContainsKey(temp) && dicFung[temp] != fClr)
                    {
                        Trace.WriteLine($"Fungus[{fClr.Name}] : {fLocation} - candidate spot taken …");
                        continue;

                    }
                    if (dicFung.ContainsKey(temp) && dicFung[temp] == fClr)
                        backtrack--;

                    fLocation = temp;
                    dicFung.TryAdd(temp, fClr);
                    fGdi.SetBBPixel(fLocation.X, fLocation.Y, fClr);
                    break;

                }

                if (backtrack <= 0)
                {
                    count--;
                    Trace.WriteLine($"FUNGUS[{fClr.Name}] : terminated, too much backtracking...");
                    // Thread.CurrentThread.Abort();
                }
            }
       
        }       

        public List<Point> GetAdj(Point cPoint)
        {
            List<Point> _lPoint = new List<Point>();
            //left
            _lPoint.Add(new Point(cPoint.X - 1, cPoint.Y));
            //top left
            _lPoint.Add(new Point(cPoint.X - 1, cPoint.Y-1));
            //top 
            _lPoint.Add(new Point(cPoint.X, cPoint.Y - 1));
            //top right
            _lPoint.Add(new Point(cPoint.X + 1, cPoint.Y - 1));
            //right
            _lPoint.Add(new Point(cPoint.X + 1, cPoint.Y));
            //bottom right;
            _lPoint.Add(new Point(cPoint.X + 1, cPoint.Y + 1));
            //bottom
            _lPoint.Add(new Point(cPoint.X, cPoint.Y + 1));
            //bottom left
            _lPoint.Add(new Point(cPoint.X - 1, cPoint.Y + 1));

            return _lPoint;
        }









    }
  
}
