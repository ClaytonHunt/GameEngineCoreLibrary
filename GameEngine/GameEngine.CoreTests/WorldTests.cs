using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GameEngine.Core.UnitTests
{
    [TestClass]
    public class WorldTests
    {
        [TestMethod]
        public void Given_GetZone_When_WorldIsPopulated_Then_ReturnMinimallyPopulatedZone()
        {
            // Arrange
            Zone world = new Zone(0, 0);
            int sizeX = 10;
            int sizeY = 10;

            // Act
            world.Generate(sizeX, sizeY);
            var zone = world.GetZone(5, 5);

            // Assert
            Assert.AreNotEqual(ZoneType.None, zone.Type);
        }

        [TestMethod]
        public void Given_ZoneIn_When_WorldIsPopulated_Then_PopulateSubZoneAndReturnPopulatedZone()
        {
            // Arrange
            Zone world = new Zone(0, 0);
            int sizeX = 10;
            int sizeY = 10;

            // Act
            world.Generate(sizeX, sizeY);
            var zone = world.ZoneIn(x: 5,y: 5,sizeX: 10,sizeY: 10);
            var subZone = zone.GetZone(1,1);

            // Assert
            Assert.AreNotEqual(ZoneType.None, subZone.Type);
        }
    }

    public class Zone
    {
        private List<Zone> _zones { get; set; }

        private EntityPosition _position;
        public EntityPosition Position { get { return _position; } }

        public ZoneType Type { get; set; }

        public List<Actor> Actors { get; set; }

        public Zone Parent { get; set; }

        public Zone(int x, int y)
        {
            this._position = new EntityPosition(x,y);
            this._zones = new List<Zone>();
            this.Actors = new List<Actor>();
        }

        public void Generate(int sizeX, int sizeY)
        {
            // This can/will change this is just temporary to get something working
            for(int i = 0; i < sizeY; i++)
                for (int j = 0; j < sizeX; j++)
                {
                    _zones.Add(new Zone(j, i)
                    {
                        // This would be a good spot for name generation, etc...
                        Type = ZoneType.Grass 
                    });
                }
        }

        public Zone ZoneIn(int x, int y, int sizeX, int sizeY)
        {
            var actors = this.Actors.Where(a => a.PartyMember).ToList();
            var parentZone = this;

            var result = ZoneIn(x, y, sizeX, sizeY, actors, parentZone);

            return result;
        }
        
        public Zone ZoneIn(int x, int y, int sizeX, int sizeY, List<Actor> actorsZoning, Zone parentZone)
        {
            var result = this.GetZone(x, y);

            result.Parent = parentZone;
            result.Generate(sizeX, sizeY);


            // Add NonPlayer Actors

            // Assign Actor List
            result.Actors.AddRange(actorsZoning);

            return result;
        }

        public Zone GetZone(int x, int y)
        {
            var result = _zones.Where(z => z.Position.X == x && z.Position.Y == y).FirstOrDefault() ?? new Zone(x, y);
            return result;
        }
    }

    public class EntityPosition
    {
        public EntityPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public enum ZoneType
    {
        None = 0,
        Grass = 1
    }
}

