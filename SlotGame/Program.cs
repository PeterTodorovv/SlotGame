using SlotGame.Logic.Dtos;
using SlotGameLogic;

GameLogic gameLogic = new GameLogic(100, 1);

var inputCommand = "";
SpinResultDto spinResult = new SpinResultDto();

while(inputCommand != "exit" && inputCommand!= "e")
{
    Console.WriteLine("Press s to spin the reels or e to exit the game");
    inputCommand = Console.ReadLine();

    Console.Clear();


    if(inputCommand == "spin" || inputCommand == "s")
    {
        try
        {
            spinResult = gameLogic.Spin();
        }
        catch(Exception e)  
        {
            Console.WriteLine(e.Message);
            continue;
        }


        var currentSymbols = spinResult.CurrentSymbols;

        PrintScreen(currentSymbols);

        if(spinResult.HasSpecialSymbol)
        {
            Console.WriteLine($"Mystery symol is replaced with {spinResult.SpecialSymbolReplacedWith}");
            PrintScreen(spinResult.ChangedSymbols);
        }

        if(spinResult.Payout > 0) 
        {
            Console.WriteLine($"You won {spinResult.Payout}$");
            Console.WriteLine("Winning lines:");

            foreach(var line in spinResult.WinningLines)
            {
                Console.WriteLine(string.Join(" ", line));
            }
        }

        Console.WriteLine($"Balance: {spinResult.Balance}$");
    }
    else if(inputCommand != "exit" && inputCommand != "e")
    {
        Console.WriteLine("Invalid command");
    }

}

void PrintScreen(int[][] currentSymbols)
{
    for (int i = 0; i < currentSymbols[0].Length; i++)
    {
        for (int j = 0; j < currentSymbols.Length; j++)
        {
            Console.Write(currentSymbols[j][i] + " ");
        }
        Console.WriteLine();
    }
}


