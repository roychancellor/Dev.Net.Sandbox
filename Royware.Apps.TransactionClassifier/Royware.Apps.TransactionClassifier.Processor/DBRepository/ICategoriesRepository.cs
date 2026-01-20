using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public interface ICategoriesRepository
    {
        Task<List<Category>> RetrieveActiveCategories();
    }
}
