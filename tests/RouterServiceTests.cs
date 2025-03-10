using browser_switch.Services;

namespace tests
{
    [TestClass]
    public sealed class RouterServiceTests
    {
        [TestMethod]
        public void TestLoadTxtFile()
        {
            // Arrange
            var routerService = new RouterService();
            var filePath = "router.txt";
            var expected = "RULE_NAME            :: REGEX_PATTERN           :: BROWSER_EXE_PATH                                      :: PARAMS\r\nTest Environment     :: uat-example             :: C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe :: --profile-directory=\"Profile 1\"\r\nRoute ALL            :: github                  :: C:\\Program Files\\Mozilla Firefox\\firefox.exe          :: \r\nRoute ALL            :: .*                      :: C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe :: --profile-directory=\"Default\"";
            // Act
            var actual = routerService.LoadTxtFile(filePath);
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLoadRouteRules()
        {
            // Arrange
            var routerService = new RouterService();
            var filePath = "router.txt";
            var expected = new Dictionary<string, string[]>
            {
                { "uat-example", new string[] { "Test Environment", "\"uat-example\"", "C:\\Program Files\\Mozilla Firefox\\firefox.exe", "--profile-directory=\"Profile 1\"" } }
            };
            // Act
            var actual = routerService.LoadRouteRules(filePath);
            // Assert
            // Pattern
            Assert.AreEqual(expected.FirstOrDefault().Value[3], actual.FirstOrDefault().Value[3]);
        }

        // test route
        [TestMethod]
        public void TestRoute_DefaultUser()
        {
            // Arrange
            var routerService = new RouterService();
            routerService.LoadRouteRules("router.txt");
            // Act
            var url = "https://www.google.com";
            bool result = routerService.Route(url);
            // Assert
            // No exception is thrown
        }

        [TestMethod]
        public void TestRoute_UserProfile1()
        {
            var routerService = new RouterService();
            routerService.LoadRouteRules(
                "router.txt"
            );
            var url = "https://uat-example.com";
            bool result = routerService.Route(url);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void TestOpenBrowser()
        {
            // Arrange
            var routerService = new RouterService();
            var browserPath = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
            var browserParams = "";
            var url = "https://www.google.com";
            // Act
            routerService.OpenBrowser(browserPath, browserParams, url);
            // Assert
            // No exception is thrown
        }
    }
}
