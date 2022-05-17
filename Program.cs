namespace ThreadSumSharp
{

    class Program
    {
        private Random rand = new Random();
        private int threadNum;
        private int[] arr;
        private int[] partsarr;
        private Thread[] thread;

        static void Main(string[] args)
        {

            for (int i = 0; i < 100; i++)//цикл для перевірки 
            {
                Program main = new Program();
                main.InitArr();
                main.threadNum = main.rand.Next(4, 11);
                if (main.threadNum >= main.arr.Length)
                    main.threadNum = main.arr.Length - 1;//якщо кількість потоків більша, ніж елементів в масиві
                                                         // main.threadNum = 1;
                (int, int) res = main.ParallelMin();
                Console.WriteLine("\nMin element: {0}\nwith index: {1}\n", res.Item1, res.Item2);

                if (res.Item1 >= 0)//перевірка на правильність
                    Console.ReadKey();
            }
        }
        private int threadCount = 0;

        private (int, int) ParallelMin()
        {
            thread = new Thread[threadNum];
            partsarr = new int[threadNum];
            int portion = arr.Length / threadNum;
            int ostacha = arr.Length % threadNum;
            Console.WriteLine("arr: {0}\nthreadNum: {1}\nostacha: {2}\nportion: {3}", arr.Length, threadNum, ostacha, portion);
            for (int i = 0; i < thread.Length - 1; i++)
            {
                thread[i] = new Thread(StarterThread);
                thread[i].Start(new Bound(i * portion, (i + 1) * portion - 1, i));
                Console.WriteLine("thread[{0}] = new Bound({1}, {2})", i, i * portion, (i + 1) * portion - 1);
            }
            int j = thread.Length - 1;
            Console.WriteLine("thread[{0}] = new Bound({1}, {2})", j, j * portion, (j + 1) * portion - 1 + ostacha);
            thread[j] = new Thread(StarterThread);
            thread[j].Start(new Bound(j * portion, (j + 1) * portion - 1 + ostacha, j));

            lock (lockerForCount)
            {
                while (threadCount < threadNum)
                {
                    Monitor.Wait(lockerForCount);
                }
            }
            int min = int.MaxValue;
            int minindex = -1;
            for (int i = 0; i < partsarr.Length; i++)
            {
                if (min > arr[partsarr[i]])
                {
                    min = arr[partsarr[i]];
                    minindex = partsarr[i];
                }
            }
            return (min, minindex);
        }



        private void InitArr()
        {

            int arrlength = rand.Next(12, 51);
            arr = new int[arrlength];
            for (int i = 0; i < arrlength; i++)
                arr[i] = rand.Next(0, 100);

            arr[rand.Next(0, arrlength)] = rand.Next(-99, 0);
            //arr[arr.Length - 1] = -21;
            Console.WriteLine("Array");
            for (int i = 0; i < arrlength; i++)
            {
                Console.Write(arr[i] + " ");
            }
            Console.WriteLine("\n");

            //Створюється масив рандомної довжини,
            //заповнюєтсья рандомними додатніми числами
            //та рандомно замінюється 1 елемент рандомним від'ємним числом
        }

        class Bound
        {
            public Bound(int startIndex, int finishIndex, int resindex)
            {
                StartIndex = startIndex;
                FinishIndex = finishIndex;
                ResIndex = resindex;
            }

            public int StartIndex { get; set; }
            public int FinishIndex { get; set; }
            public int ResIndex { get; set; }
        }

        private void StarterThread(object param)
        {
            if (param is Bound)
            {
                int minIndex = PartMin((param as Bound).StartIndex, (param as Bound).FinishIndex);
                partsarr[(param as Bound).ResIndex] = minIndex;
                IncThreadCount();

            }
        }

        private readonly object lockerForCount = new object();
        private void IncThreadCount()
        {
            lock (lockerForCount)
            {
                threadCount++;
                Monitor.Pulse(lockerForCount);
            }
        }



        public int PartMin(int startIndex, int finishIndex)
        {
            int min = int.MaxValue;
            int minIndex = -1;
            for (int i = startIndex; i <= finishIndex; i++)
            {
                if (min > arr[i])
                {
                    min = arr[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }
    }
}