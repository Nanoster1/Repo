namespace Repo.Abstractions.Interfaces;

public interface IObjectMapper
{
    TObjectTo CreateAndMap<TObjectFrom, TObjectTo>(TObjectFrom objectFrom);
    void MapTo<TObjectFrom, TObjectTo>(TObjectFrom objectFrom, TObjectTo objectTo);
    bool TryCreateAndMap<TObjectFrom, TObjectTo>(TObjectFrom objectFrom, out TObjectTo objectTo);
    bool TryMapTo<TObjectFrom, TObjectTo>(TObjectFrom objectFrom, TObjectTo objectTo);
}