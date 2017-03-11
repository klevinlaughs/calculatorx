using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorX
{
    static class MathProcessor
    {
        public const string CUBEROOT = "\u221B";
        public const string CUBED = "\u00B3";
        public const string SQUAREROOT = "\u221A";
        public const string SQUARED = "\u00B2";
        public const string SUPERSCRIPTN = "\u207F";
        public const string PI = "\u03c0";

        static public double factorial(double d)
        {
            if (d < 0)
            {
                throw new Exception("Math error: factorials >= 0");
            }

            if (d == 0)
            {
                return 1d;
            }

            double num = 1;
            for (int i = 1; i <= d; i++)
            {
                num *= i;
            }
            return num;
        }

        static public double perm(double first, double second)
        {
            try
            {
                if (second < 0) throw new Exception("r -ve");

                double num = factorial(first);
                double den = factorial(first - second);

                return num / den;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + ", nPr for n>=r");
            }
        }

        static public double comb(double first, double second)
        {
            try
            {
                double num = factorial(first);
                double den = factorial(first - second) * factorial(second);

                return num / den;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + ", nCr for n>=r");
            }
        }

        static public int numberOfOperands(string s)
        {
            try
            {
                Double.Parse(s);
                return 0;
            }
            catch (FormatException)
            {
                /*if (MathProcessor.isOperand(s))
                {
                    return 0;
                } */
                switch (s)
                {
                    case "+":
                    case "-":
                    case "x":
                    case "/":
                    case "^":
                    case "P":
                    case "C":
                    case "rt":
                        return 2;

                    case "sin":
                    case "cos":
                    case "tan":
                    case "asin":
                    case "acos":
                    case "atan":
                    case "exp":
                    case "log":
                    case "ln":
                    case SQUAREROOT:
                    case CUBEROOT:
                        return 1;

                    default:
                        throw new Exception("Unknown error");
                }
            }
        }

        static public int operatorLevel(string s)
        {
            switch (s)
            {
                case "(":
                case ")":
                    return 4;
                case "^":
                case CUBED:
                case SQUARED:
                    return 3;
                case "x":
                case "/":
                    return 2;
                case "+":
                case "-":
                    return 1;
                default:
                    return 0;
            }
        }

        static public bool isOperand(string s)
        {
            if (s.ToCharArray()[0].CompareTo('.') == 0 || Char.IsDigit(s.ToCharArray()[0]))
                return true;

            return false;
        }
    }
}
