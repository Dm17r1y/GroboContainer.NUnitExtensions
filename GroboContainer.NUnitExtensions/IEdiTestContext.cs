using GroboContainer.Core;

using JetBrains.Annotations;

namespace GroboContainer.NUnitExtensions
{
    public interface IEdiTestContext
    {
        [NotNull]
        IContainer Container { get; }

        bool TryGetContextItem([NotNull] string itemName, out object itemValue);
    }
}