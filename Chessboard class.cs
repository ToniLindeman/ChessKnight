using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessKnight
{
    class ChessBoard
    {
        public static int X_MAX = 7;
        public static int X_MIN = 0;
        public static int Y_MAX = 7;
        public static int Y_MIN = 0;
        private static List<int[]> pathList = new List<int[]>();
        private static List<int[]> currentPath;

        // Method to build coordinate dictionary.
        private static Dictionary<int, int[]> buildCoordDict()
        {
            Dictionary<int, int[]> returnDict = new Dictionary<int, int[]>();

            // Double loop to create coordinates.
            for (int i = 0; i <= Y_MAX; i++)
            {
                for (int j = 0; j <= X_MAX; j++)
                {
                    int[] newCoords = new int[] { j, i };
                    int currentPos = j + (i * (Y_MAX + 1));
                    returnDict.Add(currentPos, newCoords);
                }
            }
            return returnDict;
        }
        // Dictionary to hold coordinate(key) -> int(value) pairs.
        private static Dictionary<int, int[]> coordDictionary = buildCoordDict();

        // Method to get dictionary value
        public static int[] getSetupCoord(int mKey)
        {
            return coordDictionary[mKey];
        }

        // Rebuild dictionary
        public static void rebuildDict()
        {
            coordDictionary = buildCoordDict();
        }

        // pxy => int array that holds current player coordinates.
        // txy => Target coordinates.
        public static void drawBoard(int[] pxy, int[] txy, bool withPath, List<int[]> path)
        {
            currentPath = path;
            for (int i = 0; i < (Y_MAX + 2) * 2; i++)
            {
                // Draw x header
                if (i == 0)
                {
                    Console.Write(" ");
                    for (int j = 0; j < X_MAX + 1; j++)
                    {
                        string xHeader;
                        if (j < 11)
                        {
                            xHeader = "   " + j.ToString();
                        }
                        else
                        {
                            xHeader = "  " + j.ToString();
                        }
                        Console.Write(xHeader);
                    }
                }
                else if (i % 2 == 1)
                {
                    Console.Write("  -");
                    for (int p = 0; p <= X_MAX; p++)
                    {
                        Console.Write("----");
                    }
                }
                else
                {
                    int yInt = i / 2 - 1;
                    string yStr = yInt.ToString();
                    if (yInt < 10)
                    {
                        Console.Write(yStr + " |");
                    }
                    else
                    {
                        Console.Write(yStr + "|");
                    }
                    for (int j = 0; j < X_MAX + 1; j++)
                    {
                        if (i / 2 - 1 == pxy[1] && j == pxy[0] && i / 2 - 1 == txy[1] && j == txy[0])
                        {
                            Console.Write(" X |");
                        }
                        else if (i / 2 - 1 == pxy[1] && j == pxy[0])
                        {
                            Console.Write(" K |");
                        }
                        else if (i / 2 - 1 == txy[1] && j == txy[0])
                        {
                            Console.Write(" T |");
                        }
                        else if(withPath && checkPath(j, i / 2 - 1) != -1)
                        {
                            int val = checkPath(j, i / 2 - 1);
                            Console.Write("-{0}-|", val);
                        }
                        else
                        {
                            Console.Write("   |");
                        }
                    }
                }
                Console.WriteLine("");
            }
        }
        // Check if given coordinates are part of path
        private static int checkPath(int x, int y)
        {
            for(int i = 0; i < currentPath.Count; i++)
            {
                if (x == currentPath[i][0] && y == currentPath[i][1])
                {
                    return i;
                }
            }
            return -1;
        }
        // Clear pathList
        public static void clearPathList()
        {
            pathList.Clear();
        }
        // Draw coordinate selection board
        public static void drawCoordSelection(int targetPosition, int knightPosition, bool drawNumbers)
        {
            for (int i = 0; i < (Y_MAX + 2) * 2; i++)
            {
                // Draw x header
                if (i == 0)
                {
                    Console.Write(" ");
                    for (int j = 0; j < X_MAX + 1; j++)
                    {
                        string xHeader;
                        if (j < 11)
                        {
                            xHeader = "   " + j.ToString();
                        }
                        else
                        {
                            xHeader = "  " + j.ToString();
                        }
                        Console.Write(xHeader);
                    }
                }
                else if (i % 2 == 1)
                {
                    Console.Write("  -");
                    for (int p = 0; p <= X_MAX; p++)
                    {
                        Console.Write("----");
                    }
                }
                else
                {
                    int yInt = i / 2 - 1;
                    string yStr = yInt.ToString();
                    if(yInt < 10)
                    {
                        Console.Write(yStr + " |");
                    }
                    else
                    {
                        Console.Write(yStr + "|");
                    }
                    for (int j = 0; j < X_MAX + 1; j++)
                    {
                        int currentPos = (j + ((i / 2 - 1) * (X_MAX + 1)));
                        string positionStr = currentPos.ToString();
                        if (currentPos == targetPosition)
                        {
                            Console.Write(" T |");
                        }
                        else if (currentPos == knightPosition)
                        {
                            Console.Write(" K |");
                        }
                        else if (!drawNumbers)
                        {
                            Console.Write("   |");
                        }
                        else if (positionStr.Length == 1)
                        {
                            Console.Write(" " + positionStr + " |");
                        }
                        else if (positionStr.Length == 2)
                        {
                            Console.Write(" " + positionStr + "|");
                        }
                        else
                        {
                            Console.Write("" + positionStr + "|");
                        }

                    }
                }
                Console.WriteLine("");
            }
        }
        // Draw simulated wall system demo.
        public static void simWallDemo()
        {
            int targetPosition, knightPosition, mXmax, mYmax;

            for(int m = 0; m < 2; m++)
            {
                Console.WriteLine();
                if (m == 0)
                {
                    targetPosition = 9;
                    knightPosition = 46;
                    mXmax = 6;
                    mYmax = 5;
                    Console.WriteLine("1)");
                }
                else
                {
                    targetPosition = 9;
                    knightPosition = 36;
                    mXmax = 4;
                    mYmax = 4;
                    Console.WriteLine("2)");
                }
                for (int i = 0; i < 18; i++)
                {
                    // Draw x header
                    if (i == 0)
                    {
                        Console.Write(" ");
                        for (int j = 0; j < 8; j++)
                        {
                            string xHeader;
                            if (j < 11)
                            {
                                xHeader = "   " + j.ToString();
                            }
                            else
                            {
                                xHeader = "  " + j.ToString();
                            }
                            Console.Write(xHeader);
                        }
                    }
                    else if (i % 2 == 1)
                    {
                        Console.Write("  -");
                        for (int p = 0; p < 8; p++)
                        {
                            Console.Write("----");
                        }
                    }
                    else
                    {
                        int yInt = i / 2 - 1;
                        string yStr = yInt.ToString();
                        if (yInt < 10)
                        {
                            Console.Write(yStr + " |");
                        }
                        else
                        {
                            Console.Write(yStr + "|");
                        }
                        for (int j = 0; j < 7 + 1; j++)
                        {
                            int currentPos = (j + ((i / 2 - 1) * (7 + 1)));
                            string positionStr = currentPos.ToString();
                            if (currentPos == targetPosition)
                            {
                                Console.Write(" T |");
                            }
                            else if (currentPos == knightPosition)
                            {
                                Console.Write(" K |");
                            }
                            else if (((i - 1) / 2) > mYmax || j > mXmax)
                            {
                                Console.Write("###|");
                            }
                            else
                            {
                                Console.Write("   |");
                            }

                        }
                    }
                    Console.WriteLine("");
                }
            }
            
        }
    }
}
