namespace SlotGame.Logic.Dtos
{
    public class SpinResultDto
    {
        public decimal Balance { get; set; }
        public decimal Payout { get; set; }
        public int[][] CurrentSymbols { get; set; }
        public bool HasSpecialSymbol { get; set; }
        public int[][] ChangedSymbols { get; set; }
        public int SpecialSymbolReplacedWith { get; set; }
        public int[][] WinningLines { get; set; }
    }
}
