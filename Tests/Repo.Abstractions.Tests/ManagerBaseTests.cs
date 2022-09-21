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
    public class TestModelWithId : IModelWithId<int>
    {
        public TestModelWithId(int id) => Id = id;
        public int Id { get; set; }
    }
    
    public class TestManager : ManagerBase<TestModelWithId, int>
    {
        public TestManager(IValidator<TestModelWithId> validator, IRepository<TestModelWithId, int> repository, IUnitOfWork unitOfWork) : base(validator, repository, unitOfWork)
        {
        }
    }

    private Mock<IValidator<TestModelWithId>> _validatorMock;
    private Mock<IRepository<TestModelWithId, int>> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    
    private TestModelWithId GetTestModel(int id) => new TestModelWithId(id);
    private TestManager GetTestManager() => new TestManager(_validatorMock.Object, _repositoryMock.Object, _unitOfWorkMock.Object);

    private void UnitOfWorkSetUp(Func<int> function)
    {
        _unitOfWorkMock.Setup(work => work.SaveChanges())
            .Returns(function);
        _unitOfWorkMock.Setup(work => work.SaveChangesAsync())
            .Returns(() => Task.FromResult(function()));
    }

    private void ValidatorSetUp(Func<TestModelWithId, bool> function, Exception ex)
    {
        _validatorMock.Setup(validator => validator.Validate(It.IsAny<TestModelWithId>()))
            .Returns(function);
        _validatorMock.Setup(validator => validator.ValidateAsync(It.IsAny<TestModelWithId>()))
            .Returns<TestModelWithId>(model => Task.FromResult(function(model)));
        _validatorMock.Setup(validator => validator.ValidateAndThrow(It.IsAny<TestModelWithId>()))
            .Callback(() => throw ex);
        _validatorMock.Setup(validator => validator.ValidateAndThrowAsync(It.IsAny<TestModelWithId>()))
            .Callback(() => throw ex);
    }
    
    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<TestModelWithId>>();
        _repositoryMock = new Mock<IRepository<TestModelWithId, int>>();
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
            await testManager.CreateAsync(testModel);
        });
    }

    [Test]
    public async Task Create_SuccessWay_CreateTestModel()
    {
        //Arrange
        _validatorMock.SetReturnsDefault(true);
        TestModelWithId? modelAfterRepository = null;
        TestModelWithId? modelAfterSaveChanges = null;
        var testModel = GetTestModel(5);
        var modelId = testModel.Id;
        _repositoryMock.Setup(repository => repository.CreateAsync(It.IsAny<TestModelWithId>()))
            .Returns<TestModelWithId>(model =>
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
        var result = await testManager.CreateAsync(testModel);
        
        //Assert
        Assert.IsTrue(ReferenceEquals(testModel, modelAfterSaveChanges));
        Assert.AreEqual(modelId, result);
    }
    
    
}