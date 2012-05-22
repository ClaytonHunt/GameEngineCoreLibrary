using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Core
{
    public class Pathing
    {
        public List<Node> Obstructions { get; set; }

        public int LowerXLimit { get; set; }
        public int LowerYLimit { get; set; }
        public int UpperXLimit { get; set; }
        public int UpperYLimit { get; set; }
        public int Range { get; set; }

        public Pathing(List<Node> obstructions = null, int lowerXLimit = 0, int lowerYLimit = 0, int upperXLimit = 32, int upperYLimit = 32, int range = 1)
        {
            Obstructions = obstructions;
            this.LowerXLimit = lowerXLimit;
            this.UpperXLimit = upperXLimit;
            this.LowerYLimit = lowerYLimit;
            this.UpperYLimit = upperYLimit;
            this.Range = range;
        }

        public IList<Node> CreatePath(Node startPoint, Node endPoint)
        {
            var temp = new List<Node>();

            // Calculate startPoints Heuristic and Final Cost
            startPoint.HeuristicCost = GetHeuristic(startPoint, endPoint);

            temp.Add(startPoint);

            var current = startPoint;

            while (temp.Where(x => x.State == NodeState.Open).Any() && (current.X != endPoint.X || current.Y != endPoint.Y))
            {
                foreach (var item in GetNeighbors(current))
                {
                    if (!temp.Where(j => j.X == item.X && j.Y == item.Y).Any())
                    {
                        item.HeuristicCost = GetHeuristic(item, endPoint);
                        temp.Add(item);
                    }
                    else if (temp.Where(j => j.X == item.X && j.Y == item.Y && j.GivenCost > item.GivenCost).Any())
                    {
                        temp.Remove(temp.Where(j => j.X == item.X && j.Y == item.Y && j.GivenCost > item.GivenCost).First());
                        item.HeuristicCost = GetHeuristic(item, endPoint);
                        temp.Add(item);
                    }
                }

                temp.Where(x => x.State == NodeState.Open && x.X == current.X && x.Y == current.Y).First().State = NodeState.Closed;
                current = temp.Where(x => x.State == NodeState.Open).OrderBy(x => x.HeuristicCost).FirstOrDefault();
                

                if (Obstructions != null && Obstructions.Where(x => x.X == endPoint.X && x.Y == endPoint.Y).Any() && WithinRange(endPoint, current, Range))
                {
                    endPoint = current;
                    break;
                }
            }


            var result = new List<Node>();

            result = DeterminePath(temp, endPoint);

            result.Reverse();

            return result.ToList();
        }

        private bool WithinRange(Node target, Node current, int range)
        {
            var result = false;
            // North/South
            if (current.X == target.X && Math.Abs(current.Y - target.Y) <= range)
                result = true;

            // East/West
            if (Math.Abs(target.X - current.X) <= range && current.Y == target.Y)
                result = true;

            return result;
        }

        private List<Node> DeterminePath(List<Node> nodes, Node target, List<Node> path = null)
        {
            if (path == null)
                path = new List<Node>();

            if (nodes.Where(j => j.X == target.X && j.Y == target.Y).FirstOrDefault() == null)
            {
                var newTarget = nodes.OrderBy(x => x.HeuristicCost).Take(4).OrderBy(x => x.FinalCost).First();

                return DeterminePath(nodes, newTarget, path);
            }

            var item = nodes.Where(j => j.X == target.X && j.Y == target.Y).First();

            path.Add(item);

            if (item.Parent == null)
                return path;

            return DeterminePath(nodes, item.Parent, path);
        }

        private int GetHeuristic(Node start, Node end)
        {
            int result = 0;

            result = (int)Math.Sqrt(((start.X - end.X) * (start.X - end.X)) + ((start.Y - end.Y) * (start.Y - end.Y)));

            return result * 10;
        }

        protected IList<Node> GetNeighbors(Node parent)
        {
            var result = new List<Node>();

            // North
            if ((Obstructions == null || !Obstructions.Where(x => x.X == parent.X && x.Y == parent.Y - 1).Any()) && WithinLimits(parent.X, parent.Y - 1))
            {
                var north = new Node(parent.X, parent.Y - 1, parent);
                result.Add(north);
            }
            // South
            if ((Obstructions == null || !Obstructions.Where(x => x.X == parent.X && x.Y == parent.Y + 1).Any()) && WithinLimits(parent.X, parent.Y + 1))
            {
                var south = new Node(parent.X, parent.Y + 1, parent);
                result.Add(south);
            }
            // East
            if ((Obstructions == null || !Obstructions.Where(x => x.X == parent.X + 1 && x.Y == parent.Y).Any()) && WithinLimits(parent.X + 1, parent.Y))
            {
                var east = new Node(parent.X + 1, parent.Y, parent);
                result.Add(east);
            }
            // West
            if ((Obstructions == null || !Obstructions.Where(x => x.X == parent.X - 1 && x.Y == parent.Y).Any()) && WithinLimits(parent.X - 1, parent.Y))
            {
                var west = new Node(parent.X - 1, parent.Y, parent);
                result.Add(west);
            }

            return result;
        }

        private bool WithinLimits(int x, int y)
        {
            return x < this.LowerXLimit ? false :
                   x > this.UpperXLimit ? false :
                   y < this.LowerYLimit ? false :
                   y > this.UpperYLimit ? false : true;
        }
    }
}
