﻿using System.IO;

namespace NetAF.Rendering.Console
{
    /// <summary>
    /// Represents a presenter for TextWriter.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public sealed class TextWriterPresenter(TextWriter writer) : IFramePresenter
    {
        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return writer.ToString();
        }

        #endregion

        #region Implementation of IFramePresenter

        /// <summary>
        /// Write a string.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public void Write(string value)
        {
            writer.Write(value);
        }

        #endregion
    }
}
