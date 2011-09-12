using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;

namespace PropertyPathManager_Specification
{

    public class when_reading : context
    {
        [TestCase("", "", PathComponentType.None, "", PathComponentType.None)]
        [TestCase(null, "", PathComponentType.None, "", PathComponentType.None)]
        [TestCase(".", "", PathComponentType.None, "", PathComponentType.None)]
        [TestCase(":", "", PathComponentType.None, "", PathComponentType.None)]
        [TestCase("Property", "Property", PathComponentType.Member, "", PathComponentType.None,
            TestName = "a property is read")]
        [TestCase(":Indexer", "Indexer", PathComponentType.Indexer, "", PathComponentType.None,
            TestName = "an indexer is read")]
        [TestCase("Property.Property2", "Property", PathComponentType.Member, "Property2", PathComponentType.Member,
            TestName = "a property is read and the leftover starts after the dot")]
        [TestCase(":Indexer.Property2", "Indexer", PathComponentType.Indexer, "Property2", PathComponentType.Member,
            TestName = "an indexer is read and the leftover starts after the dot")]
        [TestCase("Property:Indexer", "Property", PathComponentType.Member, "Indexer", PathComponentType.Indexer,
            TestName = "a member is read until an indexer and the leftover contains the column")]
        public void reading_path_components(string value, string parsedPart, PathComponentType type, string parsedPartTwo,PathComponentType result2)
        {
            var components = new PathManager().ReadComponents(value).ToList();

            var parseResult1 = components.Count > 0 ? components[0] : new PathComponent();
            var parseResult2 = components.Count > 1 ? components[1] : new PathComponent();
            
            parseResult1.ParsedValue.ShouldBe(parsedPart);
            parseResult1.Type.ShouldBe(type);

            parseResult2.ParsedValue.ShouldBe(parsedPartTwo);
            parseResult2.Type.ShouldBe(result2);
        }
        [TestCase(":0", ":0", PathComponentType.Member, new[]{""})]
        [TestCase("", "", PathComponentType.Constructor, new[] { "" })]
        [TestCase("Customer", "", PathComponentType.Constructor, new[] { "Customer", "c" },
            TestName = "the first prefix is a constructor")]
        [TestCase("c", "", PathComponentType.Constructor, new[] { "Customer", "c" },
            TestName="the second prefix is a constructor")]
        [TestCase("c.Name", "Name", PathComponentType.Member, new[] { "Customer", "c" })]
        [TestCase("Customer.Name", "Name", PathComponentType.Member, new[] { "Customer", "c" },
            TestName = "prefix is stripped and member is returned")]
        [TestCase("customer.Name", "Name", PathComponentType.Member, new[] { "Customer", "c" },
            TestName = "prefix is matched case-insensitively")]
        public void getting_path_type(string path, string parsedPart, PathComponentType componentType, string[] prefixes)
        {
            var pathType = new PathManager().GetPathType(prefixes, path);

            pathType.Type.ShouldBe(componentType);
            pathType.ParsedValue.ShouldBe(parsedPart);
        }
    }
}
