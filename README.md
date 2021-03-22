# appSettingsEditor
app settings editor - powered by Roslyn

## What it does

Autogenerates controller API for appsettings.json

Optional, it has also an endpoint for a HTML GUI editor

This is how swagger looks :

<img src='https://github.com/ignatandrei/appSettingsEditor/blob/main/docs/images/swagger.png?raw=true' />

And you can browse to /settingsUI to have this editor ( powered by https://github.com/json-editor/json-editor )
	
<img src='https://github.com/ignatandrei/appSettingsEditor/blob/main/docs/images/settingsui.png?raw=true' />
	


## How to use 

  Add 2 NUGET  references:

  appSettingEditorAPI
  appSettingsEditor

For your convenience, please add to the csproj :
	
	
```xml
	
<ItemGroup>
    <PackageReference Include="appSettingsEditor" Version="2021.3.21.2300" />
    <PackageReference Include="appSettingsEditorAPI" Version="2021.3.21.2300" />
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

If you want to see a GUI for settings ( /settingsUI) , please do
	
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
//partial void BeforeGet(appsettings data)
//{

//}
//partial void BeforeSave(appsettings data, appsettings original)
//{

//}

}
```
	  
## Known problems and solving

1. I have a versioning API .       


2. I have another file name app_custom_settings.json, not appsettings.json


3. I have multiple appsettings.json