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
                throw new InvalidOperationException(string.Format("{0} is not set in context: {1}", itemName, ctx));
            return (TItem)itemValue;
        }
    }
}