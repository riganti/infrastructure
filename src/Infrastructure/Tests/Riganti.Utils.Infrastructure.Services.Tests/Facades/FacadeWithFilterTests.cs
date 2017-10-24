using System;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Riganti.Utils.Infrastructure.AutoMapper;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Riganti.Utils.Infrastructure.Services.Tests.Facades
{
    public class FacadeWithFilterTests
    {
        private readonly ITestOutputHelper output;
        private const int EmployeesCount = 10;

        public FacadeWithFilterTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GetList_WithNullFilter_Returns_AllItems()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EmployeeProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "Employees" + Guid.NewGuid().ToString().Substring(10))
                .Options;
            PrepareInMemoryDbContext(options);
            var facade = GetFacade(options);

            // Act
            var list = facade.GetList(filter: null).ToList();

            // Assert
            Assert.Equal(EmployeesCount, list.Count);
        }

        [Fact]
        public void GetList_WithEqualFilter_Returns_AllItems()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EmployeeProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "Employees" + Guid.NewGuid().ToString().Substring(10))
                .Options;
            PrepareInMemoryDbContext(options);
            var facade = GetFacade(options);

            var filter = new FilterConditionDTO
            {
                FieldName = nameof(Employee.Id),
                Operator = FilterOperatorType.Equal,
                Value = 3
            };

            // Act
            var list = facade.GetList(filter).ToList();

            // Assert
            Assert.Equal(1, list.Count);
            Assert.Equal(3, list.Single().Id);
        }

        [Fact]
        public void GetList_WithBetweenFilter_Returns_AllItems()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EmployeeProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "Employees" + Guid.NewGuid().ToString().Substring(10))
                .Options;
            PrepareInMemoryDbContext(options);
            var facade = GetFacade(options);

            var filter = new FilterGroupDTO
            {
                Logic = FilterLogicType.And,
                Filters = new FilterDTOBase[]
                {
                    new FilterConditionDTO
                    {
                        FieldName = nameof(Employee.Id),
                        Operator = FilterOperatorType.GreaterThan,
                        Value = 3
                    },
                    new FilterConditionDTO
                    {
                        FieldName = nameof(Employee.Id),
                        Operator = FilterOperatorType.LessThanOrEqual,
                        Value = 5
                    }
                }
            };

            // Act
            var list = facade.GetList(filter).ToList();

            // Assert
            Assert.Equal(2, list.Count);
            Assert.True(list.Any(e => e.Id == 4));
            Assert.True(list.Any(e => e.Id == 5));
        }

        private static void PrepareInMemoryDbContext(DbContextOptions<EmployeeProjectDbContext> options)
        {
            var employees = Enumerable.Range(1, EmployeesCount)
                .Select(i => new Employee { Id = i })
                .ToList();

            using (var ctx = new EmployeeProjectDbContext(options))
            {
                ctx.Employees.AddRange(employees);
                ctx.SaveChanges();
            }
        }

        private EmployeesFacade GetFacade(DbContextOptions<EmployeeProjectDbContext> options)
        {
            MappingHelper.Config();
            
            var dateTimeProvider = new LocalDateTimeProvider();
            IUnitOfWorkProvider unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider<EmployeeProjectDbContext>(new AsyncLocalUnitOfWorkRegistry(), () => new EmployeeProjectDbContext(options));
            Func<IQuery<EmployeeDTO>> queryFactory = () => new EmployeesQuery(unitOfWorkProvider);
            var entityMapper = new EntityDTOMapper<Employee, EmployeeDTO>();
            var repository = new EntityFrameworkRepository<Employee, int>(unitOfWorkProvider, dateTimeProvider);
            var facade = new EmployeesFacade(queryFactory, repository, entityMapper, unitOfWorkProvider);
            return facade;
        }
    }
}