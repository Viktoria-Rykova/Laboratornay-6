using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace компилятор_2
{
    internal class LexicalAnalyzer
    {
        public class MatchResult
        {
            public string Value { get; set; }
            public int Position { get; set; }
            public string Message { get; set; }
        }

        public List<MatchResult> FindMatches(string input, string type)
        {
            List<MatchResult> results = new List<MatchResult>();
            string pattern = "";
            string message = "";

            switch (type)
            {
                case "Американские адреса":
                    pattern = @"(?<=\s|^)\d{5}(-\d{4})?(?=\s|[.,;!?]|$)";
                    message = "Найден ZIP код";
                    break;

                case "Номер карты Visa":
                    pattern = @"\b4\d{3}([ ]?\d{4}){3}\b";
                    message = "Найден номер карты Visa";
                    break;

                case "Надежность пароля":
                    return FindStrongPasswordsByAutomaton(input);

                default:
                    return results;
            }

            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                results.Add(new MatchResult
                {
                    Value = match.Value,
                    Position = match.Index,
                    Message = message
                });
            }

            return results;
        }

        private List<MatchResult> FindStrongPasswordsByAutomaton(string input)
        {
            List<MatchResult> results = new List<MatchResult>();
            string[] words = input.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string specialChars = "#?!@$_%/^|&*-\\.";

            foreach (string word in words)
            {
                if (word.Length < 12) continue;

                int state = 0; 

                foreach (char c in word)
                {
                    if (char.IsUpper(c))
                        state |= 1; 
                    if (char.IsLower(c))
                        state |= 2; 
                    if (char.IsDigit(c))
                        state |= 4; 
                    if (specialChars.Contains(c))
                        state |= 8; 
                }

                if ((state & 15) == 15)  
                {
                    int pos = input.IndexOf(word, StringComparison.Ordinal);
                    results.Add(new MatchResult
                    {
                        Value = word,
                        Position = pos,
                        Message = "Найден надежный пароль (автомат)"
                    });
                }
            }

            return results;
        }
    }
}
