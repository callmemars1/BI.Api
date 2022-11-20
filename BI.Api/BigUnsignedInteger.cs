using System.Text;

namespace BI.Api;

public class BigUnsignedInteger
{
    private LinkedList<byte> _digits;
    
    private static bool IsValidIntegerString(string value)
    {
        if (value == string.Empty)
            return false;

        if (value.Any(c => char.IsDigit(c) == false))
            return false;
        
        if (value.StartsWith('0') && value.Length > 1)
            return false;

        return true;
    }

    private static bool TryParseDigits(string value, out LinkedList<byte> digits)
    {
        digits = null!;
        if (IsValidIntegerString(value) == false) 
            return false;

        digits = new LinkedList<byte>();
        foreach (var digit in value) digits.AddLast((byte)(digit - '0'));
        return true;
    }

    public BigUnsignedInteger(string value)
    {
        if (TryParseDigits(value, out _digits) == false)
            throw new ArgumentException("Not valid integer");
    }

    private BigUnsignedInteger()
    {
        _digits = new LinkedList<byte>();
    }

    public static BigUnsignedInteger operator +(BigUnsignedInteger a, BigUnsignedInteger b)
    {
        var result = new BigUnsignedInteger();
        var resultDigits = result._digits;

        var aSize = a._digits.Count;
        var bSize = b._digits.Count;

        var smallestLength = aSize < bSize ? aSize : bSize;
        var aNode = a._digits.Last;
        var bNode = b._digits.Last;
        byte carry = 0;


        while (aNode is not null && bNode is not null) // Пока оба узла не null
        {
            var digit = aNode.Value + bNode.Value + carry;
            carry = (byte)(digit / 10);
            digit %= 10;
            result._digits.AddFirst((byte)digit);

            aNode = aNode?.Previous;
            bNode = bNode?.Previous;
        }

        if (aNode is null && bNode is null) // Если оба узла null (одинаковое число разрядов)
        {
            if (carry != 0) resultDigits.AddFirst(carry);
            return result;
        }

        var notNullNode = aNode ?? bNode; // Если мы попали сюда, значит только один из узлов null
        resultDigits.AddFirst((byte)(notNullNode!.Value + carry));
        notNullNode = notNullNode.Previous;

        while (notNullNode is not null) // добавляем оставшиеся разряды
        {
            resultDigits.AddFirst(notNullNode.Value);
            notNullNode = notNullNode.Previous;
        }

        return result;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var digit in _digits)
            builder.Append(digit);

        return builder.ToString();
    }

    public static bool TryParse(string value, out BigUnsignedInteger result)
    {
        result = new BigUnsignedInteger();

        if (TryParseDigits(value, out result._digits) == false)
            return false;

        return true;
    }
}