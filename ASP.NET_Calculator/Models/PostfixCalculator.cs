using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ASP.NET_Calculator.Models
{
    public class PostfixCalculator
    {
        private readonly Stack<double> _buffer = new Stack<double>();
        private Dictionary<string, Func<double>> _operations;

        private void InitDict() => _operations = new Dictionary<string, Func<double>>
        {
            {"+", () => _buffer.Pop() + _buffer.Pop()},
            {
                "-", () =>
                {
                    var num2 = _buffer.Pop();
                    var num1 = _buffer.Pop();
                    return num1 - num2;
                }
            },
            {"*", () => _buffer.Pop() * _buffer.Pop()},
            {
                "/", () =>
                {
                    var num2 = _buffer.Pop();
                    var num1 = _buffer.Pop();
                    if (num2 == 0) throw new Exception("Cannot divide by zero");
                    return num1 / num2;
                }
            },
            {
                "%", () =>
                {
                    var num2 = _buffer.Pop();
                    var num1 = _buffer.Pop();
                    if (num2 == 0) throw new Exception("Cannot modulo by zero");
                    return num1 % num2;
                }
            },
            {"sin", () => Math.Sin(_buffer.Pop())},
            {"cos", () => Math.Cos(_buffer.Pop())},
            {"tg", () => Math.Tan(_buffer.Pop())},
            {
                "cotg", () =>
                {
                    var num = _buffer.Pop();
                    if (num == 0) throw new Exception("Cotg() cannot be zero");
                    return Math.Cos(num) / Math.Sin(num);
                }
            },
            {"pow", () => Math.Pow(_buffer.Pop(), 2)},
            {
                "sqrt", () =>
                {
                    var num = _buffer.Pop();
                    if (num < 0) throw new Exception("Sqrt() cannot be < 0");
                    return Math.Sqrt(num);
                }
            },
            {
                "log", () =>
                {
                    var num = _buffer.Pop();
                    if (num <= 0) throw new Exception("Log() cannot be ≤ 0");
                    return Math.Log10(num);
                }
            },
            {
                "ln", () =>
                {
                    var num = _buffer.Pop();
                    if (num <= 0) throw new Exception("Ln() cannot be ≤ 0");
                    return Math.Log(num);
                }
            },
            {"abs", () => Math.Abs(_buffer.Pop())},
            {"π", () => Math.PI},
            {"e", () => Math.E}
        };

        public double CalculatePostfix(IEnumerable<dynamic> commands)
        {
            InitDict();
            foreach (var item in commands)
                try
                {
                    _buffer.Push(_operations.ContainsKey(Convert.ToString(item))
                        ? (double) _operations[Convert.ToString(item)]()
                        : (double) Convert.ToDouble(item));
                }
                catch (InvalidOperationException)
                {
                    throw new Exception("Invalid syntax");
                }
                catch (FormatException)
                {
                    throw new Exception("Invalid syntax");
                }

            if (_buffer.Count > 1) throw new Exception("Invalid syntax"); // more than one number in result

            return _buffer.Pop();
        }

        public IEnumerable<dynamic> InfixToPostfix(StringBuilder infix)
        {
            if (string.IsNullOrEmpty(infix.ToString())) throw new Exception("Empty expression");

            InitDict();
            var postfix = new List<dynamic>();
            var stack = new Stack<string>();

            //replace constants
            infix = infix.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture));
            infix = infix.Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

            // fix for negative numbers -> adds 0 if '-' is after '(' or at the start of infix 
            if (infix[0] == '-')
                infix.Insert(0, '0');
            for (var i = 1; i < infix.Length; i++)
            {
                if (infix[i] == '-' && infix[i - 1] == '(')
                    infix.Insert(i, '0');
            }

            foreach (Match item in Regex.Matches(infix.ToString(), @"[+\%\-\*\/()]|\d*\.?\d+|[a-z]+"))
            {
                if (item.Value == "(") // start of stack region
                    stack.Push("(");
                else if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.InvariantCulture,
                             out var d)) // number
                    postfix.Add(d);
                else if (item.Value == ")") // pushes all of stack to postfix until '(' is met
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                        postfix.Add(stack.Pop());
                    if (stack.Count > 0) stack.Pop();
                }
                else // pushes operator/function to stack by precedence
                {
                    while (stack.Count > 0 && stack.Peek() != "(" &&
                           GetPrecedence(stack.Peek()) >= GetPrecedence(item.ToString()))
                        postfix.Add(stack.Pop());
                    stack.Push(item.Value);
                }
            }

            while (stack.Count > 0) // empty stack
                postfix.Add(stack.Pop());

            return postfix.ToArray();
        }

        private static int GetPrecedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                case "%":
                    return 2;
                default:
                    return 3;
            }
        }
    }
}