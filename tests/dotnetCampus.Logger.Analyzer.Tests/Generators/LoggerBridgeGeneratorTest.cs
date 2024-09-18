using dotnetCampus.Logger.Generators;

using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace dotnetCampus.Logger.Analyzer.Tests.Generators;

[TestClass]
public class LoggerBridgeGeneratorTest
{
    [TestMethod]
    public void TestGenerateLoggerBridge()
    {
        var loggerBridgeGenerator = new LoggerBridgeGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(loggerBridgeGenerator);

        var compilation = CSharpCompilation.Create
        (
            "Test",
            syntaxTrees: 
            [
                CSharpSyntaxTree.ParseText
                (
                    """
                    using System;
                    using System.Diagnostics;
                    using System.Threading;
                    using System.Threading.Tasks;
                    using dotnetCampus.Logging.Attributes;
                    using dotnetCampus.Logging.Configurations;
                    using dotnetCampus.Logging.Writers;
                    
                    namespace LoggerSample.MainApp;
                    
                    [ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
                    [ImportLoggerBridge<global::LoggerSample.LoggerIndependentProject.Logging.ILoggerBridge>]
                    internal partial class LoggerBridgeLinker;
                    """
                )
            ],
            references:
            new[]
            {
                MetadataReference.CreateFromFile(typeof(LoggerSample.LoggerIndependentProject.SourceReferenceTarget).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(LoggerSample.LoggerIndependentLibrary.SourceReferenceTarget).Assembly.Location)
            } // 加上整个 dotnet 的基础库
            .Concat(MetadataReferenceProvider.GetDotNetMetadataReferenceList())
        );

        driver = driver.RunGenerators(compilation);
        var result = driver.GetRunResult();
        Assert.IsNotNull(result);
    }
}