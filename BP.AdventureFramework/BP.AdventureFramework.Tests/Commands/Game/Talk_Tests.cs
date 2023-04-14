﻿using BP.AdventureFramework.Commands.Game;
using BP.AdventureFramework.Assets;
using BP.AdventureFramework.Assets.Characters;
using BP.AdventureFramework.Assets.Interaction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BP.AdventureFramework.Tests.Commands.Game
{
    [TestClass]
    public class Talk_Tests
    {
        [TestMethod]
        public void GivenNoTarget_WhenInvoke_ThenNone()
        {
            var command = new Talk(null);

            var result = command.Invoke();

            Assert.AreEqual(ReactionResult.None, result.Result);
        }

        [TestMethod]
        public void GivenTargetIsDead_WhenInvoke_ThenNone()
        {
            var npc = new NonPlayableCharacter(Identifier.Empty, Description.Empty, null, false, null);
            var command = new Talk(npc);

            var result = command.Invoke();

            Assert.AreEqual(ReactionResult.None, result.Result);
        }

        [TestMethod]
        public void GivenTarget_WhenInvoke_ThenReacted()
        {
            var npc = new NonPlayableCharacter(Identifier.Empty, Description.Empty);
            var command = new Talk(npc);

            var result = command.Invoke();

            Assert.AreEqual(ReactionResult.Reacted, result.Result);
        }
    }
}
