#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#nullable enable
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning disable
#endif

namespace Bannerlord.ModuleManager
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A dictionary by module ID that, unlike <see cref="Dictionary{TKey,TValue}"/>, takes a null ID too: IDs come from
    /// SubModule.xml and JSON, and the lookups it replaces matched a null ID with string.Equals
    /// </summary>
    internal sealed class ModuleIdDictionary<TValue>
    {
        private readonly Dictionary<string, TValue> _values = new(StringComparer.Ordinal);
        private bool _hasNull;
        private TValue _null = default!;

        public bool TryGetValue(string? id, out TValue value)
        {
            if (id is null)
            {
                value = _null;
                return _hasNull;
            }
            return _values.TryGetValue(id, out value!);
        }

        public bool ContainsKey(string? id) => id is null ? _hasNull : _values.ContainsKey(id);

        public void Add(string? id, TValue value)
        {
            if (id is null)
            {
                if (_hasNull)
                    throw new ArgumentException("An element with the same key already exists.");
                _hasNull = true;
                _null = value;
                return;
            }
            _values.Add(id, value);
        }

        /// <summary>
        /// Keeps the first value of an ID, like FirstOrDefault over a list does
        /// </summary>
        public void AddIfMissing(string? id, TValue value)
        {
            if (!ContainsKey(id))
                Add(id, value);
        }
    }

    /// <summary>
    /// Modules by instance: records compare by value, a cache only needs the same instance
    /// </summary>
    internal sealed class ModuleReferenceComparer : IEqualityComparer<ModuleInfoExtended>
    {
        public static readonly ModuleReferenceComparer Instance = new();

        public bool Equals(ModuleInfoExtended? x, ModuleInfoExtended? y) => ReferenceEquals(x, y);

        public int GetHashCode(ModuleInfoExtended obj) => RuntimeHelpers.GetHashCode(obj);
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}
