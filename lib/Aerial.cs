using System;

namespace Life.lib
{
    class Aerial
    {
        private bool[,] InnerArray;
        private readonly int xDimension;
        private readonly int yDimension;
        private readonly Random rnd;
        private int iteration;
        public Aerial(int x, int y)
        {
            xDimension = x;
            yDimension = y;
            InnerArray = new bool[x, y];
            rnd = new Random();
            iteration = 0;
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
        }

        public StepResult Step()
        {
            if (!AnyAlive()) return StepResult.DEAD;
            bool eq = true;
            var tmp = new bool[xDimension, yDimension];
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    tmp[x, y] = Calculate(x, y, InnerArray[x, y]);
                    if (tmp[x,y] != InnerArray[x,y])
                    {
                        eq = false;
                    }
                }
            }
            if (eq) return StepResult.CYCLING;
            InnerArray = tmp;
            iteration++;
            return StepResult.NORMAL;
        }
        private bool Calculate(int x, int y, bool state)
        {
            switch (AliveNeighbors(x, y))
            {
                case 0:
                case 1:
                    return false;
                case 2:
                    if (state) return true;
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
