using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRetrieveCategories
{
    public interface ICategoriesRetrieve
    {
        Task<List<Category>> RetrieveActiveCategories();
    }
}
