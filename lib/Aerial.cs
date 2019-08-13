using System;
using System.Collections.Generic;

namespace Life.lib
{
    class Aerial
    {
        private bool[][] InnerArray;
        private int xDimension;
        private int yDimension;
        private readonly Random rnd;
        private readonly List<bool[][]> History;
        private int iteration;
        public Aerial(int x, int y)
        {
            xDimension = x;
            yDimension = y;
            InnerArray = ArrayHelper.CreateArray(x, y);
            rnd = new Random();
            iteration = 0;
            History = new List<bool[][]>();
        }

        public void Sow(double sowFactor = 0.1)
        {
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    InnerArray[x][y] = rnd.NextDouble() <= sowFactor ? true : InnerArray[x][y];
                }
            }
            ClearHistory();
        }
        public int Iteration() => iteration;
        public int XDimension => xDimension;
        public int YDimension => yDimension;
        public bool Get(int x, int y) => InnerArray[x][y];
        public void Set(int x, int y, bool value)
        {
            InnerArray[x][y] = value;
            ClearHistory();
        }
        public void Clear()
        {
            InnerArray = ArrayHelper.CreateArray(xDimension, yDimension);
            iteration = 0;
            ClearHistory();
        }
        public void ClearHistory() => History.Clear();

        public StepResult Step()
        {
            if (!AnyAlive()) return StepResult.DEAD;

            History.Add(InnerArray);
            var tmp = ArrayHelper.CreateArray(xDimension, yDimension);
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    tmp[x][y] = Calculate(x, y);
                }
            }
            InnerArray = tmp;
            iteration++;
            if (CheckHistory(History, tmp)) return StepResult.CYCLING;
            return StepResult.NORMAL;
        }
        private bool CheckHistory(List<bool[][]> history, bool[][] obj) => history.Exists(x => ArrayHelper.ArrayEqual(x, obj));

        private bool Calculate(int x, int y)
        {
            switch (AliveNeighbors(x, y))
            {
                case 0:
                case 1:
                    return false;
                case 2:
                    if (Get(x, y)) return true;
                    else return false;
                case 3:
                    return true;
                default:
                    return false;
            }
        }

        public AerialStateObject GetState()
        {
            var ret = new AerialStateObject
            {
                XDimension = XDimension,
                YDimension = YDimension,
                Version = Config.Version,
                Aerial = InnerArray
            };
            return ret;
        }

        public bool LoadState(AerialStateObject load)
        {
            if (load.Version != Config.Version) return false;

            xDimension = load.XDimension;
            yDimension = load.YDimension;
            InnerArray = load.Aerial;
            iteration = 0;
            History.Clear();
            return true;
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
                        if (InnerArray[i][j]) count++;
                    }
                    catch { }
                }
            }
            if (InnerArray[x][y]) count--;
            return count;
        }
        private bool AnyAlive()
        {
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    if (InnerArray[x][y]) return true;
                }
            }
            return false;
        }
    }
}
