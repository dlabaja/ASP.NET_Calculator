using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public class Calculator
{
    private string _result = "";
    private const string Blacklist = "()+*/-";
    private bool _mathError;

    private readonly Dictionary<char, Func<double, double, double>> _pairResult =
        new Dictionary<char, Func<double, double, double>>
        {
            {'-', (num1, num2) => num1 - num2},
            {'+', (num1, num2) => num1 + num2},
            {'*', (num1, num2) => num1 * num2},
            {'/', (num1, num2) => num1 / num2}
        };

    public string Calculate(string query)
    {
        _result = query;
        _result = _result.Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));
        _result = _result.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture));

        /*try
        {*/
            while (_result.Any(c => "+-*/".Contains(c)))
            {
                for (int i = 0; i < _result.Length; i++)
                {
                    if (!"*/".Contains(_result[i])) continue;
                    CalculatePair(_result[i], i);
                }

                for (int i = 0; i < _result.Length; i++)
                {
                    if (!"+-".Contains(_result[i])) continue;
                    CalculatePair(_result[i], i);
                }

                if (_result.Count(x => x == '-') == 1 && _result[0] == '-') break;
            }
        /*}
        catch
        {
            return "Math ERROR";
        }*/

        return _mathError ? "Math ERROR" : _result;
    }

    private void CalculatePair(char symbol, int index)
    {
        var _pair = FindPair(symbol, index);
        if (_pair == null) return;
        var pair = _pair.GetValueOrDefault();
        _result = _result.Replace(
            $"{pair.Num1.ToString(CultureInfo.InvariantCulture)}{pair.Symbol}{pair.Num2.ToString(CultureInfo.InvariantCulture)}",
            _pairResult[symbol](pair.Num1, pair.Num2).ToString(CultureInfo.InvariantCulture));
        _result = Regex.Replace(_result, @"\(([-+]?\d+(\.\d+)?)\)", "$1");
        _result = Regex.Replace(_result, @"([+-])\s*([+-])", m =>
        {
            if (m.Groups[1].Value == "+")
            {
                return m.Groups[2].Value;
            }

            return "-" + m.Groups[2].Value;
        });
    }

    private NumberPair? FindPair(char symbol, int index)
    {
        if (index == 0 || "()".Contains(_result[index - 1]) || "()".Contains(_result[index + 1])) return null;
        var pair = new NumberPair(FindNum1(index), FindNum2(index), symbol);
        if (("*/".Contains(_result.ElementAtOrDefault(index + pair.Num2.ToString().Length + 1)) ||
            "*/".Contains(_result.ElementAtOrDefault(index - pair.Num1.ToString().Length - 1))) && "-+".Contains(symbol))
            return null;
        if (symbol == '/' && pair.Num2 == 0) _mathError = true;
        return pair;
    }

    private double FindNum2(int index)
    {
        // finding num2 until bracket/symbol is met
        for (int i = index + 1; i < _result.Length; i++)
        {
            if (!Blacklist.Contains(_result[i])) continue;
            return double.Parse(_result.Substring(index + 1, Math.Abs(index - i) - 1), CultureInfo.InvariantCulture);
        }

        return double.Parse(_result.Substring(index + 1, _result.Length - index - 1), CultureInfo.InvariantCulture);
    }

    private double FindNum1(int index)
    {
        // finding num1 until bracket/symbol is met, checking if it's negative
        for (int i = index - 1; i >= 0; i--)
        {
            if (_result[i] == '-') return double.Parse(_result.Substring(i, index - i), CultureInfo.InvariantCulture);
            if (!Blacklist.Contains(_result[i])) continue;
            return double.Parse(_result.Substring(i + 1, index - (i + 1)), CultureInfo.InvariantCulture);
        }

        return double.Parse(_result.Substring(0, index), CultureInfo.InvariantCulture);
    }


    private struct NumberPair
    {
        public double Num1;
        public double Num2;
        public char Symbol;

        public NumberPair(double num1, double num2, char symbol)
        {
            Num1 = num1;
            Num2 = num2;
            Symbol = symbol;
        }
    }
}