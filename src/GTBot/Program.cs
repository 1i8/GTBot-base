using GTBot.Common;

namespace GTProxy
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Write GrowID");
            var gi = Console.ReadLine();
            Console.WriteLine("Write password");
            var gp = Console.ReadLine();
            var bot = new Bot(gi, gp);
            Thread.Sleep(600);
            bot.connect();
        }
    }
}
