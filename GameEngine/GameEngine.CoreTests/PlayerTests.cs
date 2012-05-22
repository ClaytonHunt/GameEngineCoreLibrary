using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameEngine.Core.UnitTests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Given_UpdateParent_When_ActorAssignsDelegate_Then_ParentShouldBeNotified()
        {
            // Arrange
            TestableActor parent = new TestableActor();
            TestableActor child = new TestableActor();

            // Act
            child.Health = 10;
            parent.OnChildUpdate += delegate(Actor me)
            {
                parent.ChildWasUpdated = true;
            };
            child.OnUpdateParent += parent.OnChildUpdate;
            child.Health -= 5;

            // Assert
            Assert.IsTrue(parent.ChildWasUpdated);
        }

        [TestMethod]
        public void Given_OnChildUpdate_When_ChildHealthDescreases_Then_IncreaseChildHealthByHalfOfWhatWasLost()
        {
            // Arrange
            Actor parent = new Actor();
            Actor child = new Actor();

            // Act
            child.Health = 10;
            parent.OnChildUpdate += delegate(Actor son)
            {
                lock (son)
                {
                    son.UpdateParent = false;

                    if (son.HealthChanged.Direction == StatDirection.Lower)
                    {
                        son.Health += (int)son.HealthChanged.Amount / 2;
                    }

                    son.UpdateParent = true;
                }
            };
            child.OnUpdateParent += parent.OnChildUpdate;
            child.Health -= 4;

            // Assert
            Assert.AreEqual(8, child.Health);
        }
    }

    public class TestableActor : Actor
    {
        public bool ChildWasUpdated = false;
    }

    public class Actor
    {
        /// <summary>
        /// UpdateParent controls whether or not the parent is updated.
        /// This is important because with out this control the parent
        /// would get in an infinite loop of updates if they tried to
        /// change the child.
        /// </summary>
        public bool UpdateParent = true;

        /// <summary>
        /// Determines whether this actor is part of the players party
        /// </summary>
        public bool PartyMember = false;

        /// <summary>
        /// Health Changed is obviously updated on actor health set.
        /// The purpose of HealthChanged is to indicate to the parent
        /// wheather the health was not changed, lowered, or raised.
        /// </summary>
        public StatChange HealthChanged { get; set; }
        
        private int _health = 0;
        /// <summary>
        /// Actor Health - When this reaches zero typicall the actor is dead.
        /// </summary>
        public int Health
        {
            get
            {
                return _health;            
            }
            set
            {
                if (this.UpdateParent)
                {
                    HealthChanged.Amount = _health - value;

                    if (value < _health)
                        HealthChanged.Direction = StatDirection.Lower;

                    if (value > _health)
                        HealthChanged.Direction = StatDirection.Higher;
                }

                _health = value;

                if (this.UpdateParent)
                    this.OnUpdateParent(this);
            }
        }

        public Actor()
        {
            HealthChanged = new StatChange()
            {
                Direction = StatDirection.None,
                Amount = 0
            };
        }

        public Action<Actor> OnChildUpdate = delegate(Actor child)
        {
            // Here we need to determine what exactly happens when a child
            // recieves and update. It is a delegate so we can add to it
            // dynamically as needed.
        };

        public Action<Actor> OnUpdateParent = delegate(Actor me) { };
        
    }

    public class StatChange
    {
        public StatDirection Direction { get; set; }
        public int Amount { get; set; }
    }

    public enum StatDirection
    {
        None,
        Lower,
        Higher
    }
}
