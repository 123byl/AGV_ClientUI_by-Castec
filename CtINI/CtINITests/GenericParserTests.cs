using Microsoft.VisualStudio.TestTools.UnitTesting;
using CtTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using CtGenericParser;

namespace CtTesting.Tests {

    [TestClass()]
    public class CtGenericTests {

        [TestMethod()]
        public void TryParsePrimitive() {
            TryParseTest((int)10);
        }

        [TestMethod()]
        public void TryParseEnum() {
            TryParseTest(StringSplitOptions.None);
        }

        [TestMethod()]
        public void TryParseClass() {
            TryParseTest(new CtPoint(10, 10));
        }

        [TestMethod()]
        public void TryParseInterface() {
            GenericParser.Assignment<ICtPoint>(CtPoint.TryParse);
            TryParseTest((ICtPoint)new CtPoint(10, 10));
        }

        public void TryParseTest<T>(T val) {
            T result = default(T);
            if (!GenericParser.TryParse(val.ToString(), out result)) {
                Assert.Fail("TryParse失敗");
            } else {
                if (val is IComparable) {
                    bool isEquals = 0 == (val as IComparable).CompareTo(result);
                    Assert.IsTrue(isEquals, "TryParse結果不符");
                } else {
                    Assert.Fail("無法比對結果");
                }
            }
        }

    }

    public class CtPoint : ICtPoint {
        public int X { get; set; }
        public int Y { get; set; }

        public CtPoint() {

        }

        public CtPoint(int x, int y) {
            X = x;
            Y = y;
        }
        public override string ToString() {
            return $"{X},{Y}";
        }
        public static bool TryParse(string content, out CtPoint instance) {
            instance = null;
            string[] split = content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Count() != 2) {
                return false;
            }
            int x, y;
            if (int.TryParse(split[0], out x) && int.TryParse(split[1], out y)) {
                instance = new CtPoint(x, y);
                return true;
            } else {
                return false;
            }
        }
        public static bool TryParse(string content, out ICtPoint instance) {
            instance = null;
            string[] split = content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Count() != 2) {
                return false;
            }
            int x, y;
            if (int.TryParse(split[0], out x) && int.TryParse(split[1], out y)) {
                instance = new CtPoint(x, y);
                return true;
            } else {
                return false;
            }
        }

        public static bool TryObject(string content, out object instance) {
            CtPoint point = null;
            bool ret = TryParse(content, out point);
            instance = point;
            return ret;
        }

        public static bool operator ==(CtPoint p1, CtPoint p2) {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(CtPoint p1, CtPoint p2) {
            return !(p1 == p2);
        }

        public override bool Equals(object obj) {
            if (obj is ICtPoint) {
                ICtPoint p = obj as ICtPoint;
                return p.X == this.X && p.Y == this.Y;
            } else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public int CompareTo(object obj) {
            if (obj is ICtPoint) {
                ICtPoint p = obj as ICtPoint;
                int thisTotal = this.X + this.Y;
                int objTotal = p.X + p.Y;
                if (thisTotal < objTotal) return -1;
                else if (thisTotal == objTotal) return 0;
                else return 1;
            } else {
                return -1;
            }
        }
    }

    public interface ICtPoint : IComparable {
        int X { get; set; }
        int Y { get; set; }

        string ToString();
    }

}