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
    <PackageReference Include="appSettingsEditor" Version="2021.3.21.2000" />
    <PackageReference Include="appSettingsEditorAPI" Version="2021.3.21.2000" />
  	<CompilerVisibleItemMetadata Include="AdditionalFiles" MetadataName="generateAPI" />
    <AdditionalFiles Include="appsettings.json" generateAPI="true" >
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
	  
      
