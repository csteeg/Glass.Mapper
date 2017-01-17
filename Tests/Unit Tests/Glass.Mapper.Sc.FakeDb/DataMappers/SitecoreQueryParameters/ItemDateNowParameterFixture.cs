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
using Glass.Mapper.Sc.DataMappers.SitecoreQueryParameters;
using NUnit.Framework;
using Sitecore.FakeDb;

namespace Glass.Mapper.Sc.FakeDb.DataMappers.SitecoreQueryParameters
{
    [TestFixture]
    public class ItemDateNowParameterFixture 
    {
        #region Method - GetValue

        [Test]
        public void GetValue_ReturnsItemFullPath()
        {
            //Assign
            using (Db database = new Db
            {
                new Sitecore.FakeDb.DbItem("Target")
            })
            {
                var param = new ItemDateNowParameter();
                var item =
                    database.GetItem(
                        "/sitecore/content/Target");

                //Act
                var result = param.GetValue(item);

                //Assert
                Assert.AreEqual(Sitecore.DateUtil.ToIsoDate(DateTime.Now), result);
            }
        }

        #endregion
    }
}




