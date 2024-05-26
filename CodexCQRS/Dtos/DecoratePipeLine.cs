namespace CodexCQRS.Dtos
{
    internal class DecoratePipeLine : IEquatable<DecoratePipeLine>
    {
        public DecoratePipeLine() { }

        public int Order { get; init; }

        public Type DecoratorType { get; init; }

        public bool Equals(DecoratePipeLine? other)
        {
            if (other is null)
                return false;

            return Order == other.Order && DecoratorType == other.DecoratorType;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || !(obj is DecoratePipeLine other))
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Order.GetHashCode() ^ (DecoratorType?.GetHashCode() ?? 0);
        }
    }

    internal class DecorateAfterPipeLine : DecoratePipeLine
    {
        public DecorateAfterPipeLine() { }    
    }

    internal class DecorateBeforePipeLine : DecoratePipeLine
    { 
        public DecorateBeforePipeLine() { }
    }
}
