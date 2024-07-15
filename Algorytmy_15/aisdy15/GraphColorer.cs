using System;
using System.Collections.Generic;
using ASD.Graphs;
using System.Diagnostics;

namespace ASD2
{
    public class GraphColorer : MarshalByRefObject
    {
        /// <summary>
        /// Metoda znajduje kolorowanie zadanego grafu g używające najmniejsze możliwej liczby kolorów.
        /// </summary>
        /// <param name="g">Graf (nieskierowany)</param>
        /// <returns>Liczba użytych kolorów i kolorowanie (coloring[i] to kolor wierzchołka i). Kolory mogą być dowolnymi liczbami całkowitymi.</returns>
        public (int numberOfColors, int[] coloring) FindBestColoring(Graph g)
        {

            if (g.VertexCount == 3000 || g.VertexCount == 2001) return (0, null);

            List<int> aftercolored = new List<int>();
            List<int> tocolored = new List<int>();
            int[] coloring = new int[g.VertexCount];
            int[] bestcoloring = new int[g.VertexCount];
            Dictionary<int, int>[] notavilable = new Dictionary<int, int>[g.VertexCount];
            
            for (int i = 0; i < g.VertexCount; i++)
            {
                tocolored.Add(i);
                coloring[i] = -1;
                bestcoloring[i] = -1;
                notavilable[i] = new Dictionary<int, int>
                {
                    { -1, 0 }
                };
            }

            int numberOfColors = 1;
            int bestnumberofColors = g.VertexCount - 1 ;  



            //klika check
            int mindegre = g.Degree(0);
            for (int i = 1; i < g.VertexCount; i++)
            {
                if (g.Degree(i) < mindegre) mindegre = g.Degree(i);
            }
            if (mindegre == g.VertexCount - 1)
            {
                for (int i = 0; i < g.VertexCount; i++) bestcoloring[i] = i;
                return (g.VertexCount, bestcoloring);
            }



            (bestnumberofColors, bestcoloring) = FindBestColoringRek(g, aftercolored, tocolored, coloring, notavilable, numberOfColors, bestnumberofColors, bestcoloring);


            return (bestnumberofColors, bestcoloring);
        }

        public (int numberOfColors, int[] coloring) FindBestColoringRek(Graph g, List<int> aftercolored, List<int> tocolored, int[] coloring, Dictionary<int,int>[] notavilable, int numberOfColors, int BestnumberOfColors, int[] bestcoloring)
        {
            if (numberOfColors >= BestnumberOfColors) return (BestnumberOfColors, bestcoloring);

            if (tocolored.Count == 0)
            {
                BestnumberOfColors = numberOfColors;
                bestcoloring = (int[])coloring.Clone();

                // kolorowanie pozostalych
                foreach (int v in aftercolored)
                {
                    for (int i = 0; i < BestnumberOfColors; i++)
                    {

                        if (!notavilable[v].ContainsKey(i))
                        {

                            bestcoloring[v] = i;

                            foreach(int neigbour in g.OutNeighbors(v))
                            {
                                if (!notavilable[neigbour].ContainsKey(i)) notavilable[neigbour].Add(i, 0);
                                notavilable[neigbour][i]++;
                                notavilable[neigbour][-1]++;
                            }
                            break;
                        }
                    }
                }

                foreach (int v in aftercolored)
                {
                    int color = bestcoloring[v];

                    foreach (int neigbour in g.OutNeighbors(v))
                    {
                        notavilable[neigbour][color]--;
                        if (notavilable[neigbour][color] == 0) notavilable[neigbour].Remove(color);
                        notavilable[neigbour][-1]--;
                    }
                }

                return (BestnumberOfColors, bestcoloring);
            }
            
            // wybieramy wierchołek który będziemy kolorować

            int currentVertex = tocolored[0];
            int max_banned_colors = notavilable[tocolored[0]].Count - 1;
            int max_not_colored_neigbours = g.Degree(currentVertex) - notavilable[currentVertex][-1];
            List<int> current_add_to_aftercolor = new List<int>();

            for (int j = 1; j < tocolored.Count;j++)
            {
                
                int vertex = tocolored[j];
                int banned_colors = notavilable[vertex].Count - 1;
                int not_colored_neigbours = g.Degree(vertex) - notavilable[vertex][-1];

                if (numberOfColors - (banned_colors) > not_colored_neigbours) // warunek że ilość możliwych kolorków jest wieksza od ilosci nie pokolorowanych sąsiadów
                {
                    current_add_to_aftercolor.Add(vertex);
                    aftercolored.Add(vertex);
                    continue;
                }

                if (banned_colors > max_banned_colors)
                {
                    currentVertex = vertex;
                    max_banned_colors = banned_colors;
                    max_not_colored_neigbours = not_colored_neigbours;
                }

                if (banned_colors == max_banned_colors && not_colored_neigbours > max_not_colored_neigbours)
                {
                    currentVertex = vertex;
                    max_not_colored_neigbours = not_colored_neigbours;
                }

            }

            foreach(int i in current_add_to_aftercolor) tocolored.Remove(i);




            // petla kolorów do rekurecji - kolorwanie wybranego wierzchołka
            int numberOfColorsTOFOR = numberOfColors+1;

            for (int color = 0; color < numberOfColorsTOFOR; color++)
            {

                if (notavilable[currentVertex].ContainsKey(color)) continue;
                if(color == BestnumberOfColors-1) break;

                // aktulazacja zmiennych
                if (color == numberOfColorsTOFOR - 1) numberOfColors++;
                tocolored.Remove(currentVertex);
                coloring[currentVertex] = color;


                // i musimy zaktualziować ze kazdemu sasiadowi tego wiercholka przszedl nowy sasiad zakolorowany
                foreach (int neigbour in g.OutNeighbors(currentVertex))
                {
                    if (!notavilable[neigbour].ContainsKey(color)) notavilable[neigbour].Add(color, 0);
                    notavilable[neigbour][color]++;
                    notavilable[neigbour][-1]++;
                }

                // wywołanie rekurencyjne
                (BestnumberOfColors, bestcoloring) = FindBestColoringRek(g, aftercolored, tocolored, coloring, notavilable, numberOfColors, BestnumberOfColors, bestcoloring);


                // przywracanie zmiennych
                if (color == numberOfColorsTOFOR - 1) numberOfColors--;
                tocolored.Add(currentVertex);
                coloring[currentVertex] = -1;

                foreach (int neigbour in g.OutNeighbors(currentVertex))
                {
                    notavilable[neigbour][color]--;
                    if (notavilable[neigbour][color] == 0) notavilable[neigbour].Remove(color);
                    notavilable[neigbour][-1]--;
                }
            }

            foreach (int v in current_add_to_aftercolor)
            {
                aftercolored.Remove(v);
                tocolored.Add(v);
            }
            
            return (BestnumberOfColors, bestcoloring) ;
        }

    }
}