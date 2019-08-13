using System;
using System.Collections.Generic;

namespace Life.lib
{
    class Aerial
    {
        private bool[][] _innerArray;
        private readonly Random _rnd;
        private readonly List<HistoryElement> _history;
        private int _iteration;
        public Aerial(int x, int y)
        {
            XDimension = x;
            YDimension = y;
            _innerArray = ArrayHelper.CreateArray(x, y);
            _rnd = new Random();
            _iteration = 0;
            _history = new List<HistoryElement>();
        }

        public void Sow(double sowFactor = 0.1)
        {
            for (var x = 0; x < XDimension; x++)
            {
                for (var y = 0; y < YDimension; y++)
                {
                    _innerArray[x][y] = _rnd.NextDouble() <= sowFactor || _innerArray[x][y];
                }
            }
            ClearHistory();
        }

        public int Iteration => _iteration;

        public int XDimension { get; private set; }

        public int YDimension { get; private set; }

        public bool Get(int x, int y) => _innerArray[x][y];
        public void Set(int x, int y, bool value)
        {
            _innerArray[x][y] = value;
            ClearHistory();
        }
        public void Clear()
        {
            _innerArray = ArrayHelper.CreateArray(XDimension, YDimension);
            _iteration = 0;
            ClearHistory();
        }
        public void ClearHistory() => _history.Clear();

        public StepResult Step()
        {
            if (!AnyAlive()) return StepResult.DEAD;

            _history.Add(new HistoryElement()
            {
                Data = _innerArray,
                Iteration = Iteration,
            });
            var tmp = ArrayHelper.CreateArray(XDimension, YDimension);
            for (var x = 0; x < XDimension; x++)
            {
                for (var y = 0; y < YDimension; y++)
                {
                    tmp[x][y] = Calculate(x, y);
                }
            }
            _innerArray = tmp;
            _iteration++;
            return CheckHistory(tmp) ? StepResult.CYCLING : StepResult.NORMAL;
        }
        private bool CheckHistory(bool[][] obj) => _history.Exists(x => ArrayHelper.ArrayEqual(x.Data, obj));

        private void TrimHistory()
        {
            if (_history.Count > Config.HistoryLimit)
            {
                _history.RemoveAt(0);
            }
        }
        private bool Calculate(int x, int y)
        {
            switch (AliveNeighbors(x, y))
            {
                case 0:
                case 1:
                    return false;
                case 2:
                    return Get(x, y);
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
                Aerial = _innerArray
            };
            return ret;
        }

        public bool LoadState(AerialStateObject load)
        {
            if (load.Version != Config.Version) return false;

            XDimension = load.XDimension;
            YDimension = load.YDimension;
            _innerArray = load.Aerial;
            _iteration = 0;
            _history.Clear();
            return true;
        }
        private int AliveNeighbors(int x, int y)
        {
            var count = 0;
            for (var i = x - 1; i <= x + 1; i++)
            {
                for (var j = y - 1; j <= y + 1; j++)
                {
                    var correctedI = i;
                    var correctedJ = j;
                    if (i >= _innerArray.Length)
                    {
                        correctedI -= _innerArray.Length;
                    }
                    if (i < 0)
                    {
                        correctedI += _innerArray.Length;
                    }
                    if (j >= _innerArray[correctedI].Length)
                    {
                        correctedJ -= _innerArray[correctedI].Length;
                    }
                    if (j < 0)
                    {
                        correctedJ += _innerArray[correctedI].Length;
                    }
                    if (_innerArray[correctedI][correctedJ]) count++;
                }
            }
            if (_innerArray[x][y]) count--;
            return count;
        }
        private bool AnyAlive()
        {
            for (var x = 0; x < XDimension; x++)
            {
                for (var y = 0; y < YDimension; y++)
                {
                    if (_innerArray[x][y]) return true;
                }
            }
            return false;
        }
    }
}
