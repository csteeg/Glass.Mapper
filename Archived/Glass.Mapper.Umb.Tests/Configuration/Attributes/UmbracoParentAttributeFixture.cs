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
using FluentAssertions;
using Glass.Mapper.Umb.Configuration;
using Glass.Mapper.Umb.Configuration.Attributes;
using NUnit.Framework;
using Glass.Mapper.Configuration.Attributes;

namespace Glass.Mapper.Umb.Tests.Configuration.Attributes
{
    [TestFixture]
    public class UmbracoParentAttributeFixture
    {
        [Test]
        public void Does_UmbracoParentAttribute_Extend_ParentAttribute()
        {
            //Assign
            var type = typeof(ParentAttribute);

            //Act

            //Assert
            type.IsAssignableFrom(typeof(UmbracoParentAttribute)).Should().BeTrue();
        }

        [Test]
        [TestCase("IsLazy")]
        [TestCase("InferType")]
        public void Does_UmbracoNodeAttributee_Have_Properties(string propertyName)
        {
            //Assign
            var properties = typeof(UmbracoParentAttribute).GetProperties();

            //Act

            //Assert
            properties.Any(x => x.Name == propertyName).Should().BeTrue();
        }

        [Test]
        public void Does_Constructor_Set_IsLazy_True()
        {
            //Assign
            var parentAttribute = new UmbracoParentAttribute();

            //Act

            //Assert
            parentAttribute.IsLazy.Should().BeTrue();
        }

        [Test]
        public void Does_Constructor_Set_InferType_False()
        {
            //Assign
            var parentAttribute = new UmbracoParentAttribute();

            //Act

            //Assert
            parentAttribute.InferType.Should().BeFalse();
        }

        #region Method - Configure

        [Test]
        public void Configure_ConfigureCalled_UmbracoInfoConfigurationReturned()
        {
            //Assign
            var attr = new UmbracoParentAttribute();
            var propertyInfo = typeof(StubClass).GetProperty("DummyProperty");

            //Act
            var result = attr.Configure(propertyInfo) as UmbracoParentConfiguration;

            //Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region Stubs

        public class StubClass
        {
            public string DummyProperty { get; set; }
        }

        #endregion
    }
}




