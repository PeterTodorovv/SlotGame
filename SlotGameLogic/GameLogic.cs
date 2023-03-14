using Newtonsoft.Json;
using SlotGame.Logic.Dtos;
using System;

namespace SlotGameLogic
{
    public class GameLogic
    {
        ConfigData config = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText("config.json"));

        private List<Tile> tiles;
        private List<List<List<int>>> reels;
        private int[][] lines;
        private int[][] pays;
        private int reelsCount;
        private int symbolsOnReel;
        private int[][] currentSymbols;
        private decimal balance;
        private decimal bet;
        private decimal betPerLine;

        public GameLogic(decimal _balance, decimal _bet)
        {
            tiles = config.Tiles;
            reels = config.Reels;
            lines = config.Lines;
            pays = config.Pays;
            reelsCount = 5;
            symbolsOnReel = 3;
            currentSymbols = new int[reelsCount][];
            balance = _balance;
            bet = _bet;

            betPerLine = bet / lines.Length;
        }


        public SpinResultDto Spin()
        {
            if(balance < bet)
            {
                throw new Exception("You dont have enough money for this spin");
            }

            Random random = new Random();

            for (int i = 0; i < reelsCount; i++)
            {
                currentSymbols[i] = new int[symbolsOnReel];

                for (int j = 0; j < symbolsOnReel; j++)
                {
                    var currentReel = reels[0][i];
                    currentSymbols[i][j] = currentReel[random.Next(currentReel.Count)];
                }
            }

            
            bool hasMysterySymbol = HasSpecailSymbols(currentSymbols);
            int SpecialSymbolReplacedWith = -1;

            int[][] changedSymbols = currentSymbols;

            if(hasMysterySymbol)
            {
                var changeSmbolsResult = ChangeMysterySymbols(currentSymbols);
                changedSymbols = changeSmbolsResult.Key;
                SpecialSymbolReplacedWith = changeSmbolsResult.Value;
            }

            var winningLinesDto = (FindWinningLines(changedSymbols, lines));
            var Payout = CalculatePayout(winningLinesDto.WinningCombinations, betPerLine);
            balance = CalculateBalace(balance, bet, Payout);

            SpinResultDto result = new SpinResultDto()
            {
                Balance = balance,
                Payout = Payout,
                CurrentSymbols = currentSymbols,
                ChangedSymbols = changedSymbols,
                HasSpecialSymbol = hasMysterySymbol,
                SpecialSymbolReplacedWith = SpecialSymbolReplacedWith,
                WinningLines = winningLinesDto.WinningLines.ToArray()
            };

            return result;
        }

        private decimal CalculateBalace(decimal balance, decimal bet, decimal winninga)
        {
            return balance - bet + winninga;
        }

        private decimal CalculatePayout(List<KeyValuePair<int, int>> winningCombinations, decimal betPerLine)
        {

            decimal totalPayout = winningCombinations
                .SelectMany(winningLine =>
                    pays.Where(pay => pay[0] == winningLine.Key && pay[1] == winningLine.Value)
                        .Select(pay => pay[2] * betPerLine))
                .Sum();


            return totalPayout;
        }
        
        private KeyValuePair<int[][], int> ChangeMysterySymbols(int[][] currentSymbols)
        {
           
                Random random = new Random();
                var symbolToReplaceWith = random.Next(1, tiles.Count - 1);

            currentSymbols = currentSymbols.Select(
                    subArray => subArray.Select(
                         num => num == 10 ? symbolToReplaceWith : num
                        ).ToArray()
                     ).ToArray();

            var result = new KeyValuePair<int[][], int>(currentSymbols, symbolToReplaceWith);
            return result;

        }

        private bool HasSpecailSymbols(int[][] currentSymbols)
        {
            var hasMysterySymbol = false;

            foreach (var reel in currentSymbols)
            {
                foreach (var symbol in reel)
                {
                    if (tiles.Where(x => x.Id == symbol).First().Type == "mystery")
                    {
                        hasMysterySymbol = true;
                        break;
                    }
                }
            }

            return hasMysterySymbol;
        }

        private WinningLinesDto FindWinningLines(int[][] currentSymbols, int[][] lines)
        {
            var winningLinesDto = new WinningLinesDto();

            for (int i = 0; i < lines.Length; i++)
            {
                int sequenceCount = 1;
                var symbol = -1;

                for (int j = 0; j < lines[i].Length -1; j++)
                {
                    var reelNumber = lines[i][j];
                    var nextReelNumber = lines[i][j + 1];
                    symbol = currentSymbols[j][reelNumber];

                    if (currentSymbols[j][reelNumber] == currentSymbols[j + 1][nextReelNumber])
                    {
                        sequenceCount++;
                    }
                    else
                    {
                        if (sequenceCount >= 3)
                        {
                            winningLinesDto.WinningCombinations.Add(new KeyValuePair<int, int>(symbol, sequenceCount));
                            winningLinesDto.WinningLines.Add(lines[i]);
                            sequenceCount = 1;

                            break;
                        }
                        
                        if(j >= 2)
                        {
                            sequenceCount = 1;
                            break;
                        }

                        sequenceCount = 1;
                    }
                }

                if (sequenceCount >= 3)
                {
                    winningLinesDto.WinningCombinations.Add(new KeyValuePair<int, int>(symbol, sequenceCount));
                    winningLinesDto.WinningLines.Add(lines[i]);
                }
            }

            return winningLinesDto;
        }

    }
}