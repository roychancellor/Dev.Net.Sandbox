using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.DBRepository;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRetrieveCategories
{
    public class CategoriesRetriever : ICategoriesRetrieve
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly ICategoriesRepository _repo;

        public CategoriesRetriever(ICategoriesRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Category>> RetrieveActiveCategories()
        {
            try
            {
                return await _repo.RetrieveActiveCategories();
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"While retrieving active categories");
                throw;
            }
        }
    }
}
