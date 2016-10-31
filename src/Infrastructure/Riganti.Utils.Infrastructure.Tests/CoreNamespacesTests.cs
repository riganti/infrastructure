using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.Tests
{
    [TestClass]
    public class CoreNamespacesTests
    {
        /// <summary>
        /// If this test fail you have to correct the name space. 
        /// If you are using Resharper you should set NamespaceProvider to false in folder properties 
        /// </summary>
        [TestMethod]
        public void AllClassesHaveCorrectNameSpace_Test()
        {
            var correctNameSpace = "Riganti.Utils.Infrastructure.Core";
            var infrastructureCoreAssembly = typeof(IRepository<,>).Assembly;

            var incorrectTypes = infrastructureCoreAssembly.GetTypes()
                                                          .Where(t => t.Namespace != correctNameSpace)
                                                          .Select(t => t.FullName)
                                                          .ToArray();

            Assert.IsFalse(incorrectTypes.Any(), $"Incorect types: {string.Join(", ", incorrectTypes)}");
        }
    }
}