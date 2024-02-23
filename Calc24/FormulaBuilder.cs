using System;
using System.Collections.Generic;
using System.Linq;

namespace Calc24
{
    public class FormulaBuilder
    {
        /// <summary>
        /// 数字对象序列号，由于区分相同数值的数字
        /// </summary>
        private static int seq = 0;

        public delegate bool OnCalcResult(CalcItem calcItem);

        public static bool NoOptimization = false;

        public abstract class CalcItem
        {
            public readonly int seq = FormulaBuilder.seq++;
            abstract protected double GetValue();

            abstract protected string GetText();

            abstract protected int GetOPLevel();

            public int Level { get { return GetOPLevel(); } }

            public double Value { get { return GetValue(); } }

            public override string ToString()
            {
                return GetText();
            }

            public static bool operator >(CalcItem l, CalcItem r)
            {
                if (l.Value == r.Value)
                    return l.seq > r.seq;
                else
                    return l.Value > r.Value;
            }

            public static bool operator <(CalcItem l, CalcItem r)
            {
                if (l.Value == r.Value)
                    return l.seq < r.seq;
                else
                    return l.Value < r.Value;
            }
        }

        protected class CalcNum : CalcItem
        {
            protected double value;

            public CalcNum(double value)
            {
                this.value = value;
            }

            protected override int GetOPLevel()
            {
                return 0;
            }

            protected override string GetText()
            {
                //return string.Format("{0}[{1}]",value, seq);
                return string.Format("{0}", value);
            }


            protected override double GetValue()
            {
                return value;
            }


        }

        protected abstract class CalcOPItem : CalcItem
        {
            public readonly CalcItem first;
            public readonly CalcItem second;
            public CalcOPItem(CalcItem first, CalcItem second)
            {
                this.first = first;
                this.second = second;
            }

            protected abstract string GetOPText();

            protected override string GetText()
            {
                var sf = first.ToString();
                if (first is CalcOPItem)
                {
                    if (((CalcOPItem)first).Level < Level)
                        sf = "(" + sf + ")";
                }

                var ss = second.ToString();
                if (second is CalcOPItem)
                {
                    if (((CalcOPItem)second).Level <= Level)
                        ss = "(" + ss + ")";
                }
                return sf + " " + GetOPText() + " " + ss;
            }

        }

        protected class CalcPlus : CalcOPItem
        {
            public CalcPlus(CalcItem first, CalcItem second) : base(first, second)
            {
            }


            protected override double GetValue()
            {
                return this.first.Value + this.second.Value;
            }

            protected override int GetOPLevel()
            {
                return 1;
            }

            protected override string GetOPText()
            {
                return "+";
            }
        }

        protected class CalcMinus : CalcOPItem
        {

            public CalcMinus(CalcItem first, CalcItem second) : base(first, second)
            {

            }

            protected override double GetValue()
            {
                return this.first.Value - this.second.Value;
            }

            protected override int GetOPLevel()
            {
                return 1;
            }

            protected override string GetOPText()
            {
                return "-";
            }

        }

        protected class CalcMultiply : CalcOPItem
        {
            public CalcMultiply(CalcItem first, CalcItem second) : base(first, second)
            {

            }

            protected override double GetValue()
            {
                return this.first.Value * this.second.Value;
            }
            protected override int GetOPLevel()
            {
                return 2;
            }

            protected override string GetOPText()
            {
                return "×";
            }
        }

        protected class CalcDivide : CalcOPItem
        {
            public CalcDivide(CalcItem first, CalcItem second) : base(first, second)
            {
            }

            protected override double GetValue()
            {
                return this.first.Value / this.second.Value;
            }
            protected override int GetOPLevel()
            {
                return 2;
            }

            protected override string GetOPText()
            {
                return "÷";
            }
        }

        protected class CalcOPFactory
        {
            /// <summary>
            /// 是否使用加法
            /// </summary>
            /// <param name="first"></param>
            /// <param name="second"></param>
            /// <returns></returns>
            protected static bool UseAddition(CalcItem first, CalcItem second)
            {
                // 同级不需要
                if (second.Level == 1) return false;
                // 先加后减
                if (first is CalcMinus) return false;
                // 从小加大
                if (first > second)
                {
                    if (first is not CalcPlus item)
                        return false;
                    else if (item.second > second)
                        return false;
                }

                return true;
            }

            /// <summary>
            /// 是否使用减法
            /// </summary>
            /// <param name="first"></param>
            /// <param name="second"></param>
            /// <returns></returns>
            protected static bool UseSubstraction(CalcItem first, CalcItem second)
            {
                // 同级不需要
                if (second.Level == 1) return false;
                // 负数不需要
                if (first < second) return false;
                // 连减时先减大数
                if (first is CalcMinus item && item.second < second) return false;

                return true;
            }

            protected static bool UseMultiplication(CalcItem first, CalcItem second)
            {
                // 同级不需要
                if (second.Level == 2) return false;
                // 先乘后除
                if (first is CalcDivide) return false;
                // 从小到大
                if (first > second)
                {
                    if (first is not CalcMultiply item)
                        return false;
                    else if (item.second > second)
                        return false;
                }

                return true;
            }


            protected static bool UseDivision(CalcItem first, CalcItem second)
            {
                // 同级不需要
                if (second.Level == 2) return false;
                // 连除时先除大数
                if (first is CalcDivide item && item.second < second) return false;

                return true;
            }

            public static IEnumerable<CalcItem> All(CalcItem first, CalcItem second)
            {
                List<CalcItem> list = [];

                if (FormulaBuilder.NoOptimization == false && first is CalcOPItem && second is CalcOPItem && first.seq > second.seq)
                {
                    return list;
                }


                if (FormulaBuilder.NoOptimization || UseAddition(first, second)) 
                {
                    list.Add(new CalcPlus(first, second));
                }

                if (FormulaBuilder.NoOptimization || UseSubstraction(first, second)) 
                {
                    list.Add(new CalcMinus(first, second));
                }

                if (FormulaBuilder.NoOptimization || UseMultiplication(first, second)) 
                {
                    list.Add(new CalcMultiply(first, second));
                }

                if (FormulaBuilder.NoOptimization || UseDivision(first, second)) 
                {
                    list.Add(new CalcDivide(first, second));
                }

                return list;
            }

        }

        protected static bool CalcRecusive(IList<CalcItem> items, OnCalcResult onCalcResult)
        {
            if (items.Count == 1)
            {
                return onCalcResult(items[0]);
            }

            int l = items.Count;
            HashSet<double> iSet = [], jSet = [];
            for (int i = 0; i < l; i++)
            {
                if (!FormulaBuilder.NoOptimization)
                {
                    if (iSet.Contains(items[i].Value)) continue;
                    iSet.Add(items[i].Value);
                }

                var first = items[i];
                items.RemoveAt(i);

                jSet.Clear();
                for (int j = 0; j < l - 1; j++)
                {
                    if (!FormulaBuilder.NoOptimization)
                    {
                        if (jSet.Contains(items[j].Value)) continue;
                        jSet.Add(items[j].Value);
                    }

                    var second = items[j];
                    items.RemoveAt(j);

                    foreach (var item in CalcOPFactory.All(first, second))
                    {
                        items.Insert(0, item);
                        if (!CalcRecusive(items, onCalcResult)) return false;
                        items.RemoveAt(0);
                    }

                    items.Insert(j, second);
                }

                items.Insert(i, first);
            }
            return true;
        }


        private static readonly Random random = new(System.DateTime.Now.Microsecond);
        public static IList<int> GetFourNumbers(IEnumerable<int> nums)
        {
            List<int> temp = new(4);
            foreach (var num in nums)
            {
                temp.Add(num);
                if (temp.Count == 4) break;
            }
            while (temp.Count < 4) temp.Add(random.Next(1, 10));

            return temp;
        }

        public static string Execute(IEnumerable<int> nums, int target)
        {
            string result = string.Empty;
            Execute(nums, item =>
            {
                if ((int)((item.Value + 0.0005) * 1000) == target * 1000)
                {
                    result = item.ToString();
                }
                return false;
            });
            return result;
        }

        public static void Execute(IEnumerable<int> nums, OnCalcResult onCalcResult)
        {
            List<CalcItem> list = new();
            FormulaBuilder.seq = 0;
            foreach (var num in nums) list.Add(new CalcNum(num));
            CalcRecusive(list, onCalcResult);
        }

    }
}