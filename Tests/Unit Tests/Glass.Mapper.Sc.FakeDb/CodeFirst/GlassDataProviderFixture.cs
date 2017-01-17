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
using System.Reflection;
using Glass.Mapper.Sc.CodeFirst;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.FakeDb.CodeFirst.Templates.Level1;
using Glass.Mapper.Sc.FakeDb.CodeFirst.Templates.Level1.Level2;
using NUnit.Framework;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Data.DataProviders;
using Sitecore.SecurityModel;

namespace Glass.Mapper.Sc.FakeDb.CodeFirst
{
    [TestFixture]
    public class GlassDataProviderFixture
    {
        private SecurityDisabler _disabler;
        private Database _datebase;
        private GlassDataProvider _dataProvider;
        private Context _context;
        private Db _db;


        [SetUp]
        public void Setup()
        {
            _db = new Db
            {
            };

            _disabler = new SecurityDisabler();
            _datebase = _db.Database;

            _dataProvider = new GlassDataProvider("master", Context.DefaultContextName);
            GlassDataProvider.GetSqlProviderFunc = providers => providers.FirstOrDefault(x => x is FakeDataProvider);

            _context = Context.Create(Utilities.CreateStandardResolver());
            //GlassDataProvider._setupComplete = false;
            InjectionDataProvider(_datebase, _dataProvider);

        }
        [TearDown]
        public void TearDown()
        {
            var providers = GetProviders(_datebase);
            var toRemove = providers.Where(x => x is GlassDataProvider).ToList();
            toRemove.ForEach(x => providers.Remove(x));

            var path = "/sitecore/templates/glasstemplates";
            var rootFolder = _datebase.GetItem(path);
            if (rootFolder != null)
                rootFolder.Delete();
            _disabler.Dispose();

            _datebase = null;
            _dataProvider = null;
            _disabler = null;

        }


        [Test]
        [Category("LocalOnly")]
        public void GlassDataProvider_ReturnsGlassTemplateFolder()
        {
            //Assign

            _dataProvider.Initialise(_datebase);

            var path = "/sitecore/templates/glasstemplates";

            _datebase.Caches.DataCache.Clear();
            _datebase.Caches.ItemCache.Clear();
            _datebase.Caches.ItemPathsCache.Clear();
            _datebase.Caches.StandardValuesCache.Clear();
            _datebase.Caches.PathCache.Clear();
            //Act
            var folder = _datebase.GetItem(path);

            //Assert
            Assert.AreEqual(folder.Name, "GlassTemplates");
        }

        [Test]
        [Category("LocalOnly")]
        public void GlassDataProvider_ReturnsTemplate()
        {
            //Assign
            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass1>()
                  .TemplateId("D3595652-AC29-4BD4-A8D7-DD2120EE6460")
                  .TemplateName("CodeFirstClass1")
                  .CodeFirst();

            _context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/CodeFirstClass1";


            _dataProvider.Initialise(_datebase);

            //Act
            var folder = _datebase.GetItem(path);

            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass1");

        }

        [Test]
        [Category("LocalOnly")]
        public void GlassDataProvider_TemplateInNamespace_ReturnsTemplate()
        {
            //Assign
            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass2>()
                  .TemplateId("E33F1C58-FAB2-475A-B2FE-C26F5D7565A2")
                  .TemplateName("CodeFirstClass2")
                  .CodeFirst();

            _context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/Level1/CodeFirstClass2";

            _dataProvider.Initialise(_datebase);

            //Act
            var folder = _datebase.GetItem(path);
            string xml = Factory.GetConfiguration().OuterXml;

            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass2");

        }

        [Test]
        [Category("LocalOnly")]
        public void GlassDataProvider_TemplateInNamespaceTwoDeep_ReturnsTemplate()
        {
            //Assign
            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass3>()
                  .TemplateId("E33F1C58-FAB2-475A-B2FE-C26F5D7565A2")
                  .TemplateName("CodeFirstClass2")
                  .CodeFirst();

            _context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/Level1/Level2/CodeFirstClass2";

            _dataProvider.Initialise(_datebase);

            //Act
            var folder = _datebase.GetItem(path);
            string xml = Factory.GetConfiguration().OuterXml;

            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass2");

        }

        [Test]
        [Category("LocalOnly")]
        public void GlassDataProvider_TemplateInNamespaceTwoDeep_ReturnsTemplateTwoTemplates()
        {
            //Assign
            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass3>()
                  .TemplateId("E33F1C58-FAB2-475A-B2FE-C26F5D7565A2")
                  .TemplateName("CodeFirstClass3")
                  .CodeFirst();

            loader.Add<CodeFirstClass4>()
                 .TemplateId("{42B45E08-20A4-434B-8AC7-ED8ABCE5B3BE}")
                 .TemplateName("CodeFirstClass4")
                 .CodeFirst();

            _context.Load(loader);

            var path1 = "/sitecore/templates/glasstemplates/Level1/Level2/CodeFirstClass3";
            var path2 = "/sitecore/templates/glasstemplates/Level1/Level2/CodeFirstClass4";

            _dataProvider.Initialise(_datebase);

            //Act
            var template1 = _datebase.GetItem(path1);
            var template2 = _datebase.GetItem(path2);

            //Assert
            Assert.AreEqual(template1.Name, "CodeFirstClass3");
            Assert.AreEqual(template2.Name, "CodeFirstClass4");

        }

        [Test]
        [Category("LocalOnly")]
        public void GlassDataProvider_ReturnsTemplateWithSectionAndField()
        {
            //Assign
            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass1>()
                  .TemplateId("D3595652-AC29-4BD4-A8D7-DD2120EE6460")
                  .TemplateName("CodeFirstClass1")
                  .CodeFirst()
                  .Fields(x =>
                          x.Field(y => y.Field1)
                           .IsCodeFirst()
                           .FieldId("32FE1520-EAD4-4CF8-A69F-A4717E2F07F6")
                           .SectionName("TestSection")
                );

            _context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/CodeFirstClass1";

            _dataProvider.Initialise(_datebase);

            //Act
            var folder = _datebase.GetItem(path);

            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass1");
            var section = folder.Children.FirstOrDefault(x => x.Name == "TestSection");
            Assert.IsNotNull(section);
            var field = section.Children.FirstOrDefault(x => x.Name == "Field1");
            Assert.IsNotNull(field);

        }

        [Test]
        [Category("LocalOnly")]
        public void GlassDataProvider_ReturnsTemplateWithSectionAndField_AllPropertiesSet()
        {
            //Assign

            var loader = new SitecoreFluentConfigurationLoader();

            var fieldId = new ID("32FE1520-EAD4-4CF8-A69F-A4717E2F07F6");
            var sectionName = "TestSection";
            var fieldSortOrder = 123;
            var fieldName = "FieldName";
            var fieldTitle = "TestTitle";
            var fieldSource = "/source";
            var fieldType = SitecoreFieldType.Date;
            var sectionSortOrder = 100;
            var validationErrorText = "TextValidation";
            var validationRegEx = "testregex";

            loader.Add<CodeFirstClass1>()
                  .TemplateId("D3595652-AC29-4BD4-A8D7-DD2120EE6460")
                  .TemplateName("CodeFirstClass1")
                  .CodeFirst()
                  .Fields(x =>
                          x.Field(y => y.Field1)
                           .IsCodeFirst()
                           .FieldId(fieldId.ToString())
                           .SectionName(sectionName)
                           .FieldSortOrder(fieldSortOrder)
                           .FieldName(fieldName)
                           .FieldSource(fieldSource)
                           .FieldTitle(fieldTitle)
                           .FieldType(fieldType)
                           .IsRequired()
                           .IsShared()
                           .IsUnversioned()
                           .SectionSortOrder(sectionSortOrder)
                           .ValidationErrorText(validationErrorText)
                           .ValidationRegularExpression(validationRegEx)
                );

            _context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/CodeFirstClass1";

            _dataProvider.Initialise(_datebase);

            //Act
            var folder = _datebase.GetItem(path);

            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass1");
            var section = folder.Children.FirstOrDefault(x => x.Name == sectionName);
            Assert.IsNotNull(section);
            Assert.AreEqual(sectionSortOrder.ToString(), section[FieldIDs.Sortorder]);

            var field = section.Children.FirstOrDefault(x => x.Name == fieldName);
            Assert.IsNotNull(field);
            Assert.AreEqual(fieldSortOrder.ToString(), field[FieldIDs.Sortorder]);
            Assert.AreEqual(fieldId, field.ID);
            Assert.AreEqual(fieldSource, field[TemplateFieldIDs.Source]);
            Assert.AreEqual(fieldTitle, field[TemplateFieldIDs.Title]);
            Assert.AreEqual(fieldType.ToString(), field[TemplateFieldIDs.Type]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.ValidateButtonFieldId]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.WorkflowFieldId]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.ValidatorBarFieldId]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.QuickActionBarFieldId]);
            Assert.AreEqual("1", field[TemplateFieldIDs.Shared]);
            Assert.AreEqual("1", field[TemplateFieldIDs.Unversioned]);
            Assert.AreEqual(validationErrorText, field[TemplateFieldIDs.ValidationText]);
            Assert.AreEqual(validationRegEx, field[TemplateFieldIDs.Validation]);
        }

        public class CodeFirstClass1
        {
            public virtual string Field1 { get; set; }
        }

        private void InjectionDataProvider(Database db, DataProvider provider)
        {
            var providers = GetProviders(db);

            providers.Insert(0, provider);
            //var addMethod = typeof(Database).GetMethod("AddDataProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            //addMethod.Invoke(db, new[] { provider });
        }

        public DataProviderCollection GetProviders(Database db)
        {
            var providersField = typeof(Database).GetField("_dataProviders", BindingFlags.NonPublic | BindingFlags.Instance);
            var providers = providersField.GetValue(db) as DataProviderCollection;
            return providers;
        }


    }

    namespace Templates.Level1
    {
        public class CodeFirstClass2 { }
    }

    namespace Templates.Level1.Level2
    {
        public class CodeFirstClass3 { }
        public class CodeFirstClass4 { }
    }
}



