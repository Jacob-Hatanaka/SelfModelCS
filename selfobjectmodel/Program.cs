using System;

namespace Lojban
{
    public class Program
    {
        object[] data = new object[255];
        public Program(object val)
        {

        }
        public bool isVowel(char c)
        {
            return "aeiou".IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
        public bool isConsonant(char c)
        {
            return "bcdfgjklmnpqrstvwxyz".IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
        public void Scanner(string input)
        {
            string[] line = input.Split('i');
            foreach (string word in line)
            {
                char[] delimiters = { ' ', '\t', '\n' };
                string[] tokens = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                Parser(tokens);
            }
        }
        public void Parser(string[] tokens)
        {
            //1=cmavo, 2=brivla, 3=numbers, 4=names
            int[] types = new int[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                if (token.Length == 2)
                {
                    //CV
                    if (isVowel(token[1]) && isConsonant(token[0]))
                    {
                        types[i] = 1; // cmavo
                    }
                    continue;
                }
                if (token.Length == 5)
                {
                    //CVCCV or CCVCV
                    if (isVowel(token[0]) && isConsonant(token[1]) && isConsonant(token[2]) && isVowel(token[3]) && isVowel(token[4]))
                    {
                        types[i] = 2; // brivla
                    }
                    else if (isConsonant(token[0]) && isConsonant(token[1]) && isVowel(token[2]) && isConsonant(token[3]) && isVowel(token[4]))
                    {
                        types[i] = 2; // brivla
                    }
                    continue;
                }
                if (int.TryParse(token, out int val))
                {
                    if (val.ToString() == token)
                    {
                        types[i] = 3; // numbers
                        continue;
                    }
                }
                if (token.Length > 2)
                {
                    if (token[0] == '.' && token[token.Length - 1] == '.')
                    {
                        types[i] = 4; // names
                        continue;
                    }
                }
                throw new Exception($"Invalid token: {token}");

            }
        }
        public void Evaluator(int[] types, string[] tokens)
        {

        }
        public static void Main(string[] args)
        {

        }
    }
}