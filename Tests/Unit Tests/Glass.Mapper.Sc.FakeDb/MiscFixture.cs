﻿/*
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Glass.Mapper.Pipelines.ConfigurationResolver.Tasks.OnDemandResolver;
using Glass.Mapper.Profilers;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Fields;
using Glass.Mapper.Sc.Pipelines.DataMapper;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.SecurityModel;

namespace Glass.Mapper.Sc.FakeDb
{
    [TestFixture]
    public class MiscFixture
    {
        [Test]
        public void InterfaceIssueInPageEditorWhenInterfaceInheritsFromAnInterfaceWithSimilarName()
        {
            /*
             * This test is in response to issue 53 raised on the Glass.Sitecore.Mapper
             * project. When two interfaces have similar names are created as proxies
             * the method GetTypeConfiguration returns the wrong config.
             */

            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            var fieldName = "Field";

            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {fieldName, ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                var context = Context.Create(Utilities.CreateStandardResolver());
                context.Load(new OnDemandLoader<SitecoreTypeConfiguration>(typeof(IBase)));
                context.Load(new OnDemandLoader<SitecoreTypeConfiguration>(typeof(IBasePage)));

                var db = database.Database;
                var scContext = new SitecoreContext(db);

                var glassHtml = GetGlassHtml(scContext);
                var instance = scContext.GetItem<IBasePage>("/sitecore");

                //Act
                glassHtml.Editable(instance, x => x.Title);

                //This method should execute without error

            }
        }

        /// <summary>
        /// https://github.com/mikeedwards83/Glass.Mapper/issues/261
        /// </summary>
        [Test]
        public void Interface_LazyLoadDisabledWithIgnoreAttrubute_ReturnsModel()
        {
           
            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            var fieldName = "Field";

            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {fieldName, ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                var context = Context.Create(Utilities.CreateStandardResolver());
                var fluentLoader = new SitecoreFluentConfigurationLoader();
                var stub = fluentLoader.Add<IBase>();
                stub.Ignore(x => x.Id);
                context.Load(fluentLoader);
                
                var db = database.Database;
                var scContext = new SitecoreService(db, context);

                //Act
                var instance = scContext.GetItem<IBase>("/sitecore", isLazy:false);

                //Assert
                Assert.IsNotNull(stub);
            }
        }

        [Test]
        public void GenericModelTest_CanRetrieveAGenericModel_WithGenericChildren()
        {
            /*
             * Tests that we can save to an item property.
             */

            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            var fieldName = "Field";

            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {fieldName, ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                {
                    new Sitecore.FakeDb.DbItem("Tests")
                },

            })
            {
                var context = Context.Create(Utilities.CreateStandardResolver());

                var db = database.Database;
                var scContext = new SitecoreService(db);
                string path = "/sitecore/content/Target";



                //Act
                var instance1 = scContext.GetItem<Parent<Child1>>(path);
                var instance2 = scContext.GetItem<Parent<Child2>>(path);


                //Assert
                Assert.IsNotNull(instance1);
                Assert.Greater(instance1.Children.Count(), 0);
                Assert.IsNotNull(instance1.Children.FirstOrDefault(x => x.Name == "Tests"));

                Assert.IsNotNull(instance2);
                Assert.Greater(instance2.Children.Count(), 0);
                Assert.IsNotNull(instance2.Children.FirstOrDefault(x => x.Name == "Tests"));
            }
        }

        [Test]
        public void CustomProfiler_OutputsTimingsInTasks()
        {
            /*
             * Tests that we can save to an item property.
             */

            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            var fieldName = "Field";

            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {fieldName, ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                {
                    new Sitecore.FakeDb.DbItem("Tests")
                },

            })
            {
                var context = Context.Create(Utilities.CreateStandardResolver());

                var db = database.Database;
                var scContext = new SitecoreService(db);
                var sb = new StringBuilder();

                scContext.Profiler = new AccmulatorProfiler(new StringWriter(sb));
                string path = "/sitecore/content/Target";



                //Act
                var instance1 = scContext.GetItem<ParentNotLazy<Child1>>(path);
                var instance2 = scContext.GetItem<ParentNotLazy<Child1>>(path);


                //Assert
                Assert.Greater(sb.Length, 0);
               Console.Write(sb.ToString());
            }
        }

        public class Parent<T>
        {
            public virtual IEnumerable<T> Children { get; set; }
        }

        public class ParentNotLazy<T>
        {
            [SitecoreChildren(IsLazy = false)]
            public virtual IEnumerable<T> Children { get; set; }
        }

        public class Child1
        {
            public virtual string Name { get; set; }
        }
        public class Child2
        {
            public virtual string Name { get; set; }

        }

        [Test]
        public void ItemPropertySave_SavesItemOnProperty_SetsField()
        {
            /*
             * Tests that we can save to an item property.
             */

            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            var fieldName = "Field1";

            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {fieldName, ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                var context = Context.Create(Utilities.CreateStandardResolver());

                var db = database.Database;
                var scContext = new SitecoreContext(db);
                string path = "/sitecore/content/Target";

                var expected = "some expected value";
                var item = db.GetItem(path);

                using (new ItemEditing(item, true))
                {
                    item["Field1"] = string.Empty;
                }

                var instance = scContext.GetItem<ItemPropertySaveStub>(path);

                //Act
                instance.Field1 = expected;

                using (new SecurityDisabler())
                {
                    scContext.Save(instance);
                }

                //Assert
                Assert.AreEqual(expected, instance.Item["Field1"]);

            }
        }

        [Test]
        public void FieldWithSpacesReturningNullIssue()
        {
            /*
             * This test is in response to issue 53 raised on the Glass.Sitecore.Mapper
             * project. When two interfaces have similar names are created as proxies
             * the method GetTypeConfiguration returns the wrong config.
             */


            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            var fieldName1 = "Field With Space";
            var fieldName2 = "Image Space";

            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {fieldName2, ""},
                    {fieldName1, ""},
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {

                string path = "/sitecore/content/Target";
                string expected = "Hello space";
                string imageValue =
                    "<image mediaid=\"{C2CE5623-1E36-4535-9A01-669E1541DDAF}\" mediapath=\"/Tests/Dayonta\" src=\"~/media/C2CE56231E3645359A01669E1541DDAF.ashx\" />";

                var context = Context.Create(Utilities.CreateStandardResolver());
               
                context.Load(new OnDemandLoader<SitecoreTypeConfiguration>(typeof(FieldWithSpaceIssue)));

                var db = database.Database;

                var item = db.GetItem(path);

                using (new ItemEditing(item, true))
                {
                    item[fieldName2] = imageValue;
                    item[fieldName1] = expected;
                }

                var scContext = new SitecoreContext(db);

                var glassHtml = GetGlassHtml(scContext);

                //Act
                var instance = scContext.GetItem<FieldWithSpaceIssue>(path);


                //Assert
                Assert.AreEqual(expected, instance.FieldWithSpace);
                Assert.IsNotNull(instance.ImageSpace);
            }
        }

        [Test]
        public void FieldWithSpacesAutoMap()
        {


            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            var fieldName1 = "Field With Space";
            var fieldName2 = "Field With Space 1";
            var fieldName3 = "Image Space";

            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {fieldName1, ""},
                    {fieldName2, ""},
                    {fieldName3, ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId),

            })
            {
                string path = "/sitecore/content/Target";
                string expected = "Hello space";
                string expectedSpace1 = "Hello space";
                string imageValue =
                    "<image mediaid=\"{C2CE5623-1E36-4535-9A01-669E1541DDAF}\" mediapath=\"/Tests/Dayonta\" src=\"~/media/C2CE56231E3645359A01669E1541DDAF.ashx\" />";

                var resolver = Utilities.CreateStandardResolver();
                resolver.DataMapperResolverFactory.Add(() => new AbstractDataMapperFieldsWithSpace());
                var context = Context.Create(resolver);
                context.Load(new OnDemandLoader<SitecoreTypeConfiguration>(typeof(FieldWithSpaceAutoMap)));
                ;  

                var db = Factory.GetDatabase("master");

                var item = db.GetItem(path);

                using (new ItemEditing(item, true))
                {
                    item["Field With Space"] = expected;
                    item["Field With Space 1"] = expectedSpace1;
                    item["Image Space"] = imageValue;
                }

                var scContext = new SitecoreContext(db);

                var glassHtml = new GlassHtml(scContext);

                //Act
                var instance = scContext.GetItem<FieldWithSpaceAutoMap>(path);


                //Assert
                Assert.AreEqual(expected, instance.FieldWithSpace);
                Assert.IsNotNull(instance.ImageSpace);
                SitecoreFieldConfiguration imageFieldTypeConfig =
                    scContext.GlassContext.TypeConfigurations[typeof(FieldWithSpaceAutoMap)].Properties.First(
                        x => x.PropertyInfo.Name == "ImageSpace") as SitecoreFieldConfiguration;
                Assert.AreEqual("Image Space", imageFieldTypeConfig.FieldName);

                SitecoreFieldConfiguration stringFieldTypeConfig =
                    scContext.GlassContext.TypeConfigurations[typeof(FieldWithSpaceAutoMap)].Properties.First(
                        x => x.PropertyInfo.Name == "FieldWithSpace") as SitecoreFieldConfiguration;
                Assert.AreEqual("Field With Space", stringFieldTypeConfig.FieldName);

                SitecoreFieldConfiguration stringNumberFieldTypeConfig =
                    scContext.GlassContext.TypeConfigurations[typeof(FieldWithSpaceAutoMap)].Properties.First(
                        x => x.PropertyInfo.Name == "FieldWithSpace1") as SitecoreFieldConfiguration;
                Assert.AreEqual("Field With Space 1", stringNumberFieldTypeConfig.FieldName);
            }
        }

        [Test]
        public void LazyLoadTest_LazyIsFalseAndServiceIsDisposed_NoException()
        {
            //Assign

            var templateId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                {
                    new Sitecore.FakeDb.DbItem("Child1"),
                    new Sitecore.FakeDb.DbItem("Child2"),
                    new Sitecore.FakeDb.DbItem("Child3")
                }


            })
            {

                var fluentConfig = new SitecoreFluentConfigurationLoader();

                var typeConfig = fluentConfig.Add<LazyLoadParent>();
                typeConfig.AutoMap();
                typeConfig.Children(x => x.Children).IsNotLazy();

                var context = Context.Create(Utilities.CreateStandardResolver());
                context.Load(fluentConfig);

                var service = new SitecoreService(database.Database, context);

                //Act
                var result =
                    service.GetItem<LazyLoadParent>("/sitecore/content/Target");
                service.Dispose();

                //Assert

                Assert.AreEqual(3, result.Children.Count());

                foreach (var child in result.Children)
                {
                    Assert.AreNotEqual(Guid.Empty, child.Id);
                }

            }
        }

        [Test]
        public void LazyLoadTest_LazyIsTrueAndServiceIsDisposed_ThrowsException()
        {
            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                {
                    new Sitecore.FakeDb.DbItem("Child1"),
                    new Sitecore.FakeDb.DbItem("Child2"),
                    new Sitecore.FakeDb.DbItem("Child3")
                }


            })
            {

                var fluentConfig = new SitecoreFluentConfigurationLoader();

                var typeConfig = fluentConfig.Add<LazyLoadParent>();
                typeConfig.AutoMap();
                typeConfig.Children(x => x.Children);

                var context = Context.Create(Utilities.CreateStandardResolver());
                context.Load(fluentConfig);

                var service = new SitecoreService(database.Database, context);

                //Act
                var result =
                    service.GetItem<LazyLoadParent>("/sitecore/content/Tests/DataMappers/SitecoreChildrenMapper/Parent");
                service.Dispose();

                //Assert

                Assert.Throws<NullReferenceException>(() =>
                {
                    Assert.AreEqual(3, result.Children.Count());

                    foreach (var child in result.Children)
                    {
                        Assert.AreNotEqual(Guid.Empty, child.Id);
                    }
                });


            }
        }


        [Test]
        public void OrderOfIgnoreIssue1_ConfiguredShouldBeSet_TitleShouldBeIgnored()
        {
            //Assign

            var templateId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"Title","" }
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                {
                }


            })
            {
                string path = "/sitecore/content/Target";
                string expected = "Hello space";


                var fluentConfig = new SitecoreFluentConfigurationLoader();

                var typeConfig = fluentConfig.Add<FieldOrderOnIgnore>();
                typeConfig.AutoMap();
                typeConfig.Ignore(x => x.Title);
                typeConfig.Field(x => x.ConfiguredTitle).FieldName("Title");

                var context = Context.Create(Utilities.CreateStandardResolver());
                context.Load(fluentConfig);

                var db = Factory.GetDatabase("master");

                var item = db.GetItem(path);

                using (new ItemEditing(item, true))
                {
                    item["Title"] = expected;
                }

                var scContext = new SitecoreContext(db);


                //Act
                var instance = scContext.GetItem<FieldOrderOnIgnore>(path);


                //Assert
                Assert.AreEqual(expected, instance.ConfiguredTitle);
                Assert.IsNull(instance.Title);
            }
        }

        [Test]
        public void OrderOfIgnoreIssue2_ConfiguredShouldBeSet_TitleShouldBeIgnored()
        {
            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"Title", ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                {
                }


            })
            {
                string path = "/sitecore/content/Target";
                string expected = "Hello space";


                var fluentConfig = new SitecoreFluentConfigurationLoader();

                var typeConfig = fluentConfig.Add<FieldOrderOnIgnore>();
                typeConfig.AutoMap();
                typeConfig.Field(x => x.ConfiguredTitle).FieldName("Title");
                typeConfig.Ignore(x => x.Title);

                var context = Context.Create(Utilities.CreateStandardResolver());
                context.Load(fluentConfig);

                var db = database.Database;

                var item = db.GetItem(path);

                using (new ItemEditing(item, true))
                {
                    item["Title"] = expected;
                }

                var scContext = new SitecoreContext(db);


                //Act
                var instance = scContext.GetItem<FieldOrderOnIgnore>(path);


                //Assert
                Assert.AreEqual(expected, instance.ConfiguredTitle);
                Assert.IsNull(instance.Title);
            }
        }

        [Test]
        public void FieldLoopIssue()
        {
            //Arrange
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"RelatedItems", ""}
                },
                new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                {
                }


            })
            {
                Guid itemId = targetId.Guid;

                var db = database.Database;
                var item = db.GetItem(new ID(itemId));

                using (new ItemEditing(item, true))
                {
                    item["RelatedItems"] = itemId.ToString();
                }

                var context = Context.Create(Utilities.CreateStandardResolver());
                var scContext = new SitecoreService(db, context);

                //Act

                var fieldLoop = scContext.GetItem<FieldLoop>(itemId);

                //Assert
                Assert.AreEqual(itemId, fieldLoop.Id);
                Assert.AreEqual(itemId, fieldLoop.RelatedItems.First().Id);
                Assert.AreEqual(itemId, fieldLoop.RelatedItems.First().RelatedItems.First().Id);

            }
        }

        [Test]
        public void GetItem_ClassHasItemChildrenCollectionAndParent_ReturnsItem()
        {
            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
                new DbTemplate(templateId)
                {
                    {"RelatedItems", ""}
                },
                new Sitecore.FakeDb.DbItem("Parent") {
                    new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                    {
                        new Sitecore.FakeDb.DbItem("Child1")
                    }
                }

            })
            {
                var context = Context.Create(Utilities.CreateStandardResolver());


                var path = "/sitecore/content/Parent/Target";

                var db = database.Database;
                var service = new SitecoreService(db);

                var item = db.GetItem(path);

                //Act
                var result = service.GetItem<ItemWithItemProperties>(path);

                //Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(item.ParentID, result.Parent.ID);
                Assert.AreEqual(item.Children.Count, result.Children.Count());
            }
        }

        [Test]
        public void GetItem_ClassHasItemField_ReturnsItem()
        {
            //Assign
            var templateId = ID.NewID;
            var targetId = ID.NewID;
            using (Db database = new Db
            {
               
                new Sitecore.FakeDb.DbItem("Parent") {
                    new Sitecore.FakeDb.DbItem("Target", targetId, templateId)
                    {
                        
                    },
                    new DbField("Field") {Value = targetId.ToString()}
                }

            })
            {
                var context = Context.Create(Utilities.CreateStandardResolver());


                var path = "/sitecore/content/Parent";

                var db = database.Database;
                var service = new SitecoreService(db);

                var item = db.GetItem(path);

                //Act
                var result = service.GetItem<ItemWithItemField>(path);

                //Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(targetId, result.Field.ID);
            }
        }

        private IGlassHtml GetGlassHtml(ISitecoreContext sitecoreContext)
        {
            return sitecoreContext.GlassHtml;
        }

        #region Stubs


        public class LazyLoadParent
        {
            public virtual IEnumerable<LazyLoadChild> Children { get; set; }
        }

        public class LazyLoadChild
        {
            public virtual Guid Id { get; set; }
        }



        public class ItemWithItemProperties
        {
            public Item Parent { get; set; }
            public IEnumerable<Item> Children { get; set; }
        }

        public class ItemWithItemField
        {
            public Item Field { get; set; }
        }


        [SitecoreType]
        public interface IBase
        {
            [SitecoreId]
            Guid Id { get; set; }
        }

        [SitecoreType]
        public interface IBasePage : IBase
        {
            [SitecoreField]
            string Title { get; set; }
        }

        [SitecoreType]
        public class FieldWithSpaceIssue
        {
            [SitecoreField("Field With Space")]
            public virtual string FieldWithSpace { get; set; }

            [SitecoreField("Image Space")]
            public virtual Image ImageSpace { get; set; }
        }

        [SitecoreType(AutoMap = true)]
        public class FieldWithSpaceAutoMap
        {
            public virtual string FieldWithSpace { get; set; }

            public virtual string FieldWithSpace1 { get; set; }

            public virtual Image ImageSpace { get; set; }
        }

        public class FieldOrderOnIgnore
        {
            public virtual string Title { get; set; }
            public virtual string ConfiguredTitle { get; set; }
        }

        [SitecoreType(AutoMap = true)]
        public class FieldLoop
        {
            public virtual Guid Id { get; set; }

            public virtual IEnumerable<FieldLoop> RelatedItems { get; set; }
        }

        public class ItemPropertySaveStub
        {
            public virtual Item Item { get; set; }
            public virtual string Field1 { get; set; }
        }

        #endregion
    }
}
