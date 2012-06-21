using System;
using System.Collections.Generic;
using System.Linq;
using CodeLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameEngine.CoreTests.GameConsole
{
    [TestClass]
    public class GameConsoleTests
    {
        [TestMethod]
        public void Given_LogMessage_When_MessageProvided_Then_LogShouldContainMessage()
        {
            // Arrange
            var console = GameConsole.Instance;

            // Act
            console.LogMessage("Test Message");

            // Assert
            Assert.AreEqual("Test Message", console.LastLogged());
        }
    }

    public class GameConsole : SingletonBase<GameConsole>
    {
        private GameConsole()
        {
            _logList = new List<string>();
        }

        private List<string> _logList;

        public void LogMessage(string message)
        {
            _logList.Add(message);
        }

        public string LastLogged()
        {
            return _logList.LastOrDefault();
        }
    }
}