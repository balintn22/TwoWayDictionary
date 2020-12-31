using System;
using System.Collections.Generic;

namespace TwoWayDict
{
    /// <summary>
    /// Implements a two-way dictionary, providing efficient lookup both ways.
    /// Maintains consistency across additions and updates.
    /// Actually uses two dictionaries for lookups either way.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TwoWayDictionary<TKey, TValue>
    {
        private object _lockObject;
        internal Dictionary<TKey, TValue> _forwardDict;
        internal Dictionary<TValue, TKey> _reverseDict;

        public TwoWayDictionary()
        {
            _lockObject = new object();
            _forwardDict = new Dictionary<TKey, TValue>();
            _reverseDict = new Dictionary<TValue, TKey>();
        }

        public bool ContainsKey(TKey key) => _forwardDict.ContainsKey(key);
        public bool ContainsValue(TValue key) => _reverseDict.ContainsKey(key);


        public TValue GetByKey(TKey key) => this[key];

        public TKey GetByValue(TValue value) => _reverseDict[value];

        public void SetByKey(TKey key, TValue value)
        {
            lock (_lockObject)
            {
                // If the specified key doesn't exist, we can set its value.
                if (!_forwardDict.ContainsKey(key))
                    throw new ArgumentException("The specified key was not found.");

                // Remove the corresponding item from the reverse dictionary
                TValue oldValue = _forwardDict[key];
                _reverseDict.Remove(oldValue);

                // Replace the old value in the forward dictionary
                _forwardDict[key] = value;

                // Add the reverse entry to the reverse dictionary
                _reverseDict.Add(value, key);
            }
        }

        public void SetByValue(TValue value, TKey key)
        {
            lock (_lockObject)
            {
                // If the specified key doesn't exist, we can set its value.
                if (!_reverseDict.ContainsKey(value))
                    throw new ArgumentException("The specified right key was not found.");

                // Remove the corresponding item from the forward dictionary
                TKey oldKey = _reverseDict[value];
                _forwardDict.Remove(oldKey);

                // Replace the old value in the reverse dictionary
                _reverseDict[value] = key;

                // Add the corresponding entry to the forward dictionary
                _forwardDict.Add(key, value);
            }
        }

        public TValue this[TKey key]
        {
            get { return _forwardDict[key]; }
            set { SetByKey(key, value); }
        }

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (_lockObject)
            {
                if (ContainsKey(item.Key))
                    throw new ArgumentException($"The collection already contains an item for {item.Key}.");

                if (ContainsValue(item.Value))
                    throw new ArgumentException($"The collection already contains an item for {item.Value}.");

                // None of the item keys exist, good to add
                _forwardDict.Add(item.Key, item.Value);
                _reverseDict.Add(item.Value, item.Key);
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => _forwardDict.Keys;

        public Dictionary<TKey, TValue>.ValueCollection Values => _forwardDict.Values;
    }
}
