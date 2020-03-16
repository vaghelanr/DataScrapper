using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        IWebDriver driver;
        static void Main(string[] args)
        {
            var program = new Program();
            program.startBrowser();
            //program.ExportNames();
            program.closeBrowser();
        }

        public void startBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("start-maximized");
            //options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArguments("--disable-infobars");
            driver = new ChromeDriver("..\\..\\Resource", options);
            driver.Url = "https://www.salesgenie.com/sign-in/";
        }

        public void ExportNames()
        {
            try
            {
                driver.Url = "https://www.salesgenie.com/sign-in/";
                Console.WriteLine("Login in website and navigate to first page. Press any key once you are done...");
                Console.ReadLine();
                var csv = new StringBuilder();
                int pageCount = Int32.Parse(driver.FindElement(By.CssSelector("div.page-input")).Text.Split(new string[] { "of" }, StringSplitOptions.None)[1].Trim());
                int recordCounter = 0;
                for (int j = 0; j < pageCount; j++)
                {
                    Thread.Sleep(3 * 1000);
                    var mainTableRows = driver.FindElements(By.CssSelector("table.grid__table.grid-scrolling-table tr"));

                    for (int i = 0; i < mainTableRows.Count - 1; i++)
                    {
                        var mainRow = mainTableRows[i];
                        var name = mainRow.FindElement(By.CssSelector("div.consumer-name-column")).Text;
                        var address = "\"" + mainRow.FindElement(By.CssSelector("div.address-column")).Text.Replace("\n", ", ").Replace("\r", "") + "\"";
                        recordCounter++;
                        csv.AppendLine($"{recordCounter},{name},{address}");
                    }

                    if (!IsNextElementDisabled())
                    {
                        driver.FindElement(By.CssSelector("div.next")).Click();
                    }
                    else
                    {
                        break;
                    }

                }

                File.WriteAllText($"..\\..\\ExportedFiles\\{DateTime.Now.ToString("MMMdd-Hmms")}-{recordCounter}.csv", csv.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private bool IsNextElementDisabled()
        {
            try
            {
                driver.FindElement(By.CssSelector("div.next.disabled"));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void closeBrowser()
        {
            driver.Close();
            driver.Quit();
        }
    }
}
