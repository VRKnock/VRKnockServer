using System.ServiceModel;
using System.ServiceModel.Web;

namespace KnockServer
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "status")]
        [return:MessageParameter(Name="Status")]
        Status GetStatus(string code);
        
        
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "triggerKnock")]
        [return:MessageParameter(Name="Status")]
        Status TriggerKnock(string code, string message="Knock Knock");
        
        
    }

    public class Status
    {
        public int status { get; set; }
        public string msg { get; set; }
        public string host { get; set; }
        public string device { get; set; }
        public string game { get; set; }
    }
}