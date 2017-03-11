using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorX
{
    class Node
    {
        Node left;
        Node right;

        string expression;

        public Node(string current, Queue<string> prefix)
        {
            expression = current;

            int numOperands = MathProcessor.numberOfOperands(current);

            switch (numOperands)
            {
                case 0:
                    left = null;
                    right = null;
                    break;
                case 1:
                    left = new Node(prefix.Dequeue(), prefix);
                    right = null;
                    break;
                case 2:
                    left = new Node(prefix.Dequeue(), prefix);
                    right = new Node(prefix.Dequeue(), prefix);
                    break;
                default:
                    throw new Exception("Unknown error");
            }
        }

        public bool isLeaf()
        {
            return (left == null && right == null);
        }

        public double evaluate()
        {
            if (isLeaf())
            {
                return Double.Parse(expression);
            }
            else
            {
                switch (expression)
                {
                    case "+": return left.evaluate() + right.evaluate();
                    case "-": return left.evaluate() - right.evaluate();
                    case "x": return left.evaluate() * right.evaluate();
                    case "/": return left.evaluate() / right.evaluate();
                    case "^": return Math.Pow(left.evaluate(), right.evaluate());
                    case "P": return MathProcessor.perm(left.evaluate(), right.evaluate());
                    case "C": return MathProcessor.comb(left.evaluate(), right.evaluate());
                    case "rt": return Math.Pow(right.evaluate(), 1f / left.evaluate());

                    case "sin": return Math.Sin(left.evaluate());
                    case "cos": return Math.Cos(left.evaluate());
                    case "tan": return Math.Tan(left.evaluate());
                    case "asin": return Math.Asin(left.evaluate());
                    case "acos": return Math.Acos(left.evaluate());
                    case "atan": return Math.Atan(left.evaluate());
                    case MathProcessor.CUBEROOT: return Math.Pow(left.evaluate(), 1f / 3f);
                    case MathProcessor.SQUAREROOT: return Math.Pow(left.evaluate(), 1f / 2f);

                    case "exp": return Math.Exp(left.evaluate());
                    case "log": return Math.Log10(left.evaluate());
                    case "ln": return Math.Log(left.evaluate(), Math.E);

                    default: throw new Exception("Node cannot evaluate");
                }


            }
        }
    }
}
