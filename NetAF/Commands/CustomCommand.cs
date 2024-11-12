﻿using NetAF.Assets;
using NetAF.Assets.Interaction;
using NetAF.Interpretation;
using NetAF.Serialization;

namespace NetAF.Commands
{
    /// <summary>
    /// Provides a custom command.
    /// </summary>
    /// <param name="help">The help for this command.</param>
    /// <param name="isPlayerVisible">If this is visible to the player.</param>
    /// <param name="interpretIfNotPlayerVisible">If this command can be interpreted when the IsPlayerVisible is false.</param>
    /// <param name="callback">The callback to invoke when this command is invoked.</param>
    public class CustomCommand(CommandHelp help, bool isPlayerVisible, bool interpretIfNotPlayerVisible, CustomCommandCallback callback) : ICommand, IPlayerVisible, IRestoreFromObjectSerialization<CustomCommandSerialization>
    {
        #region Properties

        /// <summary>
        /// Get the callback.
        /// </summary>
        private CustomCommandCallback Callback { get; } = callback;

        /// <summary>
        /// Get or set the arguments.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Get the help for this command.
        /// </summary>
        public CommandHelp Help { get; } = help;

        /// <summary>
        /// Get if this command can be interpreted when the IsPlayerVisible is false.
        /// </summary>
        public bool InterpretIfNotPlayerVisible { get; set; } = interpretIfNotPlayerVisible;

        #endregion

        #region Implementation of ICommand

        /// <summary>
        /// Invoke the command.
        /// </summary>
        /// <param name="game">The game to invoke the command on.</param>
        /// <returns>The reaction.</returns>
        public Reaction Invoke(Logic.Game game)
        {
            return Callback.Invoke(game, Arguments);
        }

        #endregion

        #region Implementation of IPlayerVisible

        /// <summary>
        /// Get or set if this is visible to the player.
        /// </summary>
        public bool IsPlayerVisible { get; set; } = isPlayerVisible;

        #endregion

        #region Implementation of IRestoreFromObjectSerialization<CustomCommandSerialization>

        /// <summary>
        /// Restore this object from a serialization.
        /// </summary>
        /// <param name="serialization">The serialization to restore from.</param>
        public void RestoreFrom(CustomCommandSerialization serialization)
        {
            IsPlayerVisible = serialization.IsPlayerVisible;
        }

        #endregion
    }
}
