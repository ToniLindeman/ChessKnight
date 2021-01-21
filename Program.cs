using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessKnight
{
    class Program
    {
        // Target coordinates
        private static int[] targetCoords;
        private static int targetChoice = -1;

        // Knight coordinates.
        private static int[] knightCoords;
        private static int knightChoice = -1;

        private static bool exitProgram = false;

        static void Main(string[] args)
        {
            bool drawTrace;

            // Make a knight
            Knight mKnight = new Knight();

            while (true)
            {
                
                // User selects target coordinates.
                selectCoordinates(0);
                Console.Clear();

                // User selects target coordinates.
                selectCoordinates(1);
                Console.Clear();

                mKnight.initKnight(new int[] { knightCoords[0], knightCoords[1] });

                // Draw board with target and knight without numbers.
                ChessBoard.drawCoordSelection(targetChoice, knightChoice, false);

                // Inform user the program is finding solutions.
                Console.WriteLine("Please wait while program is finding solutions...");

                //ChessBoard.drawBoard(mKnight.currentCoords, target);
                mKnight.genMovesSystematic7(new int[] { targetCoords[0], targetCoords[1] });

                // Clear console
                Console.Clear();

                // Find best move.
                mKnight.findBestMove(targetCoords);

                

                while (true)
                {
                    mKnight.printResults();
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine("0 - Exit program.");
                    Console.WriteLine("1 - Playback first best solution.");
                    Console.WriteLine("2 - Playback all optimum solutions.");
                    Console.WriteLine("3 - Assign new coordinates.");
                    Console.WriteLine("4 - Change board size.");
                    if(mKnight.getSimWallState())
                    {
                        Console.WriteLine("5 - Simulated wall system (ON), toggle + info.");
                    }
                    else
                    {
                        Console.WriteLine("5 - Simulated wall system (OFF), toggle + info.");
                    }
                    Console.Write("Select: ");
                    string answer = Console.ReadLine();
                    if (answer == "0")
                    {
                        exitProgram = true;
                        break;
                    }
                    else if (answer == "1")
                    {
                        Console.Write("Show path(Y/n): ");
                        string pathAnswer = Console.ReadLine();
                        Console.Clear();
                        if(pathAnswer == "n") { drawTrace = false; }
                        else { drawTrace = true; }
                        mKnight.playbackBest(targetCoords, drawTrace);
                        
                    }
                    else if (answer == "2")
                    {
                        Console.Write("Show path(Y/n): ");
                        string pathAnswer = Console.ReadLine();
                        Console.Clear();
                        if (pathAnswer == "n")
                        {
                            drawTrace = false;
                        }
                        else
                        {
                            drawTrace = true;
                        }
                        mKnight.playbackAllBest(targetCoords, drawTrace);
                    }
                    else if (answer == "3")
                    {
                        targetChoice = -1;
                        knightChoice = -1;
                        exitProgram = false;
                        Console.Clear();
                        break;
                    }
                    else if (answer == "4")
                    {
                        Console.WriteLine("---------------------------------------------------------------------------------");
                        Console.WriteLine("*********************************************************************************");
                        Console.WriteLine("Note that larger boards require more time to find solutions for.");
                        Console.WriteLine("The program may take up to several minutes to solve tasks on the largest boards.");
                        Console.WriteLine("*********************************************************************************");
                        Console.WriteLine("---------------------------------------------------------------------------------");
                        while (true)
                        {
                            try
                            {
                                
                                Console.Write("Size (min:5 max:15): ");
                                string sizeAnswer = Console.ReadLine();
                                int sizeInt = int.Parse(sizeAnswer);
                                if(sizeInt < 5 || sizeInt > 15)
                                {
                                    Console.WriteLine("Given value is outside acceptable range.");
                                }
                                else
                                {
                                    ChessBoard.X_MAX = sizeInt - 1;
                                    ChessBoard.Y_MAX = sizeInt - 1;
                                    ChessBoard.rebuildDict();
                                    break;
                                }
                            }
                            catch(System.Exception)
                            {
                                Console.WriteLine("Invalid value given.");
                            }
                        }

                        exitProgram = false;
                        Console.Clear();
                        targetChoice = -1;
                        knightChoice = -1;
                        break;
                    }
                    else if (answer == "5")
                    {
                        Console.WriteLine("---------------------------------------------------------------------------------");
                        Console.WriteLine("*********************************************************************************");
                        Console.WriteLine("The sim wall system attempts to remove inefficient moves,");
                        Console.WriteLine("while also trying to retain most optimum moves.");
                        Console.WriteLine("It will find less solutions but will also be considerably faster,");
                        Console.WriteLine("especially when working with the largest boards.");
                        Console.WriteLine("*********************************************************************************");
                        Console.WriteLine("---------------------------------------------------------------------------------");
                        
                        while (true)
                        {
                            try
                            {

                                Console.WriteLine("0 = Cancel");
                                Console.WriteLine("1 = Toggle On/Off");
                                Console.WriteLine("2 = Demonstration");
                                Console.Write("Select: ");
                                string simAnswer = Console.ReadLine();
                                int simAnswerInt = int.Parse(simAnswer);
                                if (simAnswerInt < 0 || simAnswerInt > 2)
                                {
                                    Console.WriteLine("Given value is outside acceptable range.");
                                }
                                else
                                {
                                    if (simAnswerInt == 0)
                                    {
                                        break;
                                    }
                                    else if (simAnswerInt == 1)
                                    {
                                        mKnight.toggleSimWall();
                                        break;
                                    }
                                    else if(simAnswerInt == 2)
                                    {
                                        ChessBoard.simWallDemo();
                                        Console.WriteLine();
                                    }
                                    
                                }
                            }
                            catch (System.Exception)
                            {
                                Console.WriteLine("Invalid value given.");
                            }
                        }
                        if (mKnight.getSimWallState())
                        {
                            Console.WriteLine("Simulated wall system is now ON.");
                        }
                        else
                        {
                            Console.WriteLine("Simulated wall system is now OFF.");
                        }
                        exitProgram = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.");
                    }
                }

                if(exitProgram)
                {
                    break;
                }
            }
            Console.WriteLine("Program finished.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public static void selectCoordinates(int setOption)
        {
            int maxVal = (ChessBoard.X_MAX + 1) * (ChessBoard.Y_MAX + 1) - 1;
            // setOption 0 => Set target coordinates and target option.
            // setOption 1 => Set knight coordinates and target option.
            while (true)
            {
                ChessBoard.drawCoordSelection(targetChoice, -1, true);
                Console.WriteLine();
                if(setOption == 0)
                {
                    Console.Write("Select target position: ");
                }
                else if(setOption == 1)
                {
                    Console.Write("Select knight position: ");
                }
                
                string choice = Console.ReadLine();
                try
                {
                    int choiceInt = int.Parse(choice);
                    if (choiceInt < 0 || choiceInt > maxVal)
                    {
                        Console.Clear();
                        Console.WriteLine("Value given is not within the acceptable range (0-{0}).", maxVal);
                    }
                    else if (setOption == 1 && choiceInt == targetChoice)
                    {
                        Console.Clear();
                        Console.WriteLine("Knight may not start from same position as target.");
                    }
                    else
                    {
                        // Input was valid, set target coordinates and break out from loop.
                        if(setOption == 0)
                        {
                            targetCoords = new int[] { ChessBoard.getSetupCoord(choiceInt)[0], ChessBoard.getSetupCoord(choiceInt)[1] };
                            targetChoice = choiceInt;
                        }
                        else if(setOption == 1)
                        {
                            knightCoords = new int[] { ChessBoard.getSetupCoord(choiceInt)[0], ChessBoard.getSetupCoord(choiceInt)[1] };
                            knightChoice = choiceInt;
                        }
                        break;
                    }
                }
                // All exceptions may result in same action, that is inform user of invalid input.
                catch (System.Exception)
                {
                    Console.Clear();
                    Console.WriteLine("Input format was invalid, please provide a integer in range 0-{0}).", maxVal);
                }
            }

        }
    }
      
}


