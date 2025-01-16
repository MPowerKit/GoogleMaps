namespace MPowerKit.GoogleMaps;

/// <summary>
/// Extends the IDictionary interface to encompass the TryXxxx operations
/// </summary>
public interface IDictionaryEx<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
{
    /// <summary>
    /// Adds a key/value pair to the  <see cref="T:System.Collections.Generic.IDictionary`2"/> if the key does not already exist.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value to be added, if the key does not already exist.</param>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    TValue GetOrAdd(TKey key, TValue value);

    /// <summary>
    /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    bool TryAdd(TKey key, TValue value);

    /// <summary>
    /// Updates an element with the provided key to the value if it exists.
    /// </summary>
    /// <returns>Returns true if the key provided was found and updated to the value.</returns>
    /// <param name="key">The object to use as the key of the element to update.</param>
    /// <param name="value">The new value for the key if found.</param>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    bool TryUpdate(TKey key, TValue value);

    /// <summary>
    /// Updates an element with the provided key to the value if it exists.
    /// </summary>
    /// <returns>Returns true if the key provided was found and updated to the value.</returns>
    /// <param name="key">The object to use as the key of the element to update.</param>
    /// <param name="value">The new value for the key if found.</param>
    /// <param name="comparisonValue">The value that is compared to the value of the element with key.</param>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    bool TryUpdate(TKey key, TValue value, TValue comparisonValue);

    /// <summary>
    /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </summary>
    /// <returns>
    /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </returns>
    /// <param name="key">The key of the element to remove.</param>
    /// <param name="value">The value that was removed.</param>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    bool TryRemove(TKey key, out TValue value);
}