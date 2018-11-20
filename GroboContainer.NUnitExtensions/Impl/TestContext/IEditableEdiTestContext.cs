using JetBrains.Annotations;

namespace GroboContainer.NUnitExtensions.Impl.TestContext
{
    public interface IEditableEdiTestContext : IEdiTestContext
    {
        void AddItem([NotNull] string itemName, [NotNull] object itemValue);
        bool RemoveItem([NotNull] string itemName);
    }
}