using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web.Helpers;

public class Calculator
{
	private string result = "";
	private Dictionary<char, Func<int, int, int>> strPairToResult = new Dictionary<char, Func<int, int, int>>()
	{
		{'-', (int num1, int num2) => num1-num2 },
		{'+', (int num1, int num2) => num1+num2 }
	};

	public string Calculate(string query)
	{
		result = query;
		try
		{
			CalculateStringToResult('-');
			CalculateStringToResult('+');
		}
		catch { }
		return result;
	}

	private void CalculateStringToResult(char symbol)
	{
		var match = new Regex($@"(\d+)\{symbol}(\d+)").Match(result); // finds pair with a given symbol
		var pair = match.ToString().Split(symbol);
		var funcResult = 0;

		if (!match.Success) return;
		if (match.Index >= 1 && result[match.Index - 1] == '-')
		{
			funcResult = strPairToResult[symbol](int.Parse($"-{pair[0]}"), int.Parse(pair[1]));
			result = result.Replace("-" + match.Value, funcResult.ToString());
			return;
		}

		funcResult = strPairToResult[symbol](int.Parse(pair[0]), int.Parse(pair[1]));
		result = result.Replace(match.Value, funcResult.ToString());
	}
}