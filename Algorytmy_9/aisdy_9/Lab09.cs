
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
{
    /// <summary>
    /// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
    /// <returns>Rozmiar największej kliki</returns>
    /// <remarks>
    /// Nie wolno modyfikować badanego grafu.
    /// </remarks>
    public static int MaxClique(this Graph g, out int[] clique)
    {
        clique = null;
        List<int> S = new List<int>();
        List<int> bestS = new List<int>();
        MaxCliqueRec(g,S,bestS,0);

        clique = bestS.ToArray();
        return bestS.Count;
    }

    private static void MaxCliqueRec(this Graph g, List<int> S, List<int> bestS, int k)
    {
        List<int> C = new List<int>();
        for (int i = k; i < g.VertexCount; i++)
        {
            bool isgood = true;
            foreach(int v in S)
            {
                if (!g.HasEdge(i, v))
                {
                    isgood = false; break;

                }
            }
            if(isgood)C.Add(i);
        }
        if (C.Count + S.Count <= bestS.Count) return;
        else
        {
            if(S.Count > bestS.Count)
            {
                bestS.Clear();
                foreach(int v in S) bestS.Add(v);
            }
        }
        foreach(int m in C)
        {
            S.Add(m);
            MaxCliqueRec(g, S,bestS,m + 1);
            S.Remove(m);
        }


    }

    /// <summary>
    /// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Pierwszy badany graf</param>
    /// <param name="h">Drugi badany graf</param>
    /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
    /// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
    /// <remarks>
    /// 1) Uwzględniamy wagi krawędzi
    /// 3) Nie wolno modyfikować badanych grafów.
    /// </remarks>
    public static bool IsomorphismTest(this Graph<int> g, Graph<int> h, out int[] map)
    {
        map = null;
        bool[] used = new bool[g.VertexCount];
        for(int i = 0;i < used.Length;i++) used[i] = false;
        List<int> s = new List<int>();
        bool flag = IsomorphismTestRec(g, h, s,0, used);

        if (flag)
        {
            map = s.ToArray();
            return true;
        }
        return false;
    }
    
    public static bool IsomorphismTestRec (this Graph<int> g, Graph<int> h, List<int> map, int k, bool[]used)
    {
        if(k == g.VertexCount)
        {
            return true;
        }
        for(int m = 0; m < g.VertexCount; m++)
        {
            if (!used[m])
            {
                bool flaga = false;
                if (g.Degree(m) != h.Degree(k)) flaga = true;

                for (int i = 0; i < k; i++)
                {
                    if (g.HasEdge(m, map[i]) && !h.HasEdge(k, i)) flaga = true;
                    if (!g.HasEdge(m, map[i]) && h.HasEdge(k, i)) flaga = true;
                    if (g.HasEdge(m, map[i]) && h.HasEdge(k,i) && g.GetEdgeWeight(m, map[i]) != h.GetEdgeWeight(k,i)) flaga = true;
                    if(flaga) break;
                }
                
                if (!flaga)
                {
                    used[m] = true;
                    map.Add(m);
                    if( IsomorphismTestRec(g, h, map, k + 1, used))
                        return true;
                    
                    used[m] = false;
                    map.Remove(m);
                }
            }
        }
        return false;
    }

}

