using Scriban;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RoslynSettingEditor.Json2Class
{
    class GeneratorFromJSON
    {
        List<ClassModel> classModels =null;
        public string GenerateFile(string content, string rootTypeName, string namespaceName, string NameFile)
        {
            var json = JsonDocument.Parse(content);
            if (classModels == null)
            {
                classModels = new List<ClassModel>();
                var jsonElement = json.RootElement;
                ResolveTypeRecursive(classModels, jsonElement, rootTypeName.Replace(".","_"), null);
            }
            var appSettingsClass = classModels.First(it => it.parentClass == null);
            var t = EmbeddedResource.GetContent(NameFile);
            var template = Template.Parse(t);
          
            string generatedCode = template.Render(new
            {
                fullNameFile = rootTypeName,
                nameFileNoExt = rootTypeName,
                NamespaceName = namespaceName,
                ClassModels = classModels,
                ToolName = ThisAssembly.Info.Product,
                ToolVersion = ThisAssembly.Info.Version,
                appSettingsClass = appSettingsClass
            }, member => member.Name);
            return generatedCode;
        }
        /// <summary>
        /// Reads json and fills the classModels list with relevant type definitions.
        /// </summary>
        /// <param name="classModels">A list that needs to be populated with resolved types</param>
        /// <param name="jsonElement">The current json element that is being read</param>
        /// <param name="typeName">The current type name that is being read</param>
        private static void ResolveTypeRecursive(List<ClassModel> classModels, JsonElement jsonElement, string typeName, ClassModel parent)
       {
           var classModel = new ClassModel(typeName,parent);

           // Arrays should be enumerated and handled individually
           if (jsonElement.ValueKind == JsonValueKind.Array)
           {
               var jsonArrayEnumerator = jsonElement.EnumerateArray();
               while (jsonArrayEnumerator.MoveNext())
               {
                   ResolveTypeRecursive(classModels, jsonArrayEnumerator.Current, typeName,classModel);
               }

               return;
           }

           if (jsonElement.ValueKind == JsonValueKind.Object)
           {
               int orderCounter = 0;

               // Iterate the properties of the json element, they will become model properties
               foreach (JsonProperty prop in jsonElement.EnumerateObject())
               {
                   string propName = RenameIfDuplicateOrConflicting(GetValidName(prop.Name), classModel);

                   if (propName.Length > 0)
                   {
                       PropertyModel propertyModel;

                       // The json value kind of the property determines how to map it to a C# type
                       switch (prop.Value.ValueKind)
                       {
                           case JsonValueKind.Array:
                               {
                                   string arrPropName = GetValidName(prop.Name, true);

                                   // Look at the first element in the array to determine the type of the array
                                   var arrEnumerator = prop.Value.EnumerateArray();
                                   if (arrEnumerator.MoveNext())
                                   {
                                       if (arrEnumerator.Current.ValueKind == JsonValueKind.Number)
                                       {
                                           arrPropName = FindBestNumericType(arrEnumerator.Current);
                                       }
                                       else if (arrEnumerator.Current.ValueKind == JsonValueKind.String)
                                       {
                                           arrPropName = FindBestStringType(arrEnumerator.Current);
                                       }
                                       else if (arrEnumerator.Current.ValueKind == JsonValueKind.True || arrEnumerator.Current.ValueKind == JsonValueKind.False)
                                       {
                                           arrPropName = "bool";
                                       }
                                       else
                                       {
                                           ResolveTypeRecursive( classModels, prop.Value, arrPropName, classModel);
                                       }

                                       propertyModel = new PropertyModel(prop.Name, $"IList<{arrPropName}>", propName, orderCounter++)
                                       {
                                           Init = $"new List<{arrPropName}>()"
                                       };
                                   }
                                   else
                                   {
                                       propertyModel = new PropertyModel(prop.Name, $"IList<object>", propName, orderCounter++)
                                       {
                                           Init = $"new List<object>()"
                                       };
                                   }

                                   break;
                               }
                           case JsonValueKind.String: propertyModel = new PropertyModel(prop.Name, FindBestStringType(prop.Value), propName, orderCounter++); break;
                           case JsonValueKind.Number: propertyModel = new PropertyModel(prop.Name, FindBestNumericType(prop.Value), propName, orderCounter++); break;
                           case JsonValueKind.False:
                           case JsonValueKind.True: propertyModel = new PropertyModel(prop.Name, "bool", propName, orderCounter++); break;
                           case JsonValueKind.Object:
                               {
                                   string objectPropName = GetValidName(prop.Name, true);

                                   // Create a separate type for objects
                                   ResolveTypeRecursive( classModels, prop.Value, objectPropName, classModel);

                                   propertyModel = new PropertyModel(prop.Name, objectPropName, propName, orderCounter++);
                                   break;
                               }
                           case JsonValueKind.Undefined:
                           case JsonValueKind.Null:
                           default: propertyModel = new PropertyModel(prop.Name, "object", propName, orderCounter++); break;
                       }

                       classModel.Properties.Add(propertyModel);
                   }
               }
           }

           // If there is already a model defined that matches by name, then we add any new properties by merging the models
           var matchingClassModel = classModels.FirstOrDefault(
               c => string.Equals(c.ClassName, classModel.ClassName, StringComparison.InvariantCulture));
           if (matchingClassModel != null)
           {
               matchingClassModel.Merge(classModel);
           }
           else
           {
               // No need to merge, just add the new class model
               classModels.Add(classModel);
           }
       }

       /// <summary>
       /// Based on the value specified, determine an appropriate numeric type.
       /// </summary>
       /// <param name="propertyValue">Example value of the property</param>
       /// <returns>The name of the numeric type</returns>
       private static string FindBestNumericType(JsonElement propertyValue)
       {
           if (propertyValue.TryGetInt32(out _))
           {
               return "int";
           }

           if (propertyValue.TryGetInt64(out _))
           {
               return "long";
           }

           if (propertyValue.TryGetDouble(out var doubleVal)
               && propertyValue.TryGetDecimal(out var decimalVal)
               && Convert.ToDecimal(doubleVal) == decimalVal)
           {
               return "double";
           }

           if (propertyValue.TryGetDecimal(out _))
           {
               return "decimal";
           }

           return "object";
       }
       /// <summary>
       /// Based on the value specified, determine if anything better than "string" can be used.
       /// </summary>
       /// <param name="current">Example value of the property</param>
       /// <returns>string or something better</returns>
       private static string FindBestStringType(JsonElement propertyValue)
       {
           if (propertyValue.TryGetDateTime(out _))
           {
               return "DateTime";
           }

           return "string";
       }

       private static readonly char[] forbiddenCharacters = new[] { ' ', '-', ':', ';', '.' };

       /// <summary>
       /// Gets a name that is valid in C# and makes it Pascal-case.
       /// Optionally, it can singularize the name, so that a list property has a proper model class.
       /// E.g. Cars will have a model type of Car.
       /// </summary>
       /// <param name="typeName">The type name that is possibly not valid in C#</param>
       /// <param name="singularize">If true, the name will be singularized if it is plural</param>
       /// <returns>A valid C# Pascal-case name</returns>
       private static string GetValidName(string typeName, bool singularize = false)
       {

           List<char> newTypeName = new List<char>();
           bool nextCharUpper = true;
           for (int i = 0; i < typeName.Length; i++)
           {
               // Strip spaces and other characters
               if (forbiddenCharacters.Contains(typeName[i]))
               {
                   nextCharUpper = true;
                   continue;
               }

               // Pascal casing
               if (nextCharUpper)
               {
                   nextCharUpper = false;
                   if (!char.IsUpper(typeName[i]))
                   {
                       newTypeName.Add(char.ToUpper(typeName[i], CultureInfo.InvariantCulture));
                       continue;
                   }
               }

               newTypeName.Add(typeName[i]);
           }

           string validName = new string(newTypeName.ToArray());
           if (validName == "System")
           {
               return "SystemX";
           }

           return validName;
       }
       private static readonly Regex parseNumberFromPropertyName = new Regex("(.*Property)([0-9]+)", RegexOptions.Compiled); 


       private static string RenameIfDuplicateOrConflicting(string propertyName, ClassModel classModel)
       {
           const string postFix = "Property";
           string newPropertyName = propertyName;
           if (string.Equals(propertyName, classModel.ClassName, StringComparison.InvariantCulture))
           {
               // Property name conflicts with class name, so add a postfix
               newPropertyName = $"{propertyName}{postFix}";
           }

           while (classModel.Properties.Any(p => p.PropertyName == newPropertyName))
           {
               var match = parseNumberFromPropertyName.Match(newPropertyName);
               if (match.Success)
               {
                   newPropertyName = $"{match.Groups[1].Value}{int.Parse(match.Groups[2].Value) + 1}";
               }
               else
               {
                   newPropertyName = $"{newPropertyName}2";
               }
           }
           if(long.TryParse(newPropertyName.Substring(0,1), out var _))
            {
                newPropertyName = "Number_" + newPropertyName;
            }
           return newPropertyName;
       }

    
    }
}
