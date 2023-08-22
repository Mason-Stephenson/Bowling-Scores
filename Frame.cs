using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingScore
{
    public class Frame
    {
        private const int NUMBER_OF_PINS = 10;
        public bool IsLastFrame { get; set; } // Used to indicate if the frame is the last frame
        public bool FramePlayed { get; set; } // Used to indicate if a frame has been played, used for calculating scores
        public bool MarkAchieved { get; set; } // Used to indicate if a mark (strike or spare) was achieved in the frame
        public List<int> Scores { get; set; } // Number of pins knocked down for each delivery in a single frame. The Count of this list is the number of deliveries
        public List<bool> Fouls { get; set; } // Used to indicate if the player fouled on a delivery
        public List<bool> Splits { get; set; } // Used to indicate if the results of a delivery were a split
        public List<string> ScoresToDisplay { get; set; } // The values that will be displayed on the scoreboard to represent individual deliveries, marks filled in where appropriate
        public int TotalPinsKnocked { get; set; } // Number of pins knocked down in a single frame
        public int TotalForFrame { get; set; } // Total score achieved in a single frame, factoring in extra deliveries achieved from marks
        public int GameTotal { get; set; } // The player's total score up to the current frame. The sum of TotalForFrame for current and all previous frames

        public Frame()
        {
            IsLastFrame = false;
            FramePlayed = false;
            MarkAchieved = false;
            Scores = new List<int>();
            Fouls = new List<bool>();
            Splits = new List<bool>();
            ScoresToDisplay = new List<string>();
            TotalPinsKnocked = -1;
            TotalForFrame = -1;
            GameTotal = -1;
        }

        /// <summary>
        /// Validates user input to ensure that the value is correct
        /// If valid input was entered, the score is recorded for the delivery
        /// Valid inputs are "0-10", "F", and "S(0-10)"
        /// </summary>
        /// /// <param name="input">User input to be validated, if valid the score will be add to the Scores list</param>
        /// <returns>Rerturns true if the user input is valid, returns false if the input is invalid</returns>
        public bool ValidateAndAddScore(string input)
        {
            bool validScore = false;
            int score = -1;

            if (input == "F")
            {
                AddScore(input);
                validScore = true;
            }
            else
            {
                bool isInt;
                if (input.Count() == 2 && input[0] == 'S' && Char.IsDigit(input[1]))
                {
                    isInt = int.TryParse(input[1].ToString(), out score);
                }
                else
                {
                    isInt = int.TryParse(input, out score);
                }

                if (IsLastFrame && isInt) // Special validation required for the 10th frame
                {
                    if (MarkAchieved)
                    {
                        if (Scores.Last() == NUMBER_OF_PINS) // Strike in the last frame
                        {
                            if (score >= 0 && score <= NUMBER_OF_PINS)
                            {
                                try
                                {
                                    AddScore(input);
                                    validScore = true;
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS}, F, S(0-{NUMBER_OF_PINS}))");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS}, F, S(0-{NUMBER_OF_PINS}))");
                            }
                        }
                        else if (Scores.Sum() == NUMBER_OF_PINS) // Spare in the last frame
                        {
                            if (score >= 0 && score <= NUMBER_OF_PINS)
                            {
                                try
                                {
                                    AddScore(input);
                                    validScore = true;
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS}, F, S(0-{NUMBER_OF_PINS}))");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS}, F, S(0-{NUMBER_OF_PINS}))");
                            }
                        }
                        else if (Scores.Last() < NUMBER_OF_PINS) //Strike in first delivery, Open in second
                        {
                            if (score >= 0 && (Scores.Last() + score) <= NUMBER_OF_PINS)
                            {
                                try
                                {
                                    AddScore(input);
                                    validScore = true;
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS - Scores.Last()}, F, S(0-{NUMBER_OF_PINS - Scores.Last()}))");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS - Scores.Last()}, F, S(0-{NUMBER_OF_PINS - Scores.Last()}))");
                            }
                        }
                    }
                    else if (score >= 0 && Scores.Sum() + score <= NUMBER_OF_PINS) // Open in the last frame
                    {
                        try
                        {
                            AddScore(input);
                            validScore = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS - Scores.Sum()}, F, S(0-{NUMBER_OF_PINS - Scores.Sum()}))");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS - Scores.Sum()}, F, S(0-{NUMBER_OF_PINS - Scores.Sum()}))");
                    }
                }
                else if (score >= 0 && (Scores.Sum() + score) <= NUMBER_OF_PINS && isInt) // Validation for frames other than the 10th frame
                {
                    try
                    {
                        AddScore(input);
                        validScore = true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS - Scores.Sum()}, F, S(0-{NUMBER_OF_PINS - Scores.Sum()}))");
                    }
                }
                else
                {
                    Console.WriteLine($"Incorrect input, please enter a valid score (0-{NUMBER_OF_PINS - Scores.Sum()}, F, S(0-{NUMBER_OF_PINS - Scores.Sum()}))");
                }
            }

            return validScore;
        }

        /// <summary>
        /// Add the number of pins knocked down to the list of scores. If 10 pins were knocked down, set MarkAchieved to true to indicate a strike or spare
        /// If a foul is reported, a score of 0 is added and a foul is recorded for the delivery
        /// If a split is reported, the appropriate score is added and a split is recorded for the delivery
        /// </summary>
        /// <param name="score">Number of pins knocked down</param>
        private void AddScore(string score)
        {
            if (score == "F")
            {
                Fouls.Add(true);
                score = "0";
                Splits.Add(false);
            }
            else if (score[0] == 'S')
            {
                Splits.Add(true);
                score = score[1].ToString();
                Fouls.Add(false);
            }
            else
            {
                Fouls.Add(false);
                Splits.Add(false);
            }

            Scores.Add(int.Parse(score));
            if (Scores.Sum() == 10)
            {
                MarkAchieved = true;
            }
        }

        /// <summary>
        /// Adjust scores to be displayed on the scoreboard with the appropriate marks
        /// </summary>
        public void FinalizeFrame()
        {
            FramePlayed = true;
            TotalPinsKnocked = Scores.Sum();

            if (MarkAchieved) // All pins knocked down, replace scores with appropriate marks
            {
                if (Scores[0] == 10) // If a score of 10 is achieved on the 1st delivery, the player scored a strike
                {
                    if (IsLastFrame) // If the strike happened on the 10th frame, display all three deliveries
                    {
                        DisplaySingleScore(0);
                        // Display the results of the 2nd and 3rd deliveries appropriately
                        if (Scores[1] == 10) // If a strike was scored on the 2nd delivery of the 10th frame
                        {
                            DisplaySingleScore(1);

                            // Display the results of the thrid devliery appropriately
                            DisplaySingleScore(2);
                        }
                        else if ((Scores[1] + Scores[2]) == 10) // If a spare was scored on the 2nd and 3rd deliveries of the 10th frame
                        {
                            DisplaySingleScore(1);
                            ScoresToDisplay.Add("/");
                        }
                        else //If a mark was not acheived in the 2nd and 3rd delivery of the 10th frame
                        {
                            DisplaySingleScore(1);
                            DisplaySingleScore(2);
                        }
                    }
                    else // If the strike happened on a frame other than the 10th frame, display an "X" appropriately
                    {
                        ScoresToDisplay.Add(" ");
                        DisplaySingleScore(0);
                    }
                }
                else if ((Scores[0] + Scores[1]) == 10) // If a score of 10 is achieved in 2 deliveries, the player scored a spare, display score for first delivery and a "/" for the second
                {
                    DisplaySingleScore(0);
                    ScoresToDisplay.Add("/");

                    if (IsLastFrame) // If the spare happened on the 10th frame. Display the 3rd delivery
                    {
                        DisplaySingleScore(2);
                    }
                }
            }
            else // If an open is achieved, display scores for both deliveries
            {
                DisplaySingleScore(0);
                DisplaySingleScore(1);

                if (IsLastFrame) //If an open was achieved on the last frame, add an empty score so the scoreboard displays properly
                {
                    ScoresToDisplay.Add(" ");
                }
            }
        }

        /// <summary>
        /// Takes a single score and converts it to the proper string values to be displayed on the scoreboard
        /// </summary>
        /// <param name="score">Score to be displayed on the scoreboard</param>
        private void DisplaySingleScore(int indexOfScore)
        {
            if (Scores[indexOfScore] == 10)
            {
                ScoresToDisplay.Add("X");
            }
            else if (Scores[indexOfScore] == 0)
            {
                if (Fouls[indexOfScore])
                {
                    ScoresToDisplay.Add("F");
                }
                else
                {
                    ScoresToDisplay.Add("-");
                }
            }
            else
            {
                if (Splits[indexOfScore])
                {
                    ScoresToDisplay.Add($"S{Scores[indexOfScore]}");
                }
                else
                {
                    ScoresToDisplay.Add($"{Scores[indexOfScore]}");
                }
            }
        }
    }
}
