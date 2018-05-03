using Microsoft.VisualStudio.TestTools.UnitTesting;
using CtINI.PropertiesSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using INITesting;
using System.Collections;
using CtParamEditor.Comm;

namespace CtINI.PropertiesSetting.Tests {
    [TestClass()]
    public class PropertiesSettingTests {
        [TestMethod()]
        public void ReadTest() {
            string filePath = @"D:\AGV_Param.ini";
            /*-- 以IniMth產生AGV參數檔 --*/
            IniMth.Write(filePath, "Test", "One", "0");
            IniMth.Write(filePath, "Test", "Two", "1");
            IniMth.Write(filePath, "Test", "Three", "2");

            IniMth.Write(filePath, "AutoCharge", "Type", typeof(bool).Name);
            IniMth.Write(filePath, "AutoCharge", "Value", "True");
            IniMth.Write(filePath, "AutoCharge", "Description", "Auto charge");
            IniMth.Write(filePath, "AutoCharge", "Default", "True");

            IniMth.Write(filePath, "CarType", "Type", typeof(string).Name);
            IniMth.Write(filePath, "CarType", "Value", "P2");
            IniMth.Write(filePath, "CarType", "Description", "The type of AGV");
            IniMth.Write(filePath, "CarType", "Default", "P2");

            IniMth.Write(filePath, "Acce", "Type", typeof(float).Name);
            IniMth.Write(filePath, "Acce", "Value", "5");
            IniMth.Write(filePath, "Acce", "Description", "The acce of AGV");
            IniMth.Write(filePath, "Acce", "Max", "20");
            IniMth.Write(filePath, "Acce", "Min", "5");
            IniMth.Write(filePath, "Acce", "Default", "5");

            IniMth.Write(filePath, "Velocity", "Type", typeof(int).Name);
            IniMth.Write(filePath, "Velocity", "Value", "45");
            IniMth.Write(filePath, "Velocity", "Description", "The velocity of AGV");
            IniMth.Write(filePath, "Velocity", "Max", "500");
            IniMth.Write(filePath, "Velocity", "Min", "0");
            IniMth.Write(filePath, "Velocity", "Default", "50");

            IniMth.Write(filePath, "TestParam", "Type", "Test");
            IniMth.Write(filePath, "TestParam", "Value", "Two");
            IniMth.Write(filePath, "TestParam", "Description", "Test Param");
            IniMth.Write(filePath, "TestParam", "Default", "One");

            ///*-- 讀取參數檔 --*/
            //var data = IniMth.ReadValues(filePath);
            ///*-- 讀取Enum定義 --*/
            //CtEnumData.ReadINI(data);
            ///*-- 讀取參數設定 --*/
            //List<IAgvToDgvCol> dgvCol = data.ToAGVofProperty();
            
            ///*-- 將參數定義寫入內存 --*/
            //StringBuilder sb =  CtEnumData.GetStringBuilder();
            ///*-- 將參數設定寫入內存 --*/
            //foreach (var col in dgvCol) {
            //    sb.WriteProperty(col);
            //}
            ///*-- 將資料寫入ini檔 --*/
            //File.WriteAllText(@"D:\TestFile.ini", sb.ToString(), Encoding.UTF8);
            //sb.Clear();
            //sb = null;

            //List<IAgvToDgvCol> chkData = IniMth.ReadValues(@"D:\TestFile.ini").ToAGVofProperty();
            //int srcCounst = dgvCol.Count();
            //int destCount = chkData.Count();
            //Assert.IsTrue(chkData.Count() == dgvCol.Count(),"參數筆數不符");
            //for(int i = 0;i < chkData.Count(); i++) {
            //    ChkParam(dgvCol, chkData, i);
            //}
        }

        [TestMethod]
        public void WriteTest() {

        }

        private void ChkParam(List<IParamColumn> src,List<IParamColumn> dest,int i) {
            Assert.IsTrue(src[i].Value.ToString() == dest[i].Value.ToString(),"Value不符");
            Assert.IsTrue(src[i].Description == dest[i].Description,"Description不符");
            Assert.IsTrue(src[i].Type == dest[i].Type,"Type不符");
            Assert.IsTrue(src[i].Max?.ToString() == dest[i].Max?.ToString(),"Max不符");
            Assert.IsTrue(src[i].Min?.ToString() == dest[i].Min?.ToString(),"Min不符");
            Assert.IsTrue(src[i].Default?.ToString() == dest[i].Default?.ToString(),"Def不符");
        }

    }
}