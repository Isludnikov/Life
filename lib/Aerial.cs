using System;
using System.Collections.Generic;

namespace Life.lib
{
    class Aerial
    {
        private bool[,] InnerArray;
        private readonly int xDimension;
        private readonly int yDimension;
        private readonly Random rnd;
        private List<bool[,]> History;
        private int iteration;
        public Aerial(int x, int y)
        {
            xDimension = x;
            yDimension = y;
            InnerArray = new bool[x, y];
            rnd = new Random();
            iteration = 0;
            History = new List<bool[,]>();
        }

        public void Sow(double sowFactor = 0.1)
        {
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    InnerArray[x, y] = rnd.NextDouble() <= sowFactor ? true : InnerArray[x, y];
                }
            }
        }
        public int Iteration() => iteration;
        public int XDimension() => xDimension;
        public int YDimension() => yDimension;
        public bool Get(int x, int y) => InnerArray[x, y];
        public void Set(int x, int y, bool value) => InnerArray[x, y] = value;
        public void Clear()
        {
            InnerArray = new bool[xDimension, yDimension];
            iteration = 0;
            History = new List<bool[,]>();
        }

        public StepResult Step()
        {
            if (!AnyAlive()) return StepResult.DEAD;

            History.Add(InnerArray);
            var tmp = new bool[xDimension, yDimension];
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    tmp[x, y] = Calculate(x, y);
                }
            }
            InnerArray = tmp;
            iteration++;
            if (CheckHistory(History, tmp)) return StepResult.CYCLING;
            return StepResult.NORMAL;
        }
        private bool CheckHistory(List<bool[,]> history, bool[,] obj) => history.Exists(x => ArrayEqual(x, obj));

        private bool ArrayEqual(bool[,] a, bool[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1)) return false;
            for (var x = 0; x < a.GetLength(0); x++)
            {
                for (var y = 0; y < a.GetLength(1); y++)
                {
                    if (a[x, y] != b[x, y]) return false;
                }
            }
            return true;
        }

        private bool Calculate(int x, int y)
        {
            switch (AliveNeighbors(x, y))
            {
                case 0:
                case 1:
                    return false;
                case 2:
                    if (Get(x,y)) return true;
                    else return false;
                case 3:
                    return true;
                default:
                    return false;
            }
        }
        public string GetMarker(int x, int y)
        {
            var result = InnerArray[x, y] ? "X" : "";
            if (Config.debug) result += AliveNeighbors(x, y);
            return result;
        }
        private int AliveNeighbors(int x, int y)
        {
            int count = 0;
            for (var i = x - 1; i <= x + 1; i++)
            {
                for (var j = y - 1; j <= y + 1; j++)
                {
                    try
                    {
                        if (InnerArray[i, j]) count++;
                    }
                    catch { }
                }
            }
            if (InnerArray[x, y]) count--;
            return count;
        }
        private bool AnyAlive()
        {
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    if (InnerArray[x, y]) return true;
                }
            }
            return false;
        }
    }
}
