using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Reactive;

using System.Threading;
using System.Threading.Tasks;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace IAsyncEnumerable_Demo2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private CancellationTokenSource? _cancellationTokenSource;
    
    [Reactive] public int Progress { get; set; }
    [Reactive] public int Minimum { get; set; } = 1;
    [Reactive] public int Maximum { get; set; } = 10;
    [Reactive] public string? Output { get; set; }
    
    public ObservableCollection<int> Numbers { get; } = [];
    
    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    public MainWindowViewModel()
    {
        IProgress<int> progress = new Progress<int>(i => Progress = i);
        
        StartCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var numbers = Core.Numbers.GetNumbersAsync(Minimum, Maximum, _cancellationTokenSource.Token);
                await WriteNumbers(numbers, progress, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException e)
            {
                Output = $"Operation canceled: {e.Message}";
            }
        });

        StopCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _cancellationTokenSource?.CancelAsync()!;
        });
    }
    
    private async Task WriteNumbers(IAsyncEnumerable<int> numbers, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        Numbers.Clear();
        
        await foreach (var number in numbers.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            Numbers.Add(number);
            progress?.Report(number);
            //await Task.Yield();
        }
    }
}