using RivalsGG.Core.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RivalsGG.Test
{
    public class PlayerDTOTests
    {
        [Fact]
        public void PlayerDTO_WithValidData_PassesValidation()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = "ValidName",
                PlayerColor = "#FF5500",
                PlayerIcon = "https://example.com/icon.png"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                playerDto,
                new ValidationContext(playerDto),
                validationResults,
                true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void PlayerDTO_WithInvalidName_FailsValidation()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = "Invalid<Name>BadChars",
                PlayerColor = "#FF5500",
                PlayerIcon = "https://example.com/icon.png"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                playerDto,
                new ValidationContext(playerDto),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("PlayerName"));
        }

        [Fact]
        public void PlayerDTO_WithEmptyName_FailsValidation()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = "", // cannot be empty
                PlayerColor = "#FF5500",
                PlayerIcon = "https://example.com/icon.png"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                playerDto,
                new ValidationContext(playerDto),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("PlayerName"));
        }

        [Fact]
        public void PlayerDTO_WithInvalidColor_FailsValidation()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = "ValidName",
                PlayerColor = "Green", // Invalid color format
                PlayerIcon = "https://example.com/icon.png"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                playerDto,
                new ValidationContext(playerDto),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("PlayerColor"));
        }

        [Fact]
        public void PlayerDTO_WithInvalidIconUrl_FailsValidation()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = "ValidName",
                PlayerColor = "#FF5500",
                PlayerIcon = "not_a_valid_url" // bad url 
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                playerDto,
                new ValidationContext(playerDto),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("PlayerIcon"));
        }

        [Fact]
        public void PlayerDTO_WithNullIcon_PassesValidation()
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = "ValidName",
                PlayerColor = "#FF5500",
                PlayerIcon = null 
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                playerDto,
                new ValidationContext(playerDto),
                validationResults,
                true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("AB", true)] // Minimum length 2
        [InlineData("ABCDEFGHIJKLMNOPQrst", true)] // Maximum length 20
        [InlineData("A", false)] // Below 2 so too short
        [InlineData("ABCDEFGHIJKLMNOPQrstu", false)] // Over 20 so too long
        [InlineData("Valid-Name_123", true)] // Valid characters
        [InlineData("Invalid*Name", false)] // Invalid character
        public void PlayerName_LengthAndCharacterValidation(string name, bool shouldBeValid)
        {
            // Arrange
            var playerDto = new PlayerDTO
            {
                PlayerName = name,
                PlayerColor = "#FF5500"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                playerDto,
                new ValidationContext(playerDto),
                validationResults,
                true);

            // Assert
            Assert.Equal(shouldBeValid, isValid);
        }
    }
}
