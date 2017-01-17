/*
   Copyright 2012 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/ 
//-CRE-


using System.Linq;
using Glass.Mapper.Pipelines.ConfigurationResolver.Tasks.OnDemandResolver;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using NUnit.Framework;
using Sitecore.Data;

namespace Glass.Mapper.Sc.FakeDb.Configuation.Attributes
{
    [TestFixture]
    public class SitecoreAttributeConfigurationLoaderFixture
    {

        [Test]
        public void Load_StubClassConfigured_ReturnsStubClassAndProperties()
        {
            //Assign
            var loader = new OnDemandLoader<SitecoreTypeConfiguration>(typeof(StubClass));

            //Act
            var results = loader.Load();
            
            //Assert
            Assert.GreaterOrEqual(results.Count(), 0);

            var typeConfig = results.First(x => x.Type == typeof (StubClass));
            Assert.IsNotNull(typeConfig);

            var propertyNames = new[] {"Children", "Field", "Id", "Info", "Linked", "Node", "Parent", "Query"};

            foreach(var propertyName in propertyNames)
            {
                var propInfo = typeof (StubClass).GetProperty(propertyName);
                Assert.IsTrue(typeConfig.Properties.Any(x=>x.PropertyInfo == propInfo));
            }

        }


        #region Stubs

        [SitecoreType]
        public class StubClass
        {
            [SitecoreChildren]
            public string Children { get; set; }

            [SitecoreField]
            public string Field { get; set; }

            [SitecoreId]
            public ID Id { get; set; }

            [SitecoreInfo]
            public string Info { get; set; }

            [SitecoreLinked]
            public string Linked { get; set; }

            [SitecoreNode]
            public string Node { get; set; }

            [SitecoreParent]
            public string Parent { get; set; }

            [SitecoreQuery("")]
            public string Query { get; set; }

            
        }

        #endregion
    }
}




