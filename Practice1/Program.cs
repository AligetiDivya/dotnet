using VBInterop.VBInterop;

namespace ResourceDemo
{
    /// <summary>
    /// Demonstrates file handling, IDisposable, and VB.NET interop in C#.
    /// </summary>
    class Program
    {
        static void Main()
        {
            // 1. Using 'using' block for unmanaged resource:
            try
            {
                //ensures the stream is automatically closed and disposed even if an exception occurs
                using (var stream1 = new FileStream("data1.txt", FileMode.OpenOrCreate))
                {
                    Console.WriteLine("File stream opened with 'using'.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error opening file1: {ex.Message}");
            }

            // 2. Manually calling Dispose:
            FileStream? stream2 = null;
            try
            {
                //if dispose not called stream stays open until GC collects it
                stream2 = new FileStream("data2.txt", FileMode.OpenOrCreate);
                Console.WriteLine("File stream2 opened.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error opening file2: {ex.Message}");
            }
            finally
            {
                stream2?.Dispose();
                Console.WriteLine("Disposed file stream2 manually.");
            }

            // 3. Forcing Garbage Collection:
            GC.Collect(); // Forcing GC
            GC.WaitForPendingFinalizers(); // waiting for finalizers to run
            Console.WriteLine("GC triggered manually.");

            // 4. Using IDisposable in a custom class:
            using (var res = new MyResource())
            {
                Console.WriteLine("Inside using block for MyResource.");
            }
            //Dispose is called when using block ends

            // 5. Interop: Using VB.NET integer in C#
            int cInt = 10;
            int vbInt = VBHelper.GetNumber();
            int result = vbInt + cInt;
            Console.WriteLine($"VB.NET Integer + C# int = {result}");
        }
    }

    /// <summary>
    /// Example of custom resource class implementing IDisposable.
    /// </summary>
    public class MyResource : IDisposable
    {
        private bool _disposed = false; 
        public MyResource()
        {
            Console.WriteLine("Constructor: MyResourse obj created.");
        }

        /// <summary>
        /// Dispose pattern implementation to clean up resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method.
        /// </summary>
        /// <param name="disposing">Whether called from Dispose() or finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            //direcly calling Dispose(false) may give exception due partial/finalized cleaning of Managed objects by GC
            if (disposing)
            {
                // Free managed resources here.
                Console.WriteLine("Releasing managed resources.");
            }
            // Free unmanaged resources here.
            Console.WriteLine("Releasing unmanaged resources.");

            _disposed = true;
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~MyResource()
        {
            Dispose(false);
        }
    }
}
    
