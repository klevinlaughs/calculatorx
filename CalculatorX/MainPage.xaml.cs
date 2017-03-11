using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CalculatorX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(480, 800);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            this.SizeChanged += MainPage_SizeChanged;
        }

        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 600)
            {
                VisualStateManager.GoToState(this, "Small_Layout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "FullScreen_Layout", true);
            }
        }

        /// <summary>
        /// Calculates a number from the expression typed in textBoxCalcInput
        /// </summary>
        /// <returns></returns>
        private double calculate()
        {
            Queue<string> infix = decomposeString(textBoxCalcInput.Text);
            Queue<string> prefix = infixToPrefix(infix);

            Node head = new Node(prefix.Dequeue(), prefix);

            return head.evaluate();
        }

        /// <summary>
        /// Decomposes the input string into it's components for an infix expression
        /// </summary>
        /// <param name="expression">Mathematical expression in string format</param>
        /// <returns></returns>
        private Queue<string> decomposeString(string expression)
        {
            Queue<string> infix = new Queue<string>();

            string trimmed = textBoxCalcInput.Text.Trim();
            trimmed = trimmed.ToLower();
            char[] chars = trimmed.ToCharArray();

            bool negative = false;

            int i = 0;
            while (i < chars.Length)
            {
                if (Char.IsWhiteSpace(chars[i]))
                {
                    i++;
                    continue;
                }
                if (MathProcessor.isOperand(chars[i].ToString()))
                {
                    string number = "";
                    while (MathProcessor.isOperand(chars[i].ToString()))
                    {
                        number += chars[i].ToString();
                        i++;

                        if (i >= chars.Length) break;
                    }
                    if (number.CompareTo(".") == 0)
                    {
                        throw new Exception(number + " is not a valid number");
                    }
                    number = ((negative) ? "-" : "") + number;
                    infix.Enqueue(number);
                    negative = false;
                }
                else
                {
                    switch (chars[i].ToString())
                    {
                        case "s":
                            infix.Enqueue("sin");
                            infix.Enqueue("(");
                            i += 4;
                            break;

                        case "c":
                            try
                            {
                                if (MathProcessor.isOperand(chars[i + 1].ToString()))
                                {
                                    infix.Enqueue("C");
                                    i++;
                                }
                                else
                                {
                                    infix.Enqueue("cos");
                                    infix.Enqueue("(");
                                    i += 4;
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                throw new Exception("Syntax Error" + e.Message);
                            }

                        case "t":
                            infix.Enqueue("tan");
                            infix.Enqueue("(");
                            i += 4;
                            break;

                        case "a":
                            try
                            {
                                switch (chars[i + 1].ToString())
                                {
                                    case "s":
                                        infix.Enqueue("asin");
                                        infix.Enqueue("(");
                                        i += 5;
                                        break;

                                    case "c":
                                        infix.Enqueue("acos");
                                        infix.Enqueue("(");
                                        i += 5;
                                        break;

                                    case "t":
                                        infix.Enqueue("atan");
                                        infix.Enqueue("(");
                                        i += 5;
                                        break;

                                    default:
                                        throw new Exception("Unknown Operation");
                                }
                                break;
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                throw new Exception("Syntax Error" + e.Message);
                            }

                        case "p":
                            infix.Enqueue("P");
                            i++;
                            break;

                        case "e":
                            infix.Enqueue("exp");
                            infix.Enqueue("(");
                            i += 4;
                            break;

                        case "l":
                            try
                            {
                                if (chars[i + 1].ToString().CompareTo("o") == 0)
                                {
                                    infix.Enqueue("log");
                                    i += 4;
                                }
                                else
                                {
                                    infix.Enqueue("ln");
                                    i += 3;
                                }
                                infix.Enqueue("(");
                                break;
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                throw new Exception("Syntax Error" + e.Message);
                            }

                        case MathProcessor.SUPERSCRIPTN:
                            infix.Enqueue("rt");
                            infix.Enqueue("(");
                            i += 3;
                            break;

                        case MathProcessor.SQUAREROOT:
                        case MathProcessor.CUBEROOT:
                        case "^":
                            infix.Enqueue(chars[i].ToString());
                            infix.Enqueue("(");
                            i += 2;
                            break;

                        case MathProcessor.SQUARED:
                            infix.Enqueue("^");
                            infix.Enqueue("(");
                            infix.Enqueue("2");
                            infix.Enqueue(")");
                            i++;
                            break;
                        case MathProcessor.CUBED:
                            infix.Enqueue("^");
                            infix.Enqueue("(");
                            infix.Enqueue("3");
                            infix.Enqueue(")");
                            i++;
                            break;

                        case "x":
                        case "/":
                        case "(":
                        case ")":
                            infix.Enqueue(chars[i].ToString());
                            i++;
                            break;

                        case "-":
                            if (infix.Count == 0)
                            {
                                negative = true;
                                i++;
                                break;
                            }

                            try
                            {
                                Double.Parse(infix.Last());
                                infix.Enqueue(chars[i].ToString());
                                i++;
                                negative = false;
                            }
                            catch (FormatException)
                            {
                                i++;
                                negative = true;
                            }
                            break;

                        case "+":
                            if (infix.Count == 0)
                            {
                                i++;
                                break;
                            }

                            try
                            {
                                Double.Parse(infix.Last());
                                infix.Enqueue(chars[i].ToString());
                                i++;
                            }
                            catch (FormatException)
                            {
                                i++;
                            }
                            break;

                        case MathProcessor.PI:
                            infix.Enqueue(Math.PI.ToString());
                            i++;
                            break;

                        default: throw new Exception("Currently Unsupported");
                    }
                }
            }

            return infix;
        }

        /// <summary>
        /// Converts infix expression to prefix : reverse infix expression, convert reverse expression to postfix, then reverse postfix.
        /// </summary>
        /// <param name="infix">infix expression implemented as a Queue</param>
        /// <returns></returns>
        private Queue<string> infixToPrefix(Queue<string> infix)
        {
            Queue<string> postfix = new Queue<string>();
            Queue<string> prefix;

            Stack<string> operatorStack = new Stack<string>();

            Queue<string> reverse = new Queue<string>(infix.Reverse());

            string current;

            while (reverse.Count > 0)
            {
                current = reverse.Dequeue();

                try
                {
                    Double.Parse(current);
                    postfix.Enqueue(current);
                }
                catch (FormatException)
                {
                    //if (MathProcessor.isOperand(current))
                    //{
                    //operandStack.Push(reverse.Dequeue());

                    //}

                    if (current.CompareTo("(") == 0)
                    {
                        while (operatorStack.Peek().CompareTo(")") != 0)
                        {
                            postfix.Enqueue(operatorStack.Pop());
                        }
                        operatorStack.Pop();
                    }

                    else if (current.CompareTo(")") == 0 || operatorStack.Count == 0 || MathProcessor.operatorLevel(operatorStack.Peek()) < MathProcessor.operatorLevel(current))
                    {
                        operatorStack.Push(current);
                    }

                    else if (MathProcessor.operatorLevel(operatorStack.Peek()) >= MathProcessor.operatorLevel(current))
                    {
                        //   while (operatorStack.Count > 0 && operatorLevel(operatorStack.Peek()) >= operatorLevel(current))
                        //  {
                        string top = operatorStack.Peek();

                        if (MathProcessor.operatorLevel(top) < MathProcessor.operatorLevel("("))
                        {
                            postfix.Enqueue(operatorStack.Pop());
                        }
                        //   }
                        operatorStack.Push(current);
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                postfix.Enqueue(operatorStack.Pop());
            }

            prefix = new Queue<string>(postfix.Reverse());
            return prefix;
        }

        private void appendInput(string s)
        {
            if (textBoxCalcInput.Text.Count() == 0)
            {
                textBoxCalcInput.Text += s;
                return;
            }
            if (s.CompareTo(textBoxCalcInput.Text.Last().ToString()) != 0)
            {
                textBoxCalcInput.Text += s;
            }
        }

        private void preAppendX()
        {
            if (textBoxCalcInput.Text.CompareTo("") == 0) return;
            if (MathProcessor.isOperand(textBoxCalcInput.Text.Last().ToString()))
            {
                textBoxCalcInput.Text += "x";
            }
        }

        //===============================================================================
        private async void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxCalcInput.Text.Count() == 0) return;
            try
            {
                double ans = calculate();
                textBlockLastInput.Text = textBoxCalcInput.Text;
                ans = Math.Round(ans, 11);
                textUserData.Text = ans.ToString();
                textBoxCalcInput.Text = "";
            }
            catch (Exception ex)
            {
                textUserData.Text = "Error";
                var popup = new Windows.UI.Popups.MessageDialog(ex.Message + "\n" + ex.StackTrace);
                await popup.ShowAsync();
            }

        }

        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "0";
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "1";
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "2";
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "3";
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "4";
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "5";
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "6";
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "7";
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "8";
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "9";
        }

        private void btnDecimal_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxCalcInput.Text.Last().CompareTo('.') != 0)
            {
                textBoxCalcInput.Text += ".";
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            int stringLength = textBoxCalcInput.Text.Length;
            if (stringLength > 0)
            {
                textBoxCalcInput.Text = textBoxCalcInput.Text.Remove(stringLength - 1);
            }
        }

        private void btnAC_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxCalcInput.Text.Length > 0)
            {
                textBoxCalcInput.Text = "";
            }
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            appendInput("+");
        }

        private void btnSub_Click(object sender, RoutedEventArgs e)
        {
            appendInput("-");
        }

        private void btnMult_Click(object sender, RoutedEventArgs e)
        {
            appendInput("x");
        }

        private void btnDiv_Click(object sender, RoutedEventArgs e)
        {
            appendInput("/");
        }

        private void btnSin_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "sin(";
        }

        private void btnCos_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "cos(";
        }

        private void btnTan_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "tan(";
        }

        private void btnAsin_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "asin(";
        }

        private void btnAcos_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "acos(";
        }

        private void btnAtan_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "atan(";
        }

        private void btnRoot_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text = "(" + textBoxCalcInput.Text + ")" + MathProcessor.SUPERSCRIPTN + MathProcessor.SQUAREROOT + "(";
        }

        private void btnPower_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "^(";
        }

        private void btnCubeRoot_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += (MathProcessor.CUBEROOT + "(");
        }

        private void btnPerm_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "P";
        }

        private void btnComb_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "C";
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "log(";
        }

        private void btn10x_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += "x10^(";
        }

        private void btnLn_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "ln(";
        }

        private void btnLeftBrace_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "(";
        }

        private void btnRightBrace_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += ")";
        }

        private void btnCubed_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += MathProcessor.CUBED;
        }

        private void btnExp_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += "exp(";
        }

        private void btnSquareRoot_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += (MathProcessor.SQUAREROOT + "(");
        }

        private void btnSquare_Click(object sender, RoutedEventArgs e)
        {
            textBoxCalcInput.Text += MathProcessor.SQUARED;
        }

        private void btnAns_Click(object sender, RoutedEventArgs e)
        {
            foreach (char c in textUserData.Text)
            {
                if (!MathProcessor.isOperand(c.ToString()))
                {
                    return;
                }
            }

            textBoxCalcInput.Text += textUserData.Text;
        }

        private void btnPi_Click(object sender, RoutedEventArgs e)
        {
            preAppendX();
            textBoxCalcInput.Text += MathProcessor.PI;
        }


        //private void OnTheFly(object sender, TextChangedEventArgs e)
        //{
        //    txtUserData.Text = txtBoxCalculateInput.Text;
        //}

        //private void OnTheFly(TextBox sender, TextBoxTextChangingEventArgs args)
        //{
        //    txtUserData.Text = txtBoxCalculateInput.Text;
        //}
    }
}
