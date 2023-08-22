using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BowlingScore
{
    public class Program
    {
        static void Main(string[] args)
        {
            PlayGame();
            Console.ReadKey();
        }

        /// <summary>
        /// Call to start playing a game
        /// </summary>
        public static Frame[] PlayGame()
        {
            const int NUMBER_OF_FRAMES = 10;
            const int LAST_FRAME_INDEX = NUMBER_OF_FRAMES - 1;
            int currentFrame = 0;
            int frameToCalculate = 0;
            Frame[] frames = new Frame[NUMBER_OF_FRAMES];
            for (int i = 0; i < NUMBER_OF_FRAMES; i++)
            {
                frames[i] = new Frame();
            }
            frames[LAST_FRAME_INDEX].IsLastFrame = true;

            foreach (Frame frame in frames)
            {
                PrintFrames(frames);
                Console.WriteLine($"\n*Frame {currentFrame + 1}*");

                do
                {
                    Console.Write("Number of pins knocked down in first delivery: ");
                } while (!frame.ValidateAndAddScore(Console.ReadLine().Trim().ToUpper()));

                if (frame.IsLastFrame) // The player always bowls twice on the 10th frame
                {
                    do
                    {
                        Console.Write("Number of pins knocked down in second delivery: ");
                    } while (!frame.ValidateAndAddScore(Console.ReadLine().Trim().ToUpper()));

                    if (frame.MarkAchieved) // If the player achieved a mark anytime in the 10th frame, a third delivery is needed
                    {
                        do
                        {
                            Console.Write("Number of pins knocked down in third delivery: ");
                        } while (!frame.ValidateAndAddScore(Console.ReadLine().Trim().ToUpper()));
                    }
                }
                else if (!frame.MarkAchieved) // If a mark was not achieved in the first delivery, the player did not score a strike, a second delivery is needed
                {
                    do
                    {
                        Console.Write("Number of pins knocked down in second delivery: ");
                    } while (!frame.ValidateAndAddScore(Console.ReadLine().Trim().ToUpper()));
                }
                frame.FinalizeFrame();

                CalculateScores(ref frames, ref frameToCalculate, currentFrame);

                currentFrame++;
            }

            PrintFrames(frames);

            return frames;
        }

        /// <summary>
        /// Attempts to calculate the GameTotal for the current frame. If the score cannot be calculated due to marks, continue play until proper conditions are met
        /// </summary>
        /// <param name="frames">The current frames for the game played</param>
        /// <param name="frameToCalculate">The next frame to attempt to calculate the score of</param>
        /// <param name="lastFramePlayed">The most recently played frame</param>
        public static void CalculateScores(ref Frame[] frames, ref int frameToCalculate, int lastFramePlayed)
        {
            // Get the GameTotal up to this point in the game to be used for calculations
            int sumOfScores;
            if (frameToCalculate == 0) // If trying to calculate the first frame, the score at this point in the game is 0
            {
                sumOfScores = 0;
            }
            else // Otherwise, get GameTotal from the frame before the one being calculated
            {
                sumOfScores = frames[frameToCalculate - 1].GameTotal;
            }

            while (frameToCalculate <= lastFramePlayed)
            {
                if (frames[frameToCalculate].MarkAchieved) // All pins knocked down, calculate appropriately based on the mark
                {
                    if (frames[frameToCalculate].Scores[0] == 10) // Strike, calculate with the current frame and the next two deliveries
                    {
                        if (frames[frameToCalculate].IsLastFrame) // If the frame being calculated is the 10th frame, calculate the score using the 10th frame only
                        {
                            frames[frameToCalculate].TotalForFrame = frames[frameToCalculate].TotalPinsKnocked;
                            sumOfScores += frames[frameToCalculate].TotalForFrame;
                            frames[frameToCalculate].GameTotal = sumOfScores;
                            frameToCalculate++;
                        }
                        else if (frames[frameToCalculate + 1].FramePlayed) // The next frame needs to be played before an attempt can be made to calculate scores
                        {
                            if (frames[frameToCalculate + 1].Scores.Count() >= 2) // If two deliveries were played in the next frame, only the next frame is needed
                            {
                                frames[frameToCalculate].TotalForFrame = frames[frameToCalculate].TotalPinsKnocked + frames[frameToCalculate + 1].Scores[0] + frames[frameToCalculate + 1].Scores[1];
                                sumOfScores += frames[frameToCalculate].TotalForFrame;
                                frames[frameToCalculate].GameTotal = sumOfScores;
                                frameToCalculate++;
                            }
                            else if (frames[frameToCalculate + 2].FramePlayed) // If two deliveries were not played in the next frame, the next two frames are needed
                            {
                                frames[frameToCalculate].TotalForFrame = frames[frameToCalculate].TotalPinsKnocked + frames[frameToCalculate + 1].TotalPinsKnocked + frames[frameToCalculate + 2].Scores[0];
                                sumOfScores += frames[frameToCalculate].TotalForFrame;
                                frames[frameToCalculate].GameTotal = sumOfScores;
                                frameToCalculate++;
                            }
                            else
                            {
                                // Proper conditons have not been met to calculate scores, break out of loop and continue play
                                break;
                            }
                        }
                        else
                        {
                            // Proper conditons have not been met to calculate scores, break out of loop and continue play
                            break;
                        }
                    }
                    else if ((frames[frameToCalculate].Scores[0] + frames[frameToCalculate].Scores[1]) == 10) //Spare, calculate with the current frame and the next delivery
                    {
                        if (frames[frameToCalculate].IsLastFrame) // If the frame being calculated is the 10th frame, calculate the score using the 10th frame only
                        {
                            frames[frameToCalculate].TotalForFrame = frames[frameToCalculate].TotalPinsKnocked;
                            sumOfScores += frames[frameToCalculate].TotalForFrame;
                            frames[frameToCalculate].GameTotal = sumOfScores;
                            frameToCalculate++;
                        }
                        else if (frames[frameToCalculate + 1].FramePlayed) // Can only calculate the score if the next frame has been played
                        {
                            frames[frameToCalculate].TotalForFrame = frames[frameToCalculate].TotalPinsKnocked + frames[frameToCalculate + 1].Scores[0];
                            sumOfScores += frames[frameToCalculate].TotalForFrame;
                            frames[frameToCalculate].GameTotal = sumOfScores;
                            frameToCalculate++;
                        }
                        else
                        {
                            // Proper conditons have not been met to calculate scores, break out of loop and continue play
                            break;
                        }
                    }
                }
                else // No mark achieved, calculate with just the current frame
                {
                    frames[frameToCalculate].TotalForFrame = frames[frameToCalculate].TotalPinsKnocked;
                    sumOfScores += frames[frameToCalculate].TotalForFrame;
                    frames[frameToCalculate].GameTotal = sumOfScores;
                    frameToCalculate++;
                }
            }
        }

        /// <summary>
        /// Print a list of frames to the screen
        /// </summary>
        /// <param name="frames">Frames to print to the screen</param>
        public static void PrintFrames(Frame[] frames)
        {
            Console.Clear();

            Console.WriteLine("|   1   |   2   |   3   |   4   |   5   |   6   |   7   |   8   |   9   |     10    |");
            Console.WriteLine("|_______|_______|_______|_______|_______|_______|_______|_______|_______|___________|");

            string frameString = "|";
            string totalsString = "|";
            foreach (Frame frame in frames)
            {
                // If a frame has been played, display the results of that frame
                if (frame.FramePlayed)
                {
                    foreach (string score in frame.ScoresToDisplay)
                    {
                        frameString += $"{score} |".PadLeft(4);
                    }
                }
                else // If a frame hasn't been played, no scores to display
                {
                    if (frame.IsLastFrame)
                    {
                        frameString += $"   |   |   |";
                    }
                    else
                    {
                        frameString += $"   |   |";
                    }
                }

                if (frame.GameTotal > -1) // If the GameTotal for the frame has been calculated, display it
                {
                    if (frame.IsLastFrame)
                    {
                        totalsString += $"{frame.GameTotal}".PadRight(11) + "|";
                    }
                    else
                    {
                        totalsString += $"{frame.GameTotal}".PadRight(7) + "|";
                    }
                }
                else // If the GameTotal for the frame has not been calculated, display a blank
                {
                    if (frame.IsLastFrame)
                    {
                        totalsString += $"           |";
                    }
                    else
                    {
                        totalsString += $"       |";
                    }
                }
            }

            Console.WriteLine(frameString);
            Console.WriteLine(totalsString);
        }
    }
}
