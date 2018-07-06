namespace RS
{
    /// <summary>
    /// Provides indexed elements.
    /// </summary>
    /// <typeparam name="T">The type of element being provided.</typeparam>
    interface IndexedProvider<T>
    {
        /// <summary>
        /// Provides the element at the provided index.
        /// </summary>
        /// <param name="index">The index of the element to provide.</param>
        /// <returns>The element at the provided index.</returns>
        T Provide(int index);
    }
}
