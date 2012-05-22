using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Core
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int GivenCost { get; set; }
        public int HeuristicCost { get; set; }
        public int FinalCost
        {
            get
            {
                return GivenCost + HeuristicCost;
            }
        }

        public bool Walkable { get; set; }
        public Node Parent { get; set; }
        public NodeState State { get; set; }

        public Node(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Walkable = true;
            this.State = NodeState.Open;
        }

        public Node(int x, int y, Node parent)
            : this(x, y)
        {
            this.Parent = parent;
            this.GivenCost = this.Parent.GivenCost + 10;
        }
    }

    public enum NodeState
    {
        Open,
        Closed
    }
}
