using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labratoria_ASD2_2024
{
    public class Lab14 : MarshalByRefObject
    {
        /// <summary>
        /// Znajduje wszystkie maksymalne palindromy długości przynajmniej 2 w zadanym słowie. Wykorzystuje Algorytm Manachera.
        ///
        /// Palindromy powinny być zwracane jako lista par (indeks pierwszego znaku, długość palindromu),
        /// tzn. para (i, d) oznacza, że pod indeksem i znajduje się pierwszy znak d-znakowego palindromu.
        ///
        /// Kolejność wyników nie ma znaczenia.
        ///
        /// Można założyć, że w tekście wejściowym nie występują znaki '#' i '$' - można je wykorzystać w roli wartowników
        /// </summary>
        /// <param name="text">Tekst wejściowy</param>
        /// <returns>Tablica znalezionych palindromów</returns>
        public (int startIndex, int length)[] FindPalindromes(string text)
        {
            // Dodałem szczegółowe komentarze bo jest to troche inne podejście niż nakreśliło polecenie

            // robimy nowy text między każde dwa znaki dajemy # by pozbyć się problemu rozpatrywania osobno waraiantów parzytych i nieparzystych
            // dzięki temu będziemy rozpatrywać jedynie przypadki nieparzyste
            
            char[] newText = new char[2 * text.Length + 1];
            for (int i = 0; i < text.Length; i++)
            {
                newText[2 * i] = '#';
                newText[2 * i + 1] = text[i];
            }
            newText[2 * text.Length] = '#';

          
            int[] R = new int[newText.Length];
            R[0] = 0; // pierwszy znaku jako cetrum nie bedzie mial palindromu -> nie ma nic po lewej
            R[1] = 1; // 2 znak jako cetrum na pewno bedzie palindrom o długości 1 bo #x# gdzie x - dowolny znak
            int center = 0, right = 0; // zmienne które będą nam trzymały informacje o aktulanie najdalszym maksymalnym palindromie od lewej

            for (int i = 2; i < newText.Length - 1; i++)
            {
                if (i < right) // jesteśmy w środku najdalszego palindromu (mierząc od początku) 
                {
                    R[i] = right - i > R[2 * center - i]? R[2 * center - i]: right - i;
                    // 1. right - i to jest to co nam zostaje po prawej, a że jesteśmy w palindromie to musi być i po lewej
                    // 2. Używamy symetrii: R[2 * center - i] to odbicie lustrzane względem centrum
                    // wybieramy mniejsze by było napewno prawdziwe
                }

                // sprawdzamy czy dany palidrom jest maksymalny
                while (i + R[i] + 1 < newText.Length && i - R[i] - 1 >= 0 && newText[i + R[i] + 1] == newText[i - R[i] - 1])
                {
                    R[i]++;
                }
                // jeżeli wchodzimy w następny palindrom to zapisujemy o tym informacje
                if (i + R[i] > right)
                {
                    center = i;
                    right = i + R[i];
                }
            }

            List<(int, int)> palindromes = new List<(int, int)>();
            for (int i = 1; i < newText.Length - 1; i++)
            {
                if (R[i] > 0)
                {
                    // jeżeli R[i] to mamy jakiś palindrom
                    int start = (i - R[i]) / 2; // obliczamy start jakby tych znaków # nie było
                    int palLength = R[i] * 2 + 1;
                    if (palLength >= 5) // większe od 5 bo uwzgledniamy znaki #
                    {
                        palindromes.Add((start, R[i]));
                    }
                }
            }
            return palindromes.ToArray();
        }

    }
}
