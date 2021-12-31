using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyanVideos
{
    class Ordering
    {
        public static List<string> OrderAlphanumeric(List<string> input)
        {

            List<List<long>> output = new List<List<long>>();
            int i = 0;
            foreach (string stringa in input)
            {
                output.Add(Encode(i, stringa));
                i++;
            }

            List<string> output_str = Ordered(output, input);


            return output_str;

        }
        public static string[] OrderAlphanumeric(string[] input)
        {
            return OrderAlphanumeric(input.ToList()).ToArray();
        }

        private static List<string> Ordered(List<List<long>> input, List<string> strings)
        {
            List<string> output_str = new List<string>();
            List<long> output = new List<long>();

            int maxLenght = 0;
            foreach (List<long> list in input)
            {
                if (list.Count >= maxLenght) maxLenght = list.Count;
            }
            foreach (List<long> list in input)
            {
                for (int i = list.Count; i < maxLenght; i++) list.Add(0);
            }

            List<List<long>> pool;
            List<List<long>> ordered = new List<List<long>>();

            for (int i = 0; i < input.Count; i++)
            {
                pool = new List<List<long>>();
                foreach (var list in input) if (!ordered.Contains(list)) pool.Add(list);
                List<long> selected = ExtractMinimum(pool);
                if (selected != null)
                {
                    ordered.Add(selected);
                    pool.Remove(selected);
                }
            }
            foreach (var list in ordered)
            {
                output_str.Add(strings[(int)list[0]]);
            }

            return output_str;
        }

        private static List<long> ExtractMinimum(List<List<long>> pool)
        {
            if (pool.Count == 0) return null;
            int count = pool[0].Count();
            int puntator = 0;
            long minimum = 999999999999999999;
            for (int i = 0; i < count - 1; i++)
            {
                minimum = 999999999999999999;
                puntator += 1;
                for (int j = 0; j < pool.Count(); j++)
                {
                    if (pool[j][puntator] < minimum) minimum = pool[j][puntator];
                }
                for (int j = pool.Count() - 1; j >= 0; j--)
                {
                    if (pool[j][puntator] > minimum) { pool.RemoveAt(j); }
                }
                if (pool.Count == 1) { break; }
            }
            return pool[0];
        }

        private static List<long> Encode(long i, string input)
        {
            List<long> output = new List<long>();
            output.Add(i);
            string character = "";
            foreach (char ch in input)
            {
                if (!char.IsDigit(ch) && character.All(char.IsDigit) && character != "")
                {
                    if (character.Length >= 18)
                    {
                        foreach (char ch1 in character) output.Add(ch1);
                    }
                    else output.Add(Convert.ToInt32(character));
                    output.Add((int)ch); character = ""; continue;
                }
                else if (!char.IsDigit(ch)) { output.Add((int)ch + 99999999999); continue; }
                else character += ch;
            }
            return output;
        }

        private static void print(List<long> list)
        {
            foreach (var intero in list) Console.Write(intero + " ");
            Console.WriteLine();
        }
    }
}
