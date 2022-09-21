namespace Repo.EntityFramework.Interfaces;

/// <summary>
/// An interface that allows you to transform a business model into a database entity and vice versa
/// </summary>
/// <typeparam name="TDbModel">Type of database entity</typeparam>
/// <typeparam name="TBsModel">Type of business model</typeparam>
public interface IRelatedDbModel<TDbModel, TBsModel> 
    where TDbModel : class, IRelatedDbModel<TDbModel, TBsModel>
    where TBsModel : class
{
    /// <summary>
    /// Creates database entity from business model
    /// </summary>
    /// <param name="bsModel">Business model for converting</param>
    /// <returns>New database entity</returns>
    static abstract TDbModel CreateFromBusinessModel(TBsModel bsModel);
    /// <summary>
    /// Converts this database entity to business model
    /// </summary>
    /// <returns>New business model</returns>
    TBsModel ConvertToBusinessModel();
    /// <summary>
    /// Updates this database entity properties from business model
    /// </summary>
    /// <param name="bsModel">Business model with new data</param>
    void UpdateDataFromBusinessModel(TBsModel bsModel);
}
