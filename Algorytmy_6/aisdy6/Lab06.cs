using ASD.Graphs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace ASD
{
    public class Lab06 : MarshalByRefObject
    {
        /// <summary>Etap 1</summary>
                    /// <param name="n">Liczba kolorów (równa liczbie wierzchołków w c)</param>
                    /// <param name="c">Graf opisujący możliwe przejścia między kolorami. Waga to wysiłek.</param>
                    /// <param name="g">Graf opisujący drogi w mieście. Waga to kolor drogi.</param>
                    /// <param name="target">Wierzchołek docelowy (dom Grzesia).</param>
                    /// <param name="start">Wierzchołek startowy (wejście z lasu).</param>
                    /// <returns>Pierwszy element pary to informacja, czy rozwiązanie istnieje. Drugi element pary, to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (pierwszy musi być start, ostatni target). W przypadku, gdy nie ma rozwiązania, ma być tablica o długości 0.</returns>
        
        public (bool possible, int[] path) Stage1(int n, DiGraph<int> c, Graph<int> g, int target, int start)
        {

            Stack<(int,int)> stack = new Stack<(int,int)>();
            List<int> path = new List<int>();
            bool flaga = false;
            int finalx = target, finaly = 0;

            (int i, int j)[,] road = new (int, int)[g.VertexCount, n];
            int[,] colors = new int[n,n];

            foreach(Edge<int> e in c.BFS().SearchAll()) colors[e.From,e.To] = 1;




            // uzupełniamy tablice wartoscami (-1,-1) - oznacza ze nie odwiedzony
            for (int i = 0;i < g.VertexCount;i++)
                for(int j = 0; j < n; j++) road[i,j] = (-1,-1);

            // wypełniamy wierzcholek start - odwiedzony przez kazdy kolor by z niego mozna bylo tez ruszyc kazdym kolorem
            for (int z = 0; z < n; z++) road[start, z] = (1, -1);

            stack.Insert((start,-1));

            while(stack.Count > 0)
            {
                (int actual,int prevcolor) = stack.Pop();

                if(actual == target)
                {
                    flaga = true;
                    break;
                }

                
                foreach(Edge<int> e in g.OutEdges(actual))
                {
                    bool isfind = false;

                    // sprawdzamy czy danym kolorem mozemy isc oraz jesli tak to zapisujemy jaki byl poprzedni kolor
                    if (prevcolor == -1 || colors[prevcolor,e.Weight] == 1 || prevcolor == e.Weight)
                    {
                        isfind = true;
                    }

                    // jesli weszlismy tym samym kolorem do wiercholka juz odwiedzonego tym kolorem to nie idziemy dalej
                    if (!road[e.To,e.Weight].Equals((-1,-1))) continue;

                    // w przeciwnym wypadku dodajemy wiercholek na stacka
                    if (isfind)
                    {
                        stack.Insert((e.To,e.Weight));
                        road[e.To, e.Weight] = (e.From,prevcolor);
                    }
                }
            }

            if(!flaga)return (false, new int[0]);

            // lokalizujemy ostani wiercholek - jaki mkolorem do neigo weszlismy
            while (road[finalx,finaly].Equals((-1,-1))) finaly++;

            // odzyskujemy sciezke
            while(finalx != start)
            {
                path.Add(finalx);
                (finalx,finaly) = road[finalx,finaly];
            }

            path.Add(finalx);
            path.Reverse();

            return (true, path.ToArray());

        }

        /// <summary>Drugi etap</summary>
                    /// <param name="n">Liczba kolorów (równa liczbie wierzchołków w c)</param>
                    /// <param name="c">Graf opisujący możliwe przejścia między kolorami. Waga to wysiłek.</param>
                    /// <param name="g">Graf opisujący drogi w mieście. Waga to kolor drogi.</param>
                    /// <param name="target">Wierzchołek docelowy (dom Grzesia).</param>
                    /// <param name="starts">Wierzchołki startowe (wejścia z lasu).</param>
                    /// <returns>Pierwszy element pary to koszt najlepszego rozwiązania lub null, gdy rozwiązanie nie istnieje. Drugi element pary, tak jak w etapie 1, to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (pierwszy musi być start, ostatni target). W przypadku, gdy nie ma rozwiązania, ma być tablica o długości 0.</returns>
        public (int? cost, int[] path) Stage2(int n, DiGraph<int> c, Graph<int> g, int target, int[] starts)
        {
            DiGraph<int> result = new DiGraph<int>(n * g.VertexCount + 2);
            bool flaga = false;
            List<int> odp = new List<int>();

            foreach (Edge<int> e in g.BFS().SearchAll())
            {
                int from = e.From + e.Weight * g.VertexCount;
                foreach (Edge<int> s in c.OutEdges(e.Weight))
                {
                    int To = e.To + s.To * g.VertexCount;
                    result.AddEdge(from, To, s.Weight + 1);
                }
                int To_same_color = e.To + e.Weight * g.VertexCount;
                result.AddEdge(from, To_same_color, 1);
            }

            foreach (var start in starts)
            {
                for (int i = 0; i < n; i++)
                {
                    result.AddEdge(n * g.VertexCount, start + i * g.VertexCount, 0);
                }
            }

            for (int i = 0; i < n; i++)
                result.AddEdge(target + i * g.VertexCount, n * g.VertexCount + 1, 0);


            foreach (Edge<int> a in result.DFS().SearchFrom(n * g.VertexCount))
            {
                if (a.To == n * g.VertexCount + 1)
                {
                    flaga = true;
                    break;
                }
            }
            if(!flaga) return (null, new int[0]);

            PathsInfo<int> pathsInfo = Paths.Dijkstra(result,n*g.VertexCount);
            int[] pathVertex = pathsInfo.GetPath(n * g.VertexCount, n * g.VertexCount + 1);
            if(pathVertex == null) return (null, new int[0]);
            int cost = 0;
            
            for(int i = 1;i < pathVertex.Length;i++)
            {
                if (pathVertex[i] < n * g.VertexCount)
                    odp.Add(pathVertex[i]%g.VertexCount);
                cost += result.GetEdgeWeight(pathVertex[i - 1], pathVertex[i]);
            }

            return(cost, odp.ToArray());
        }
    }
}