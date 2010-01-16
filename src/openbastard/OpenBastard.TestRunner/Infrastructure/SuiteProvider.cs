using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;
using OpenBastard.Environments;
using OpenRasta.Security;
using OpenRasta.Testing;
using OpenRasta.Web;

[NUnitAddin]
public class SuiteProvider : IAddin
{
    public bool Install(IExtensionHost host)
    {
        host.GetExtensionPoint("SuiteBuilders").Install(new SuiteBuilder());
        return true;
    }
}

public class SuiteBuilder : ISuiteBuilder
{
    static readonly IEnumerable<Type> _testFixtures;
    static double _uselessData;

    static SuiteBuilder()
    {
        _uselessData = GlobalSettings.DefaultFloatingPointTolerance;
        _testFixtures = Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof(environment_context).IsAssignableFrom(x) && !x.IsAbstract);
    }

    public Test BuildFrom(Type type)
    {
        // need to create a top level test suite for each environment
        var environment = (IEnvironment)Activator.CreateInstance(type);
        var testSuite = new EnvironmentTestSuite(environment);

// testSuite.Fixture = environment;
        foreach (var originalFixture in _testFixtures.Select(x => CreateTests(x, environment)))
        {
            testSuite.Add(originalFixture);
        }
        return testSuite;
    }

    public bool CanBuildFrom(Type type)
    {
        return !type.IsAbstract && typeof(IEnvironment).IsAssignableFrom(type);
    }

    void CreateFixtureInstanceWithEnvironment(Test testFixture, IEnvironment environment1)
    {
        var fixture = Activator.CreateInstance(testFixture.FixtureType) as environment_context;
        fixture.SetEnvironment(environment1);
        testFixture.Fixture = fixture;
    }

    Test CreateTests(Type fixtureType, IEnvironment environment)
    {
        var fixture = new ScenarioTestFixture(fixtureType);
        CreateFixtureInstanceWithEnvironment(fixture, environment);
        foreach (var method in fixtureType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.DeclaringType != typeof(object) && x.Name.Contains("_")))
        {
            var testMethod = new ScenarioTestMethod(method);
            testMethod.Parent = fixture;
            testMethod.Fixture = fixture.Fixture;
            fixture.Tests.Add(testMethod);
        }
        return fixture;
    }
}

public class ScenarioTestFixture : NUnitTestFixture
{
    public ScenarioTestFixture(Type fixtureType) : base(fixtureType)
    {
        base.TestName.Name = fixtureType.Name.Replace("_", " ");
    }
}

public class ScenarioTestMethod : NUnitTestMethod
{
    public ScenarioTestMethod(MethodInfo method) : base(method)
    {
        base.TestName.Name = method.Name.Replace("_", " ");
    }
}


public class EnvironmentTestSuite : TestSuite
{
    readonly IEnvironment _environment;

    public EnvironmentTestSuite(IEnvironment environment)
        : base(environment.GetType().Name)
    {
        _environment = environment;
        base.TestName.Name = base.TestName.FullName = environment.Name;
    }

    protected override void DoOneTimeSetUp(TestResult suiteResult)
    {
        _environment.Initialize();
    }

    protected override void DoOneTimeTearDown(TestResult suiteResult)
    {
        _environment.Dispose();
    }
}

public abstract class environment_context
{
    protected IEnvironment Environment { get; set; }
    protected IClientRequest Request { get; set; }
    protected IResponse Response { get; set; }

    public void SetEnvironment(IEnvironment environment)
    {
        Environment = environment;
    }

    protected IClientRequest given_request_to(string uri)
    {
        Request = Environment.CreateRequest(uri);
        return Request;
    }

    protected void then_response_should_be_200_ok()
    {
        Response.StatusCode.ShouldBe(200);
    }

    protected virtual void when_retrieving_the_response()
    {
        Response = Environment.ExecuteRequest(Request);
    }
}

public interface IClientRequest : IRequest
{
    Credentials Credentials { get; set; }
}