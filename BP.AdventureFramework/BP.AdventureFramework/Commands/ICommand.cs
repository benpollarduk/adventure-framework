﻿using BP.AdventureFramework.Assets.Interaction;

namespace BP.AdventureFramework.Commands
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Invoke the command.
        /// </summary>
        /// <returns>The reaction.</returns>
        Reaction Invoke();
    }
}
