﻿namespace NetAF.Commands.Frame
{
    /// <summary>
    /// Represents the CommandsOn command.
    /// </summary>
    public sealed class CommandsOn : ICommand
    {
        #region StaticProperties

        /// <summary>
        /// Get the command help.
        /// </summary>
        public static CommandHelp CommandHelp { get; } = new("CommandsOn", "Turn commands on");

        #endregion

        #region Implementation of ICommand

        /// <summary>
        /// Invoke the command.
        /// </summary>
        /// <param name="game">The game to invoke the command on.</param>
        /// <returns>The reaction.</returns>
        public Reaction Invoke(Logic.Game game)
        {
            if (game == null)
                return new(ReactionResult.Error, "No game specified.");

            game.Configuration.DisplayCommandListInSceneFrames = true;
            return new(ReactionResult.Inform, "Commands have been turned on.");
        }

        #endregion
    }
}