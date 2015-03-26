using System;
using System.Collections.Generic;
using System.Linq;

using DiadocSys.Core.Json;

using JetBrains.Annotations;

using SKBKontur.Catalogue.Objects;
using SKBKontur.Catalogue.ServiceLib.Logging;

namespace SKBKontur.Catalogue.NUnit.Extensions.EdiTestMachinery.Impl.TestContext
{
    public abstract class EdiTestContextData : IEdiTestContextData
    {
        protected EdiTestContextData()
        {
            items = new Dictionary<string, ItemValueHolder>();
        }

        public void AddItem([NotNull] string itemName, [NotNull] object itemValue)
        {
            if(items.ContainsKey(itemName))
                throw new InvalidProgramStateException(string.Format("Item with the same name is already added: {0}", itemName));
            items.Add(itemName, new ItemValueHolder(items.Count, itemValue));
        }

        [NotNull]
        public object GetItem([NotNull] string itemName)
        {
            object itemValue;
            if(!TryGetItem(itemName, out itemValue) || itemValue == null)
                throw new InvalidProgramStateException(string.Format("Item is not set: {0}", itemName));
            return itemValue;
        }

        public bool TryGetItem([NotNull] string itemName, out object itemValue)
        {
            itemValue = null;
            ItemValueHolder holder;
            var result = items.TryGetValue(itemName, out holder);
            if(result)
                itemValue = holder.ItemValue;
            return result;
        }

        public bool RemoveItem([NotNull] string itemName)
        {
            return items.Remove(itemName);
        }

        public void Destroy()
        {
            foreach(var kvp in items.OrderByDescending(x => x.Value.Order))
            {
                var disposableItem = kvp.Value.ItemValue as IDisposable;
                if(disposableItem != null)
                    TryDisposeItem(kvp.Key, disposableItem);
            }
            items.Clear();
        }

        private void TryDisposeItem([NotNull] string itemName, [NotNull] IDisposable disposableItem)
        {
            try
            {
                disposableItem.Dispose();
            }
            catch(Exception e)
            {
                Log.For(this).Fatal(string.Format("Failed to dispose item: {0}", itemName), e);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", items.ToPrettyJson());
        }

        private readonly Dictionary<string, ItemValueHolder> items;

        private class ItemValueHolder
        {
            public ItemValueHolder(int order, [NotNull] object itemValue)
            {
                Order = order;
                ItemValue = itemValue;
            }

            public int Order { get; private set; }

            [NotNull]
            public object ItemValue { get; private set; }
        }
    }
}