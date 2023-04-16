﻿using BP.AdventureFramework.Assets.Interaction;

namespace BP.AdventureFramework.Commands.Global
{
    /// <summary>
    /// Represents the New command.
    /// </summary>
    internal class New : ICommand
    {
        #region Properties

        /// <summary>
        /// Get the game.
        /// </summary>
        public Logic.Game Game { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the New class.
        /// </summary>
        /// <param name="game">The game.</param>
        public New(Logic.Game game)
        {
            Game = game;
        }

        #endregion

        #region Implementation of ICommand

        /// <summary>
        /// Invoke the command.
        /// </summary>
        /// <returns>The reaction.</returns>
        public Reaction Invoke()
        {
            if (Game == null)
                return new Reaction(ReactionResult.None, "No game specified.");

            Game.End();
            return new Reaction(ReactionResult.Reacted, "New game.");
        }

        #endregion
    }
}