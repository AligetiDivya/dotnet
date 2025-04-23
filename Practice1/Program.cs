using System.IO;
using System.Runtime.InteropServices;
using VBInterop;
class Program
{
    static void Main()
    {
        Console.WriteLine("1. Using 'using' block for unmanaged resource:");
        //ensures the stream is automatically closed and disposed even if an exception occurs
        using (var stream1 = new FileStream("data1.txt", FileMode.OpenOrCreate))
        {
            Console.WriteLine("file stream opened with 'using'");
        }

        Console.WriteLine("\n 2.Manually calling Dispose:");
        //if dispose not called stream stays open until GC collects it
        var stream2 = new FileStream("data2.txt", FileMode.OpenOrCreate);
        stream2.Dispose();
        Console.WriteLine("Disposed file stream manually");

        Console.WriteLine("\n 3. Forcing Garbage Colllection:");
        GC.Collect(); // Forcing GC
        GC.WaitForPendingFinalizers(); // waiting for finalizers to run
        Console.WriteLine("GC triggered manually");

        Console.WriteLine("\n 4️. Using IDisposable in a custom class:");
        using (MyResource res = new MyResource())
        {
            Console.WriteLine("Inside using block");
        }
        //Dispose is called when using block ends

        Console.WriteLine("\n 5️. Interop: Using VB.NET integer in C#");
        int cInt = 10;
        int vbInt = VBHelper.GetNumber();
        int result = vbInt + cInt;
        Console.WriteLine($"VB.NET Integer + C# int = {result}");
    }
}

// Sample class with IDisposable implementation
class MyResource : IDisposable
{
    private bool disposed = false; //used to detect redundant calls
    public MyResource()
    {
        Console.WriteLine("Constructor: MyResourse obj created");
    }
    
    //public method to cleanup
    public void Dispose()
    {
        Console.WriteLine("Called by user to clean up");
        Dispose(true);
        GC.SuppressFinalize(this); //this stops GC from calling finalizer MyResource()
       //there is no need for GC to call finalizer if Dispose() already cleaned up manually
        Console.WriteLine("GC.SuppressFinalize called");
    }

    //protected virtual method
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            //direcly calling Dispose(false) may give exception due partial/finalized cleaning of Managed objects by GC
            if(disposing)
            {
                //free managed resources
                Console.WriteLine("Releasing managed resources");
            }
            //free unmanged resources
            Console.WriteLine("Releasing unmanaged resources");
            disposed = true;
        }
        else
        {
            Console.WriteLine("Already cleaned/disposed");
        }
    }
    ~MyResource()
    {
        Console.WriteLine("Finalizer: ~MyResourse() called by GC"); // GC cleaning up (didnt called Dispose() manually)
        Dispose(false); //only unmanaged resources will be cleaned up here because anyway GC cleans up Managed resources
    }
}
    