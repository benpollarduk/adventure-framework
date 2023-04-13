﻿namespace BP.AdventureFramework.Interaction
{
    /// <summary>
    /// Represents an item that can be used within the game.
    /// </summary>
    public class Item : ExaminableObject, IInteractWithItem
    {
        #region Fields

        private string morphicType;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set if this is takeable.
        /// </summary>
        public bool IsTakeable { get; protected set; }

        /// <summary>
        /// Get or set the interaction.
        /// </summary>
        public InteractionCallback Interaction { get; set; } = (i, target) => new InteractionResult(InteractionEffect.NoEffect, i);

        /// <summary>
        /// Get or set the morphic type of this item. This allows correct file IO for the type of this item if it has morphed into a new type.
        /// </summary>
        protected string MorphicType
        {
            get { return morphicType ?? GetType().Name; }
            set { morphicType = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="identifier">This Items identifier.</param>
        /// <param name="description">A description of this Item.</param>
        /// <param name="isTakeable">Specify if this item is takeable.</param>
        public Item(Identifier identifier, Description description, bool isTakeable)
        {
            Identifier = identifier;
            Description = description;
            IsTakeable = isTakeable;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle item morphing.
        /// </summary>
        /// <param name="item">The item to morph into.</param>
        public void Morph(Item item)
        {
            Identifier = item.Identifier;
            Description = item.Description;
            IsPlayerVisible = item.IsPlayerVisible;
            IsTakeable = item.IsTakeable;
            MorphicType = item.GetType().Name;
        }

        /// <summary>
        /// Use this item on a target.
        /// </summary>
        /// <param name="target">The target to use the item on.</param>
        /// <retunrs>The result of the interaction.</retunrs>
        public InteractionResult Use(IInteractWithItem target)
        {
            return target.Interact(this);
        }

        #endregion

        #region IInteractWithItem Members

        /// <summary>
        /// Interact with an item.
        /// </summary>
        /// <param name="item">The item to interact with.</param>
        /// <returns>The result of the interaction.</returns>
        public InteractionResult Interact(Item item)
        {
            return Interaction.Invoke(item, this);
        }

        #endregion
    }
}