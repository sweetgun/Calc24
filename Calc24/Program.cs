// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.Write("准备……");
Console.WriteLine("…………开始啦");
Console.InputEncoding = System.Text.Encoding.UTF8;
//Console.OutputEncoding = System.Text.Encoding.Unicode;

Stopwatch sw = new();

while(true)
{
    Console.WriteLine("输入数字，用空格分隔: ");
    var s = Console.ReadLine() ?? string.Empty;
    if (s == "quit" || s == "exit" || s == "bye")
    {
        break;
    }

    var numItems = s.Trim().Split()
    .Where(s=>{
        return int.TryParse(s, out int i);
    })
    .Select((x,i)=>
    {
        return int.Parse(x);
    });

    if (numItems.Count() < 3)
    {
        numItems = Calc24.FormulaBuilder.GetFourNumbers(numItems);
        Console.WriteLine(string.Join(' ', numItems));
    }

    sw.Start();

    int c = 0, t = 0;
    //Calc24.NoOptimization = true;
    Calc24.FormulaBuilder.Execute(numItems, item => {
        t++;
        if ((int)((item.Value + 0.0005) * 1000) == 24000)
        {
            Console.WriteLine("[{0,4}] 24 = {1}", ++c, item);
        }
        return true;
    });

    if (c == 0)
        Console.WriteLine(":( 算不出24");
    
    sw.Stop();
    Console.WriteLine("共计算 {0} 次，成功 {1} 次，耗时 {2}", t, c, sw);
    
    /*
    int count = 0;
    Perm.Execute(s.Split(), items=>
    {
        Console.WriteLine("{0,6} [{1}]", ++count, string.Join(", ", items.Cast<string>())); 
        return true;
    });
    */
}

Console.WriteLine("Finished");