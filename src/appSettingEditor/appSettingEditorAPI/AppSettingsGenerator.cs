using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using RoslynSettingEditor.Json2Class;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace appSettingEditorAPI
{
    [Generator]
    public class AppSettingsGenerator : ISourceGenerator
    {

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }
        Diagnostic DoDiagnostic(DiagnosticSeverity ds, string message)
        {
            //info  could be seen only with 
            // dotnet build -v diag
            var dd = new DiagnosticDescriptor(ThisAssembly.Info.Product, $"StartExecution", $"{message}", "SkinnyControllers", ds, true);
            var d = Diagnostic.Create(dd, Location.None, "andrei.txt");
            return d;
        }
        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                string namespaceName = context.Compilation?.AssemblyName;

                //var settings = context.AdditionalFiles.FirstOrDefault(file =>
                //        file.Path.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase)
                //        &&
                //        file.Path.IndexOf("appsettings", StringComparison.InvariantCultureIgnoreCase) > -1
                //        );

                var allSettings = context.AdditionalFiles.Where(
                    file =>
                    {
                        var f = context.AnalyzerConfigOptions.GetOptions(file);
                        if (!f.TryGetValue("build_metadata.AdditionalFiles.generateAPI", out string val))
                            return false;

                        return (val == "true");
                    }
                    ).ToArray();
                ;

                if (!allSettings.Any())
                {
                    context.ReportDiagnostic(DoDiagnostic(DiagnosticSeverity.Warning,
                        $"do not have an appsettings.json with generateAPI"));
                    return;
                }
                foreach (var settings in allSettings)
                {

                    string nameSettings = Path.GetFileNameWithoutExtension(settings.Path);
                    //TODO : diagnostic
                    var jsonFileText = settings.GetText();
                    if (jsonFileText == null)
                    {
                        context.ReportDiagnostic(DoDiagnostic(DiagnosticSeverity.Warning,
        $"is this any content in {nameSettings} ?"));

                        return;
                    }
                    //var generatedCode = GenerateFile(jsonFileText.ToString(), namespaceName);
                    //var g = new GeneratorData();
                    //var generatedCode= g.GenerateFile(jsonFileText.ToString(),namespaceName);
                    //context.AddSource($"{nameSettings}.gen.cs", SourceText.From(generatedCode, Encoding.UTF8));
                    //var json = JsonDocument.Parse(jsonFileText.ToString());

                    var g = new GeneratorFromJSON();
                    var generatedCode = g.GenerateFile(jsonFileText.ToString(), nameSettings, namespaceName, "appsettings.txt");
                    context.AddSource($"{nameSettings}.gen.cs", SourceText.From(generatedCode, Encoding.UTF8));

                    generatedCode = g.GenerateFile(jsonFileText.ToString(), nameSettings, namespaceName, "Controller.txt");
                    context.AddSource($"{nameSettings}Controller.gen.cs", SourceText.From(generatedCode, Encoding.UTF8));
                }
            }
            catch(Exception ex)
            {
                context.ReportDiagnostic(DoDiagnostic(DiagnosticSeverity.Error,$"error:"+ex.Message));

            }
        }

    }
}