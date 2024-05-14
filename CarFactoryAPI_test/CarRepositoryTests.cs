using CarAPI.Entities;
using CarFactoryAPI.Entities;
using CarFactoryAPI.Repositories_DAL;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarFactoryAPI_test
{
    public class CarRepositoryTests
    {
        private Mock<FactoryContext> factoryContextMock;
        private CarRepository carRepository;
        public CarRepositoryTests()
        {
            // Create Mock of dependencies
            factoryContextMock = new Mock<FactoryContext>();

            // use fake object as dependency
            carRepository = new CarRepository(factoryContextMock.Object);
        }
        [Fact]
        [Trait("Author", "Ali")]
        [Trait("Priority", "9")]

        public void GetOwnerById_AskForOwner10_ReturnOwner()
        {
            // Arrange
            List<Car> cars = new List<Car>() { new Car() { Id = 10 } } ;
            factoryContextMock.Setup(fcm=>fcm.Cars).ReturnsDbSet(cars);
            // Act 
            Car car = carRepository.GetCarById(10);
            // Aassert
            Assert.NotNull(car);
        }
    }
}
