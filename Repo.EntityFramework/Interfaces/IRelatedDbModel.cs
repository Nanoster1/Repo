namespace Repo.EntityFramework.Interfaces;

public interface IRelatedDbModel<TDbModel, TBsModel> 
    where TDbModel : class, IRelatedDbModel<TDbModel, TBsModel>
    where TBsModel : class
{
    static abstract TDbModel CreateFromBusinessModel(TBsModel bsModel);
    TBsModel ConvertToBusinessModel();
    void UpdateDataFromBusinessModel(TBsModel bsModel);
}