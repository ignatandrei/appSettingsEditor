cd appSettingEditorAPI
dotnet pack -o ../bin/ --include-source --include-symbols 
cd ..
cd appSettingsEditor
dotnet pack -o ../bin/ --include-source --include-symbols 
