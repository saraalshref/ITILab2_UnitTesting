﻿using CarAPI.Entities;
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
    public class OwnerRepositoryTests
    {
        private Mock<FactoryContext> factoryContextMock;
        private OwnerRepository ownerRepository;
        public OwnerRepositoryTests()
        {
            // Create Mock of dependencies
            factoryContextMock = new Mock<FactoryContext>();

            // use fake object as dependency
            ownerRepository = new OwnerRepository(factoryContextMock.Object);
        }
        [Fact]
        [Trait("Author", "ahmed")]
        [Trait("Priority", "9")]

        public void GetCarById_AskForCar10_ReturnCar()
        {
            // Arrange

            // Build the mock Data
            List<Owner> cars = new List<Owner>() { new Owner() { Id = 10 } };

            // setup called DbSets
            factoryContextMock.Setup(fcm => fcm.Owners).ReturnsDbSet(cars);

            // Act 
            Owner owner = ownerRepository.GetOwnerById(10);

            // Aassert

            Assert.NotNull(owner);
        }
    }
}
