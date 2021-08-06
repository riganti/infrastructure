using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests.Query
{
    public class QueryBaseTests : QueryTestsBase
    {
        [Fact]
        public void Skip_SkipSetToOne_SkipsFirstItem()
        {
            var one = 1;
            var querySUT = CreateQueryBaseStub();
            querySUT.Skip = one;

            var queryResult = querySUT.Execute();

            Assert.Equal(customers.Skip(one), queryResult);
        }

        [Fact]
        public void Take_TakeSetToOne_TakeFirstItem()
        {
            var one = 1;
            var querySUT = CreateQueryBaseStub();
            querySUT.Take = one;

            var queryResult = querySUT.Execute();

            Assert.Equal(customers.Take(1), queryResult);
        }

        [Fact]
        public void AddSortCriteria_SortByFirstnameAscending()
        {
            var querySUT = CreateQueryBaseStub();
            Expression<Func<CustomerTestData, string>> sortExpression = customer => customer.FirstName;
            querySUT.AddSortCriteria("FirstName", SortDirection.Ascending);

            var queryResult = querySUT.Execute();

            Assert.Equal(customers.OrderBy(sortExpression), queryResult);
        }

        [Fact]
        public void AddSortCriteria_SortByFirstnameDescending()
        {
            var querySUT = CreateQueryBaseStub();
            Expression<Func<CustomerTestData, string>> sortExpression = customer => customer.FirstName;
            querySUT.AddSortCriteria("FirstName", SortDirection.Descending);

            var queryResult = querySUT.Execute();

            Assert.Equal(customers.OrderByDescending(sortExpression), queryResult);
        }


        [Fact]
        public void AddSortCriteriaLambda_SortByFirstnameAscending()
        {
            var querySUT = CreateQueryBaseStub();
            Expression<Func<CustomerTestData, string>> sortExpression = customer => customer.FirstName;
            querySUT.AddSortCriteria(sortExpression, SortDirection.Ascending);

            var queryResult = querySUT.Execute();

            Assert.Equal(customers.OrderBy(sortExpression), queryResult);
        }

        [Fact]
        public void AddSortCriteriaLambda_SortByFirstnameDescending()
        {
            var querySUT = CreateQueryBaseStub();
            Expression<Func<CustomerTestData, string>> sortExpression = customer => customer.FirstName;
            querySUT.AddSortCriteria(sortExpression, SortDirection.Descending);

            var queryResult = querySUT.Execute();

            Assert.Equal(customers.OrderByDescending(sortExpression), queryResult);
        }

        [Fact]
        public void AddSortCriteriaString_SortUsingNestedEntity()
        {
            var querySUT = CreateQueryBaseStub();
            Expression<Func<CustomerTestData, string>> sortExpression = customer => customer.Address.City;
            querySUT.AddSortCriteria($"{nameof(Address)}.{nameof(Address.City)}", SortDirection.Descending);

            var queryResult = querySUT.Execute();

            Assert.Equal(customers.OrderByDescending(sortExpression), queryResult);
        }


        [Fact]
        public void GetTotalRowCount_ReturnsDataCount()
        {
            var querySUT = CreateQueryBaseStub();

            var rowCount = querySUT.GetTotalRowCount();

            Assert.Equal(customers.Count(), rowCount);
        }

        [Fact]
        public void GetTotalRowCount_SkipAndTakeIsSet_ReturnsDataCount()
        {
            var querySUT = CreateQueryBaseStub();
            querySUT.Skip = 1;
            querySUT.Take = 2;

            var rowCount = querySUT.GetTotalRowCount();

            Assert.Equal(customers.Count(), rowCount);
        }

        [Fact]
        public async Task GetTotalRowCountAsync_ReturnsDataCount()
        {
            var querySUT = CreateQueryBaseStub();

            var rowCount = await querySUT.GetTotalRowCountAsync();

            Assert.Equal(customers.Count(), rowCount);
        }

        [Fact]
        public async Task GetTotalRowCountAsync_SkipAndTakeIsSet_ReturnsDataCount()
        {
            var querySUT = CreateQueryBaseStub();
            querySUT.Skip = 1;
            querySUT.Take = 2;

            var rowCount = await querySUT.GetTotalRowCountAsync();

            Assert.Equal(customers.Count(), rowCount);
        }

        [Fact]
        public void Execute_ReturnsWholeData()
        {
            var querySUT = CreateQueryBaseStub();

            var queryResult = querySUT.Execute();

            Assert.Equal(customers, queryResult);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsWholeData()
        {
            var querySUT = CreateQueryBaseStub();

            var queryResult = await querySUT.ExecuteAsync();

            Assert.Equal(customers, queryResult);
        }

        private QueryBaseStub CreateQueryBaseStub()
        {
            return new QueryBaseStub(customers);
        }

        class QueryBaseStub : QueryBase<CustomerTestData>
        {
            private readonly IQueryable<CustomerTestData> customers;

            public QueryBaseStub(IQueryable<CustomerTestData> customers)
            {
                this.customers = customers;
            }


            protected override Task<IList<CustomerTestData>> ExecuteQueryAsync(IQueryable<CustomerTestData> query, CancellationToken cancellationToken)
            {
                return Task.FromResult((IList<CustomerTestData>)GetQueryable().ToList());
            }

            protected override IQueryable<CustomerTestData> GetQueryable()
            {
                return customers;
            }

            public override Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(GetQueryable().Count());
            }
        }
    }
}