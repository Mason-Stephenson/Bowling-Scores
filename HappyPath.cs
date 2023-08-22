using System;
using System.IO;
using BowlingScore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BowlingScores.Tests
{
    [TestClass]
    public class HappyPath
    {
        /// <summary>
        /// Ensure that scores get added to frame objects properly
        /// F is translated to a score of 0 
        /// Splits are translated to the appropriate score achieved
        /// </summary>
        /// <param name="scoresToAdd">List of score values to add</param>
        /// <param name="expectedScores">List of values expected to be added to the frame object</param>
        [DataTestMethod]
        [DataRow(new string[] { "10" }, new object[] { new int[] { 10 } })]
        [DataRow(new string[] { "1", "2" }, new object[] { new int[] { 1, 2 } })]
        [DataRow(new string[] { "5", "5" }, new object[] { new int[] { 5, 5 } })]
        [DataRow(new string[] { "0", "0" }, new object[] { new int[] { 0, 0 } })]
        [DataRow(new string[] { "F", "9" }, new object[] { new int[] { 0, 9 } })]
        [DataRow(new string[] { "F", "0" }, new object[] { new int[] { 0, 0 } })]
        [DataRow(new string[] { "F", "F" }, new object[] { new int[] { 0, 0 } })]
        [DataRow(new string[] { "S8", "2" }, new object[] { new int[] { 8, 2 } })]
        [DataRow(new string[] { "S8", "0" }, new object[] { new int[] { 8, 0 } })]
        [DataRow(new string[] { "S5", "S3" }, new object[] { new int[] { 5, 3 } })]
        public void ValidateScoresAreAddedToFrame(string[] scoresToAdd, int[] expectedScores)
        {
            // Arrange
            var frame = new Frame();

            // Act
            foreach (string score in scoresToAdd)
            {
                frame.ValidateAndAddScore(score);
            }

            // Assert
            for (int i = 0; i < expectedScores.Length; i++)
            {
                Assert.AreEqual(expectedScores[i], frame.Scores[i]);
            }
        }

        /// <summary>
        /// Ensure that scores get added to the last frame object properly when a mark is scored
        /// F is translated to a score of 0 
        /// Splits are translated to the appropriate score achieved
        /// </summary>
        /// <param name="scoresToAdd"></param>
        /// <param name="expectedScores"></param>
        [DataTestMethod]
        [DataRow(new string[] { "5", "3" }, new object[] { new int[] { 5, 3 } })]
        [DataRow(new string[] { "F", "8" }, new object[] { new int[] { 0, 8 } })]
        [DataRow(new string[] { "10", "10", "10" }, new object[] { new int[] { 10, 10, 10 } })]
        [DataRow(new string[] { "10", "10", "0" }, new object[] { new int[] { 10, 10, 0 } })]
        [DataRow(new string[] { "10", "0", "10" }, new object[] { new int[] { 10, 0, 10 } })]
        [DataRow(new string[] { "0", "10", "10" }, new object[] { new int[] { 0, 10, 10 } })]
        [DataRow(new string[] { "S5", "5", "10" }, new object[] { new int[] { 5, 5, 10 } })]
        [DataRow(new string[] { "5", "5", "S5" }, new object[] { new int[] { 5, 5, 5 } })]
        [DataRow(new string[] { "S5", "5", "0" }, new object[] { new int[] { 5, 5, 0 } })]
        [DataRow(new string[] { "F", "10", "F" }, new object[] { new int[] { 0, 10, 0 } })]
        public void ValidateScoresAreAddedToLastFrame(string[] scoresToAdd, int[] expectedScores)
        {
            // Arrange
            var frame = new Frame();
            frame.IsLastFrame = true;

            // Act
            foreach (string score in scoresToAdd)
            {
                frame.ValidateAndAddScore(score);
            }

            // Assert
            for (int i = 0; i < expectedScores.Length; i++)
            {
                Assert.AreEqual(expectedScores[i], frame.Scores[i]);
            }
        }

        /// <summary>
        /// Ensure scores in a frame are translated to the proper values that would be displayed on the scoreboard
        /// Ensure the total of the scores is calculated properly for the frame
        /// 0 is translated to "-"
        /// </summary>
        /// <param name="score1">Score of the first delivery</param>
        /// <param name="score2">Score of the second delivery</param>
        /// <param name="expectedScoreToDisplay1">Expected value displayed on the scoreboard for the first delivery</param>
        /// <param name="expectedScoreToDisplay2">Expected value displayed on the scoreboard for the second delivery</param>
        /// <param name="expectedTotal">Expected total of the two scores in the frame</param>
        [DataTestMethod]
        [DataRow("1", "6", "1", "6", 7)]
        [DataRow("7", "2", "7", "2", 9)]
        [DataRow("0", "0", "-", "-", 0)]
        [DataRow("0", "3", "-", "3", 3)]
        [DataRow("F", "0", "F", "-", 0)]
        [DataRow("F", "7", "F", "7", 7)]
        [DataRow("F", "F", "F", "F", 0)]
        [DataRow("S5", "2", "S5", "2", 7)]
        [DataRow("S4", "S3", "S4", "S3", 7)]
        [DataRow("S8", "F", "S8", "F", 8)]
        [DataRow("S6", "0", "S6", "-", 6)]
        [DataRow("5", "5", "5", "/", 10)]
        [DataRow("7", "3", "7", "/", 10)]
        [DataRow("F", "10", "F", "/", 10)]
        [DataRow("0", "10", "-", "/", 10)]
        [DataRow("S8", "2", "S8", "/", 10)]
        public void ValidateThatAFrameIsFinalizedProperly(string score1, string score2, string expectedScoreToDisplay1, string expectedScoreToDisplay2, int expectedTotal)
        {
            // Arrange
            var frame = new Frame();
            frame.ValidateAndAddScore(score1);
            frame.ValidateAndAddScore(score2);

            // Act
            frame.FinalizeFrame();

            // Assert
            Assert.AreEqual(expectedTotal, frame.TotalPinsKnocked);
            Assert.AreEqual(expectedScoreToDisplay1, frame.ScoresToDisplay[0]);
            Assert.AreEqual(expectedScoreToDisplay2, frame.ScoresToDisplay[1]);
        }

        /// <summary>
        /// Ensure a strike is translated to the proper values that would be displayed on the scoreboard
        /// Ensure the score for the frame is 10
        /// </summary>
        [TestMethod]
        public void ValidateThatAStrikeIsFinalizedProperly()
        {
            // Arrange
            string scoreToAdd = "10";
            int expectedScore = 10;
            string expectedScoreToDisplay1 = " ";
            string expectedScoreToDisplay2 = "X";
            var frame = new Frame();
            frame.ValidateAndAddScore(scoreToAdd);

            // Act
            frame.FinalizeFrame();

            // Assert
            Assert.AreEqual(expectedScore, frame.TotalPinsKnocked);
            Assert.AreEqual(expectedScoreToDisplay1, frame.ScoresToDisplay[0]);
            Assert.AreEqual(expectedScoreToDisplay2, frame.ScoresToDisplay[1]);
        }

        /// <summary>
        /// Ensure scores in the final frame are translated to the proper values that would be displayed on the scoreboard
        /// Ensure the total of the scores is calculated properly for the frame
        /// </summary>
        /// <param name="score1">Score of the first delivery</param>
        /// <param name="score2">Score of the second delivery</param>
        /// <param name="score3">Score of the third delivery</param>
        /// <param name="expectedScoreToDisplay1">Expected value displayed on the scoreboard for the first delivery</param>
        /// <param name="expectedScoreToDisplay2">Expected value displayed on the scoreboard for the second delivery</param>
        /// <param name="expectedScoreToDisplay3">Expected value displayed on the scoreboard for the third delivery</param>
        /// <param name="expectedTotal">Expected total of the scores in the frame</param>
        [DataTestMethod]
        [DataRow("10", "10", "10", "X", "X", "X", 30)]
        [DataRow("F", "10", "10", "F", "/", "X", 20)]
        [DataRow("7", "3", "10", "7", "/", "X", 20)]
        [DataRow("10", "10", "0", "X", "X", "-", 20)]
        [DataRow("S8", "2", "S5", "S8", "/", "S5", 15)]
        [DataRow("F", "10", "S5", "F", "/", "S5", 15)]
        [DataRow("5", "3", "7", "5", "3", " ", 8)] // This test shows that a 3rd delievery on the 10th frame is ignored if a mark was not achieved
        public void ValidateThatFinalFrameIsFinalizedProperly(string score1, string score2, string score3, string expectedScoreToDisplay1, string expectedScoreToDisplay2, string expectedScoreToDisplay3, int expectedTotal)
        {
            // Arrange
            var frame = new Frame();
            frame.IsLastFrame = true;
            frame.ValidateAndAddScore(score1);
            frame.ValidateAndAddScore(score2);
            frame.ValidateAndAddScore(score3);

            // Act
            frame.FinalizeFrame();

            // Assert
            Assert.AreEqual(expectedTotal, frame.TotalPinsKnocked);
            Assert.AreEqual(expectedScoreToDisplay1, frame.ScoresToDisplay[0]);
            Assert.AreEqual(expectedScoreToDisplay2, frame.ScoresToDisplay[1]);
            Assert.AreEqual(expectedScoreToDisplay3, frame.ScoresToDisplay[2]);
        }

        /// <summary>
        /// Ensure that a perfect game of all strikes is calculated properly
        /// </summary>
        [TestMethod]
        public void ValidateThatAPerfectGameIsCalculatedProperly()
        {
            // Arrange
            const int NUMBER_OF_FRAMES = 10;
            const int LAST_FRAME_INDEX = NUMBER_OF_FRAMES - 1;
            int frameToCalculate = 0;
            int currentFrame = 0;
            Frame[] frames = new Frame[NUMBER_OF_FRAMES];
            for (int i = 0; i < NUMBER_OF_FRAMES; i++)
            {
                frames[i] = new Frame();
                frames[i].ValidateAndAddScore("10");
                if (i == LAST_FRAME_INDEX) //Special condition for the last frame
                {
                    frames[LAST_FRAME_INDEX].IsLastFrame = true;
                    frames[i].ValidateAndAddScore("10");
                    frames[i].ValidateAndAddScore("10");
                }
                frames[i].FinalizeFrame();
                currentFrame = i;
            }

            // Act
            Program.CalculateScores(ref frames, ref frameToCalculate, currentFrame);

            // Assert
            // The score of each frame should be a multiple of 30
            int expectedScore = 30;
            currentFrame = 1;
            foreach (Frame frame in frames)
            {
                Assert.AreEqual(expectedScore * currentFrame, frame.GameTotal);
                currentFrame++;
            }
        }

        /// <summary>
        /// Ensure a full game of varying scores is calculated properly
        /// </summary>
        [TestMethod]
        public void ValidateThatAFullGameIsCalculatedProperly()
        {
            // Arrange
            int[] expectedScores = new int[10] { 30, 57, 76, 85, 95, 104, 124, 143, 152, 180 };
            const int NUMBER_OF_FRAMES = 10;
            int frameToCalculate = 0;
            Frame[] frames = new Frame[NUMBER_OF_FRAMES];
            for (int i = 0; i < NUMBER_OF_FRAMES; i++)
            {
                frames[i] = new Frame();
            }

            frames[0].ValidateAndAddScore("10");
            frames[0].FinalizeFrame();

            frames[1].ValidateAndAddScore("10");
            frames[1].FinalizeFrame();

            frames[2].ValidateAndAddScore("10");
            frames[2].FinalizeFrame();

            frames[3].ValidateAndAddScore("7");
            frames[3].ValidateAndAddScore("2");
            frames[3].FinalizeFrame();

            frames[4].ValidateAndAddScore("S8");
            frames[4].ValidateAndAddScore("2");
            frames[4].FinalizeFrame();

            frames[5].ValidateAndAddScore("F");
            frames[5].ValidateAndAddScore("9");
            frames[5].FinalizeFrame();

            frames[6].ValidateAndAddScore("10");
            frames[6].FinalizeFrame();

            frames[7].ValidateAndAddScore("7");
            frames[7].ValidateAndAddScore("3");
            frames[7].FinalizeFrame();

            frames[8].ValidateAndAddScore("9");
            frames[8].ValidateAndAddScore("0");
            frames[8].FinalizeFrame();

            frames[9].IsLastFrame = true;
            frames[9].ValidateAndAddScore("10");
            frames[9].ValidateAndAddScore("10");
            frames[9].ValidateAndAddScore("8");
            frames[9].FinalizeFrame();

            // Act
            Program.CalculateScores(ref frames, ref frameToCalculate, 9);

            // Assert
            for (int i = 0; i < NUMBER_OF_FRAMES; i++)
            {
                Assert.AreEqual(expectedScores[i], frames[i].GameTotal);
            }
        }

        /// <summary>
        /// Ensure a game of all zeros is calculated properly
        /// </summary>
        [TestMethod]
        public void ValidateThatAFullGameOfZerosIsCalculatedProperly()
        {
            // Arrange
            const int NUMBER_OF_FRAMES = 10;
            const int LAST_FRAME_INDEX = NUMBER_OF_FRAMES - 1;
            int frameToCalculate = 0;
            int currentFrame = 0;
            Frame[] frames = new Frame[NUMBER_OF_FRAMES];
            for (int i = 0; i < NUMBER_OF_FRAMES; i++)
            {
                frames[i] = new Frame();
                frames[i].ValidateAndAddScore("0");
                frames[i].ValidateAndAddScore("0");
                if (i == LAST_FRAME_INDEX)
                {
                    frames[LAST_FRAME_INDEX].IsLastFrame = true;
                }
                frames[i].FinalizeFrame();
                currentFrame = i;
            }

            // Act
            Program.CalculateScores(ref frames, ref frameToCalculate, currentFrame);

            // Assert
            int expectedScore = 0;
            foreach (Frame frame in frames)
            {
                Assert.AreEqual(expectedScore, frame.GameTotal);
            }
        }

        /// <summary>
        /// Ensure that a perfect game of all strikes can be played through the console and is calulated properly
        /// </summary>
        [TestMethod]
        public void ValidateThatAFullPerfectGameCanBePlayed()
        {
            // Arrange
            // String of inputs that is sent to the console for running the game
            string input = "10\r\n10\r\n10\r\n10\r\n10\r\n10\r\n10\r\n10\r\n10\r\n10\r\n10\r\n10\r\n";
            int[] expectedScores = new int[10] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300 };

            // Act
            Frame[] finalFrames = PlayFullGame(input);

            // Assert
            AssertGameResultsAreCorrect(finalFrames, expectedScores);
        }

        /// <summary>
        /// Ensure that a full game can be played through the console and is calulated properly
        /// </summary>
        [TestMethod]
        public void ValidateThatAFullGameCanBePlayed()
        {
            // Arrange
            // String of inputs that is sent to the console for running the game
            string input = "10\r\n10\r\n10\r\n7\r\n2\r\nS8\r\n2\r\nF\r\n9\r\n10\r\n7\r\n3\r\n9\r\n0\r\n10\r\n10\r\n8\r\n";
            int[] expectedScores = new int[10] { 30, 57, 76, 85, 95, 104, 124, 143, 152, 180 };

            // Act
            Frame[] finalFrames = PlayFullGame(input);

            // Assert
            AssertGameResultsAreCorrect(finalFrames, expectedScores);
        }

        /// <summary>
        /// Ensure that a full game of all zeros can be played through the console and is calulated properly
        /// </summary>
        [TestMethod]
        public void ValidateThatAFullGameOfZerosCanBePlayed()
        {
            // Arrange
            // String of inputs that is sent to the console for running the game
            string input = "0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n0\r\n";
            int[] expectedScores = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            // Act
            Frame[] finalFrames = PlayFullGame(input);


            // Assert
            AssertGameResultsAreCorrect(finalFrames, expectedScores);
        }

        /// <summary>
        /// Sets up StringReader with consoleInputs to send inputs to the console and play a full game
        /// Simulates user input through the console
        /// </summary>
        /// <param name="consoleInputs">String of inputs to be sent to the console, the string must have enough inputs for a full game</param>
        public Frame[] PlayFullGame(string consoleInputs)
        {
            StringReader stringReader = new StringReader(consoleInputs);

            Console.SetIn(stringReader);

           return Program.PlayGame();
        }

        public void AssertGameResultsAreCorrect(Frame[] finalFrames, int[] expectedScores)
        {
            for (int i = 0; i < finalFrames.Length; i++)
            {
                Assert.AreEqual(expectedScores[i], finalFrames[i].GameTotal);
            }
        }
    }
}
