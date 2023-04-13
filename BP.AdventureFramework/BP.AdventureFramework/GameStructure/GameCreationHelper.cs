﻿using System;
using BP.AdventureFramework.Characters;
using BP.AdventureFramework.Locations;
using BP.AdventureFramework.Parsing;
using BP.AdventureFramework.Rendering.Frames;

namespace BP.AdventureFramework.GameStructure
{
    /// <summary>
    /// Represents a class that helps to build games.
    /// </summary>
    public class GameCreationHelper
    {
        #region Methods

        /// <summary>
        /// Initializes a new instance of the GameCreationHelper class.
        /// </summary>
        private GameCreationHelper()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the game creator.
        /// </summary>
        public GameCreator Creator { get; protected set; }

        #endregion

        #region StaticMethods

        /// <summary>
        /// Create a new GameCreationHelper.
        /// </summary>
        /// <param name="name">The name of the game.</param>
        /// <param name="description">A description of the game.</param>
        /// <param name="overworldGenerator">A function to generate the Overworld with.</param>
        /// <param name="playerGenerator">The function to generate the Player with.</param>
        /// <param name="completionCondition">The callback used to to check game completion.</param>
        /// <param name="titleFrame">The title Frame.</param>
        /// <param name="completionFrame">The completion Frame.</param>
        /// <param name="help">The help Frame.</param>
        /// <returns>A new GameCreationHelper that will create a GameCreator with the parameters specified.</returns>
        public static GameCreationHelper Create(string name, string description, Func<Overworld> overworldGenerator, Func<PlayableCharacter> playerGenerator, CompletionCheck completionCondition, TitleFrame titleFrame, Frame completionFrame, HelpFrame help)
        {
            var helper = new GameCreationHelper
            {
                Creator = () => new Game(name, description, playerGenerator.Invoke(), overworldGenerator.Invoke()) { TitleFrame = titleFrame, CompletionFrame = completionFrame, CompletionCondition = completionCondition, HelpFrame = help }
            };

            return helper;
        }

        #endregion
    }
}