using AutoMapper;
using EcommerceAPI.Controllers;
using EcommerceAPI.Data.Repository.IRepository;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using Moq;
using Nest;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _categoryService;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IValidator<Category>> _categoryValidator;
    private CategoryController categoryController;

    public CategoryControllerTests()
    {
        _categoryService = new Mock<ICategoryService>();
        _configuration = new Mock<IConfiguration>();
        _categoryValidator = new Mock<IValidator<Category>>();
        categoryController = new CategoryController(_categoryService.Object, _configuration.Object, _categoryValidator.Object);
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
        var result = await categoryController.Post(categoryCreateDto);

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
        var result = await categoryController.Post(createCategoryDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }


    [Fact]
    public async Task Put_ReturnsOkResult_WithSuccessMessage_WhenCategoryIsUpdated()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test", DisplayOrder = 1 };
        var categoryDto = new CategoryDto { CategoryName = "Updated Test", DisplayOrder = 2 };
        _categoryService.Setup(x => x.GetCategory(1)).ReturnsAsync(category);
        _categoryService.Setup(x => x.UpdateCategory(category)).Returns(Task.CompletedTask);
        _categoryValidator.Setup(x => x.ValidateAsync(category, default)).ReturnsAsync(new ValidationResult());

        // Act
        var result = await categoryController.Update(1, categoryDto);

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