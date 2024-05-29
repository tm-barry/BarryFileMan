namespace BarryFileMan.Rename.Extensions
{
    public static class HashExtensions
    {
        public static int GetSequenceHashCode<T>(this IList<T> sequence) where T : notnull
        {
            const int seed = 487;
            const int modifier = 31;

            unchecked
            {
                return sequence.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
            }
        }
    }
}
