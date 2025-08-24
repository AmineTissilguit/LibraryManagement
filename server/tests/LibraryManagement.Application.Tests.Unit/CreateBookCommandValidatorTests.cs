using System;
using FluentAssertions;
using LibraryManagement.Application.Books.Commands.CreateBook;

namespace LibraryManagement.Application.Tests.Unit;

public class CreateBookCommandValidatorTests
{
    private readonly CreateBookCommandValidator _sut;

    public CreateBookCommandValidatorTests()
    {
        _sut = new CreateBookCommandValidator();
    }

    [Fact]
    public void Validate_ShouldReturnValid_WhenAllPropertiesAreValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldReturnError_WhenIsbnIsNullOrEmpty(string isbn)
    {
        // Arrange
        var command = CreateValidCommand() with { Isbn = isbn };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Isbn) && 
                                           x.ErrorMessage == "ISBN is required");
    }

    [Theory]
    [InlineData("123-abc-456")]
    [InlineData("isbn123")]
    [InlineData("abc-def-ghi")]
    public void Validate_ShouldReturnError_WhenIsbnContainsInvalidCharacters(string isbn)
    {
        // Arrange
        var command = CreateValidCommand() with { Isbn = isbn };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Isbn) && 
                                           x.ErrorMessage == "ISBN should only contain numbers and dashes");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345678901234567890")]
    public void Validate_ShouldReturnError_WhenIsbnLengthIsInvalid(string isbn)
    {
        // Arrange
        var command = CreateValidCommand() with { Isbn = isbn };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Isbn) && 
                                           x.ErrorMessage == "ISBN must be between 10 and 17 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldReturnError_WhenTitleIsNullOrEmpty(string title)
    {
        // Arrange
        var command = CreateValidCommand() with { Title = title };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Title) && 
                                           x.ErrorMessage == "Title is required");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var longTitle = new string('A', 201); // 201 characters
        var command = CreateValidCommand() with { Title = longTitle };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Title) && 
                                           x.ErrorMessage == "Title cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldReturnError_WhenAuthorIsNullOrEmpty(string author)
    {
        // Arrange
        var command = CreateValidCommand() with { Author = author };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Author) && 
                                           x.ErrorMessage == "Author is required");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenAuthorExceedsMaxLength()
    {
        // Arrange
        var longAuthor = new string('A', 101); // 101 characters
        var command = CreateValidCommand() with { Author = longAuthor };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Author) && 
                                           x.ErrorMessage == "Author cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldReturnError_WhenPublisherIsNullOrEmpty(string publisher)
    {
        // Arrange
        var command = CreateValidCommand() with { Publisher = publisher };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Publisher) && 
                                           x.ErrorMessage == "Publisher is required");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPublisherExceedsMaxLength()
    {
        // Arrange
        var longPublisher = new string('A', 101); // 101 characters
        var command = CreateValidCommand() with { Publisher = longPublisher };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Publisher) && 
                                           x.ErrorMessage == "Publisher cannot exceed 100 characters");
    }

    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldReturnError_WhenPublicationYearIsInvalid(int year)
    {
        // Arrange
        var command = CreateValidCommand() with { PublicationYear = year };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.PublicationYear) && 
                                           x.ErrorMessage == "Publication year must be greater than 1000");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPublicationYearIsInFuture()
    {
        // Arrange
        var futureYear = DateTime.UtcNow.Year + 1;
        var command = CreateValidCommand() with { PublicationYear = futureYear };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.PublicationYear) && 
                                           x.ErrorMessage == "Publication year cannot be in the future");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldReturnError_WhenGenreIsNullOrEmpty(string genre)
    {
        // Arrange
        var command = CreateValidCommand() with { Genre = genre };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Genre) && 
                                           x.ErrorMessage == "Genre is required");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenGenreExceedsMaxLength()
    {
        // Arrange
        var longGenre = new string('A', 51); // 51 characters
        var command = CreateValidCommand() with { Genre = longGenre };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Genre) && 
                                           x.ErrorMessage == "Genre cannot exceed 50 characters");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldReturnError_WhenTotalCopiesIsZeroOrNegative(int totalCopies)
    {
        // Arrange
        var command = CreateValidCommand() with { TotalCopies = totalCopies };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.TotalCopies) && 
                                           x.ErrorMessage == "Total copies must be greater than 0");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTotalCopiesExceedsMaximum()
    {
        // Arrange
        var command = CreateValidCommand() with { TotalCopies = 1001 };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.TotalCopies) && 
                                           x.ErrorMessage == "Total copies cannot exceed 1000");
    }

    
    // Helper method
    private static CreateBookCommand CreateValidCommand()
    {
        return new CreateBookCommand(
            "978-0-547-92822-7",
            "The Hobbit", 
            "J.R.R. Tolkien",
            "Houghton Mifflin",
            1937,
            "Fantasy",
            3);
    }
}
