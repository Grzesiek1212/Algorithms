using System;
using System.Collections.Generic;

namespace ASD
{
    public class WaterCalculator : MarshalByRefObject
    {

        /*
         * Metoda sprawdza, czy przechodząc p1->p2->p3 skręcamy w lewo 
         * (jeżeli idziemy prosto, zwracany jest fałsz).
         */
        private bool leftTurn(Point p1, Point p2, Point p3)
        {
            Point w1 = new Point(p2.x - p1.x, p2.y - p1.y);
            Point w2 = new Point(p3.x - p2.x, p3.y - p2.y);
            double vectProduct = w1.x * w2.y - w2.x * w1.y;
            return vectProduct > 0;
        }


        /*
         * Metoda wyznacza punkt na odcinku p1-p2 o zadanej współrzędnej y.
         * Jeżeli taki punkt nie istnieje (bo cały odcinek jest wyżej lub niżej), zgłaszany jest wyjątek ArgumentException.
         */
        private Point getPointAtY(Point p1, Point p2, double y)
        {
            if (p1.y != p2.y)
            {
                double newX = p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y);
                if ((newX - p1.x) * (newX - p2.x) > 0)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point(p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y), y);
            }
            else
            {
                if (p1.y != y)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point((p1.x + p2.x) / 2, y);
            }
        }


        /// <summary>
        /// Funkcja zwraca tablice t taką, że t[i] jest głębokością, na jakiej znajduje się punkt points[i].
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double[] PointDepthfirstPart(Point[] points)
        {
            double[] ToRight = new double[points.Length];
            double[] ToLeft = new double[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                ToLeft[i] = double.MaxValue;
                ToRight[i] = double.MaxValue;
            }
            Stack<int> stack = new Stack<int>();
            bool flag = false;
            double heightmax = double.MaxValue;
            ToRight[0] = 0;
            ToLeft[points.Length - 1] = 0;

            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].y < points[i - 1].y && !flag)
                {
                    heightmax = points[i - 1].y;
                    ToRight[i - 1] = 0;
                    flag = true;
                }

                if (points[i].y >= heightmax)
                {
                    while (stack.Count > 0)
                    {
                        int current = stack.Pop();
                        ToRight[current] = heightmax - points[current].y;
                    }
                    heightmax = points[i].y;
                    ToRight[i] = 0;
                    continue;
                }
                if (flag) stack.Push(i);

            }
            heightmax = double.MaxValue;
            stack.Clear();
            flag = false;
            for (int i = points.Length - 2; i >= 0; i--)
            {
                if (points[i].y < points[i + 1].y && !flag)
                {
                    heightmax = points[i + 1].y;
                    ToLeft[i + 1] = 0;
                    flag = true;
                }

                if (points[i].y >= heightmax)
                {
                    while (stack.Count > 0)
                    {
                        int current = stack.Pop();
                        ToLeft[current] = heightmax - points[current].y;
                    }
                    heightmax = points[i].y;
                    ToLeft[i] = 0;
                    continue;
                }
                if (flag) stack.Push(i);

            }

            for (int i = 0; i < points.Length; i++)
            {
                if (ToRight[i] > ToLeft[i]) ToRight[i] = ToLeft[i];
                if (ToRight[i] == double.MaxValue) ToRight[i] = 0;
            }

            return ToRight;
        }

        public double[] PointDepths(Point[] points)
        {

            List<double> result = new List<double>();
            List<Point> list = new List<Point>();
            bool flag = false;

            for (int i = 0; i < points.Length-1; i++)
            {
                if (points[i].x <= points[i + 1].x)
                {
                    if (!flag)
                    {
                        if (result.Count != 0) result.RemoveAt(result.Count - 1);
                        list.Add(points[i]);
                        flag = true;
                    }
                    list.Add(points[i + 1]);
                    continue;
                }

                if (points[i].x > points[i + 1].x && !flag)
                {
                    result.Add(0);
                    continue;
                }

                if (points[i].x > points[i + 1].x && flag)
                {
                    double[] tmp = PointDepthfirstPart(list.ToArray());
                    result.AddRange(tmp);
                    flag = false;
                    list.Clear();
                    result.Add(0);
                }
            }
            if (flag)
            {
                double[] tmp = PointDepthfirstPart(list.ToArray());
                result.AddRange(tmp);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Funkcja zwraca objętość wody, jaka zatrzyma się w górach.
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double WaterVolume(Point[] points)
        {
            double score = 0;
            double[] result = PointDepths(points);
            int indexstart = -1, indexend = -1;
            bool findfirstpoint = false;
            for(int i = 0;i < result.Length; i++)
            {
                if (result[i] > 0 && !findfirstpoint) { indexstart = i - 1; findfirstpoint = true; continue; }
                if (result[i] == 0 && findfirstpoint) 
                {
                    indexend = i;
                    List<Point> list = new List<Point>();
                    if (points[indexstart].y < points[indexend].y) {
                        Point last = getPointAtY(points[indexend - 1], points[indexend], points[indexstart].y);
                        for(int j = indexstart; j < indexend; j++)
                        {
                            list.Add(points[j]);
                        }
                        list.Add(last);
                    }
                    else
                    {
                        Point first = getPointAtY(points[indexstart], points[indexstart + 1], points[indexend].y);
                        list.Add(first);
                        for(int j = indexstart + 1; j <= indexend; j++)
                        {
                            list.Add(points[j]);
                        }
                    }

                   
                    score += CalculateArea(list);
                    list.Clear();
                    indexstart = -1;
                    findfirstpoint = false;
                }

            }
            return score;
        }

        public double CalculateArea(List<Point> points)
        {
            double scorep = 0;
            scorep += (points[0].y - points[1].y) * (points[1].x - points[0].x) / 2;
            int i = 1;
            while(i < points.Count - 2) {
                double a = points[0].y - points[i].y;
                double b = points[0].y - points[i + 1].y;
                double h = Math.Abs(points[i + 1].x - points[i].x);
                scorep += (a + b) * h / 2;
                i++;
            }
            scorep += (points[points.Count-1].y - points[points.Count-2].y) * Math.Abs(points[points.Count-1].x - points[points.Count-2].x) / 2;

            return scorep;
        }
    }

    [Serializable]
    public struct Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
