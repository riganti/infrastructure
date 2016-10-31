
using System.Linq;
using Xunit;

namespace Riganti.Utils.Infrastructure.Core.Tests
{
    public class CoreNamespacesTests
    {
        /// <summary>
        /// If this test fail you have to correct the name space. 
        /// If you are using Resharper you should set NamespaceProvider to false in folder properties 
        /// </summary>
        [Fact]
        public void AllClassesHaveCorrectNameSpace_Test()
        {
            var correctNameSpace = "Riganti.Utils.Infrastructure.Core";
            var infrastructureCoreAssembly = typeof(IRepository<,>).Assembly;

            var incorrectTypes = infrastructureCoreAssembly.GetTypes()
                                                          .Where(t => t.Namespace != correctNameSpace)
                                                          .Select(t => t.FullName)
                                                          .ToArray();

            Assert.False(incorrectTypes.Any(), $"Incorect types: {string.Join(", ", incorrectTypes)}");
        }
    }
}
