namespace PSWM_backend
{
    public interface IadditionalService
    {
     public string tokenAuthentication(string id);
     public void CheckDateValidation(string deviceid);

       public void CheckRemainingQuantity(string deviceid);


    }
   
}
