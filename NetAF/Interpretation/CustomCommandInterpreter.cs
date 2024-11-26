﻿using System;
using System.Collections.Generic;
using System.Linq;
using NetAF.Commands;
using NetAF.Extensions;
using NetAF.Logic;

namespace NetAF.Interpretation
{
    /// <summary>
    /// Provides an object that can be used for interpreting custom commands.
    /// </summary>
    public sealed class CustomCommandInterpreter : IInterpreter
    {
        #region Implementation of IInterpreter

        /// <summary>
        /// Get an array of all supported commands.
        /// </summary>
        public CommandHelp[] SupportedCommands { get; } = null;

        /// <summary>
        /// Interpret a string.
        /// </summary>
        /// <param name="input">The string to interpret.</param>
        /// <param name="game">The game.</param>
        /// <returns>The result of the interpretation.</returns>
        public InterpretationResult Interpret(string input, Game game)
        {
            if (string.IsNullOrEmpty(input))
                return InterpretationResult.Fail;
            
            var entries = input.Split(" ".ToCharArray(), StringSplitOptions.None);
            var commandName = entries[0];
            var args = entries.Remove(commandName);

            List<CustomCommand> commands = [];
            
            foreach (var examinable in game.GetAllPlayerVisibleExaminables().Where(x => x.Commands != null))
                commands.AddRange(examinable.Commands.Where(x => x.IsPlayerVisible || x.InterpretIfNotPlayerVisible));

            // check looking for just command, not including args
            var command = commands.Find(x => x.Help.Command.InsensitiveEquals(commandName));

            if (command != null)
            {
                command.Arguments = args;
                return new InterpretationResult(true, command);
            }

            //  maybe the command had a space in it?
            command = commands.Find(x => x.Help.Command.InsensitiveEquals(input));

            return command == null ? InterpretationResult.Fail : new InterpretationResult(true, command);
        }

        /// <summary>
        /// Get contextual command help for a game, based on its current state.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>The contextual help.</returns>
        public CommandHelp[] GetContextualCommandHelp(Game game)
        {
            List<CommandHelp> help = [];

            foreach (var examinable in game.GetAllPlayerVisibleExaminables().Where(x => x.Commands != null)) 
                help.AddRange(examinable.Commands.Where(x => x.IsPlayerVisible).Select(command => command.Help));

            return [.. help];
        }

        #endregion
    }
}
