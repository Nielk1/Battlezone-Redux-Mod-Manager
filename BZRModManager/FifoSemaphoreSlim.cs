using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class FifoSemaphoreSlim
{
    private SemaphoreSlim lockSim = new SemaphoreSlim(1, 1);

    private List<SemaphoreSlim> WaitingQueue = new List<SemaphoreSlim>();

    private SemaphoreSlim RequestNewSemaphore()
    {
        lockSim.Wait();
        try
        {
            SemaphoreSlim newSemaphore = new SemaphoreSlim(1, 1);
            return newSemaphore;
        }
        finally
        {
            lockSim.Release();
        }
    }

    public void Release()
    {
        lockSim.Wait();
        try
        {
            WaitingQueue.RemoveAt(0);
            if (WaitingQueue.Count > 0)
                WaitingQueue[0].Release();
        }
        finally
        {
            lockSim.Release();
        }
    }

    public void Wait()
    {
        SemaphoreSlim semaphore = RequestNewSemaphore();
        lockSim.Wait();
        try
        {
            WaitingQueue.Add(semaphore);

            if (WaitingQueue.Count > 1)
                semaphore.Wait(); // if it's in the queue it should have 0 permits
        }
        finally
        {
            lockSim.Release();
        }
        semaphore.Wait(); // intended wait
    }
    public async Task WaitAsync()
    {
        SemaphoreSlim semaphore = RequestNewSemaphore();
        await lockSim.WaitAsync();
        try
        {
            WaitingQueue.Add(semaphore);

            if (WaitingQueue.Count > 1)
                await semaphore.WaitAsync(); // if it's in the queue it should have 0 permits
        }
        finally
        {
            lockSim.Release();
        }
        await semaphore.WaitAsync(); // intended wait
    }
}