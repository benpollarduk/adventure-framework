﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using BP.AdventureFramework.Assets.Characters;
using BP.AdventureFramework.Assets.Interaction;
using BP.AdventureFramework.Assets.Locations;
using BP.AdventureFramework.Commands;
using BP.AdventureFramework.Commands.Game;
using BP.AdventureFramework.Extensions;
using BP.AdventureFramework.Interpretation;
using BP.AdventureFramework.Rendering;
using BP.AdventureFramework.Rendering.FrameBuilders;
using BP.AdventureFramework.Rendering.FrameBuilders.Legacy;
using BP.AdventureFramework.Rendering.LayoutBuilders;
using BP.AdventureFramework.Rendering.MapBuilders.Legacy;

namespace BP.AdventureFramework.Logic
{
    /// <summary>
    /// Represents the structure of the game
    /// </summary>
    public sealed class Game : IDisposable
    {
        #region Constants

        /// <summary>
        /// Get the default error prefix.
        /// </summary>
        public const string DefaultErrorPrefix = "Oops";

        #endregion

        #region StaticProperties

        /// <summary>
        /// Get the default interpreter.
        /// </summary>
        public static IInterpreter DefaultInterpreter => new InputInterpreter(new FrameCommandInterpreter(), new GlobalCommandInterpreter(), new GameCommandInterpreter());

        /// <summary>
        /// Get the default size.
        /// </summary>
        public static Size DefaultSize { get; } = new Size(80, 50);

        #endregion

        #region Fields

        private PlayableCharacter player;
        private bool displayCommandListInSceneFrames = true;
        private KeyType sceneMapKeyType;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the current state.
        /// </summary>
        private GameState State { get; set; }

        /// <summary>
        /// Get or set if the command list is displayed in scene frames.
        /// </summary>
        public bool DisplayCommandListInSceneFrames
        {
            get { return displayCommandListInSceneFrames; }
            set
            {
                displayCommandListInSceneFrames = value;

                if (State == GameState.Active)
                    DrawFrame(FrameBuilders.SceneFrameBuilder.Build(Overworld.CurrentRegion.CurrentRoom, Player, string.Empty, value, SceneMapKeyType, DisplaySize.Width, DisplaySize.Height));
            }
        }

        /// <summary>
        /// Get or set the type of key to use on the scene map.
        /// </summary>
        public KeyType SceneMapKeyType
        {
            get { return sceneMapKeyType; }
            set
            {
                sceneMapKeyType = value;

                if (State == GameState.Active)
                    DrawFrame(FrameBuilders.SceneFrameBuilder.Build(Overworld.CurrentRegion.CurrentRoom, Player, string.Empty, DisplayCommandListInSceneFrames, value, DisplaySize.Width, DisplaySize.Height));
            }
        }

        /// <summary>
        /// Get the player.
        /// </summary>
        public PlayableCharacter Player
        {
            get { return player; }
            private set
            {
                if (player != null)
                    player.Died -= Player_Died;

                player = value;

                if (player != null)
                    player.Died += Player_Died;
            }
        }

        /// <summary>
        /// Get the Overworld.
        /// </summary>
        public Overworld Overworld { get; }

        /// <summary>
        /// Get the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Get or set the error prefix.
        /// </summary>
        public string ErrorPrefix { get; set; }

        /// <summary>
        /// Get if this is executing.
        /// </summary>
        public bool IsExecuting { get; private set; }

        /// <summary>
        /// Get or set the interpreter.
        /// </summary>
        private IInterpreter Interpreter { get; set; }

        /// <summary>
        /// Get or set the output stream.
        /// </summary>
        internal TextWriter Output { get; set; }

        /// <summary>
        /// Get or set input stream.
        /// </summary>
        internal TextReader Input { get; set; }

        /// <summary>
        /// Get or set the error stream.
        /// </summary>
        internal TextWriter Error { get; set; }

        /// <summary>
        /// Get the size of the display area.
        /// </summary>
        public Size DisplaySize { get; }

        /// <summary>
        /// Get if this game has ended.
        /// </summary>
        public bool HasEnded { get; private set; }

        /// <summary>
        /// Get or set the exit mode for this game.
        /// </summary>
        internal ExitMode ExitMode { get; set; } = ExitMode.ReturnToTitleScreen;

        /// <summary>
        /// Get or set the collection of frame builders used to render this game.
        /// </summary>
        private FrameBuilderCollection FrameBuilders { get; set; }

        /// <summary>
        /// Get or set the current Frame.
        /// </summary>
        private Frame CurrentFrame { get; set; }

        /// <summary>
        /// Get or set the completion condition.
        /// </summary>
        internal CompletionCheck CompletionCondition { get; set; }

        /// <summary>
        /// Get or set the callback to invoke when waiting for key presses.
        /// </summary>
        internal WaitForKeyPressCallback WaitForKeyPressCallback { get; set; }

        /// <summary>
        /// Occurs when the game begins drawing a frame.
        /// </summary>
        internal event EventHandler<Frame> StartingFrameDraw;

        /// <summary>
        /// Occurs when the game finishes drawing a frame.
        /// </summary>
        internal event EventHandler<Frame> FinishedFrameDraw;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Game class.
        /// </summary>
        /// <param name="name">The name of this Game.</param>
        /// <param name="description">A description of this Game.</param>
        /// <param name="player">The Player to use for this Game.</param>
        /// <param name="overworld">An Overworld to use for this Game.</param>
        /// <param name="displaySize">The display size.</param>
        private Game(string name, string description, PlayableCharacter player, Overworld overworld, Size displaySize)
        {
            Name = name;
            Description = description;
            Player = player;
            Overworld = overworld;
            DisplaySize = displaySize;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute the game.
        /// </summary>
        private void Execute()
        {
            if (IsExecuting)
                return;

            IsExecuting = true;

            var input = string.Empty;

            Refresh(FrameBuilders.TitleFrameBuilder.Build(Name, Description, DisplaySize.Width, DisplaySize.Height));

            do
            {
                var displayReactionToInput = true;
                var reaction = new Reaction(ReactionResult.None, "Error.");

                if (!CurrentFrame.AcceptsInput)
                {
                    var frame = CurrentFrame;

                    while (!WaitForKeyPressCallback(Convert.ToChar(13)) && CurrentFrame == frame)
                        DrawFrame(CurrentFrame);
                }
                else
                {
                    input = Input.ReadLine();
                }

                switch (State)
                {
                    case GameState.NotStarted:
                        EnterGame();
                        displayReactionToInput = false;
                        break;
                    case GameState.Finished:
                        ResetGame();
                        displayReactionToInput = false;
                        break;
                    default:
                        {
                            if (!CurrentFrame.AcceptsInput)
                            {
                                Refresh();
                                displayReactionToInput = false;
                            }
                            else
                            {
                                var interpretation = Interpreter?.Interpret(input, this) ?? new InterpretationResult(false, new Unactionable("No interpreter."));

                                if (interpretation.WasInterpretedSuccessfully)
                                    reaction = RunCommand(interpretation.Command);
                            }

                            break;
                        }
                }

                if (!displayReactionToInput)
                    continue;

                switch (reaction.Result)
                {
                    case ReactionResult.None:
                        var message = ErrorPrefix + ": " + reaction.Description;
                        UpdateScreenWithCurrentFrame(message);
                        break;
                    case ReactionResult.Reacted:
                        UpdateScreenWithCurrentFrame(reaction.Description);
                        break;
                    case ReactionResult.SelfContained:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            while (!HasEnded);

            IsExecuting = false;
        }

        /// <summary>
        /// Update the screen with the current Frame, provided by the GameManager.Game property.
        /// </summary>
        /// <param name="message">An additional message to display to the user.</param>
        private void UpdateScreenWithCurrentFrame(string message)
        {
            DrawFrame(FrameBuilders.SceneFrameBuilder.Build(Overworld.CurrentRegion.CurrentRoom, Player, message, DisplayCommandListInSceneFrames, SceneMapKeyType, DisplaySize.Width, DisplaySize.Height));
        }

        /// <summary>
        /// Draw a Frame onto the output stream.
        /// </summary>
        /// <param name="frame">The frame to draw.</param>
        private void DrawFrame(Frame frame)
        {
            try
            {
                StartingFrameDraw?.Invoke(this, frame);

                Output.Write(frame);

                FinishedFrameDraw?.Invoke(this, frame);
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception was caught drawing the frame: {0}", e.Message);
            }
        }

        /// <summary>
        /// Handle a reaction.
        /// </summary>
        /// <param name="command">The command to react to.</param>
        /// <returns>The reaction to the command.</returns>
        private Reaction RunCommand(ICommand command)
        {
            var reaction = command.Invoke();

            var result = CompletionCondition(this);

            if (!result.IsCompleted) 
                return reaction;

            Refresh(FrameBuilders.CompletionFrameBuilder.Build(result.Title, result.Description, DisplaySize.Width, DisplaySize.Height));
            End();

            return reaction;
        }

        /// <summary>
        /// Enter the game.
        /// </summary>
        private void EnterGame()
        {
            State = GameState.Active;
            Refresh(FrameBuilders.SceneFrameBuilder.Build(Overworld.CurrentRegion.CurrentRoom, Player, string.Empty, DisplayCommandListInSceneFrames, SceneMapKeyType, DisplaySize.Width, DisplaySize.Height));
        }

        /// <summary>
        /// Reset the game.
        /// </summary>
        private void ResetGame()
        {
            State = GameState.NotStarted;
            Refresh(FrameBuilders.TitleFrameBuilder.Build(Name, Description, DisplaySize.Width, DisplaySize.Height));
        }

        /// <summary>
        /// End the Game.
        /// </summary>
        internal void End()
        {
            State = GameState.Finished;
            HasEnded = true;
        }

        /// <summary>
        /// Find an interaction target within the current scope for this Game.
        /// </summary>
        /// <param name="name">The targets name.</param>
        /// <returns>The first IInteractWithItem object which has a name that matches the name parameter.</returns>
        public IInteractWithItem FindInteractionTarget(string name)
        {
            if (name.EqualsExaminable(player))
                return Player;

            if (Player.Items.Any(name.EqualsExaminable))
            {
                Player.FindItem(name, out var i);
                return i;
            }

            if (name.EqualsExaminable(Overworld.CurrentRegion.CurrentRoom))
                return Overworld.CurrentRegion.CurrentRoom;

            if (!Overworld.CurrentRegion.CurrentRoom.ContainsInteractionTarget(name))
                return null;

            Overworld.CurrentRegion.CurrentRoom.FindInteractionTarget(name, out var target);
            return target;
        }

        /// <summary>
        /// Refresh the current frame.
        /// </summary>
        private void Refresh()
        {
            Refresh(string.Empty);
        }

        /// <summary>
        /// Refresh the current frame.
        /// </summary>
        /// <param name="message">Any message to display.</param>
        private void Refresh(string message)
        {
            Refresh(FrameBuilders.SceneFrameBuilder.Build(Overworld.CurrentRegion.CurrentRoom, Player, message, DisplayCommandListInSceneFrames, SceneMapKeyType, DisplaySize.Width, DisplaySize.Height));
        }

        /// <summary>
        /// Refresh the display.
        /// </summary>
        /// <param name="frame">The frame to display.</param>
        private void Refresh(Frame frame)
        {
            CurrentFrame = frame;
            DrawFrame(frame);
        }

        /// <summary>
        /// Display the help frame.
        /// </summary>
        public void DisplayHelp()
        {
            Refresh(FrameBuilders.HelpFrameBuilder.Build("Help", "Provides help for in game commands", Interpreter.SupportedCommands, DisplaySize.Width, DisplaySize.Height));
        }

        /// <summary>
        /// Display the map frame.
        /// </summary>
        public void DisplayMap()
        {
            Refresh(FrameBuilders.RegionMapFrameBuilder.Build(Overworld.CurrentRegion, DisplaySize.Width, DisplaySize.Height));
        }

        /// <summary>
        /// Display the about message.
        /// </summary>
        public void DisplayAbout()
        {
            Refresh(FrameBuilders.TitleFrameBuilder.Build("About", "BP.AdventureFramework by Ben Pollard 2011 - 2023", DisplaySize.Width, DisplaySize.Height));
        }

        #endregion

        #region StaticMethods

        /// <summary>
        /// Create a new callback for generating instances of a game.
        /// </summary>
        /// <param name="name">The name of the game.</param>
        /// <param name="description">A description of the game.</param>
        /// <param name="overworldGenerator">A function to generate the Overworld with.</param>
        /// <param name="playerGenerator">The function to generate the Player with.</param>
        /// <param name="completionCondition">The callback used to check game completion.</param>
        /// <returns>A new GameCreationHelper that will create a GameCreator with the parameters specified.</returns>
        public static GameCreationCallback Create(string name, string description, OverworldCreationCallback overworldGenerator, PlayerCreationCallback playerGenerator, CompletionCheck completionCondition)
        {
            var stringLayoutBuilder = new StringLayoutBuilder();

            var frameBuilderCollection = new FrameBuilderCollection(
                new LegacyTitleFrameBuilder(stringLayoutBuilder),
                new LegacySceneFrameBuilder(stringLayoutBuilder, new LegacyRoomMapBuilder(stringLayoutBuilder)),
                new LegacyRegionMapFrameBuilder(stringLayoutBuilder, new LegacyRegionMapBuilder(stringLayoutBuilder)),
                new LegacyHelpFrameBuilder(stringLayoutBuilder),
                new LegacyEndFrameBuilder(stringLayoutBuilder));

            return Create(
                name,
                description,
                overworldGenerator,
                playerGenerator,
                completionCondition,
                DefaultSize,
                frameBuilderCollection,
                ExitMode.ReturnToTitleScreen,
                DefaultErrorPrefix,
                DefaultInterpreter);
        }

        /// <summary>
        ///  Create a new callback for generating instances of a game.
        /// </summary>
        /// <param name="name">The name of the game.</param>
        /// <param name="description">A description of the game.</param>
        /// <param name="overworldGenerator">A function to generate the Overworld with.</param>
        /// <param name="playerGenerator">The function to generate the Player with.</param>
        /// <param name="displaySize">The display size.</param>
        /// <param name="completionCondition">The callback used to check game completion.</param>
        /// <param name="frameBuilders">The collection of frame builders to use to render the game.</param>
        /// <param name="exitMode">The exit mode.</param>
        /// <param name="errorPrefix">A prefix to use when displaying errors.</param>
        /// <param name="interpreter">The interpreter.</param>
        /// <returns>A new GameCreationHelper that will create a GameCreator with the parameters specified.</returns>
        public static GameCreationCallback Create(string name, string description, OverworldCreationCallback overworldGenerator, PlayerCreationCallback playerGenerator, CompletionCheck completionCondition, Size displaySize, FrameBuilderCollection frameBuilders, ExitMode exitMode, string errorPrefix, IInterpreter interpreter)
        {
            return () =>
            {
                var pC = playerGenerator?.Invoke();

                var game = new Game(name, description, pC, overworldGenerator?.Invoke(pC), displaySize)
                {
                    FrameBuilders = frameBuilders,
                    CompletionCondition = completionCondition,
                    ExitMode = exitMode,
                    ErrorPrefix = errorPrefix,
                    Interpreter = interpreter
                };

                return game;
            };
        }

        /// <summary>
        /// Execute a game.
        /// </summary>
        /// <param name="creator">The creator to use to create the game.</param>
        public static void Execute(GameCreationCallback creator)
        {
            var run = true;

            while (run)
            {
                using (var game = creator.Invoke())
                {
                    SetupConsole(game);
                    AttachToConsole(game);
                    game.Execute();

                    switch (game.ExitMode)
                    {
                        case ExitMode.ExitApplication:
                            run = false;
                            break;
                        case ExitMode.ReturnToTitleScreen:
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        /// <summary>
        /// Attach a game to the Console.
        /// </summary>
        /// <param name="game">The game.</param>
        private static void AttachToConsole(Game game)
        {
            game.Input = Console.In;
            game.Output = Console.Out;
            game.Error = Console.Error;
            game.WaitForKeyPressCallback = key => Console.ReadKey().KeyChar == key;
            game.StartingFrameDraw += (s, e) => Console.Clear();
            game.FinishedFrameDraw += (s, e) =>
            {
                Console.CursorVisible = e.ShowCursor;
                Console.SetCursorPosition(e.CursorLeft, e.CursorTop);
            };
        }

        /// <summary>
        /// Setup the console for a game.
        /// </summary>
        /// <param name="game">The game.</param>
        private static void SetupConsole(Game game)
        {
            Console.Title = game.Name;
            Console.SetWindowSize(game.DisplaySize.Width, game.DisplaySize.Height);
            Console.SetBufferSize(game.DisplaySize.Width, game.DisplaySize.Height);
        }

        #endregion

        #region EventHandlers

        private void Player_Died(object sender, string e)
        {
            Refresh(FrameBuilders.CompletionFrameBuilder.Build("Game Over", e, DisplaySize.Width, DisplaySize.Height));
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Output?.Dispose();
            Input?.Dispose();
            Error?.Dispose();
        }

        #endregion
    }
}