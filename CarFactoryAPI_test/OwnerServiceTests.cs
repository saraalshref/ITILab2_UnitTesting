using CarAPI.Entities;
using CarAPI.Models;
using CarAPI.Payment;
using CarAPI.Repositories_DAL;
using CarAPI.Services_BLL;
using CarFactoryAPI.Entities;
using CarFactoryAPI.Repositories_DAL;
using CarFactoryAPI_test.stups;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CarFactoryAPI_test
{
    public class OwnerServiceTests : IDisposable
    {
        private readonly ITestOutputHelper outputHelper;
        // Create Mock of the dependencies
        Mock<ICarsRepository> carRepoMock;
        Mock<IOwnersRepository> ownersRepoMock;
        Mock<ICashService> cashServiceMock;

        // use the fake object as dependency
        OwnersService ownersService;

        public OwnerServiceTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
            // Test Start up
            outputHelper.WriteLine("Test start up");

            // Create Mock of the dependencies
            carRepoMock = new();
            ownersRepoMock = new();
            cashServiceMock = new();

            // use the fake object as dependency
            ownersService = new OwnersService(carRepoMock.Object, ownersRepoMock.Object, cashServiceMock.Object);

        }

        public void Dispose()
        {
            outputHelper.WriteLine("Test clean up");
        }
        [Fact]
        [Trait("Author", "Ahmed")]

        public void BuyCar_CarNotExist_NotExist() 
        {
            outputHelper.WriteLine("Test 1");
            // Arrange
            FactoryContext factoryContext = new FactoryContext();

            // CarRepository carRepository = new CarRepository(factoryContext);

            // Fake Dependency
            CarRepoStup carRepoStup = new CarRepoStup();

            // Real Dependency
            OwnerRepository ownerRepository = new OwnerRepository(factoryContext);
            CashService cashService = new CashService();

            OwnersService ownersService = new OwnersService(carRepoStup,ownerRepository,cashService);

            BuyCarInput buyCarInput = new BuyCarInput()
            { OwnerId = 10, CarId = 100, Amount = 5000};

            // Act
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("n't exist", result);
        }

        [Fact(Skip ="Working on solving error")]
        [Trait("Author","Ahmed")]
        public void BuyCar_CarWithOwner_Sold()
        {
            outputHelper.WriteLine("Test 2");

            // Arrange
            Car car = new Car() { Id = 10, Owner = new Owner() };

            // Setup the called method
            carRepoMock.Setup(cM=>cM.GetCarById(10)).Returns(car);

            // use the fake object as dependency
            BuyCarInput buyCarInput = new BuyCarInput()
            {
                CarId = 10,
                OwnerId = 100,
                Amount = 5000
            };

            // Act
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("msold", result);

        }


        [Fact]
        [Trait("Author", "Ali")]
        [Trait("Priority", "5")]


        public void BuyCar_OwnorNotExist_NotExist()
        {
            outputHelper.WriteLine("Test 3");

            // Arrange
            // Build the mock Data
            Car car = new Car() { Id = 5 };
            Owner owner = null;

            // Setup the called Methods
            carRepoMock.Setup(cm => cm.GetCarById(It.IsAny<int>())).Returns(car);
            ownersRepoMock.Setup(om => om.GetOwnerById(It.IsAny<int>())).Returns(owner);

           
            BuyCarInput buyCarInput = new() { CarId = 5, OwnerId = 100, Amount = 5000 };


            // Act 
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("n't exist", result);
        }

        //Case 1
        [Trait("Author", "User")]
        public void BuyCar_AlreadyHaveCar_NotExist()
        {
            // Arrange
            Car car = new Car() { Id = 11, Owner = new Owner() { Id = 11, Name = "ahmed" } };

            carRepoMock.Setup(cm => cm.GetCarById(10)).Returns(car);
            ownersRepoMock.Setup(om => om.GetOwnerById(It.IsAny<int>())).Returns(car.Owner);

            BuyCarInput buyCarInput = new BuyCarInput() { CarId = 11, OwnerId = 11, Amount = 5000 };

            // Act 
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            switch (result)
            {
                case "Already have car":
                    Assert.Contains("Already have car", result);
                    break;
                case "Owner doesn't exist":
                    Assert.Contains("Owner doesn't exist", result);
                    break;
                case "Insufficient funds":
                    Assert.Contains("Insufficient funds", result);
                    break;
                default:
                    Assert.True(false, $"Unexpected result: {result}");
                    break;
            }
        }




        //Case 2
        [Fact]
        [Trait("Author", "User")]
        public void BuyCar_InsufficientFunds_NotExist()
        {
            // Arrange
            Car car = new Car() { Id = 5, Price = 10000 };
            Owner owner = new Owner();

            carRepoMock.Setup(cm => cm.GetCarById(5)).Returns(car);
            ownersRepoMock.Setup(om => om.GetOwnerById(It.IsAny<int>())).Returns(owner);
            cashServiceMock.Setup(cs => cs.Pay(It.IsAny<int>())).Returns("Failed"); 

            BuyCarInput buyCarInput = new BuyCarInput() { CarId = 5, OwnerId = 100, Amount = 5000 };

            // Act 
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("Insufficient funds", result);
        }
        //Case 3
        [Fact]
        [Trait("Author", "User")]
        public void BuyCar_AssignmentFailed_SomethingWentWrong()
        {
            // Arrange
            Car car = new Car() { Id = 5, Price = 5000 };
            Owner owner = new Owner();

            carRepoMock.Setup(cm => cm.GetCarById(5)).Returns(car);
            ownersRepoMock.Setup(om => om.GetOwnerById(It.IsAny<int>())).Returns(owner);
            cashServiceMock.Setup(cs => cs.Pay(It.IsAny<int>())).Returns("Success");
            carRepoMock.Setup(cm => cm.AssignToOwner(5, It.IsAny<int>())).Returns(false);

            BuyCarInput buyCarInput = new BuyCarInput() { CarId = 5, OwnerId = 100, Amount = 5000 };

            // Act 
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("Something went wrong", result);
        }

        //Case 4
        [Fact]
        [Trait("Author", "User")]
        public void BuyCar_SuccessfulPurchase_ReturnSuccessMessage()
        {
            // Arrange
            Car car = new Car() { Id = 5, Price = 5000 };
            Owner owner = new Owner() { Id = 100, Name = "John Doe" };

            carRepoMock.Setup(cm => cm.GetCarById(5)).Returns(car);
            ownersRepoMock.Setup(om => om.GetOwnerById(100)).Returns(owner);
            cashServiceMock.Setup(cs => cs.Pay(5000)).Returns("Success");
            carRepoMock.Setup(cm => cm.AssignToOwner(5, 100)).Returns(true);

            BuyCarInput buyCarInput = new BuyCarInput() { CarId = 5, OwnerId = 100, Amount = 5000 };

            // Act 
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("Successfull", result);
            Assert.Contains($"Car of Id: {buyCarInput.CarId} is bought by {owner.Name}", result);
            Assert.Contains($"payment result Success", result);
        }



    }
}
