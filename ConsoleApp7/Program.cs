using System.Timers;  // <-- this timer.. Not the Windows.Forms.Timer, because that one works on the messagequeue (to receive the timer_elapsed event on the gui thread), but you don't have a messagequeue/forms

static class Program
{
    private static int cnt;
    private static int a=1;
    private static int b = 2;
    static System.Timers.Timer timer;
    static async Task Main(string[] args)
    {
        int intResult = 0;
        //timer = new System.Timers.Timer(100);
        //timer.Elapsed += (sender, e) => MyElapsedMethod(a,b);
        //timer.Start();
        //Console.WriteLine("Timer is started");
        //Console.ReadLine();

        // static void MyElapsedMethod(int arg1,int arg2)
        // {
        //     Method1();
        //     Method2();
        //     Method3(arg1);
        //     Method4(arg1,arg2);
        //     cnt++;
        //     if (cnt == 10)
        //     {
        //         timer.Stop();
        //         timer.Dispose();
        //     };
        // }
        //await MultiAwaits();


        // await MultiAwaits("Helllow");


        // метод Parallel.Invoke выполняет три метода
        Parallel.Invoke(
            () => Method2(),
            () => intResult= Method5(1,2),
            () => PrintAsync("Hellow")
        );
        Console.WriteLine(intResult);
        Console.ReadLine();



    }

    static async Task MultiAwaits(string message)
    {

        var task1 = PrintAsync( message);
        var task2 = PrintAsync( message);
        var task3 = Method1();

        // ожидаем завершения всех задач
        await Task.WhenAll(task3, task2, task1);
    }

    public static async Task PrintAsync(string message)
    {
        await Task.Delay(1);     // имитация продолжительной операции
        Console.WriteLine(message);
    }


    public static async Task Method1()
    {
      
        for (int i = 0; i < 10; i++) {
            int j = i + 1;
            await Task.Delay(2);
            Console.WriteLine(i);
        }
    }

    private static void Method2()
    {
        Console.WriteLine("Method2 is executed at {0}", DateTime.Now);
    }

    private static void Method3(int i)
    {
        Console.WriteLine("Method3 is executed at {0}", i);
    }
    private static void Method4(int i, int j)
    {
        Console.WriteLine("Method4 is executed at {0}", i+j);
    }

    private static int Method5(int i, int j)
    {
        return i + j;
    }

}