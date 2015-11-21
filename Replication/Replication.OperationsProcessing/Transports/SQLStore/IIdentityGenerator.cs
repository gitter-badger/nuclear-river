namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public interface IIdentityGenerator
    {
        long Next();
    }
}