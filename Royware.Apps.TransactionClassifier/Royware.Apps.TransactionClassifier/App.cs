using Royware.Apps.TransactionClassifier.Processor;

namespace Royware.Apps.TransactionClassifier
{
    public sealed class App
    {
        private readonly TransactionProcessor _processor;

        public App (TransactionProcessor processor)
        {
            _processor = processor;
        }

        public async Task RunAsync()
        {
            await _processor.ProcessAsync();
        }
    }

}
