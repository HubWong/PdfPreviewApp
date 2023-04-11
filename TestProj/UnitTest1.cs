using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcLib.Db;
using MvcLib.Tools;

namespace TestProj
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MvcLibBtnTostringTest()
        {
            ButtonDom buttonDom = new ButtonDom("wyb", "createClick", "fick", "btn-primary");
            string x = buttonDom.ToString();
            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void TestGetJsonfile()
        {
            var dummy = new DummyData();
            dummy.getConfigsData();
        }
    }
}
