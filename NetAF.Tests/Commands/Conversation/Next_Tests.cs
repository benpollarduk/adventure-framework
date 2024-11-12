﻿using NetAF.Assets.Characters;
using NetAF.Assets.Interaction;
using NetAF.Commands.Conversation;
using NetAF.Conversations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetAF.Logic;

namespace NetAF.Tests.Commands.Conversation
{
    [TestClass]
    public class Next_Tests
    {
        [TestMethod]
        public void GivenNullGame_WhenInvoke_ThenError()
        {
            var command = new Next();

            var result = command.Invoke(null);

            Assert.AreEqual(ReactionResult.Error, result.Result);
        }

        [TestMethod]
        public void GivenNoConverser_WhenInvoke_ThenError()
        {
            var game = NetAF.Logic.Game.Create(new GameInfo(string.Empty, string.Empty, string.Empty), string.Empty, AssetGenerator.Retained(null, null), GameEndConditions.NoEnd, ConsoleGameConfiguration.Default).Invoke();
            var command = new Next();

            var result = command.Invoke(game);

            Assert.AreEqual(ReactionResult.Error, result.Result);
        }

        [TestMethod]
        public void GivenValidGameAndConverser_WhenInvoke_ThenInternal()
        {
            var game = NetAF.Logic.Game.Create(new GameInfo(string.Empty, string.Empty, string.Empty), string.Empty, AssetGenerator.Retained(null, null), GameEndConditions.NoEnd, ConsoleGameConfiguration.Default).Invoke();
            var converser = new NonPlayableCharacter(string.Empty, string.Empty, conversation: new(new Paragraph(string.Empty)));
            game.StartConversation(converser);
            var command = new Next();

            var result = command.Invoke(game);

            Assert.AreEqual(ReactionResult.Internal, result.Result);
        }
    }
}
