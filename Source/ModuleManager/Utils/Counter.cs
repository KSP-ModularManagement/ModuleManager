using System;

namespace ModuleManager.Utils
{
    public class Counter
    {
        public int Value { get; protected set; } = 0;

        public void Increment()
        {
            Value++;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator int(Counter counter) => counter.Value;
    }

    public class SetableCounter:Counter
    {
        public void Set(int value) { this.Value = value; }

        public static implicit operator int(SetableCounter counter) => counter.Value;
    }
}
