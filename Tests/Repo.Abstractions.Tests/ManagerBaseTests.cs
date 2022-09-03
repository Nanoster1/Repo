using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Repo.Abstractions.AbstractClasses;
using Repo.Abstractions.Interfaces;

namespace Repo.Abstractions.Tests;

public class ManagerBaseTests
{
    public class TestModel : IModel<int>
    {
        public TestModel(int id) => Id = id;
        public int Id { get; set; }
    }
    
    public class TestManager : ManagerBase<TestModel, int>
    {
        public TestManager(IValidator<TestModel> validator, IRepository<TestModel, int> repository, IUnitOfWork unitOfWork) : base(validator, repository, unitOfWork)
        {
        }
    }

    private Mock<IValidator<TestModel>> _validatorMock;
    private Mock<IRepository<TestModel, int>> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    
    private TestModel GetTestModel(int id) => new TestModel(id);
    private TestManager GetTestManager() => new TestManager(_validatorMock.Object, _repositoryMock.Object, _unitOfWorkMock.Object);

    private void UnitOfWorkSetUp(Func<int> function)
    {
        _unitOfWorkMock.Setup(work => work.SaveChanges())
            .Returns(function);
        _unitOfWorkMock.Setup(work => work.SaveChangesAsync())
            .Returns(() => Task.FromResult(function()));
    }

    private void ValidatorSetUp(Func<TestModel, bool> function, Exception ex)
    {
        _validatorMock.Setup(validator => validator.Validate(It.IsAny<TestModel>()))
            .Returns(function);
        _validatorMock.Setup(validator => validator.ValidateAsync(It.IsAny<TestModel>()))
            .Returns<TestModel>(model => Task.FromResult(function(model)));
        _validatorMock.Setup(validator => validator.ValidateAndThrow(It.IsAny<TestModel>()))
            .Callback(() => throw ex);
        _validatorMock.Setup(validator => validator.ValidateAndThrowAsync(It.IsAny<TestModel>()))
            .Callback(() => throw ex);
    }
    
    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<TestModel>>();
        _repositoryMock = new Mock<IRepository<TestModel, int>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public void Create_WithNotValidData_ThrowException()
    {
        //Arrange
        ValidatorSetUp(_ => false, new Exception());
        var testManager = GetTestManager();
        var testModel = GetTestModel(1);
        
        //Assert
        Assert.ThrowsAsync<Exception>(async () =>
        {
            await testManager.Create(testModel);
        });
    }

    [Test]
    public async Task Create_SuccessWay_CreateTestModel()
    {
        //Arrange
        _validatorMock.SetReturnsDefault(true);
        TestModel? modelAfterRepository = null;
        TestModel? modelAfterSaveChanges = null;
        var testModel = GetTestModel(5);
        var modelId = testModel.Id;
        _repositoryMock.Setup(repository => repository.Create(It.IsAny<TestModel>()))
            .Returns<TestModel>(model =>
            {
                modelAfterRepository = model;
                return Task.FromResult(model.Id);
            });
        UnitOfWorkSetUp(() =>
        {
            modelAfterSaveChanges = modelAfterRepository;
            return 1;
        });
        var testManager = GetTestManager();

        //Act
        var result = await testManager.Create(testModel);
        
        //Assert
        Assert.IsTrue(ReferenceEquals(testModel, modelAfterSaveChanges));
        Assert.AreEqual(modelId, result);
    }
    
    
}