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

public class CategoryControllerTests
{

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfCategories()
    {
        // Arrange
        var categoryList = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Test category 1" },
            new Category { CategoryId = 2, CategoryName = "Test category 2" }
        };
        var fakeCategoryService = A.Fake<ICategoryService>();
        A.CallTo(() => fakeCategoryService.GetAllCategories()).Returns(Task.FromResult(categoryList));
        var fakeCategoryValidator = A.Fake<IValidator<Category>>();
        var controller = new CategoryController(fakeCategoryService, null, fakeCategoryValidator);

        // Act
        var result = await controller.GetCategories();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeAssignableTo<List<Category>>();
        var categories = (List<Category>)okResult.Value;
        categories.Should().BeEquivalentTo(categoryList);

    }
    [Fact]
    public async Task Get_ReturnsOkResult_WithCategory()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test category" };
        var fakeCategoryService = A.Fake<ICategoryService>();
        A.CallTo(() => fakeCategoryService.GetCategory(1)).Returns(Task.FromResult(category));
        var fakeCategoryValidator = A.Fake<IValidator<Category>>();
        var controller = new CategoryController(fakeCategoryService, null, fakeCategoryValidator);

        // Act
        var result = await controller.Get(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        okResult.Value.Should().BeAssignableTo<Category>();
        var returnedCategory = (Category)okResult.Value;
        returnedCategory.CategoryId.Should().Be(1);
        returnedCategory.CategoryName.Should().Be("Test category");
    }
    [Fact]
    public async Task Get_ReturnsNotFound_WhenCategoryNotFound()
    {
        // Arrange
        var fakeCategoryService = A.Fake<ICategoryService>();
        A.CallTo(() => fakeCategoryService.GetCategory(1)).Returns(Task.FromResult<Category>(null));
        var fakeCategoryValidator = A.Fake<IValidator<Category>>();
        var controller = new CategoryController(fakeCategoryService, null, fakeCategoryValidator);

        // Act
        var result = await controller.Get(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
    [Fact]
    public async Task Post_ReturnsOkResult_WithSuccessMessage_WhenCategoryIsCreated()
    {
        // Arrange
        var categoryCreateDto = new CategoryCreateDto { CategoryName = "Test category", DisplayOrder = 1 };
        var mockCategoryService = A.Fake<ICategoryService>();
        var mockConfiguration = A.Fake<IConfiguration>();
        var mockCategoryValidator = A.Fake<IValidator<Category>>();
        A.CallTo(() => mockCategoryService.CreateCategory(categoryCreateDto)).Returns(Task.CompletedTask);
        var controller = new CategoryController(mockCategoryService, mockConfiguration, mockCategoryValidator);

        // Act
        var result = await controller.Post(categoryCreateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Category created successfully!", okResult.Value);
        A.CallTo(() => mockCategoryService.CreateCategory(categoryCreateDto)).MustHaveHappened();
    }
    [Fact]
    public async Task Post_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockCategoryService = A.Fake<ICategoryService>();
        var mockConfiguration = A.Fake<IConfiguration>();
        var mockCategoryValidator = A.Fake<IValidator<Category>>();

        var categoryController = new CategoryController(
            mockCategoryService,
            mockConfiguration,
            mockCategoryValidator
        );
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
    //[Fact]
    //public async Task Put_ReturnsOkResult_WithSuccessMessage_WhenCategoryIsUpdated()
    //{
    //    // Arrange
    //    var category = new Category { CategoryId = 1, CategoryName = "Test", DisplayOrder = 1 };
    //    var categoryDto = new CategoryDto { CategoryName = "Updated Test", DisplayOrder = 2 };
    //    var mockCategoryService = A.Fake<ICategoryService>();
    //    A.CallTo(() => mockCategoryService.GetCategory(1)).Returns(Task.FromResult(category));
    //    A.CallTo(() => mockCategoryService.UpdateCategory(category)).Returns(Task.CompletedTask);
    //    var mockCategoryValidator = A.Fake<IValidator<Category>>();
    //    A.CallTo(() => mockCategoryValidator.ValidateAsync(category, default)).Returns(Task.FromResult(new ValidationResult()));
    //    var controller = new CategoryController(mockCategoryService, null, mockCategoryValidator);

    //    // Act
    //    var result = await controller.Update(1, categoryDto);

    //    // Assert
    //    Assert.IsType<OkObjectResult>(result);
    //    var okResult = result as OkObjectResult;
    //    Assert.Equal("Category updated successfully!", okResult.Value);
    //}
    [Fact]
    public async Task Delete_ReturnsOkResult_WhenCategoryIsDeleted()
    {
        // Arrange
        var mockCategoryService = A.Fake<ICategoryService>();
        var mockCategoryValidator = A.Fake<IValidator<Category>>();
        var controller = new CategoryController(mockCategoryService, null, mockCategoryValidator);
        int id = 1;
        A.CallTo(() => mockCategoryService.DeleteCategory(id)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.Delete(id);

        // Assert
        A.CallTo(() => mockCategoryService.DeleteCategory(id)).MustHaveHappenedOnceExactly();
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("Category deleted successfully!", okResult.Value);
    }

}