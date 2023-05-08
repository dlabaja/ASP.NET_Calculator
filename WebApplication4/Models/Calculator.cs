using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.WebPages;

public class Calculator
{
    private string _result = "";
    private bool _mathError;

    private readonly Dictionary<char, Func<double, double, double>> _genResults;
    private readonly Dictionary<string, Func<double, double>> _funcResults;

    public Calculator()
    {
        _genResults = new Dictionary<char, Func<double, double, double>>
        {
            {'-', (num1, num2) => num1 - num2},
            {'+', (num1, num2) => num1 + num2},
            {'*', (num1, num2) => num1 * num2},
            {
                '/', (num1, num2) =>
                {
                    if (num2 == 0) _mathError = true;
                    return num1 / num2;
                }
            }
        };

        _funcResults = new Dictionary<string, Func<double, double>>
        {
            {"sin", Math.Sin},
            {"cos", Math.Cos},
            {"tg", Math.Tan},
            {"cotg", val => Math.Cos(val) / Math.Sin(val)},
            {"pow", val => Math.Pow(val, 2)},
            {
                "sqrt", val =>
                {
                    if (val < 0) _mathError = true;
                    return Math.Sqrt(val);
                }
            },
            {
                "log", val =>
                {
                    if (val <= 0) _mathError = true;
                    return Math.Log10(val);
                }
            },
            {"abs", Math.Abs},
            {
                "ln", val =>
                {
                    if (val <= 0) _mathError = true;
                    return Math.Log(val);
                }
            },
        };
    }

    public string Calculate(string query)
    {
        _result = query;
        _result = _result.Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));
        _result = _result.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture));

        for (int _ = 0; _ < 10; _++)
        {
            Funcs();
            ReplaceSignsAndBrackets();
            MulDiv();
            AddSub();
        }

        return _mathError ? "Math ERROR" : _result;
    }

    private void Funcs()
    {
        foreach (Match match in new Regex(@"([a-z]+)\(([+-]?\d*\.?\d+)\)").Matches(_result)) //regex for */ number pairs
        {
            var func = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            //replaces pair in string by calculated number 
            _result = _result.Replace(
                $"{match}",
                $"+{_funcResults[func](double.Parse(value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture)}");
        }
    }

    private void MulDiv()
    {
        foreach (Match match in
                 new Regex(@"([+-]?\d*\.?\d+)([*/])([+-]?\d*\.?\d+)").Matches(_result)) //regex for */ number pairs
        {
            var first = match.Groups[1].Value;
            var sign = match.Groups[2].Value.ToCharArray().FirstOrDefault();
            var second = match.Groups[3].Value;

            if (first.IsEmpty() || sign == default || second.IsEmpty()) continue;

            //replaces pair in string by calculated number 
            _result = _result.Replace(
                $"{match}",
                $"+{_genResults[sign](double.Parse(first, CultureInfo.InvariantCulture), double.Parse(second, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture)}");
        }
    }

    private void AddSub()
    {
        foreach (Match match in
                 new Regex(@"([+-]?\d*\.?\d+)([+-])([+-]?\d*\.?\d+)").Matches(_result)) //regex for +- number pairs
        {
            var first = match.Groups[1].Value;
            var sign = match.Groups[2].Value.ToCharArray().FirstOrDefault();
            var second = match.Groups[3].Value;

            if (first.IsEmpty() || sign == default || second.IsEmpty()) continue;

            //replaces pair in string by calculated number 
            _result = _result.Replace(
                $"{match}",
                $"+{_genResults[sign](double.Parse(first, CultureInfo.InvariantCulture), double.Parse(second, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture)}");
        }
    }

    private void ReplaceSignsAndBrackets()
    {
        _result = Regex.Replace(_result, @"(?<!\w)\(([-+]?\d+(\.\d+)?)\)",
            "$1"); //replace useless brackets (not the function ones)
        _result = Regex.Replace(_result, @"([+-])([+-])", m => //replace double signs
        {
            var num1 = int.Parse($"{m.Groups[1].Value}1");
            var num2 = int.Parse($"{m.Groups[2].Value}1");
            if (num1 * num2 == 1) return "+";
            return "-";
        });
    }
}