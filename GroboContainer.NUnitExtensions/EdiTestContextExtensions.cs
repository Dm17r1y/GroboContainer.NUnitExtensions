using System;

using JetBrains.Annotations;

namespace GroboContainer.NUnitExtensions
{
    public static class EdiTestContextExtensions
    {
        [NotNull]
        public static TItem GetContextItem<TItem>([NotNull] this IEdiTestContext ctx, [NotNull] string itemName)
        {
            object itemValue;
            if (!ctx.TryGetContextItem(itemName, out itemValue) || itemValue == null)
                throw new InvalidOperationException($"{itemName} is not set in context: {ctx}");
            return (TItem)itemValue;
        }
    }
}