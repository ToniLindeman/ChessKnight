using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChessKnight
{
    class Knight
    {
        // Current coordinates
        public int[] currentCoords;

        // Original coordinates, use for example when sequence fails and knight needs to move to original position.
        private int[] originalCoords;

        // Best move sequence.
        private List<int[]> bestMoveSet;

        // All best move sequences.
        private List<List<int[]>> allBestMoveSets;

        // Array to hold all possible moves.
        int[][] possibleMoves;

        // Save all successfull sequences in this list of lists of int arrays.
        List<List<int[]>> successfulSequences;

        // Sim wall variables
        private int wallXMax;
        private int wallXMin;
        private int wallYMax;
        private int wallYMin;
        private bool simWallOn;

        // Variables to hold time taken by search methods.
        DateTime methodTime;

        // Constructor(s) for knight class.
        public Knight()
        {
            // Initially set all wall values to chessboard min and max values.
            wallXMax = ChessBoard.X_MAX;
            wallXMin = ChessBoard.X_MIN;
            wallYMax = ChessBoard.Y_MAX;
            wallYMin = ChessBoard.Y_MIN;

            // Set all possible moves.
            possibleMoves = new int[][] { new int[]{ 2, -1 }, new int[]{ 1, -2 }, new int[]{ -1, -2 }, new int[]{ -2, -1 },
                new int[]{ -2, 1 }, new int[]{ -1, 2 }, new int[]{ 1, 2 }, new int[]{ 2, 1 } };
        }

        public void initKnight(int[] initCoords)
        {
            // Initialize successful sequences.
            successfulSequences = new List<List<int[]>>();

            // Initialize best move
            bestMoveSet = new List<int[]>();

            // Initialize all best moves variable
            allBestMoveSets = new List<List<int[]>>();

            // Check whether valid coordinates were given to knight.
            if (initCoords[0] >= ChessBoard.X_MIN && initCoords[0] <= ChessBoard.X_MAX && initCoords[1] >= ChessBoard.Y_MIN && initCoords[1] <= ChessBoard.Y_MAX)
            {
                currentCoords = new int[] { initCoords[0], initCoords[1] };
                originalCoords = new int[] { initCoords[0], initCoords[1] };

            }
            // If not, initialize knight at 0,7
            else
            {
                Console.WriteLine("Invalid coordinates given, defaulting to 0,7");
                currentCoords = new int[] { 0, 7 };
                originalCoords = new int[] { 0, 7 };
            }
        }

        public DateTime getMethodTimer()
        {
            return methodTime;
        }

        public void toggleSimWall()
        {
            if(simWallOn)
            {
                simWallOn = false;
            }
            else
            {
                simWallOn = true;
            }
        }
        public bool getSimWallState()
        {
            return simWallOn;
        }
        // Set all walls to chessboard values.
        private void resetWall()
        {
            wallXMax = ChessBoard.X_MAX;
            wallXMin = ChessBoard.X_MIN;
            wallYMax = ChessBoard.Y_MAX;
            wallYMin = ChessBoard.Y_MIN;
        }
        // Try to make simulated walls to decrease the amount of possibilites from the early steps, this will greatly decrease computation time while elimating some of the more obvious less optimal moves.
        private void attemptWall(int[] target)
        {
            // First reset wall positions.
            resetWall();

            if (simWallOn)
            {
                int xDiff = currentCoords[0] - target[0];
                int yDiff = currentCoords[1] - target[1];

                int xDiffAbs = Math.Abs(xDiff);
                int yDiffAbs = Math.Abs(yDiff);


                // Only make simWalls if difference of both x and y is more than (-)3 and neither difference equals chessboard max vals.
                if (Math.Abs(xDiff) != ChessBoard.X_MAX && Math.Abs(yDiff) != ChessBoard.Y_MAX)
                {
                    if (xDiffAbs > 3 && yDiffAbs > 3)
                    {
                        // Make a rectangle from currentcoords and target X,Y.
                        if(currentCoords[0] - target[0] < 0)
                        {
                            wallXMin = currentCoords[0];
                            wallXMax = target[0];
                        }
                        else
                        {
                            wallXMin = target[0];
                            wallXMax = currentCoords[0];
                        }
                        if (currentCoords[1] - target[1] < 0)
                        {
                            wallYMin = currentCoords[1];
                            wallYMax = target[1];
                        }
                        else
                        {
                            wallYMin = target[1];
                            wallYMax = currentCoords[1];
                        }
                    }
                    
                }
            }
        }

        private int move(int[] moveSteps, int[] targetCoords, bool winUpdate)
        {
            // Return -1 if move lead out of bounds.
            // Return 0 if move is good but not winning.
            // Return 1 if move is good and WINNING.

            // X out of bounds.
            if (currentCoords[0] + moveSteps[0] < wallXMin || currentCoords[0] + moveSteps[0] > wallXMax)
            {
                // Move failed so we reset current position to original.
                return -1;
            }
            // Y out of bounds.
            else if (currentCoords[1] + moveSteps[1] < wallYMin || currentCoords[1] + moveSteps[1] > wallYMax)
            {
                //Console.WriteLine("Y out of bounds!");
                return -1;
            }

            // Check if winning move has been found.
            else if (currentCoords[0] + moveSteps[0] == targetCoords[0] && currentCoords[1] + moveSteps[1] == targetCoords[1])
            {
                // If current coords x and y now equal target x and y, we have found a successful move.
                if (winUpdate)
                {
                    currentCoords[0] += moveSteps[0];
                    currentCoords[1] += moveSteps[1];
                }
                return 1;
            }
            // Else we add move to current coords and move on.
            else
            {
                currentCoords[0] += moveSteps[0];
                currentCoords[1] += moveSteps[1];
                return 0;
            }
        }
        private int moveAdv(int[] moveSteps, int[] targetCoords, bool winUpdate, Dictionary<char, int[]> steps, char checkStep)
        {
            // Return -1 if move lead out of bounds or if move is back to a previous space.
            // Return 0 if move is good but not winning.
            // Return 1 if move is good and WINNING.

            // X out of bounds.
            if (currentCoords[0] + moveSteps[0] < wallXMin || currentCoords[0] + moveSteps[0] > wallXMax)
            {
                // Move failed so we reset current position to original.
                return -1;
            }
            // Y out of bounds.
            else if (currentCoords[1] + moveSteps[1] < wallYMin || currentCoords[1] + moveSteps[1] > wallYMax)
            {
                //Console.WriteLine("Y out of bounds!");
                return -1;
            }
            // Current move equals going back to previous space, return -1.
            else if (currentCoords[0] + moveSteps[0] == steps[checkStep][0] && currentCoords[1] + moveSteps[1] == steps[checkStep][1])
            {
                return -1;
            }

            // Check if winning move has been found.
            else if (currentCoords[0] + moveSteps[0] == targetCoords[0] && currentCoords[1] + moveSteps[1] == targetCoords[1])
            {
                // If current coords x and y now equal target x and y, we have found a successful move.
                if (winUpdate)
                {
                    currentCoords[0] += moveSteps[0];
                    currentCoords[1] += moveSteps[1];
                }
                return 1;
            }
            // Else we add move to current coords and move on.
            else
            {
                currentCoords[0] += moveSteps[0];
                currentCoords[1] += moveSteps[1];
                return 0;
            }
        }

        public void seqAid(Dictionary<char, int[]> stageSteps, char[] stageSig)
        {
            List<int[]> newEntry = new List<int[]>();
            for (int i = 0; i < stageSig.Length; i++)
            {
                if (i == 0)
                {
                    newEntry.Add(new int[] { stageSteps['a'][0], stageSteps['a'][1] });
                }
                else if (i == 1)
                {
                    newEntry.Add(new int[] { stageSteps['b'][0], stageSteps['b'][1] });
                }
                else if (i == 2)
                {
                    newEntry.Add(new int[] { stageSteps['c'][0], stageSteps['c'][1] });
                }
                else if (i == 3)
                {
                    newEntry.Add(new int[] { stageSteps['d'][0], stageSteps['d'][1] });
                }
                else if (i == 4)
                {
                    newEntry.Add(new int[] { stageSteps['e'][0], stageSteps['e'][1] });
                }
                else if (i == 5)
                {
                    newEntry.Add(new int[] { stageSteps['f'][0], stageSteps['f'][1] });
                }
                else if (i == 6)
                {
                    newEntry.Add(new int[] { stageSteps['g'][0], stageSteps['g'][1] });
                }
                else if (i == 7)
                {
                    newEntry.Add(new int[] { stageSteps['h'][0], stageSteps['h'][1] });
                }
                else if (i == 8)
                {
                    newEntry.Add(new int[] { stageSteps['i'][0], stageSteps['i'][1] });
                }
                else if (i == 9)
                {
                    newEntry.Add(new int[] { stageSteps['j'][0], stageSteps['j'][1] });
                }
                else if (i == 9)
                {
                    newEntry.Add(new int[] { stageSteps['k'][0], stageSteps['k'][1] });
                }
            }

            successfulSequences.Add(newEntry);
        }

        public void genMovesSystematic7(int[] targetCoords)
        {
            // Start time for method
            DateTime startTime = DateTime.Now;

            bool setCurrentCoord = false;

            Dictionary<char, int[]> stageStep = new Dictionary<char, int[]>();
            stageStep.Add('a', new int[] { 0, 0 });
            stageStep.Add('b', new int[] { 0, 0 });
            stageStep.Add('c', new int[] { 0, 0 });
            stageStep.Add('d', new int[] { 0, 0 });
            stageStep.Add('e', new int[] { 0, 0 });
            stageStep.Add('f', new int[] { 0, 0 });
            stageStep.Add('g', new int[] { 0, 0 });
            stageStep.Add('h', new int[] { 0, 0 });
            stageStep.Add('i', new int[] { 0, 0 });
            stageStep.Add('j', new int[] { 0, 0 });
            stageStep.Add('k', new int[] { 0, 0 });

            Dictionary<char, int[]> lastCoords = new Dictionary<char, int[]>();
            lastCoords.Add('a', new int[] { 0, 0 });
            lastCoords.Add('b', new int[] { 0, 0 });
            lastCoords.Add('c', new int[] { 0, 0 });
            lastCoords.Add('d', new int[] { 0, 0 });
            lastCoords.Add('e', new int[] { 0, 0 });
            lastCoords.Add('f', new int[] { 0, 0 });
            lastCoords.Add('g', new int[] { 0, 0 });
            lastCoords.Add('h', new int[] { 0, 0 });
            lastCoords.Add('i', new int[] { 0, 0 });
            lastCoords.Add('j', new int[] { 0, 0 });

            int limit = 150;
            char[] stageSig;

            // variable to tell how many moves the program should go through.
            int stageMod;

            // Systematically go through every possible 10 move combinations
            for (int a = 0; a < possibleMoves.Length; a++)
            {
                // Stage signature
                stageSig = new char[] { 'a' };

                int limitCounter = 0;

                currentCoords[0] = originalCoords[0];
                currentCoords[1] = originalCoords[1];

                // Attempt simWalls
                attemptWall(targetCoords);

                int mIndex = a;

                // Move was successful, add to successfulSequences and continue from top.
                if (move(possibleMoves[mIndex], targetCoords, false) == 0)
                {
                    limitCounter++;

                    stageStep['a'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                    lastCoords['a'] = new int[] { currentCoords[0], currentCoords[1] };
                }
                // Sequence failed continue from top of loop
                else if (move(possibleMoves[mIndex], targetCoords, false) == -1) { continue; }
                // Else add move to runningMoves and move on.
                else
                {
                    stageStep['a'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                    seqAid(stageStep, stageSig);
                    continue;
                }

                // Move to next step
                for (int b = 0; b <= possibleMoves.Length; b++)
                {
                    // Stage signature
                    stageSig = new char[] { 'a', 'b' };
                    if (b == possibleMoves.Length)
                    {
                        setCurrentCoord = true;
                        limitCounter -= 1;
                        break;
                    }
                    if (setCurrentCoord)
                    {
                        currentCoords[0] = lastCoords['a'][0];
                        currentCoords[1] = lastCoords['a'][1];
                        setCurrentCoord = false;
                    }

                    // Attempt simWalls
                    attemptWall(targetCoords);

                    mIndex = b;
                    // Move was successful, add to successfulSequences and continue from top.
                    if (move(possibleMoves[mIndex], targetCoords, false) == 0)
                    {
                        limitCounter++;
                        if (limitCounter > limit)
                        {
                            limitCounter -= 1;
                            setCurrentCoord = true;
                            continue;
                        }

                        stageStep['b'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                        lastCoords['b'] = new int[] { currentCoords[0], currentCoords[1] };
                    }
                    // Sequence failed continue from top of loop
                    else if (move(possibleMoves[mIndex], targetCoords, false) == -1) { continue; }
                    // Else add move to runningMoves and move on.
                    else
                    {
                        stageStep['b'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                        seqAid(stageStep, stageSig);
                        continue;
                    }

                    // Move to next step
                    for (int c = 0; c <= possibleMoves.Length; c++)
                    {
                        // Stage signature
                        stageSig = new char[] { 'a', 'b', 'c' };
                        if (c == possibleMoves.Length)
                        {
                            setCurrentCoord = true;
                            limitCounter -= 1;
                            break;
                        }
                        if (setCurrentCoord)
                        {
                            currentCoords[0] = lastCoords['b'][0];
                            currentCoords[1] = lastCoords['b'][1];
                            setCurrentCoord = false;
                        }

                        // Attempt simWalls
                        attemptWall(targetCoords);

                        mIndex = c;
                        // Move was successful, add to successfulSequences and continue from top.
                        if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'a') == 0)
                        {
                            limitCounter++;
                            if (limitCounter > limit)
                            {
                                limitCounter -= 1;
                                setCurrentCoord = true;
                                continue;
                            }

                            stageStep['c'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                            lastCoords['c'] = new int[] { currentCoords[0], currentCoords[1] };
                        }
                        // Sequence failed continue from top of loop
                        else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'a') == -1) { continue; }
                        // Else add move to runningMoves and move on.
                        else
                        {
                            stageStep['c'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                            seqAid(stageStep, stageSig);
                            continue;
                        }

                        // Move to next step
                        for (int d = 0; d <= possibleMoves.Length; d++)
                        {
                            // Stage signature
                            stageSig = new char[] { 'a', 'b', 'c', 'd' };
                            if (d == possibleMoves.Length)
                            {
                                setCurrentCoord = true;
                                limitCounter -= 1;
                                break;
                            }
                            if (setCurrentCoord)
                            {
                                currentCoords[0] = lastCoords['c'][0];
                                currentCoords[1] = lastCoords['c'][1];
                                setCurrentCoord = false;
                            }

                            // Attempt simWalls
                            attemptWall(targetCoords);

                            mIndex = d;
                            // Move was successful, add to successfulSequences and continue from top.
                            if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'b') == 0)
                            {
                                limitCounter++;
                                if (limitCounter > limit)
                                {
                                    limitCounter -= 1;
                                    setCurrentCoord = true;
                                    continue;
                                }

                                stageStep['d'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                lastCoords['d'] = new int[] { currentCoords[0], currentCoords[1] };
                            }
                            // Sequence failed continue from top of loop
                            else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'b') == -1) { continue; }
                            // Else add move to runningMoves and move on.
                            else
                            {
                                stageStep['d'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                seqAid(stageStep, stageSig);
                                continue;
                            }

                            // Move to next step
                            for (int e = 0; e <= possibleMoves.Length; e++)
                            {
                                // Stage signature
                                stageSig = new char[] { 'a', 'b', 'c', 'd', 'e' };
                                if (e == possibleMoves.Length)
                                {
                                    setCurrentCoord = true;
                                    limitCounter -= 1;
                                    break;
                                }
                                if (setCurrentCoord)
                                {
                                    currentCoords[0] = lastCoords['d'][0];
                                    currentCoords[1] = lastCoords['d'][1];
                                    setCurrentCoord = false;
                                }

                                // Attempt simWalls
                                attemptWall(targetCoords);

                                mIndex = e;
                                // Move was successful, add to successfulSequences and continue from top.
                                if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'c') == 0)
                                {
                                    limitCounter++;
                                    if (limitCounter > limit)
                                    {
                                        limitCounter -= 1;
                                        setCurrentCoord = true;
                                        continue;
                                    }

                                    stageStep['e'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                    lastCoords['e'] = new int[] { currentCoords[0], currentCoords[1] };
                                }
                                // Sequence failed continue from top of loop
                                else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'c') == -1) { continue; }
                                // Else add move to runningMoves and move on.
                                else
                                {
                                    stageStep['e'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                    seqAid(stageStep, stageSig);
                                    continue;

                                }

                                // Move to next step
                                for (int f = 0; f <= possibleMoves.Length; f++)
                                {
                                    stageMod = 8;
                                    // Stage signature
                                    stageSig = new char[] { 'a', 'b', 'c', 'd', 'e', 'f' };
                                    if (f == possibleMoves.Length)
                                    {
                                        setCurrentCoord = true;
                                        limitCounter -= 1;
                                        break;
                                    }
                                    if (setCurrentCoord)
                                    {
                                        currentCoords[0] = lastCoords['e'][0];
                                        currentCoords[1] = lastCoords['e'][1];
                                        setCurrentCoord = false;
                                    }

                                    // Attempt simWalls
                                    attemptWall(targetCoords);

                                    mIndex = f;
                                    if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'd') == 0)
                                    {
                                        limitCounter++;
                                        if (limitCounter > limit)
                                        {
                                            limitCounter -= 1;
                                            setCurrentCoord = true;
                                            continue;
                                        }

                                        stageStep['f'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                        lastCoords['f'] = new int[] { currentCoords[0], currentCoords[1] };
                                    }
                                    // Sequence failed continue from top of loop
                                    else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'd') == -1) { continue; }
                                    // Else add move to runningMoves and move on.
                                    else
                                    {
                                        stageStep['f'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                        seqAid(stageStep, stageSig);
                                        continue;
                                    }

                                    for (int g = 0; g <= possibleMoves.Length; g++)
                                    {
                                        stageMod = 9;
                                        if (ChessBoard.X_MAX < stageMod)
                                        {
                                            setCurrentCoord = true;
                                            break;
                                        }
                                        // Stage signature
                                        stageSig = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };
                                        if (g == possibleMoves.Length)
                                        {
                                            setCurrentCoord = true;
                                            limitCounter -= 1;
                                            break;
                                        }
                                        if (setCurrentCoord)
                                        {
                                            currentCoords[0] = lastCoords['f'][0];
                                            currentCoords[1] = lastCoords['f'][1];
                                            setCurrentCoord = false;
                                        }

                                        // Attempt simWalls
                                        attemptWall(targetCoords);

                                        mIndex = g;
                                        // Move was successful, add to successfulSequences and continue from top.
                                        if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'e') == 0 && ChessBoard.X_MAX == stageMod)
                                        {
                                            setCurrentCoord = true;
                                            continue;
                                        }
                                        else
                                        {
                                            currentCoords[0] = lastCoords['f'][0];
                                            currentCoords[1] = lastCoords['f'][1];
                                        }
                                        if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'e') == 0)
                                        {
                                            limitCounter++;
                                            if (limitCounter > limit)
                                            {
                                                limitCounter -= 1;
                                                setCurrentCoord = true;
                                                continue;
                                            }

                                            stageStep['g'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                            lastCoords['g'] = new int[] { currentCoords[0], currentCoords[1] };
                                        }
                                        // Sequence failed continue from top of loop
                                        else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'e') == -1) { continue; }
                                        // Else add move to runningMoves and move on.
                                        else
                                        {
                                            stageStep['g'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                            seqAid(stageStep, stageSig);
                                            if (ChessBoard.X_MAX == stageMod)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        for (int h = 0; h <= possibleMoves.Length; h++)
                                        {
                                            stageMod = 10;
                                            if (ChessBoard.X_MAX < stageMod)
                                            {
                                                setCurrentCoord = true;
                                                break;
                                            }
                                            // Stage signature
                                            stageSig = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
                                            if (h == possibleMoves.Length)
                                            {
                                                setCurrentCoord = true;
                                                limitCounter -= 1;
                                                break;
                                            }
                                            if (setCurrentCoord)
                                            {
                                                currentCoords[0] = lastCoords['g'][0];
                                                currentCoords[1] = lastCoords['g'][1];
                                                setCurrentCoord = false;
                                            }

                                            // Attempt simWalls
                                            attemptWall(targetCoords);

                                            mIndex = h;
                                            // Move was successful, add to successfulSequences and continue from top.
                                            if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'f') == 0 && ChessBoard.X_MAX == stageMod)
                                            {
                                                setCurrentCoord = true;
                                                continue;
                                            }
                                            else
                                            {
                                                currentCoords[0] = lastCoords['g'][0];
                                                currentCoords[1] = lastCoords['g'][1];
                                            }
                                            if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'f') == 0)
                                            {
                                                limitCounter++;
                                                if (limitCounter > limit)
                                                {
                                                    limitCounter -= 1;
                                                    setCurrentCoord = true;
                                                    continue;
                                                }

                                                stageStep['h'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                                lastCoords['h'] = new int[] { currentCoords[0], currentCoords[1] };
                                            }
                                            // Sequence failed continue from top of loop
                                            else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'f') == -1) { continue; }
                                            // Else add move to runningMoves and move on.
                                            else
                                            {
                                                stageStep['h'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                                seqAid(stageStep, stageSig);
                                                if (ChessBoard.X_MAX == stageMod)
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }

                                            // Move to next step
                                            for (int i = 0; i <= possibleMoves.Length; i++)
                                            {
                                                stageMod = 11;
                                                if (ChessBoard.X_MAX < stageMod)
                                                {
                                                    setCurrentCoord = true;
                                                    break;
                                                }
                                                // Stage signature
                                                stageSig = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i' };
                                                if (i == possibleMoves.Length)
                                                {
                                                    setCurrentCoord = true;
                                                    limitCounter--;
                                                    break;
                                                }
                                                if (setCurrentCoord)
                                                {
                                                    currentCoords[0] = lastCoords['h'][0];
                                                    currentCoords[1] = lastCoords['h'][1];
                                                    setCurrentCoord = false;
                                                }

                                                mIndex = i;

                                                // Move was successful, add to successfulSequences and continue from top.
                                                if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'f') == 0 && ChessBoard.X_MAX == stageMod)
                                                {
                                                    setCurrentCoord = true;
                                                    continue;
                                                }
                                                else
                                                {
                                                    currentCoords[0] = lastCoords['h'][0];
                                                    currentCoords[1] = lastCoords['h'][1];
                                                }
                                                if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'f') == 0)
                                                {
                                                    limitCounter++;
                                                    if (limitCounter > limit)
                                                    {
                                                        limitCounter -= 1;
                                                        setCurrentCoord = true;
                                                        continue;
                                                    }

                                                    stageStep['i'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                                    lastCoords['i'] = new int[] { currentCoords[0], currentCoords[1] };
                                                }
                                                // Sequence failed continue from top of loop
                                                else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'f') == -1) { continue; }
                                                // Else add move to runningMoves and move on.
                                                else
                                                {
                                                    stageStep['i'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                                    seqAid(stageStep, stageSig);
                                                    if (ChessBoard.X_MAX == stageMod)
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }

                                                // Move to next step
                                                for (int j = 0; j <= possibleMoves.Length; j++)
                                                {
                                                    stageMod = 12;
                                                    if (ChessBoard.X_MAX < stageMod)
                                                    {
                                                        setCurrentCoord = true;
                                                        break;
                                                    }
                                                    // Stage signature
                                                    stageSig = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
                                                    if (j == possibleMoves.Length)
                                                    {
                                                        limitCounter--;
                                                        setCurrentCoord = true;
                                                        break;
                                                    }
                                                    if (setCurrentCoord)
                                                    {
                                                        currentCoords[0] = lastCoords['i'][0];
                                                        currentCoords[1] = lastCoords['i'][1];
                                                        setCurrentCoord = false;
                                                    }

                                                    mIndex = j;

                                                    // Move was successful, add to successfulSequences and continue from top.
                                                    if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'g') == 0 && ChessBoard.X_MAX == stageMod ||
                                                        moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'g') == 0 && ChessBoard.X_MAX == stageMod + 1)
                                                    {
                                                        setCurrentCoord = true;
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        currentCoords[0] = lastCoords['i'][0];
                                                        currentCoords[1] = lastCoords['i'][1];
                                                    }
                                                    if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'g') == 0)
                                                    {
                                                        limitCounter++;
                                                        if (limitCounter > limit)
                                                        {
                                                            limitCounter -= 1;
                                                            setCurrentCoord = true;
                                                            continue;
                                                        }

                                                        stageStep['j'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                                        lastCoords['j'] = new int[] { currentCoords[0], currentCoords[1] };
                                                    }
                                                    // Sequence failed continue from top of loop
                                                    else if (moveAdv(possibleMoves[mIndex], targetCoords, false, lastCoords, 'g') == -1) { continue; }
                                                    // Else add move to runningMoves and move on.
                                                    else
                                                    {
                                                        stageStep['j'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                                        seqAid(stageStep, stageSig);
                                                        if(ChessBoard.X_MAX == stageMod || ChessBoard.X_MAX == stageMod + 1)
                                                        {
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                        
                                                    }

                                                    for (int k = 0; k <= possibleMoves.Length; k++)
                                                    {
                                                        stageMod = 14;
                                                        if (ChessBoard.X_MAX < stageMod)
                                                        {
                                                            setCurrentCoord = true;
                                                            break;
                                                        }
                                                        // Stage signature
                                                        stageSig = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k' };
                                                        if (k == possibleMoves.Length)
                                                        {
                                                            limitCounter--;
                                                            setCurrentCoord = true;
                                                            break;
                                                        }
                                                        if (setCurrentCoord)
                                                        {
                                                            currentCoords[0] = lastCoords['j'][0];
                                                            currentCoords[1] = lastCoords['j'][1];
                                                            setCurrentCoord = false;
                                                        }

                                                        mIndex = k;

                                                        if (move(possibleMoves[mIndex], targetCoords, false) == 0)
                                                        {
                                                            setCurrentCoord = true;
                                                            continue;
                                                        }
                                                        // Sequence failed continue from top of loop
                                                        else if (move(possibleMoves[mIndex], targetCoords, false) == -1) { continue; }
                                                        // Else add move to runningMoves and move on.
                                                        else
                                                        {
                                                            stageStep['k'] = new int[] { possibleMoves[mIndex][0], possibleMoves[mIndex][1] };
                                                            seqAid(stageStep, stageSig);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            resetWall();
            DateTime endTime = DateTime.Now;
            methodTime = new DateTime(endTime.Ticks - startTime.Ticks);
        }

        // Playback first best solution found.
        public void playbackBest(int[] target, bool drawTrace)
        {
            // Path coord list
            List<int[]> pathCoords = calcCoordsFromMoves(bestMoveSet);

            currentCoords[0] = originalCoords[0];
            currentCoords[1] = originalCoords[1];

            if (bestMoveSet.Count > 0)
            {

                int counter = 0;
                Console.WriteLine("Move: {0}", 0);
                ChessBoard.drawBoard(currentCoords, target, drawTrace, pathCoords);
                Console.WriteLine("Press any key for next step.");

                // Print each step
                Console.WriteLine("---------------------------------------");
                Console.WriteLine("Current sequence:");
                int moveCounter = 0;
                foreach (int[] move in bestMoveSet)
                {
                    moveCounter++;
                    Console.WriteLine("Move {0} => x: {1} y: {2}", moveCounter, move[0], move[1]);
                }

                // Wait for user.
                Console.ReadKey();

                foreach (int[] mMove in bestMoveSet)
                {
                    counter++;
                    // Move player
                    move(mMove, target, true);
                    // Clear Console
                    Console.Clear();
                    Console.WriteLine("Move: {0}", counter);
                    ChessBoard.drawBoard(currentCoords, target, drawTrace, pathCoords);
                    Console.WriteLine("Press any key for next step.");

                    // Clear trace
                    ChessBoard.clearPathList();

                    // Pause after every move
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("No succesful sequences available.");
            }
        }

        // Playback all best solutions found.
        public void playbackAllBest(int[] target, bool drawTrace)
        {
           

            int seqCounter = 0;
            foreach(List<int[]> seq in allBestMoveSets)
            {
                // Path coord list
                List<int[]> pathCoords = calcCoordsFromMoves(seq);

                // Increment sequence counter
                seqCounter++;

                // Reset current coordinates.
                currentCoords[0] = originalCoords[0];
                currentCoords[1] = originalCoords[1];

                int counter = 0;
                Console.WriteLine("Sequence: {0}/{1}", seqCounter, allBestMoveSets.Count);
                Console.WriteLine("Move: {0}", 0);
                ChessBoard.drawBoard(currentCoords, target, drawTrace, pathCoords);
                Console.WriteLine("Press any key for next step.");

                // Print each step
                Console.WriteLine("---------------------------------------");
                foreach (int[] move in seq)
                {
                    Console.WriteLine("Current sequence => x: {0} y: {1}", move[0], move[1]);
                }

                // Wait for user.
                Console.ReadKey();

                foreach (int[] mMove in seq)
                {
                    counter++;
                    // Move player
                    move(mMove, target, true);
                    // Clear Console
                    Console.Clear();
                    Console.WriteLine("Sequence: {0}/{1}", seqCounter, allBestMoveSets.Count);
                    Console.WriteLine("Move: {0}", counter);
                    ChessBoard.drawBoard(currentCoords, target, drawTrace, pathCoords);
                    Console.WriteLine("Press any key for next step.");
                    // Pause after every move
                    Console.ReadKey();
                }

                // Reset current coordinates.
                currentCoords[0] = originalCoords[0];
                currentCoords[1] = originalCoords[1];

                // Clear trace
                ChessBoard.clearPathList();

                if (seqCounter == allBestMoveSets.Count)
                {
                    break;
                }

                Console.Write("Continue playback (Y/n): ");
                string answer = Console.ReadLine();
                
                // If player chooses to end sequence playback, break out of loop.
                if (answer == "n")
                {
                    break;
                }
                Console.Clear();
            }
        }

        public void findBestMove(int[] target)
        {
            int bestSeq = 11;

            // Go through successfulSequences and register the shortest (only one of the shortest if several exist)
            foreach (List<int[]> seq in successfulSequences)
            {
                if (seq.Count < bestSeq)
                {
                    bestMoveSet = seq;
                    bestSeq = seq.Count;
                }
            }
            foreach (List<int[]> seq in successfulSequences)
            {
                if (seq.Count == bestSeq)
                {
                    allBestMoveSets.Add(seq);
                }
            }

            ChessBoard.drawBoard(originalCoords, target, false, new List<int[]>());
        }
        public void printResults()
        {
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine("Shortest path steps: {0}", bestMoveSet.Count);
            Console.WriteLine("Succesful sequences found: {0}", successfulSequences.Count);
            Console.WriteLine("Optimum sequences found: {0}", allBestMoveSets.Count);
            Console.WriteLine("----------------------------- MM:SS:mmm");
            Console.WriteLine("Time taken to find solutions: {0}:{1}:{2} (min:sec:ms)",
                        getMethodTimer().Minute.ToString("00"),
                        getMethodTimer().Second.ToString("00"),
                        getMethodTimer().Millisecond.ToString("000"));
        }

        private List<int[]> calcCoordsFromMoves(List<int[]> moves)
        {
            List<int[]> returnList = new List<int[]>();
            int[] mCurPos = new int[] { originalCoords[0], originalCoords[1] };
            returnList.Add(new int[] { mCurPos[0], mCurPos[1] });


            for (int i = 0; i < moves.Count; i++)
            {
                mCurPos[0] += moves[i][0];
                mCurPos[1] += moves[i][1];
                returnList.Add(new int[] { mCurPos[0], mCurPos[1] });
            }

            return returnList;
        }
    }

}
