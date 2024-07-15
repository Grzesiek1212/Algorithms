
using System;

namespace ASD
{

    class ChangeMaking
    {

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// bez ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości amount ( czyli rzędu o(amount) )
        /// </remarks>
        public int? NoLimitsDynamic(int amount, int[] coins, out int[] change)
        {
            int[] T = new int[amount + 1];
            int[] P = new int[amount + 1];

            for(int x = 0;x < amount+1;x++) T[x] = int.MaxValue;

            T[0] = 0;
            for(int i = 1;i < amount + 1; i++)
            {
                T[i] = int.MaxValue;
                for(int j = 0; j < coins.Length; j++)
                {
                    if (i - coins[j] < 0) continue;
                    if (T[i - coins[j]] == int.MaxValue) continue;

                    int c = T[i - coins[j]]+ 1;
                    if(c < T[i])
                    {
                        T[i] = c;
                        P[i] = j;
                    }
                }
            }

            if (T[amount] == int.MaxValue) { change = null; return null; }

            change = new int[coins.Length];
            for (int s = 0; s < change.Length; s++) change[s] = 0;

            int kk = amount;
            while(kk > 0)
            {
                change[P[kk]] ++;
                kk -= coins[P[kk]];
            }

            return T[amount];      // zmienić
        }

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// z uwzględnieniem ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="limits">Liczba dostępnych monet danego nomimału</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// limits[i] - dostepna liczba monet i-tego rodzaju (nominału)
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości iloczynu amount*(liczba rodzajów monet)
        /// ( czyli rzędu o(amount*(liczba rodzajów monet)) )
        /// </remarks>
        public int? Dynamic(int amount, int[] coins, int[] limits, out int[] change)
        {
            int[,] T = new int[coins.Length, amount + 1];
            int[,] P = new int[coins.Length, amount + 1];

            for (int n = 0; n < coins.Length; n++)
            {
                for (int m = 0; m < amount + 1; m++)
                {
                    T[n, m] = int.MaxValue;
                    P[n, m] = 0;
                }
            }


            for (int i = 0; i < coins.Length; i++) { T[i, 0] = 0; P[i, 0] = 0; } // uzupełniamy 1 kolumne
            
            for (int i = 1; i < amount + 1; i++) // uzupełniamy pierwszy wiersz
            {
                if (i % coins[0] == 0 && i / coins[0] <= limits[0])
                {
                    T[0, i] = i / coins[0];
                    P[0, i] = i / coins[0];
                }

                else T[0, i] = int.MaxValue;
            }

            for(int i = 1; i < coins.Length; i++) // uzupełnaimy reszte
            {
                for (int j = 1; j < amount + 1; j++)
                {
                    T[i, j] = int.MaxValue;
                    int index = j, licznik = 0;
                    while(index > -1 && licznik <= limits[i])
                    {
                        if (T[i - 1, index] != int.MaxValue)
                        {
                            int c = licznik + T[i - 1, index];

                            if (c < T[i, j])
                            {
                                T[i, j] = c;
                                P[i, j] = licznik;
                            }
                        }
                        index -= coins[i];
                        licznik++;

                    }
                }
            }

            if (T[coins.Length-1, amount] == int.MaxValue) { change = null; return null; }


            change = new int[coins.Length];
            for (int s = 0; s < change.Length; s++) change[s] = 0;

            int kj = amount, ki = coins.Length - 1;
            while (kj >= 0 && ki >= 0)
            {
                change[ki] = P[ki,kj];
                kj -= P[ki,kj] * coins[ki];
                ki--;
            }

            return T[coins.Length-1,amount];      // zmienić
        }

    }

}
