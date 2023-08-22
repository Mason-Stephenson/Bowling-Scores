using System;
using BowlingScore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BowlingScores.Tests
{
    [TestClass]
    public class ErrorPath
    {
        /// <summary>
        /// Ensure that when trying to add invalid values as a score for the first delivery, they are not added to the frame object
        /// </summary>
        [DataTestMethod]
        [DataRow("NotGood")]
        [DataRow("Evil")]
        [DataRow("123")]
        [DataRow("S123")]
        [DataRow("A")]
        [DataRow("9!")]
        [DataRow("1111111111111111111111111111")]
        public void ValidateInvalidScoresAreNotAcceptedInFirstDelivery(string badScore)
        {
            // Arrange
            var frame = new Frame();

            // Act & Assert
            Assert.IsFalse(frame.ValidateAndAddScore(badScore)); // ValidateAndAddScore returns false if a bad input is passed
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => frame.Scores[0]); // ArgumentOutOfRangeException means nothing was added to the list
        }

        /// <summary>
        /// Ensure that when trying to add invalid values as a score for the second delivery, they are not added to the frame object
        /// </summary>
        [DataTestMethod]
        [DataRow("5", "NotGood")]
        [DataRow("5", "Evil")]
        [DataRow("5", "6")]
        [DataRow("5", "123")]
        [DataRow("5", "S6")]
        [DataRow("5", "S123")]
        [DataRow("5", "A")]
        [DataRow("5", "5!")]
        [DataRow("5", "1111111111111111111111111111")]
        public void ValidateInvalidScoresAreNotAcceptedInSecondDelivery(string goodScore, string badScore)
        {
            // Arrange
            var frame = new Frame();
            frame.ValidateAndAddScore(goodScore);

            // Act & Assert
            Assert.IsFalse(frame.ValidateAndAddScore(badScore)); // ValidateAndAddScore returns false if a bad input is passed
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => frame.Scores[1]); // ArgumentOutOfRangeException means nothing was added to the list
        }

        /// <summary>
        /// Ensure that when trying to add invalid values as a score for the third delivery, they are not added to the frame object
        /// </summary>
        [DataTestMethod]
        [DataRow("5", "5", "NotGood")]
        [DataRow("5", "5", "Evil")]
        [DataRow("10", "5", "6")]
        [DataRow("5", "5", "123")]
        [DataRow("10", "5", "S6")]
        [DataRow("5", "5", "S123")]
        [DataRow("5", "5", "A")]
        [DataRow("5", "5", "5!")]
        [DataRow("5", "5", "S12")]
        [DataRow("10", "10", "VeryBad")]
        [DataRow("10", "10", "1111111111111111111111111111111")]
        public void ValidateInvalidScoresAreNotAcceptedInThirdDelivery(string goodScore1, string goodScore2, string badScore)
        {
            // Arrange
            var frame = new Frame();
            frame.IsLastFrame = true; // A third delivery can only happen on the last frame
            frame.ValidateAndAddScore(goodScore1);
            frame.ValidateAndAddScore(goodScore2);

            // Act & Assert
            Assert.IsFalse(frame.ValidateAndAddScore(badScore)); // ValidateAndAddScore returns false if a bad input is passed
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => frame.Scores[2]); // ArgumentOutOfRangeException means nothing was added to the list
        }
    }
}
