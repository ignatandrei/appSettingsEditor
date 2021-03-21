# appSettingsEditor
app settings editor - powered by Roslyn


 ** C# 9.0 ONLY **

      Autogenerates controller API for appsettings.json
      Add 2 NUGET  references:

      appSettingEditorAPI
      appSettingsEditor

	The csproj looks like this
	
	
```xml
	
<ItemGroup>
	<AdditionalFiles Include="appsettings.json">
		<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
	</AdditionalFiles>
</ItemGroup>
<PropertyGroup>
	<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
	<CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

```

      If you want to see a GUI for settings , please do
	
```csharp
app.UseEndpoints(endpoints =>
{
endpoints.MapControllers(); // add next line
endpoints.MapSettingsView <appsettings>(Configuration);
});
```

      If you want to handle security, add

```csharp
public partial class appsettingsController : ControllerBase
{
//private partial void BeforeGet(appsettings data)
//{

//}
//private partial void BeforeSave(appsettings data, appsettings original)
//{

//}

}
```
	  
      
