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


using System;
using Glass.Mapper.Pipelines.DataMapperResolver;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.DataMappers;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;

namespace Glass.Mapper.Sc.FakeDb.DataMappers
{
    [TestFixture]
    public class SitecoreIdMapperFixture
    {
      


        #region Property - ReadOnly

        [Test]
        public void ReadOnly_ReturnsTrue()
        {
            //Assign
            var mapper = new SitecoreIdMapper();
            bool expected = true;

            //Act
            bool value = mapper.ReadOnly;


            //Assert
            Assert.AreEqual(expected,value);

        }



        #endregion

        #region Method - MapToProperty

        [Test]
        public void MapToProperty_ItemIdAsGuid_ReturnsIdAsGuid()
        {
            //Assign
            string targetPath = "/sitecore/content/target";

            using (Db database = new Db
            {
                new Sitecore.FakeDb.DbItem("Target")
            })
            {
                var mapper = new SitecoreIdMapper();
                var config = new SitecoreIdConfiguration();
                var property = typeof(Stub).GetProperty("GuidId");
                var item = database.GetItem("/sitecore/content/target");

                Assert.IsNotNull(item, "Item is null, check in Sitecore that item exists");

                config.PropertyInfo = property;

                mapper.Setup(new DataMapperResolverArgs(null, config));

                var dataContext = new SitecoreDataMappingContext(null, item, null);
                var expected = item.ID.Guid;

                //Act
                var value = mapper.MapToProperty(dataContext);

                //Assert
                Assert.AreEqual(expected, value);
            }
        }

        [Test]
        public void MapToProperty_ItemIdAsID_ReturnsIdAsID()
        {
            //Assign
            string targetPath = "/sitecore/content/target";

            using (Db database = new Db
            {
                new Sitecore.FakeDb.DbItem("Target")
            })
            {
                var mapper = new SitecoreIdMapper();
                var config = new SitecoreIdConfiguration();
                var property = typeof(Stub).GetProperty("IDId");

                config.PropertyInfo = property;

                mapper.Setup(new DataMapperResolverArgs(null, config));

                var item = database.GetItem("/sitecore/content/Target");

                Assert.IsNotNull(item, "Item is null, check in Sitecore that item exists");
                var dataContext = new SitecoreDataMappingContext(null, item, null);
                var expected = item.ID;

                //Act
                var value = mapper.MapToProperty(dataContext);

                //Assert
                Assert.AreEqual(expected, value);
            }
        }

        [Test]
       // [ExpectedException(typeof(NotSupportedException))]
        public void MapToProperty_ItemIdAsString_ThrowsException()
        {
            //Assign
            string targetPath = "/sitecore/content/target";

            using (Db database = new Db
            {
                new Sitecore.FakeDb.DbItem("Target")
            })
            {
                var mapper = new SitecoreIdMapper();
                var config = new SitecoreIdConfiguration();
                var property = typeof(Stub).GetProperty("StringId");

                config.PropertyInfo = property;

                
                var item = database.GetItem("/sitecore/content/target");

                Assert.IsNotNull(item, "Item is null, check in Sitecore that item exists");

                //Act
                Assert.Throws<NotSupportedException>(() =>
                {
                    mapper.Setup(new DataMapperResolverArgs(null, config));

                });

                //Assert
                //Exception, not asserts
            }
        }


        #endregion

        #region Stubs

        public class Stub
        {
            public Guid GuidId { get; set; }

            public ID IDId { get; set; }

            public string StringId { get; set; }
        }

        #endregion
    }
}




