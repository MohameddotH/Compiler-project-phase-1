using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TinyScanner
{
    // ===========================================================================
    // Phase 1 - Tiny Language Lexical Analyzer
    // عمل الطالب: محمد هشام عبدالحميد صابر السيد نصر
    // ID: 324243444 
    // ===========================================================================

    class Token
    {
        public string Lexeme { get; set; }
        public string Type { get; set; }
        public int Line { get; set; }

        public Token(string lexeme, string type, int line)
        {
            Lexeme = lexeme;
            Type = type;
            Line = line;
        }
    }

    class Scanner
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "int", "float", "string", "read", "write", "repeat", "until", 
            "if", "elseif", "else", "then", "return", "endl", "main", "end"
        };

        private static readonly List<(string Pattern, string Type)> TokenRules = new List<(string, string)>
        {
            (@"/\*[\s\S]*?\*/",          "Comment"),
            (@"""[^""]*""",              "String_Literal"),
            (@":=",                       "Assignment_Op"),
            (@"<>",                       "NotEqual_Op"),
            (@"&&",                       "LogicalAnd_Op"),
            (@"\|\|",                     "LogicalOr_Op"),
            (@"\+",                       "Plus_Op"),
            (@"-",                        "Minus_Op"),
            (@"\*",                       "Multiply_Op"),
            (@"/",                        "Divide_Op"),
            (@"<",                        "LessThan_Op"),
            (@">",                        "GreaterThan_Op"),
            (@"=",                        "Equal_Op"),
            (@";",                        "Semicolon"),
            (@",",                        "Comma"),
            (@"\(",                       "LeftParen"),
            (@"\)",                       "RightParen"),
            (@"\{",                       "LeftBrace"),
            (@"\}",                       "RightBrace"),
            (@"\d+\.\d+",                 "Decimal_Number"),
            (@"\d+",                      "Integer_Number"),
            (@"[a-zA-Z][a-zA-Z0-9]*",    "Identifier"),
            (@"[ \t\r\n]+",              "WHITESPACE"),
        };

        public List<Token> MatchTokens(string sourceCode)
        {
            List<Token> tokenList = new List<Token>();
            int position = 0;
            int currentLine = 1;

            while (position < sourceCode.Length)
            {
                bool matched = false;
                foreach (var (regexPattern, tokenType) in TokenRules)
                {
                    Regex regex = new Regex(@"\G" + regexPattern);
                    Match match = regex.Match(sourceCode, position);

                    if (match.Success)
                    {
                        string lexeme = match.Value;
                        if (lexeme.Contains("\n")) currentLine += lexeme.Count(c => c == '\n');

                        if (tokenType != "WHITESPACE")
                        {
                            string finalType = tokenType;
                            if (tokenType == "Identifier" && Keywords.Contains(lexeme))
                                finalType = "Keyword";

                            tokenList.Add(new Token(lexeme, finalType, currentLine));
                        }
                        position += match.Length;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    tokenList.Add(new Token(sourceCode[position].ToString(), "Unknown", currentLine));
                    position++;
                }
            }
            return tokenList;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "input.txt";
            if (!File.Exists(inputFile)) { Console.WriteLine("Error: input.txt not found!"); return; }

            string sourceCode = File.ReadAllText(inputFile);
            Scanner scanner = new Scanner();
            List<Token> tokenList = scanner.MatchTokens(sourceCode);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("=====================================================");
            Console.WriteLine("Student Name: Mohamed Hesham Abdelhamid");
            Console.WriteLine("ID:  324243444");
            Console.WriteLine("Project: TINY Language Scanner (Phase 1)");
            Console.WriteLine("=====================================================");
            Console.ResetColor();

            PrintTable(tokenList);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[✔] Scanning Completed Successfully!");
            Console.WriteLine($"[i] Total Tokens Found: {tokenList.Count}");
            Console.ResetColor();
        }

        static void PrintTable(List<Token> tokenList)
        {
            int lexWidth = 20; int typeWidth = 20;
            foreach (var t in tokenList) {
                if (t.Lexeme.Length > lexWidth) lexWidth = t.Lexeme.Length;
                if (t.Type.Length > typeWidth) typeWidth = t.Type.Length;
            }

            string sep = "+" + new string('-', lexWidth + 2) + "+" + new string('-', typeWidth + 2) + "+-------+";
            Console.WriteLine(sep);
            Console.WriteLine("|  " + "Lexeme".PadRight(lexWidth) + "|  " + "Token Type".PadRight(typeWidth) + "|  Line |");
            Console.WriteLine(sep);

            foreach (var token in tokenList)
            {
                if (token.Type == "Keyword") Console.ForegroundColor = ConsoleColor.Yellow;
                else if (token.Type.Contains("Number")) Console.ForegroundColor = ConsoleColor.Magenta;
                else if (token.Type == "Unknown") Console.ForegroundColor = ConsoleColor.Red;
                else if (token.Type == "Comment") Console.ForegroundColor = ConsoleColor.DarkGray;
                else Console.ForegroundColor = ConsoleColor.Cyan;

                string l = token.Lexeme.PadRight(lexWidth);
                string t = token.Type.PadRight(typeWidth);
                string ln = token.Line.ToString().PadRight(5);

                Console.WriteLine($"|  {l}|  {t}|  {ln}|");
                Console.ResetColor();
            }
            Console.WriteLine(sep);
        }
    }
}
