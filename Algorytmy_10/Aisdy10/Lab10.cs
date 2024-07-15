using System;
using System.Collections.Generic;
using ASD.Graphs;


namespace ASD
{
    public class Lab10 : MarshalByRefObject
    {

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt>">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <returns>Informację czy istnieje droga przez labirytn oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>
        public (bool routeExists, int[] route) FindEscape(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold)
        {
            List<int> road = new List<int>();
            road.Add(0);
            bool[] visited = new bool[labyrinth.VertexCount];
            for (int i = 0; i < labyrinth.VertexCount; i++) visited[i] = false;
            visited[0] = true;
            int moneytake = roomGold[0];
            bool flag = FindEscapeRec(labyrinth, startingTorches, roomTorches, debt, moneytake, roomGold, visited, 0, road);
            if (flag)
            {
                return (true, road.ToArray());
            }
            return (false, null);
        }

        public bool FindEscapeRec(Graph labyrinth, int TorchesAmount, int[] roomTorches, int debt, int moneytake, int[] roomGold, bool[] visited, int k, List<int> road)
        {
            if (k == labyrinth.VertexCount - 1 && moneytake >= debt)
            {
                return true;
            }
            foreach (int m in labyrinth.OutNeighbors(road[road.Count - 1]))
            {
                if (!visited[m])
                {
                    bool goodroom = true;
                    if (TorchesAmount == 1 && roomTorches[m] == 0 && m != labyrinth.VertexCount - 1) goodroom = false;

                    if (goodroom)
                    {

                        visited[m] = true;
                        moneytake += roomGold[m];
                        TorchesAmount -= 1;
                        TorchesAmount += roomTorches[m];
                        road.Add(m);

                        if (FindEscapeRec(labyrinth, TorchesAmount, roomTorches, debt, moneytake, roomGold, visited, m, road))
                            return true;

                        visited[m] = false;
                        road.Remove(m);
                        moneytake -= roomGold[m];
                        TorchesAmount -= roomTorches[m];
                        TorchesAmount += 1;
                    }
                }
            }
            return false;
        }

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <param name="dragonDelay">Opóźnienie z jakim wystartuje smok</param>
        /// <returns>Informację czy istnieje droga przez labirynt oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>

        public (bool routeExists, int[] route) FindEscapeWithHeadstart(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold, int dragonDelay)
        {
            List<int> road = new List<int> { 0 };
            bool[] destroyed = new bool[labyrinth.VertexCount];
            (int, int)[] edgeused = new (int, int)[labyrinth.VertexCount];
            int[] next_step = new int[labyrinth.VertexCount];

            for (int i = 0; i < labyrinth.VertexCount; i++)
            {
                destroyed[i] = false;
                next_step[i] = -1;
                edgeused[i] = (-1, -1);
                
            }

            int moneytake = roomGold[0];
            roomGold[0] = 0;
            startingTorches += roomTorches[0];
            roomTorches[0] = 0;
            edgeused[0] = (startingTorches, moneytake);

            if (FindEscapeHeadstartRec(labyrinth, startingTorches, roomTorches, debt, moneytake, roomGold, road, -1, destroyed, edgeused,0,next_step,dragonDelay,0))
                return (true, road.ToArray());

            return (false, null);
        }

        public bool FindEscapeHeadstartRec(Graph labyrinth, int TorchesAmount, int[] roomTorches, int debt, int moneytake, int[] roomGold, List<int> road, int dragonPosition, bool[] destroyed, (int,int)[] edgeused, int current, int[] next_step, int Delay, int CounterDelay)
        {
            if (dragonPosition == current) { return false; }
            
            if (current == labyrinth.VertexCount - 1)
            {
                if (moneytake >= debt) return true;
                else return false;
            }

            foreach (int m in labyrinth.OutNeighbors(current))
            {
                if (!destroyed[m])
                {
                    bool goodroom = true;
                    if (TorchesAmount == 1 && roomTorches[m] == 0 && m != labyrinth.VertexCount-1) goodroom = false;
                    if(TorchesAmount <= edgeused[m].Item1 && moneytake == edgeused[m].Item2) goodroom = false;

                    if (goodroom)
                    {
                        // ruch smoka
                        int before = 0;
                        if (CounterDelay < Delay) CounterDelay++;
                        else if(CounterDelay == Delay) { dragonPosition = 0; destroyed[0] = true; CounterDelay++; }
                        else if(CounterDelay > Delay) 
                        {
                            CounterDelay++;
                            if (dragonPosition == -1)
                            {
                                dragonPosition = 0;
                                destroyed[0] = true;
                            }
                            before = dragonPosition;
                            dragonPosition = next_step[dragonPosition];
                            destroyed[dragonPosition] = true;
                        }
                        
                        // ruch graczy
                        int remberRoomGold = roomGold[m], remeberRoomTorch = roomTorches[m], remeber_step_before = next_step[current];
                        moneytake += roomGold[m];
                        TorchesAmount += roomTorches[m];
                        roomGold[m] = 0;
                        roomTorches[m] = 0;
                        TorchesAmount--;

                        road.Add(m);
                        next_step[current] = m;
                        int remeberMoneyTake = edgeused[m].Item2, remberTorchesAmount = edgeused[m].Item1;
                        edgeused[m] = (TorchesAmount,moneytake);

                        if (FindEscapeHeadstartRec(labyrinth, TorchesAmount, roomTorches, debt, moneytake, roomGold, road, dragonPosition, destroyed, edgeused,m,next_step,Delay,CounterDelay))
                            return true;

                        // cofniecie graczy
                        road.RemoveAt(road.Count - 1);
                        edgeused[m] = (remberTorchesAmount,remeberMoneyTake);
                        roomGold[m] = remberRoomGold;
                        roomTorches[m] = remeberRoomTorch;
                        moneytake -= roomGold[m];
                        TorchesAmount -= roomTorches[m];
                        TorchesAmount++;
                        next_step[current] = remeber_step_before;

                        // cofniencie smoka
                        if (CounterDelay <= Delay) CounterDelay--;
                        else if (CounterDelay == Delay + 1) { destroyed[0] = false; CounterDelay--;dragonPosition = -1; }
                        else if (CounterDelay > Delay + 1 && dragonPosition != -1)
                        {
                            CounterDelay--;
                            destroyed[dragonPosition] = false;
                            dragonPosition = before;
                        }
                    }
                }
            }


            return false;

        }
    }
}