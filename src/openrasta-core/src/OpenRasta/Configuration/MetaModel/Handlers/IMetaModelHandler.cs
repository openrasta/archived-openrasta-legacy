namespace OpenRasta.Configuration.MetaModel.Handlers
{
    public interface IMetaModelHandler
    {
        void PreProcess(IMetaModelRepository repository);
        void Process(IMetaModelRepository repository);
    }
}