// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.Write("准备……");
Console.WriteLine("…………开始啦");
Console.InputEncoding = System.Text.Encoding.UTF8;
//Console.OutputEncoding = System.Text.Encoding.Unicode;

Stopwatch sw = new();
int target = 24;
System.Text.RegularExpressions.Regex regex = new(@"[:=?,]");

while(true)
{
    Console.WriteLine("输入数字，用空格分隔: ");
    var s = Console.ReadLine() ?? string.Empty;
    if (s == "quit" || s == "exit" || s == "bye")
    {
        break;
    }

    if (regex.IsMatch(s))
    {
        var items = regex.Split(s);
        if (items.Length!=2)
        {
            Console.WriteLine("输入格式：[24=]1 2 3 4");
            continue;
        }
        if (!int.TryParse(items[0], out target) || target <= 0)
        {
            target = 24;
        }
        s = items[1];
    }

    var numItems = s.Trim().Split()
    .Where(s=>{
        return int.TryParse(s, out int i);
    })
    .Select((x,i)=>
    {
        return int.Parse(x);
    });
    if (numItems.Count() == 1 && numItems.First() > 100)
    {
        numItems = numItems.First().ToString().ToCharArray().Select(c=>c-'0');
    }

    if (numItems.Count() < 3)
    {
        numItems = Calc24.FormulaBuilder.GetFourNumbers(numItems);
        Console.WriteLine("{0} = {1}", target, string.Join(' ', numItems));
        Console.WriteLine();
    }

    sw.Start();

    int c = 0, t = 0;
    //Calc24.FormulaBuilder.NoOptimization = true;
    Calc24.FormulaBuilder.Execute(numItems, item => {
        t++;
        if ((int)((item.Value + 0.0005) * 1000) == target*1000)
        {
            Console.WriteLine("[{0,4}] {2} = {1}", ++c, item, target);
        }
        return true;
    });

    if (c == 0)
        Console.WriteLine(":( 算不出 " + target);
    
    sw.Stop();
    Console.WriteLine("共计算 {0} 次，成功 {1} 次，耗时 {2}", t, c, sw.Elapsed);
}

Console.WriteLine("Finished");