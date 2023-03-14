namespace SlotGame.Logic.Dtos
{
    internal class WinningLinesDto
    {
        public List<KeyValuePair<int, int>> WinningCombinations= new List<KeyValuePair<int, int>>();
        public List<int[]> WinningLines = new List<int[]>();
    }
}
