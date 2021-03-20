using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RoslynSettingEditor
{
    /// <summary>
    /// Represents a class that can be generated using Scriban.
    /// </summary>
    [DebuggerDisplay("{ClassName}")]
    public class ClassModel
    {
        private static readonly string[] numericPropertyTypeOrder = new[]
        {
            "int",
            "long",
            "double",
            "decimal"
        };
        private ClassModel[] cache;
        private ClassModel[] PathParents()
        {
            if (cache != null)
                return cache;

            var parents = new List<ClassModel>();
            var parent1 = this.parentClass;
            while (parent1 != null)
            {
                parents.Add(parent1);
                parent1 = parent1.parentClass;
            }
            cache = parents.ToArray();
            return cache;

        }
        public string path
        {
            get
            {
                var p = new List<ClassModel>(PathParents());
                p.Reverse();
                p.Add(this);
                p.RemoveAt(0);
                var ret=string.Join(".", p.Select(it=>it.ClassName));
                return ret;
                
            }
        }
        public ClassModel parentClass;
        /// <summary>
        /// The name of the class, that should be valid in C#.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The properties that can be generated inside the class.
        /// </summary>
        public List<PropertyModel> Properties { get; } = new List<PropertyModel>();

        /// <summary>
        /// Create a new instance of this class based on its name.
        /// </summary>
        /// <param name="className"></param>
        public ClassModel(string className, ClassModel parent)
        {
            ClassName = className;
            this.parentClass = parent;
        }

        /// <summary>
        /// Takes properties from a similar ClassModel and adds them here, to get a more complete version of the ClassModel.
        /// </summary>
        /// <param name="classModel">The ClassModel to get additional properties from</param>
        public void Merge(ClassModel classModel)
        {
            if (classModel != null)
            {
                foreach (var property in classModel.Properties)
                {
                    var existingProp = Properties.FirstOrDefault(p => p.PropertyName == property.PropertyName);
                    if (existingProp == null)
                    {
                        Properties.Add(property);
                    }
                    else if (existingProp.PropertyType != property.PropertyType)
                    {
                        // If there is a less restrictive property type that is needed, it must be changed
                        if (numericPropertyTypeOrder.Contains(existingProp.PropertyType)
                            && numericPropertyTypeOrder.Contains(property.PropertyType)
                            && Array.IndexOf(numericPropertyTypeOrder, existingProp.PropertyType) < Array.IndexOf(numericPropertyTypeOrder, property.PropertyType))
                        {
                            existingProp.PropertyType = property.PropertyType;
                        }
                        else if (existingProp.PropertyType == "DateTime" && property.PropertyType == "string")
                        {
                            existingProp.PropertyType = property.PropertyType;
                        }
                    }
                }
            }
        }
    }
}
