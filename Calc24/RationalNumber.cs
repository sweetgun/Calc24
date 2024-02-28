namespace Calc24;
/// <summary>
/// 有理数
/// </summary>
public struct RationalNumber
{
    /// <summary>
    /// 分子
    /// </summary>
    public int Numerator;
    /// <summary>
    /// 分母
    /// </summary>
    public int Denominator;

    public RationalNumber(int value)
    {
        Numerator = value;
        Denominator = 1;
    }

    public RationalNumber(RationalNumber number)
    {
        Numerator = number.Numerator;
        Denominator = number.Denominator;
    }
}
