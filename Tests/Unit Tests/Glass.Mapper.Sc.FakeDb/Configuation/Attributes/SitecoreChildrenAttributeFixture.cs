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
using Glass.Mapper.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using NUnit.Framework;

namespace Glass.Mapper.Sc.FakeDb.Configuation.Attributes
{
    [TestFixture]
    public class SitecoreChildrenAttributeFixture
    {
        [Test]
        public void Does_SitecoreChildrenAttribute_Extend_ChildrenAttribute()
        {
            Assert.IsTrue(typeof(ChildrenAttribute).IsAssignableFrom(typeof(SitecoreChildrenAttribute)));
        }

        [Test]
        [TestCase("IsLazy")]
        [TestCase("InferType")]
        public void Does_SitecoreChildrenAttribute_Have_Properties(string propertyName)
        {
            var properties = typeof(SitecoreChildrenAttribute).GetProperties();
            Assert.IsTrue(properties.Any(x => x.Name == propertyName));
        }

        [Test]
        public void Does_Default_Constructor_Set_IsLazy_True()
        {
            Assert.IsTrue(new TestSitecoreChildrenAttribute().IsLazy);
        }

        #region Method - Configure

        [Test]
        public void Configure_ConfigureCalled_SitecoreQueryConfigurationReturned()
        {
            //Assign
            SitecoreChildrenAttribute attr = new SitecoreChildrenAttribute();
            var propertyInfo = typeof(StubClass).GetProperty("DummyProperty");


            //Act
            var result = attr.Configure(propertyInfo) as SitecoreChildrenConfiguration;

            //Assert
            Assert.IsNotNull(result);
        }

        #endregion

        #region Stubs

        public class StubClass
        {
            public string DummyProperty { get; set; }
        }

        private class TestSitecoreChildrenAttribute : SitecoreChildrenAttribute
        {
        }

        #endregion
    }
}




