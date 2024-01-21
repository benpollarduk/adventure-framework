﻿using System;
using System.IO;
using System.Text;
using BP.AdventureFramework.Extensions;
using BP.AdventureFramework.Rendering.FrameBuilders;
using BP.AdventureFramework.Rendering.FrameBuilders.Color;
using BP.AdventureFramework.Utilities;

namespace BP.AdventureFramework.Rendering.Frames
{
    /// <summary>
    /// Provides a grid based frame for displaying a command based interface.
    /// </summary>
    public sealed class GridTextFrame : IFrame
    {
        #region Constants

        /// <summary>
        /// Get the value for the NO_COLOR environment variable.
        /// </summary>
        internal const string NO_COLOR = "NO_COLOR";

        #endregion

        #region Fields

        private readonly GridStringBuilder builder;

        #endregion

        #region Properties

        /// <summary>
        /// Get the background color.
        /// </summary>
        public AnsiColor BackgroundColor { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GridTextFrame class.
        /// </summary>
        /// <param name="builder">The builder that creates the frame.</param>
        /// <param name="cursorLeft">The cursor left position.</param>
        /// <param name="cursorTop">The cursor top position.</param>
        /// <param name="backgroundColor">The background color.</param>
        public GridTextFrame(GridStringBuilder builder, int cursorLeft, int cursorTop, AnsiColor backgroundColor)
        {
            this.builder = builder;
            CursorLeft = cursorLeft;
            CursorTop = cursorTop;
            BackgroundColor = backgroundColor;
        }

        #endregion

        #region StaticMethods

        /// <summary>
        /// Determine if color is suppressed. If the NO_COLOR environment variable is present and set to anything other than '0' or 'false' this will return true.
        /// </summary>
        /// <returns>True if the NO_COLOR ebviroment variable is present and set to anything other than '0' or 'false', else false.</returns>
        internal static bool IsColorSuppressed()
        {
            var value = Environment.GetEnvironmentVariable(NO_COLOR)?.ToLower() ?? string.Empty;

            switch (value)
            {
                case "":
                case "0":
                case "false":
                    return false;
                default:
                    return true;
            }
        }

        #endregion

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (var y = 0; y < builder.DisplaySize.Height; y++)
            {
                for (var x = 0; x < builder.DisplaySize.Width; x++)
                {
                    stringBuilder.Append(builder.GetCharacter(x, y));
                }

                stringBuilder.Append(StringUtilities.Newline);
            }

            return stringBuilder.ToString();
        }

        #endregion

        #region Implementation of IFrame

        /// <summary>
        /// Get the cursor left position.
        /// </summary>
        public int CursorLeft { get; }

        /// <summary>
        /// Get the cursor top position.
        /// </summary>
        public int CursorTop { get; }

        /// <summary>
        /// Get or set if the cursor should be shown.
        /// </summary>
        public bool ShowCursor { get; set; } = true;

        /// <summary>
        /// Get or set if this Frame accepts input.
        /// </summary>
        public bool AcceptsInput { get; set; } = true;

        /// <summary>
        /// Render this frame on a writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Render(TextWriter writer)
        {
            var renderInColor = !IsColorSuppressed();
            var cursorVisible = Console.CursorVisible;
            var startColor = Console.ForegroundColor;

            if (renderInColor)
                Console.BackgroundColor = BackgroundColor.ToConsoleColor();

            Console.CursorVisible = false;

            for (var y = 0; y < builder.DisplaySize.Height; y++)
            {
                for (var x = 0; x < builder.DisplaySize.Width; x++)
                {
                    var c = builder.GetCharacter(x, y);

                    if (c != 0)
                    {
                        if (renderInColor) 
                            Console.ForegroundColor = builder.GetCellColor(x, y).ToConsoleColor();
                        
                        writer.Write(c);
                    }
                    else
                    {
                        writer.Write(" ");
                    }
                }

                if (y < builder.DisplaySize.Height - 1)
                    writer.Write(builder.LineTerminator);
            }

            Console.ForegroundColor = startColor;
            Console.CursorVisible = cursorVisible;
        }

        #endregion
    }
}