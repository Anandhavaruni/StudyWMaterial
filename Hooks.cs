using Wesleyan.Framework.Base;
using TechTalk.SpecFlow;
using BoDi;
using System;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Gherkin.Model;

namespace Portal.Test
{
    [Binding]
    public class HookInitialize : TestInitializeHook
    {

        static string actualPath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\Portal.Test\\bin\\Debug", "\\Portal.Test\\bin\\Release");
        //static string reportPath = actualPath + "ExtentReports\\Index_" + DateTime.Now.ToString("yyyyMMddHHmmss" + ".html");
        static string reportPath = actualPath + "ExtentReports\\Index_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html";
        // directory where output is to be printed

        //ExtentSparkReporter spark = new ExtentSparkReporter("user/build/name/");
        //ExtentReports extent = new ExtentReports();
        //extent.AttachReporter(spark);

        [ThreadStatic]
        private static ExtentTest feature;
        [ThreadStatic]
        private static ExtentTest scenario;
        private static ExtentReports extentReport;

        public HookInitialize(ParallelConfig parallelConfig, IObjectContainer objectContainer) : base(parallelConfig, objectContainer)
        {
        }

        [BeforeScenario]
        public void TestStart(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            HookInitialize init = new HookInitialize(_parallelConfig, _objectContainer);
            init.InitializeSettings("test");
            scenario = feature.CreateNode<Scenario>(scenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario]
        public void TestStop()
        {
            _parallelConfig.Driver.Quit();
        }
        // Extent Report Scenarios
        [BeforeTestRun]
        public static void InitializeReport()
        {
            //Initialize Extent report before test starts
            ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(reportPath);
            
            //Attach report to reporter
            extentReport = new ExtentReports();
            extentReport.AttachReporter(htmlReporter);
        }

        [AfterTestRun]
        public static void TearDownReport()
        {
            extentReport.Flush();
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            feature = extentReport.CreateTest<Feature>(featureContext.FeatureInfo.Title);

        }

        [AfterStep]
        public void InsertReportingSteps(ScenarioContext scenarioContext)
        {
            string stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            string stepInfo = scenarioContext.StepContext.StepInfo.Text;

            //to check if we missed to implement steps inside method
            string resultOfImplementation = scenarioContext.ScenarioExecutionStatus.ToString();
            var MediaEntity = _parallelConfig.CaptureScreenShot(scenarioContext.ScenarioInfo.Title.Trim());

            if (scenarioContext.TestError == null && resultOfImplementation == "OK")
            {
                if (stepType == "Given")
                    scenario.CreateNode<Given>(stepInfo);
                else if (stepType == "When")
                    scenario.CreateNode<When>(stepInfo);
                else if (stepType == "Then")
                    scenario.CreateNode<Then>(stepInfo);
                else if (stepType == "And")
                    scenario.CreateNode<And>(stepInfo);
                else if (stepType == "But")
                    scenario.CreateNode<And>(stepInfo);
            }
            else if (scenarioContext.TestError != null)
            {
                Exception innerException = scenarioContext.TestError.InnerException;
                string testError = scenarioContext.TestError.Message;


                if (stepType == "Given")
                    if (testError != null)
                        scenario.CreateNode<Given>(stepInfo).Fail(testError, MediaEntity);
                    else
                        scenario.CreateNode<Given>(stepInfo).Fail(innerException, MediaEntity);
                else if (stepType == "When")
                    if (testError != null)
                        scenario.CreateNode<When>(stepInfo).Fail(testError, MediaEntity);
                    else
                        scenario.CreateNode<When>(stepInfo).Fail(innerException, MediaEntity);
                else if (stepType == "Then")
                    if (testError != null)
                        scenario.CreateNode<Then>(stepInfo).Fail(testError, MediaEntity);
                    else
                        scenario.CreateNode<Then>(stepInfo).Fail(innerException, MediaEntity);
                else if (stepType == "And")
                    if (testError != null)
                        scenario.CreateNode<And>(stepInfo).Fail(testError, MediaEntity);
                    else
                        scenario.CreateNode<And>(stepInfo).Fail(innerException, MediaEntity);
                else if (stepType == "But")
                    if (testError != null)
                        scenario.CreateNode<But>(stepInfo).Fail(testError, MediaEntity);
                    else
                        scenario.CreateNode<But>(stepInfo).Fail(innerException, MediaEntity);

            }
            else if (resultOfImplementation == "StepDefinitionPending")
            {
                string errorMessage = "Step Definition is not implemented!";

                if (stepType == "Given")
                    scenario.CreateNode<Given>(stepInfo).Skip(errorMessage);
                else if (stepType == "When")
                    scenario.CreateNode<When>(stepInfo).Skip(errorMessage);
                else if (stepType == "Then")
                    scenario.CreateNode<Then>(stepInfo).Skip(errorMessage);
                else if (stepType == "But")
                    scenario.CreateNode<Then>(stepInfo).Skip(errorMessage);

            }

        }
    }
}
