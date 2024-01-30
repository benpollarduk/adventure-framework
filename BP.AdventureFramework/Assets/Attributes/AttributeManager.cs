﻿using System.Collections.Generic;
using System.Linq;
using BP.AdventureFramework.Extensions;

namespace BP.AdventureFramework.Assets.Attributes
{
    /// <summary>
    /// Provides a class for managing attributes.
    /// </summary>
    public sealed class AttributeManager
    {
        #region Fields

        /// <summary>
        /// Get or set the underlying attributes.
        /// </summary>
        private readonly Dictionary<Attribute, int> attributes = new Dictionary<Attribute, int>();

        #endregion

        #region Properties

        /// <summary>
        /// Get the number of attributes this manager has.
        /// </summary>
        public int Count => attributes.Count;

        #endregion

        #region Methods

        /// <summary>
        /// Get all attributes.
        /// </summary>
        /// <returns>An array of attribtes.</returns>
        public Attribute[] GetAttributes()
        {
            return attributes.Keys.ToArray();
        }

        /// <summary>
        /// Get all attributes as a dictionary.
        /// </summary>
        /// <returns>An array of attribtes.</returns>
        public Dictionary<Attribute, int> GetAsDictionary()
        {
            return attributes.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Get the value of an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The value.</returns>
        public double GetValue(string attributeName)
        {
            return GetValue(attributes.Keys.FirstOrDefault(x => x.Name.InsensitiveEquals(attributeName)));
        }

        /// <summary>
        /// Get the value of an attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The value.</returns>
        public double GetValue(Attribute attribute)
        {
            return attributes.TryGetValue(attribute, out var value) ? value : 0d;
        }

        /// <summary>
        /// Add a value to an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="value">The value.</param>
        public void Add(string attributeName, int value)
        {
            var attribute = attributes.Keys.FirstOrDefault(x => x.Name.InsensitiveEquals(attributeName));

            if (attribute == null)
                attributes.Add(new Attribute(attributeName, string.Empty, int.MinValue, int.MaxValue), value);
            else
                attributes[attribute] += value;
        }

        /// <summary>
        /// Add a value to an attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public void Add(Attribute attribute, int value)
        {
            Add(attribute.Name, value);
        }

        /// <summary>
        /// Subtract a value from an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="value">The value.</param>
        public void Subtract(string attributeName, int value)
        {
            var attribute = attributes.Keys.FirstOrDefault(x => x.Name.InsensitiveEquals(attributeName));

            if (attribute == null)
                return;

            attributes[attribute] -= value;
        }

        /// <summary>
        /// Subtract a value from an attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public void Subtract(Attribute attribute, int value)
        {
            Subtract(attribute.Name, value);
        }

        /// <summary>
        /// Remove an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        public void Remove(string attributeName)
        {
            var attribute = attributes.Keys.FirstOrDefault(x => x.Name.InsensitiveEquals(attributeName));

            if (attribute == null)
                return;

            attributes.Remove(attribute);
        }

        /// <summary>
        /// Remove an attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        public void Remove(Attribute attribute)
        {
            Remove(attribute.Name);
        }

        /// <summary>
        /// Remove all attributes.
        /// </summary>
        public void RemoveAll()
        {
            attributes.Clear();
        }

        #endregion
    }
}
