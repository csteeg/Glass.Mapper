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
using Glass.Mapper.Pipelines.DataMapperResolver;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.DataMappers;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Rules;
using Sitecore.Rules.InsertOptions;

namespace Glass.Mapper.Sc.FakeDb.DataMappers
{
    public class SitecoreFieldRulesMapperFixture 
    {
        #region Method - CanHandle

        [Test]
        public void CanHandle_TypeIsRulesList_ReturnsTrue()
        {
            //Assign
            var mapper = new SitecoreFieldRulesMapper();
            var config = new SitecoreFieldConfiguration();
            var context = (Context)null;

            config.PropertyInfo = typeof (StubClass).GetProperty("Property");

            //Act
            var result = mapper.CanHandle(config, context);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanHandle_TypeIsNotRulesList_ReturnsFalse()
        {
            //Assign
            var mapper = new SitecoreFieldRulesMapper();
            var config = new SitecoreFieldConfiguration();
            var context = (Context)null;

            config.PropertyInfo = typeof(StubClass).GetProperty("PropertyInt");

            //Act
            var result = mapper.CanHandle(config, context);

            //Assert
            Assert.IsFalse(result);
        }
        [Test]
        public void CanHandle_GenericTypeIsNotRulesList_ReturnsFalse()
        {
            //Assign
            var mapper = new SitecoreFieldRulesMapper();
            var config = new SitecoreFieldConfiguration();
            var context = (Context)null;

            config.PropertyInfo = typeof(StubClass).GetProperty("PropertyIntEnum");

            //Act
            var result = mapper.CanHandle(config, context);

            //Assert
            Assert.IsFalse(result);
        }


        #endregion

        #region Method - GetField

        [Test]
        public void GetField_FieldContains2Rules_RulesListContains2Rules()
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
                var fieldValue =
                    "<ruleset>  <rule uid=\"{A7FC7A4F-72C0-4FBE-B1C7-759E139848E0}\">    <conditions>      <condition id=\"{4F5389E9-79B7-4FE1-A43A-EEA4ECD19C94}\" uid=\"6B54B8B6B128471696870047DD93DDED\" operatorid=\"{10537C58-1684-4CAB-B4C0-40C10907CE31}\" value=\"10\" />    </conditions>  </rule>  <rule uid=\"{22BA1F43-9F29-46ED-83AF-151471297AF6}\">    <conditions>      <condition id=\"{DA0D1AEA-0144-4A40-9AF0-3123526C9163}\" uid=\"51CC5EC564494B68B9391C6B7A64A796\" fieldname=\"Test\" />    </conditions>    <actions>      <action id=\"{94C5C335-0902-4B45-B528-11B220005DD7}\" uid=\"7833EA2A3E75401AAE8B295067112DCC\" />    </actions>  </rule></ruleset>";

                var item = database.GetItem("/sitecore/content/Target");
                var field = item.Fields[fieldName];

                var mapper = new SitecoreFieldRulesMapper();
                var config = new SitecoreFieldConfiguration();
                var context = (Context) null;

                config.PropertyInfo = typeof(StubClass).GetProperty("Property");

                mapper.Setup(new DataMapperResolverArgs(context, config));

                using (new ItemEditing(item, true))
                {
                    field.Value = fieldValue;
                }

                //Act
                var result = mapper.GetField(field, null, null) as RuleList<StubRuleContext>;

                //Assert
                Assert.AreEqual(2, result.Count);

            }
        }

        [Test]
        public void GetField_FieldIsEmpty_RulesListContains0Rules()
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
                var fieldValue = "";

                var item = database.GetItem("/sitecore/content/Target");
                var field = item.Fields[fieldName];

                var mapper = new SitecoreFieldRulesMapper();
                var config = new SitecoreFieldConfiguration();
                var context = (Context) null;

                config.PropertyInfo = typeof(StubClass).GetProperty("Property");

                mapper.Setup(new DataMapperResolverArgs(context, config));

                using (new ItemEditing(item, true))
                {
                    field.Value = fieldValue;
                }

                //Act
                var result = mapper.GetField(field, null, null) as RuleList<StubRuleContext>;

                //Assert
                Assert.AreEqual(0, result.Count);

            }
        }

        #endregion

        #region Stubs

        public class StubClass
        {
            public RuleList<StubRuleContext> Property { get; set; }
            public int PropertyInt { get; set; }
            public IEnumerable<int> PropertyIntEnum { get; set; }
        }

        public class StubRuleContext : InsertOptionsRuleContext
        {

        }
        
        #endregion
    }
}




