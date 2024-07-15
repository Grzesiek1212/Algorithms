using System;
using ASD.Graphs;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - Szukanie mozliwych do odwiedzenia miast z grafu skierowanego
        /// przy zalozeniu, ze pociagi odjezdzaja co godzine.
        /// </summary>
        /// <param name="graph">Graf skierowany przedstawiający siatke pociagow</param>
        /// <param name="miastoStartowe">Numer miasta z ktorego zaczyna sie podroz pociagiem</param>
        /// <param name="K">Godzina o ktorej musi zakonczyc sie nasza podroz</param>
        /// <returns>Tablica numerow miast ktore mozna odwiedzic. Posortowana rosnaco.</returns>
        public int[] Lab04Stage1(DiGraph graph, int miastoStartowe, int K)
        {
            K -= 8;
            int[] tab = new int[graph.VertexCount];
            int[] tab1 = new int[graph.VertexCount];
            for (int i = 0; i < tab.Length; i++)
            {
                tab[i] = int.MaxValue;
                tab1[i] = 0;
            }

            tab[miastoStartowe] = 0;
            tab1[miastoStartowe] = 1;
            List<int> odp = new List<int>();
            foreach (Edge e in graph.BFS().SearchFrom(miastoStartowe))
            {
                if (tab[e.To] > tab[e.From] + 1)
                {
                    if (tab[e.To] == int.MaxValue && tab[e.From] + 1 <= K) tab1[e.To] = 1;
                    tab[e.To] = tab[e.From] + 1;
                }
            }

            for (int i = 0; i < tab1.Length; i++)
            {
                if (tab1[i] == 1)
                    odp.Add(i);
            }

            return odp.ToArray();
        }

        /// <summary>
        /// Etap 2 - Szukanie mozliwych do odwiedzenia miast z grafu skierowanego.
        /// Waga krawedzi oznacza, ze pociag rusza o tej godzinie
        /// </summary>
        /// <param name="graph">Wazony graf skierowany przedstawiający siatke pociagow</param>
        /// <param name="miastoStartowe">Numer miasta z ktorego zaczyna sie podroz pociagiem</param>
        /// <param name="K">Godzina o ktorej musi zakonczyc sie nasza podroz</param>
        /// <returns>Tablica numerow miast ktore mozna odwiedzic. Posortowana rosnaco.</returns>
        public int[] Lab04Stage2(DiGraph<int> graph, int miastoStartowe, int K)
        {
            int[] tab = new int[graph.VertexCount];
            bool[] visited = new bool[graph.VertexCount];
            List<int> odp = new List<int>();
            SafePriorityQueue<int,int> koleja = new SafePriorityQueue<int, int> ();

            for (int i = 0; i < tab.Length; i++) tab[i] = int.MaxValue;
            tab[miastoStartowe] = 7;
            koleja.Insert(miastoStartowe, 7);
            visited[miastoStartowe] = true;

            while (koleja.Count > 0)
            {
                int actual = koleja.Extract();

               foreach(var e in graph.OutEdges(actual))
                {
                    if (tab[actual] < e.Weight && tab[e.To] > e.Weight && e.Weight < K)
                    {
                        tab[e.To] = e.Weight;
                        if (visited[e.To]) 
                        {
                            koleja.UpdatePriority(e.To, e.Weight);
                        }
                        else
                        {
                            koleja.Insert(e.To, e.Weight);
                            visited[e.To] = true;
                        }

                    }
                }
            }

            for(int i = 0;i < tab.Length; i++)
                if (tab[i] < K)
                    odp.Add(i);

            return odp.ToArray();
        }
    }
}