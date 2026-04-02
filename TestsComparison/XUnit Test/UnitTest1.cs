using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace EhuTests_xUnit
{
    public class BaseTest : IDisposable
    {
        protected IWebDriver driver;

        public BaseTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        public void Dispose()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }

    [Trait("Category", "Smoke")]
    public class AboutTests : BaseTest
    {
        [Fact]
        public void Test_AboutPage()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            driver.FindElement(By.PartialLinkText("About")).Click();

            Assert.Contains("/about", driver.Url);
            Assert.Contains("About", driver.Title);
        }
    }

    [Trait("Category", "Regression")]
    public class SearchTests : BaseTest
    {
        [Theory]
        [InlineData("study programs")]
        [InlineData("business")]
        [InlineData("admission")]
        public void Test_Search(string query)
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            var js = (IJavaScriptExecutor)driver;

            IWebElement searchInput = (IWebElement)js.ExecuteScript(
                "return document.querySelector('input[name=\"s\"], input[type=\"search\"], .search-field');");

            Assert.NotNull(searchInput);

            js.ExecuteScript(
                "arguments[0].style.display='block'; arguments[0].style.visibility='visible';",
                searchInput);

            js.ExecuteScript(
                "arguments[0].value=arguments[1];",
                searchInput, query);

            IWebElement form = (IWebElement)js.ExecuteScript(
                "return arguments[0].closest('form');",
                searchInput);

            js.ExecuteScript("arguments[0].submit();", form);

            Assert.Contains(query.Replace(" ", "+"), driver.Url.ToLower());
        }
    }

    [Trait("Category", "UI")]
    public class LanguageTests : BaseTest
    {
        [Fact]
        public void Test_LanguageChange()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            var enButton = driver.FindElement(By.LinkText("EN"));
            enButton.Click();

            System.Threading.Thread.Sleep(500);

            var ltOption = driver.FindElement(By.LinkText("LT"));
            ltOption.Click();

            System.Threading.Thread.Sleep(500);

            Assert.Contains("https://lt.ehuniversity.lt/", driver.Url);
        }
    }

    [Trait("Category", "Regression")]
    public class ContactTests : BaseTest
    {
        [Fact]
        public void Test_ContactForm()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/contact/");

            System.Threading.Thread.Sleep(2000);

            var page = driver.PageSource;

            Assert.Contains("franciskscarynacr@gmail.com", page);
            Assert.Contains("+370 68 771365", page);
            Assert.Contains("+375 29 5781488", page);
            Assert.Contains("Facebook", page);
            Assert.Contains("Telegram", page);
            Assert.Contains("VK", page);
        }
    }
}