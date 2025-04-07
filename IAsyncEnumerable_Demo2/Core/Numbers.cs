using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace IAsyncEnumerable_Demo2.Core;

public static class Numbers
{
    public static async IAsyncEnumerable<int> GetNumbersAsync(int begin, int end, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (var i = begin; i <= end; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(1000, cancellationToken);
            yield return i;
        }
    }
}