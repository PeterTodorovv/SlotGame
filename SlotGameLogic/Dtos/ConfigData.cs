namespace SlotGame.Logic.Dtos
{
    internal class ConfigData
    {
        public List<Tile> Tiles = new List<Tile>();
        public List<List<List<int>>> Reels { get; set; }
        public int[][] Lines { get; set; }
        public int[][] Pays { get; set; }

    }
}
