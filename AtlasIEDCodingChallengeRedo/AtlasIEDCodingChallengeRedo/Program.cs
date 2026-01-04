using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AtlasIEDCodingChallengeRedo
{
    class TestCase
    {
        public string Phone { get; set; }
        public bool ExpectedResult { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var tests = new List<TestCase>
            {
                new TestCase { Phone = "1234567890", ExpectedResult = false },
                new TestCase { Phone = "111", ExpectedResult = false },
                new TestCase { Phone = "1111", ExpectedResult = true },
                new TestCase { Phone = "1112221", ExpectedResult = false },
                new TestCase { Phone = "11211", ExpectedResult = false },
                new TestCase { Phone = "1211", ExpectedResult = false },
                new TestCase { Phone = "12111", ExpectedResult = true },
                new TestCase { Phone = "122111", ExpectedResult = true },
                new TestCase { Phone = "1212121222", ExpectedResult = false },
                new TestCase { Phone = "1112222", ExpectedResult = false },
                new TestCase { Phone = "1111111111", ExpectedResult = true },
            };
            foreach (var test in tests)
            {
                var result = IsValid(test.Phone);
                var testResult = result == test.ExpectedResult;
                Console.WriteLine($"Phone: {test.Phone} | IsValid: {result} | Expected: {test.ExpectedResult} | Test Result: {testResult}"); 
            }
        }

        static bool IsValid(string item)
        {
            bool isTriple = false;
            bool isQuad = false;
            int numQuads = 0;
            var digitCounts = new Dictionary<char, int>();
            
            if (string.IsNullOrEmpty(item) || item.Length < 3)
            {
                return false;
            }

            // Check for triple of only one character
            char tripleChar = 'X';
            for (int i = 2; i < item.Length; i++)
            {
                var c = item[i];
                var pC = item[i - 1];
                var ppC = item[i - 2];
                if (c == pC && c == ppC)
                {
                    // Only allowed one character three or more times in a row
                    if (isTriple && c != tripleChar)
                    {
                        return false; // found a triple in a character different than the first and only one is allowed
                    }
                    tripleChar = c;
                    isTriple = true;
                }
            }
            
            // Check for one character four or more times
            foreach (var c in item)
            {
                if (digitCounts.ContainsKey(c))
                {
                    digitCounts[c]++;
                }
                else
                {
                    digitCounts.Add(c, 1);
                }
            }
            foreach (var key in digitCounts.Keys)
            {
                if (digitCounts[key] >= 4)
                {
                    numQuads++;
                }
            }
            isQuad = numQuads == 1;

            return isTriple && isQuad;
        }
    }
}
