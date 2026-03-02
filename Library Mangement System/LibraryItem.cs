using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal abstract class LibraryItem
    {
        public string ItemId { get; }
        public string Title { get; }

        protected LibraryItem(string itemId, string title)
        {
            ItemId = itemId;
            Title = title;
        }

        public abstract string GetSummary();
        public abstract bool IsAvailable();

    }
}
