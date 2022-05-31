using APIMarketplaceApp.Helpers ;
namespace APIMarketplaceApp.Models
{
    public class  ResponseModel
    {
        public ResponseModel(int responseCode, string responseMessage,object dataSet)
        {
        //ResponseCode=responseCode;
        ResponseMessage=responseMessage;
        DateSet=dataSet;
        }
        public ResponseModel(int responseCode, string responseMessage)
        {
        ResponseCode=responseCode;
        ResponseMessage=responseMessage;
        }
         public int ResponseCode { get; set; }
         public string ResponseMessage { get; set; }
         public object DateSet { get; set; }
     }
    
}
