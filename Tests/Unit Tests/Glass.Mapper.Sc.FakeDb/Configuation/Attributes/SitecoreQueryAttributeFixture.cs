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


using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using NUnit.Framework;

namespace Glass.Mapper.Sc.FakeDb.Configuation.Attributes
{
    [TestFixture]
    public class SitecoreQueryAttributeFixture
    {

        

        #region Method - Configure

        [Test]
        public void Configure_ConfigureCalled_SitecoreQueryConfigurationReturned()
        {
            //Assign
            SitecoreQueryAttribute attr = new SitecoreQueryAttribute(string.Empty);
            var propertyInfo = typeof(StubClass).GetProperty("QueryContextProperty");


            //Act
            var result = attr.Configure(propertyInfo) as SitecoreQueryConfiguration;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [Sequential]
        public void Configure_ConfigureQueryContext_QueryContextSetOnConfigObject(
            [Values(true, false)] bool queryContextValue,
            [Values(true, false)] bool expectedValue)
        {
            //Assign
            SitecoreQueryAttribute attr = new SitecoreQueryAttribute(string.Empty);
            var propertyInfo = typeof (StubClass).GetProperty("QueryContextProperty");
            
            attr.UseQueryContext = queryContextValue;
            
            //Act
            var result = attr.Configure(propertyInfo) as SitecoreQueryConfiguration;

            //Assert
            Assert.AreEqual(expectedValue, result.UseQueryContext);
        }

        #endregion

        #region Stubs

        public class StubClass
        {
            public string QueryContextProperty { get; set; }
        }

        #endregion
    }
}




