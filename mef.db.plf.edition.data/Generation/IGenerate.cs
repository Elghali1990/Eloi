namespace mef.db.plf.edition.generation.Generation
{
    public interface IGenerate<T>
    {
        public Task<FileStream> Print(T data);
    }
}
