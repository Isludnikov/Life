﻿using System;

namespace Life.lib
{
    class Aerial
    {
        private bool[,] inner_array;
        private readonly int xDimension;
        private readonly int yDimension;
        private readonly Random rnd;
        private int iteration;
        public Aerial(int x, int y)
        {
            xDimension = x;
            yDimension = y;
            inner_array = new bool[x, y];
            rnd = new Random();
            iteration = 0;
        }

        public void Sow(double sowFactor = 0.1)
        {
            for (var x=0; x<xDimension; x++)
            {
                for(var y=0; y<yDimension; y++)
                {
                    inner_array[x, y] = rnd.NextDouble() <= sowFactor ? true : inner_array[x, y];
                }
            }
        }
        public int Iteration() => iteration;
        public int XDimension() => xDimension;
        public int YDimension() => yDimension;
        public bool Get(int x, int y) => inner_array[x, y];

        public void Clear()
        {
            inner_array = new bool[xDimension, yDimension];
            iteration = 0;
        }

        public bool Step()
        {
            if (!AnyAlive()) return false;

            var tmp = new bool[xDimension, yDimension];
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    tmp[x, y] = Calculate(x, y);
                }
            }
            inner_array = tmp;
            iteration++;
            return true;
        }
        private bool Calculate(int x, int y)
        {
            switch(AliveNeighbors(x,y))
            {
                case 0:
                case 1:
                    return false;
                case 2:
                case 3:
                    return true;
                default:
                    return false;
            }
        }
        private int AliveNeighbors(int x, int y)
        {
            int count = 0;
            for (var i = x-1; i <= x+1; i++)
            {
                for (var j = y-1; j <= y+1; j++)
                {
                    try
                    {
                        if (inner_array[i, j] && i != 0 && j != 0) count++;
                    }
                    catch { }
                }
            }
            return count;
        }
        private bool AnyAlive()
        {
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    if (inner_array[x, y]) return true;
                }
            }
            return false;
        }
    }
}