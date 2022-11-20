using Microsoft.AspNetCore.Mvc;

namespace BI.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BIController : ControllerBase
{
    [HttpGet("is_palindrome")]
    public IActionResult IsPalindrome([FromQuery] string str)
    {
        if (str.Length is 0 or 1) return new JsonResult(new { IsPalindrome = true });

        var isPalindrome = true;
        for (var i = 0; i < str.Length / 2; i++)
        {
            if (str[i] == str[^(i + 1)]) continue;

            isPalindrome = false;
            break;
        }

        return new JsonResult(new { IsPalindrome = isPalindrome });
    }

    [HttpPost("accumulate")]
    public IActionResult Accumulate([FromBody] IEnumerable<int> array)
    {
        long sum = 0;
        var addNextOdd = false;
        foreach (var element in array)
        {
            if ((element & 1) != 1) continue;

            sum += addNextOdd ? element : 0;
            addNextOdd = !addNextOdd;
        }

        return new JsonResult(new { Sum = Math.Abs(sum) });
    }

    [HttpPost("bigint_sum")]
    public IActionResult BigIntSum([FromBody] IEnumerable<string> summands)
    {
        List<BigUnsignedInteger> integers = new() { Capacity = summands.Count() };
        List<string> invalidIntegers = new();
        foreach (var summand in summands)
        {
            if (BigUnsignedInteger.TryParse(summand, out var integer) == false)
                invalidIntegers.Add(summand);
            else
                integers.Add(integer);
        }

        if (invalidIntegers.Count != 0)
            return new JsonResult(new
            {
                Error = "Invalid integers were passed",
                InvalidIntegers = invalidIntegers
            }) { StatusCode = StatusCodes.Status400BadRequest };

        var result = integers.Aggregate((a, b) => a + b);
        return new JsonResult(new { Result = result.ToString() });
    }
}