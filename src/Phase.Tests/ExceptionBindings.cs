using System.Reflection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace Phase.Tests
{
    [Binding]
    public class ExceptionBindings
    {
        [AfterStep("CatchException")]
        public void CatchException()
        {
            if (ScenarioContext.Current.StepContext.StepInfo.StepDefinitionType == StepDefinitionType.When)
            {
                PropertyInfo testStatusProperty = typeof(ScenarioContext).GetProperty(nameof(ScenarioContext.Current.ScenarioExecutionStatus), BindingFlags.Public | BindingFlags.Instance);
                testStatusProperty.SetValue(ScenarioContext.Current, ScenarioExecutionStatus.OK);
            }
        }
    }
}