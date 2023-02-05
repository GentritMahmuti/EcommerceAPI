using AutoMapper;
using EcommerceAPI.Controllers;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using Moq;
using Nest;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Services.Services.IServices;
using Services.DTOs.Category;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _categoryService;
    private readonly Mock<IValidator<CategoryDto>> _categoryValidator;
    private readonly Mock<IValidator<CategoryCreateDto>> _categoryCreateValidator;
    private readonly Mock<ILogger<CategoryController>> _logger;
    private CategoryController categoryController;

    public CategoryControllerTests()
    {
        _categoryService = new Mock<ICategoryService>();
        _categoryValidator = new Mock<IValidator<CategoryDto>>();
        _categoryCreateValidator = new Mock<IValidator<CategoryCreateDto>>();
        _logger = new Mock<ILogger<CategoryController>>();
        categoryController = new CategoryController(_categoryService.Object,  _categoryValidator.Object, _categoryCreateValidator.Object, _logger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfCategories()
    {
        // Arrange
        var categoryList = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Test category 1" },
            new Category { CategoryId = 2, CategoryName = "Test category 2" }
        };
        _categoryService.Setup(x => x.GetAllCategories()).ReturnsAsync(categoryList);

        // Act
        var result = await categoryController.GetCategories();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithCategory()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test category" };
        _categoryService.Setup(x => x.GetCategory(1)).ReturnsAsync(category);

        // Act
        var result = await categoryController.Get(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeAssignableTo<Category>();
        var returnedCategory = okResult.Value as Category;
        returnedCategory.CategoryId.Should().Be(1);
        returnedCategory.CategoryName.Should().Be("Test category");
    }


    [Fact]
    public async Task Get_ReturnsNotFound_WhenCategoryNotFound()
    {
        // Arrange
        _categoryService.Setup(x => x.GetCategory(1)).ReturnsAsync((Category)null);

        // Act
        var result = await categoryController.Get(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }


    [Fact]
    public async Task Post_ReturnsOkResult_WithSuccessMessage_WhenCategoryIsCreated()
    {
        // Arrange
        var categoryCreateDto = new CategoryCreateDto { CategoryName = "Test category", DisplayOrder = 1 };
        _categoryService.Setup(x => x.CreateCategory(categoryCreateDto)).Returns(Task.CompletedTask);
        // Act
        var result = await categoryController.CreateCategory(categoryCreateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Category created successfully!", okResult.Value);
    }


    [Fact]
    public async Task Post_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        categoryController.ModelState.AddModelError("error", "some error");
        var createCategoryDto = new CategoryCreateDto()
        {
            CategoryName = "",
            DisplayOrder = 1
        };

        // Act
        var result = await categoryController.CreateCategory(createCategoryDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }


    [Fact]
    public async Task Put_ReturnsOkResult_WithSuccessMessage_WhenCategoryIsUpdated()
    {
        // Arrange
        var categoryDto = new CategoryDto { CategoryName = "Updated Test", DisplayOrder = 2 };
        _categoryValidator.Setup(x => x.ValidateAsync(categoryDto, default)).ReturnsAsync(new ValidationResult());
        _categoryService.Setup(x => x.UpdateCategory(categoryDto)).Returns(Task.CompletedTask);

        // Act
        var result = await categoryController.Update(categoryDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("Category updated successfully!", okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsOkResult_WhenCategoryIsDeleted()
    {
        // Arrange
        int id = 1;
        _categoryService.Setup(x => x.DeleteCategory(id)).Returns(Task.CompletedTask);
        // Act
        var result = await categoryController.Delete(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("Category deleted successfully!", okResult.Value);

    }

}