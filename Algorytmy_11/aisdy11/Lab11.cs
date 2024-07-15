using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class Lab11 : System.MarshalByRefObject
    {

        // iloczyn wektorowy
        private int Cross((double, double) o, (double, double) a, (double, double) b)
        {
            double value = (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1; // -1 w prawo 0 prosto 1 w lewo
        }

        // Etap 1
        // po prostu otoczka wypukła
        

        public (double, double)[] ConvexHull((double, double)[] points)
        {
            if(points.Length == 1 || points.Length == 0) return points;

            (double x, double y) first = points[0]; 
            for(int i = 1; i < points.Length;i++)
            {
                if (points[i].Item2 < first.y)
                    first = points[i];
                if (points[i].Item2 == first.y && points[i].Item1 < first.x)
                    first = points[i];
            }
            List<(double,double)> pointsList = points.ToList();
            pointsList.Remove(first);
            for (int i = 0; i < pointsList.Count; i++)
                if (points[i] == first) pointsList.Remove(first);

            if (pointsList.Count == 0) return points;
            int needsort((double, double) one, (double, double) two)
            {
                return -Cross(first, one, two);
                
            }

            pointsList.Sort(needsort);

            Stack<(double, double)> stack = new Stack<(double, double)>();
            stack.Push(first);
            stack.Push(pointsList[0]);

            for(int k = 1; k < pointsList.Count; k++)
            {
                while (true)
                {
                    if (stack.Count == 1) break;
                    (double, double) remeberlast = stack.Pop();
                    (double,double) belowtop = stack.Peek();
                    stack.Push(remeberlast);
                    if (Cross(belowtop, remeberlast, pointsList[k]) == 1) break;
                    stack.Pop();    
                }
                stack.Push(pointsList[k]);
            }
            return stack.Reverse().ToArray();
        }

        // Etap 2
        // oblicza otoczkę dwóch wielokątów wypukłych
        public (double, double)[] ConvexHullOfTwo((double, double)[] poly1, (double, double)[] poly2)
        {
            double maxx1 = poly1[0].Item1, maxx2 = poly2[0].Item1;
            int indexf1 = 0, indexf2 = 0;

            // Find the index of the first element and the value of the rightest element
            for (int i = 0;i< poly1.Length; i++)
            {
                if (poly1[i].Item1 > maxx1) maxx1 = poly1[i].Item1;
                if (poly1[i].Item2 < poly1[indexf1].Item2)
                    indexf1 = i;
                if (poly1[i].Item2 == poly1[indexf1].Item2 && poly1[i].Item1 < poly1[indexf1].Item1)
                    indexf1 = i;
            }

            for (int i = 0; i < poly2.Length; i++)
            {
                if (poly2[i].Item1 > maxx2) maxx2 = poly2[i].Item1;
                if (poly2[i].Item2 < poly2[indexf2].Item2)
                    indexf2 = i;
                if (poly2[i].Item2 == poly2[indexf2].Item2 && poly2[i].Item1 < poly2[indexf2].Item1)
                    indexf2 = i;
            }

            List<(double,double)> firstpartof1 = new List<(double,double)>();
            List<(double,double)> firstpartof2 = new List<(double, double)>();
            List<(double, double)> Secondpartof1 = new List<(double, double)>();
            List<(double, double)> Secondpartof2 = new List<(double, double)>();

            // there we create the two parts of cycle
            bool checkp1 = true, checkp2 = true;
            int end1 = indexf1, end2 = indexf2;
            while(true)
            {
                if (checkp1) firstpartof1.Add(poly1[indexf1]);
                else Secondpartof1.Add(poly1[indexf1]);
                if (poly1[indexf1].Item1 == maxx1) checkp1 = false;
                indexf1 = (indexf1+1)%poly1.Length;
                if (indexf1 == end1) break;
            }

            while(true)
            {
                if (checkp2) firstpartof2.Add(poly2[indexf2]);
                else Secondpartof2.Add(poly2[indexf2]);
                if (poly2[indexf2].Item1 == maxx2) checkp2 = false;
                indexf2 = (indexf2+1) % poly2.Length;
                if (indexf2 == end2) break;
            }

            Secondpartof1.Reverse();
            Secondpartof2.Reverse();

            // there we merge down' parts of cycyles and up' parts  of cycles
            List<(double, double)> UP = new List<(double, double)>();
            List<(double, double)> DOWN = new List<(double, double)>();

            while (firstpartof1.Count != 0 && firstpartof2.Count != 0)
            {
                if (firstpartof1[0].Item1 <= firstpartof2[0].Item1)
                {
                    if (UP.IndexOf(firstpartof1[0])== -1)UP.Add(firstpartof1[0]);
                    firstpartof1.RemoveAt(0);
                }
                else
                {
                    if (UP.IndexOf(firstpartof2[0]) == -1) UP.Add(firstpartof2[0]);
                    firstpartof2.RemoveAt(0);
                }
            }
            if(firstpartof1.Count != 0) for(int i = 0;i < firstpartof1.Count; i++) if (UP.IndexOf(firstpartof1[i]) == -1) UP.Add(firstpartof1[i]);
            if (firstpartof2.Count != 0) for (int i = 0; i < firstpartof2.Count; i++) if (UP.IndexOf(firstpartof2[i]) == -1) UP.Add(firstpartof2[i]);

            
            while (Secondpartof1.Count != 0 && Secondpartof2.Count != 0)
            {
                if (Secondpartof1[0].Item1 <= Secondpartof2[0].Item1)
                {
                    if (DOWN.IndexOf(Secondpartof1[0]) == -1) DOWN.Add(Secondpartof1[0]);
                    Secondpartof1.RemoveAt(0);
                }
                else
                {
                    if (DOWN.IndexOf(Secondpartof2[0]) == -1) DOWN.Add(Secondpartof2[0]);
                    Secondpartof2.RemoveAt(0);
                }
            }
            if (Secondpartof1.Count != 0) for (int i = 0; i < Secondpartof1.Count; i++) if (DOWN.IndexOf(Secondpartof1[i]) == -1) DOWN.Add(Secondpartof1[i]);
            if (Secondpartof2.Count != 0) for (int i = 0; i < Secondpartof2.Count; i++) if (DOWN.IndexOf(Secondpartof2[i]) == -1) DOWN.Add(Secondpartof2[i]);

            // there we crate sorted whole list and use the algoritm who find us an cycle
            List<(double, double)> newcycle = UP;
            DOWN.Reverse();
            for(int i = 0; i < DOWN.Count; i++) if (newcycle.IndexOf(DOWN[i])==-1)newcycle.Add(DOWN[i]);

            Stack<(double, double)> stack = new Stack<(double, double)>();
            stack.Push(newcycle[0]);
            stack.Push(newcycle[1]);


            for (int k = 2; k < newcycle.Count; k++)
            {
                while (true)
                {
                    if (stack.Count == 1) break;
                    (double, double) remeberlast = stack.Pop();
                    (double, double) belowtop = stack.Peek();
                    stack.Push(remeberlast);
                    if (Cross(belowtop, remeberlast, newcycle[k]) == 1) break;
                    stack.Pop();
                }
                stack.Push(newcycle[k]);
            }

            (double, double) Remeberlast = stack.Pop();
            (double, double) Belowtop = stack.Peek();
            stack.Push(Remeberlast);
            if (Cross(Belowtop, Remeberlast, newcycle[0]) != 1) stack.Pop();

            return stack.Reverse().ToArray();
        }

    }
}
