namespace PSWM_backend
{
    public interface IGetSetSPI
    {
        List<T> GetSpAllItem<T>(string sp, Func<System.Data.IDataReader, T> itemReader, params object[] sendparameters);
        public string PostSpAllItem<T>(string sp, params object[] sendparameters);


    }
}
