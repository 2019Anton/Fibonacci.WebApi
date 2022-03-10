using System.Numerics;
using System.Text.Json.Serialization;

namespace Fibonacci.Logic.Models
{
    public class FibonacciNumber
    {
        public FibonacciNumber(int index, BigInteger value)
        {
            Index = index;
            Value = value;
        }

        public int Index { get; set; }

        [JsonIgnore]
        public BigInteger Value { get; set; }

        [JsonPropertyName("Value")]
        public string StringValue => Value.ToString();

        public int ThreadId { get; set; }
    }
}