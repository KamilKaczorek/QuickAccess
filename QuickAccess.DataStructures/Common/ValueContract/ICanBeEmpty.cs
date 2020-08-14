namespace QuickAccess.DataStructures.Common.ValueContract
{
    /// <summary>
    /// The interface of an item that can be empty.
    /// </summary>
    public interface ICanBeEmpty
    {
        /// <summary>
        /// Gets the value indicating whether this instance is empty.
        /// </summary>
        bool IsEmpty { get;  }
    }
}