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
            //specials
            /*
            cmavos
            lo = the, precedes names and new predicates
            se = swap 1st and 2nd params
            brivlas
            fatci = exists, one argument, true, name exists
            sumji = plus, two arguments, returns sum as third argument “i 4 sumji 2 2” evaluates to true.
            vujni = “minus”. Argument 1 is argument 2 minus argument 3.
            dunli = “equal”. Argument 1 has the same value as argument 2. This works for numbers, names, or lists.
            steni = the empty list. The first argument is the empty list. I.e., “i lo .nothing. steni” sets the variable “nothing” to be the empty list. If you use “lo steni” as an argument, that is the empty list. Thus, “i lo steni fatci” returns true - the empty list exists.
            steko - a cons cell in a linked list. The first argument is a cons cell, with the second argument being the head, and the third argument being the rest of the list. For example, “i lo .Brook. steko lo steni” assigns “Brook” the empty list (there is no third argument). When used with “lo”, steko forms a list literal. For example, “lo steko 1 lo steko 2 lo steko 3 lo steni” is the list ( 1 2 3 ). The statement “i lo .list. steko 1 lo steko 2 lo steni” assigns the list (1 2) to the  variable “list”.???
            cmavo - declaring a predicate. The first argument is the name of the predicate (which can be either a predicate word or a name word, in both cases preceded by the short word lo). The second argument is the argument to the predicate (which can be either a name or a list of names). There must be at least one argument (otherwise, fatci should be used), and you do not have to support lists of arguments longer than five. The last argument is a predicate or list of predicates asserted when the declared predicate is used, which can be the empty list (i.e., no further action). The list of predicates should not have a hard-coded limit.
            */
        }
        public static void Main(string[] args)
        {

        }
    }
}