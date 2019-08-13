namespace Life.lib
{
    internal class ArrayHelper
    {
        public static bool[][] CreateArray(int x, int y)
        {
            var r = new bool[x][];
            for (var i = 0; i < x; i++)
            {
                r[i] = new bool[y];
            }
            return r;
        }
        public static bool ArrayEqual(bool[][] a, bool[][] b)
        {
            if (a.Length != b.Length) return false;

            for (var x = 0; x < a.Length; x++)
            {
                if (a[x].Length != b[x].Length) return false;
                for (var y = 0; y < a[x].Length; y++)
                {
                    if (a[x][y] != b[x][y]) return false;
                }
            }
            return true;
        }
    }
}
