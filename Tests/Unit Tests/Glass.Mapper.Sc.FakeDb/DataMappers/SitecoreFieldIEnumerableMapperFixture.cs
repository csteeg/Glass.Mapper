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


using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Pipelines.DataMapperResolver;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.DataMappers;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;

namespace Glass.Mapper.Sc.FakeDb.DataMappers
{
    [TestFixture]
    public class SitecoreFieldIEnumerableMapperFixture 
    {

        #region Method - CanHandle

        [Test]
        public void CanHandle_PropertyIsIEnumerable_ReturnsTrue()
        {
            //Assign
            var config = new SitecoreFieldConfiguration();
            config.PropertyInfo = typeof (StubClass).GetProperty("IEnumerable");
            var mapper = new SitecoreFieldIEnumerableMapper();

            //Act
            var result = mapper.CanHandle(config, null);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanHandle_PropertyIsIList_ReturnsTrue()
        {
            //Assign
            var config = new SitecoreFieldConfiguration();
            config.PropertyInfo = typeof(StubClass).GetProperty("IList");
            var mapper = new SitecoreFieldIEnumerableMapper();

            //Act
            var result = mapper.CanHandle(config, null);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanHandle_PropertyIsArray_ReturnsFalse()
        {
            //Assign
            var config = new SitecoreFieldConfiguration();
            config.PropertyInfo = typeof(StubClass).GetProperty("Array");
            var mapper = new SitecoreFieldIEnumerableMapper();

            //Act
            var result = mapper.CanHandle(config, null);

            //Assert
            Assert.IsFalse(result);
        }
        #endregion

        #region Method - Setup

        [Test]
        public void Setup_SubMapperIsAssigned()
        {
            //Assign
            var config = new SitecoreFieldConfiguration();
            config.PropertyInfo = typeof(StubClass).GetProperty("IList");

            var mapper = new SitecoreFieldIEnumerableMapper();
            var subMapper = new SitecoreFieldDoubleMapper();

            var args = new DataMapperResolverArgs(null, config);
            args.DataMappers = new[] {subMapper};

            //Act
            mapper.Setup(args);

            //Assert
            Assert.AreEqual(subMapper, mapper.Mapper);
        }

        [Test]
        public void Setup_SubMapperMissing_ExceptionThrown()
        {
            //Assign
            var config = new SitecoreFieldConfiguration();
            config.PropertyInfo = typeof(StubClass).GetProperty("IEnumerable");

            var mapper = new SitecoreFieldIEnumerableMapper();
            var subMapper = new SitecoreFieldDoubleMapper();

            var args = new DataMapperResolverArgs(null, config);
            args.DataMappers = new[] { subMapper };

            //Act
            Assert.Throws<MapperException>(() =>
            {
                mapper.Setup(args);

            });

            //Assert
        }


        #endregion


        #region Method - GetField

        [Test]
        public void GetField_ContainsPipeSeparatedValues_ReturnsListOfValues()
        {
            //Assign
            var templateId = ID.NewID;
            var fieldId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"Field", ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                var fieldValue = "1|2|3";
                var item =
                    database.GetItem("/sitecore/content/Target");
                var field = item.Fields["Field"];

                var config = new SitecoreFieldConfiguration();
                config.PropertyInfo = typeof(StubClass).GetProperty("IList");

                var mapper = new SitecoreFieldIEnumerableMapper();
                var subMapper = new SitecoreFieldDoubleMapper();

                var args = new DataMapperResolverArgs(null, config);
                args.DataMappers = new[] {subMapper};

                mapper.Setup(args);

                using (new ItemEditing(item, true))
                {
                    field.Value = fieldValue;
                }

                //Act

                var result = mapper.GetField(field, config, null) as List<double>;

                //Assert
                Assert.AreEqual(1D, result.Skip(0).First());
                Assert.AreEqual(2D, result.Skip(1).First());
                Assert.AreEqual(3D, result.Skip(2).First());
            }
        }

        [Test]
        public void GetField_EmptyField_ReturnsEmptyList()
        {
            //Assign
            var templateId = ID.NewID;
            var fieldId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"Field", ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                var fieldValue = "";
                var item =
                    database.GetItem("/sitecore/content/Target");
                var field = item.Fields["Field"];

                var config = new SitecoreFieldConfiguration();
                config.PropertyInfo = typeof(StubClass).GetProperty("IList");

                var mapper = new SitecoreFieldIEnumerableMapper();
                var subMapper = new SitecoreFieldDoubleMapper();

                var args = new DataMapperResolverArgs(null, config);
                args.DataMappers = new[] {subMapper};

                mapper.Setup(args);

                using (new ItemEditing(item, true))
                {
                    field.Value = fieldValue;
                }

                //Act

                var result = mapper.GetField(field, config, null) as List<double>;

                //Assert
                Assert.AreEqual(0, result.Count);
            }
        }


        #endregion

        #region SetField

        [Test]
        public void SetField_ListContainsValues_SetsPipedList()
        {
            //Assign
            var templateId = ID.NewID;
            var fieldId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"Field", ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                var expected = "3|2|1";
                var item =
                    database.GetItem("/sitecore/content/Target");
                var field = item.Fields["Field"];
                var value = new List<double>(new[] {3D, 2D, 1D});

                var config = new SitecoreFieldConfiguration();
                config.PropertyInfo = typeof(StubClass).GetProperty("IList");

                var mapper = new SitecoreFieldIEnumerableMapper();
                var subMapper = new SitecoreFieldDoubleMapper();

                var args = new DataMapperResolverArgs(null, config);
                args.DataMappers = new[] {subMapper};

                mapper.Setup(args);

                using (new ItemEditing(item, true))
                {
                    field.Value = string.Empty;
                }

                //Act
                using (new ItemEditing(item, true))
                {

                    mapper.SetField(field, value, config, null);
                }
                //Assert
                Assert.AreEqual(expected, field.Value);
            }
        }

        [Test]
        public void SetField_ListContainsNoValues_SetsEmptyField()
        {
            //Assign
            var templateId = ID.NewID;
            var fieldId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"Field", ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                var expected = string.Empty;
                var item =
                    database.GetItem("/sitecore/content/Target");
                var field = item.Fields["Field"];
                var value = new List<double>();

                var config = new SitecoreFieldConfiguration();
                config.PropertyInfo = typeof(StubClass).GetProperty("IList");

                var mapper = new SitecoreFieldIEnumerableMapper();
                var subMapper = new SitecoreFieldDoubleMapper();

                var args = new DataMapperResolverArgs(null, config);
                args.DataMappers = new[] {subMapper};

                mapper.Setup(args);

                using (new ItemEditing(item, true))
                {
                    field.Value = string.Empty;
                }

                //Act
                using (new ItemEditing(item, true))
                {

                    mapper.SetField(field, value, config, null);
                }
                //Assert
                Assert.AreEqual(expected, field.Value);
            }
        }

        #endregion
        #region Stubs

        public class StubClass
        {
            public IEnumerable<int> IEnumerable { get; set; }
            public IList<double> IList { get; set; }
            public int[] Array { get; set; }
        }

        #endregion
    }
}




