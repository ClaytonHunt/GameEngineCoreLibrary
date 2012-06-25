using System;
using GameEngine.CoreTests.GameConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameEngine.CoreTests.ClasslessLevelingSystem
{
    [TestClass]
    public class LevelingSystemTests
    {
        [TestMethod]
        public void Given_PlayerEquipWeapon_When_Empty_Then_WeaponIsHands()
        {
            // Arrange
            var player = new Actor();

            // Act
            player.EquipWeapon();

            // Assert
            Assert.AreEqual("Hands", player.Weapon.Name);
        }

        [TestMethod]
        public void Given_PlayerEquipWeapon_When_Sword_Then_WeaponIsSword()
        {
            // Arrange
            var player = new Actor();
            var sword = new EquipableWeapon() { Name = "Sword" };
            // Act
            player.EquipWeapon(sword);

            // Assert
            Assert.AreEqual("Sword", player.Weapon.Name);
        }

        [TestMethod]
        public void Given_PlayerAttack_When_NoTarget_Then_LogNothingToAttack()
        {
            // Arrange
            var player = new Actor();
            var console = GameConsole.GameConsole.Instance;

            // Act
            player.Attack();

            // Assert
            Assert.AreEqual("Player: There is no one to attack.", console.LastLogged());
        }

        [TestMethod]
        public void Given_PlayerAttack_When_TargetIsAquired_Then_LogAttack()
        {
            // Arrange
            var player = new Actor();
            var enemy = new Actor();
            var console = GameConsole.GameConsole.Instance;

            // Act
            player.SetTarget(enemy);
            player.Attack();

            // Assert
            Assert.AreEqual("Player: Attacks Enemy with his Fist", console.LastLogged());
        }
    }

    public class Actor
    {
        private EquipableWeapon _weapon;

        public EquipableWeapon Weapon
        {
            get
            {
                return this._weapon;
            }
            private set
            {
                this._weapon = value;
            }
        }

        private Actor _target;

        public void EquipWeapon(EquipableWeapon weapon = null)
        {
            if (weapon == null)
                this.Weapon = this.DefaultWeapon();
            else
                this.Weapon = weapon;
        }

        protected virtual EquipableWeapon DefaultWeapon()
        {
            var weapon = new EquipableWeapon();
            weapon.Name = "Hands";

            return weapon;
        }

        public void Attack()
        {
            if (this._target == null)
                GameConsole.GameConsole.Instance.LogMessage("Player: There is no one to attack.");
            else
                GameConsole.GameConsole.Instance.LogMessage("Player: Attacks Enemy with his Fist");
        }

        public void SetTarget(Actor target)
        {
            this._target = target;
        }
    }

    public class EquipableWeapon
    {
        public string Name { get; set; }
    }
}