using System;
using System.Collections.Generic;
using System.Linq;

public class Calculator
{
    private string _result = "";
    private const string Blacklist = "()+*/-";

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
        _result = _result.Replace("e", Math.E.ToString());
        _result = _result.Replace("π", Math.PI.ToString());

        for (int i = 0; i < _result.Length; i++)
        {
            if (!"*/".Contains(_result[i])) continue;
            CalculatePair(_result[i]);
        }

        for (int i = 0; i < _result.Length; i++)
        {
            if (!"+-".Contains(_result[i])) continue;
            CalculatePair(_result[i]);
        }

        return _result;
    }

    private void CalculatePair(char symbol)
    {
        var pair = FindPair(symbol);
        _result = _result.Replace($"{pair.num1}{pair.symbol}{pair.num2}",
            _pairResult[symbol](pair.num1, pair.num2).ToString());
    }

    private NumberPair FindPair(char symbol)
    {
        var pair = new NumberPair
        {
            symbol = symbol
        };
        var index = _result.IndexOf(symbol);
        pair.num1 = FindNum1(index);
        pair.num2 = FindNum2(index);
        return pair;
    }

    private double FindNum2(int index)
    {
        //finding num2 until bracket/symbol is met
        for (int i = index + 1; i < _result.Length; i++)
        {
            if (!Blacklist.Contains(_result[i])) continue;
            return double.Parse(_result.Substring(index + 1,  Math.Abs(index - i) - 1));
        }

        return double.Parse(_result.Substring(index + 1, _result.Length - index - 1));
    }

    private double FindNum1(int index)
    {
        //finding num1 until bracket/symbol is met, check if it's negative
        for (int i = index - 1; i >= 0; i--)
        {
            if (_result[i] == '-') return double.Parse(_result.Substring(i, index));
            if (!Blacklist.Contains(_result[i])) continue;
            return double.Parse(_result.Substring(i, index - i));
        }

        return double.Parse(_result.Substring(0, index));
    }

    private struct NumberPair
    {
        public double num1;
        public double num2;
        public char symbol;
    }
}