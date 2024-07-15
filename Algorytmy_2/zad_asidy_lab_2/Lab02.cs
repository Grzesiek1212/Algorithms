using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace ASD
{
    public class Lab02 : MarshalByRefObject
    {
        struct pole
        {
            public int i;
            public int j;
            public int allcost;
        }
        /// <summary>
        /// Etap 1 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        public (bool result, int cost, (int i, int j)[] path) Lab02Stage1(int n, int m, ((int di, int dj) step, int cost)[] moves)
        {
            List<(int i, int j)> allpath = new List<(int i, int j)>();
            pole[,] Tab = new pole[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Tab[i, j].allcost = int.MaxValue;
                }
            }
            Tab[0, 0].allcost = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (Tab[i, j].allcost == int.MaxValue) continue;
                    for (int k = 0; k < moves.Length; k++)
                    {
                        int newcost = Tab[i, j].allcost + moves[k].cost;
                        int newi = i + moves[k].step.di;
                        int newj = j + moves[k].step.dj;
                        if (newi >= n || newj >= m) continue;
                        if (Tab[newi, newj].allcost > newcost)
                        {
                            Tab[newi, newj].allcost = newcost;
                            Tab[newi, newj].i = moves[k].step.di;
                            Tab[newi, newj].j = moves[k].step.dj;

                        }
                    }

                }
            }

            int finalm = 0;
            for (int iter = 1; iter < m; iter++)
            {
                if (Tab[n - 1, iter].allcost < Tab[n - 1, finalm].allcost) finalm = iter;
            }

            if (Tab[n-1,finalm].allcost == int.MaxValue) return (false, int.MaxValue, null);

            int indexi = n - 1, indexj = finalm;
            while (indexi > 0 || indexj > 0)
            {
                allpath.Add((indexi, indexj));
                int indexitominus = Tab[indexi, indexj].i;
                int indexjtominus = Tab[indexi, indexj].j;
                indexi -= indexitominus;
                indexj -= indexjtominus;
            }
            allpath.Add((0, 0));
            allpath.Reverse();

            return (true, Tab[n - 1, finalm].allcost, allpath.ToArray());

        }


        /// <summary>
        /// Etap 2 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową - dodatkowe założenie, każdy ruch może być wykonany co najwyżej raz
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        struct pole2
        {
            public int isstep;
            public int allcost;
        }
        public (bool result, int cost, (int i, int j)[] pat) Lab02Stage2(int n, int m, ((int di, int dj) step, int cost)[] moves)
        {
            List<(int i, int j)> allpath = new List<(int i, int j)>();
            int c;
            pole2[,,] Tab = new pole2[n, m, moves.Length + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    for (int k = 0; k <= moves.Length; k++)
                    {
                        Tab[i, j, k].isstep = 0;
                        Tab[i, j, k].allcost = int.MaxValue;
                    }
                }
            }

            Tab[0, 0, 0].allcost = 0;

            for (int k = 1; k <= moves.Length; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (Tab[i, j, k - 1].allcost != int.MaxValue) // warunek jesli w poprzednim poziomie bylismy wstanie dojsc na to pole
                        {
                            c = Tab[i, j, k - 1].allcost;
                            if (c < Tab[i, j, k].allcost)
                            {
                                Tab[i, j, k].allcost = c;
                                Tab[i, j, k].isstep = 0;

                            }
                        }
                        // bedzie dodawac k-aty ruch
                        int indexi = i - moves[k - 1].step.di;
                        int indexj = j - moves[k - 1].step.dj;
                        if (indexi < 0 || indexj < 0) continue;
                        if (Tab[indexi, indexj, k - 1].allcost != int.MaxValue)
                        {
                            c = Tab[indexi, indexj, k - 1].allcost + moves[k - 1].cost;
                            if (c < Tab[i, j, k].allcost)
                            {
                                Tab[i, j, k].allcost = c;
                                Tab[i, j, k].isstep = 1;
                            }
                        }
                    }
                }
            }
            // teraz musimy sprawdzic czy sciezka istniej i jak tak to wybrac pole
            int finalm = 0;
            for (int iter = 1; iter < m; iter++)
            {
                if (Tab[n - 1, iter, moves.Length].allcost < Tab[n - 1, finalm, moves.Length].allcost) finalm = iter;

            }
            if (Tab[n - 1, finalm, moves.Length].allcost == int.MaxValue) return (false, int.MaxValue, null);
            
            // tutaj wracamy sie tworzac sciezke
            int indexpathi = n - 1, indexpathj = finalm, indexpathk = moves.Length;
            while (indexpathk > 0 && (indexpathi > 0 || indexpathi > 0))
            {
                if (Tab[indexpathi, indexpathj, indexpathk].isstep == 1)
                {
                    allpath.Add((indexpathi, indexpathj));
                    indexpathi -= moves[indexpathk - 1].step.di;
                    indexpathj -= moves[indexpathk - 1].step.dj;
                }
                indexpathk--;
            }

            allpath.Add((0, 0));
            allpath.Reverse();

            return (true, Tab[n - 1, finalm, moves.Length].allcost, allpath.ToArray());



        }
    }
}
