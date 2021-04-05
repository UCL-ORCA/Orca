using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Entities
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class User
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
        public string TenantId { get; set; }
    }

    public class Organizer
    {
        public User User { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
        public object AcsUser { get; set; }
        public object SpoolUser { get; set; }
        public object Phone { get; set; }
        public object Guest { get; set; }
        public object Encrypted { get; set; }
        public object OnPremises { get; set; }
        public object AcsApplicationInstance { get; set; }
        public object SpoolApplicationInstance { get; set; }
        public object ApplicationInstance { get; set; }
    }

    public class Participant
    {
        public User User { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
        public object AcsUser { get; set; }
        public object SpoolUser { get; set; }
        public object Phone { get; set; }
        public object Guest { get; set; }
        public object Encrypted { get; set; }
        public object OnPremises { get; set; }
        public object AcsApplicationInstance { get; set; }
        public object SpoolApplicationInstance { get; set; }
        public object ApplicationInstance { get; set; }
    }

    public class UserAgent
    {
        public string Platform { get; set; }
        public string ProductFamily { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
        public string HeaderValue { get; set; }
    }

    public class Callee
    {
        public UserAgent UserAgent { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
    }

    public class Identity
    {
        public User User { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
        public object AcsUser { get; set; }
        public object SpoolUser { get; set; }
        public object Phone { get; set; }
        public object Guest { get; set; }
        public object Encrypted { get; set; }
        public object OnPremises { get; set; }
        public object AcsApplicationInstance { get; set; }
        public object SpoolApplicationInstance { get; set; }
        public object ApplicationInstance { get; set; }
    }

    public class Caller
    {
        public Identity Identity { get; set; }
        public UserAgent UserAgent { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
    }

    public class Session
    {
        public Callee Callee { get; set; }
        public Caller Caller { get; set; }
        public DateTime EndDateTime { get; set; }
        public List<string> Modalities { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Id { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
    }

    public class ResponseHeaders
    {
        public List<string> Date { get; set; }

        [JsonProperty("Transfer-Encoding")]
        public List<string> TransferEncoding { get; set; }
        public List<string> Vary { get; set; }

        [JsonProperty("Strict-Transport-Security")]
        public List<string> StrictTransportSecurity { get; set; }

        [JsonProperty("request-id")]
        public List<string> RequestId { get; set; }

        [JsonProperty("client-request-id")]
        public List<string> ClientRequestId { get; set; }

        [JsonProperty("x-ms-ags-diagnostic")]
        public List<string> XMsAgsDiagnostic { get; set; }

        [JsonProperty("scenario-id")]
        public List<string> ScenarioId { get; set; }

        [JsonProperty("OData-Version")]
        public List<string> ODataVersion { get; set; }
    }

    public class CallRecord
    {
        public DateTime EndDateTime { get; set; }
        public string JoinWebUrl { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public List<string> Modalities { get; set; }
        public Organizer Organizer { get; set; }
        public List<Participant> Participants { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public List<Session> Sessions { get; set; }
        public string Id { get; set; }

        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }

        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }

        [JsonProperty("sessions@odata.context")]
        public string SessionsOdataContext { get; set; }
        public ResponseHeaders ResponseHeaders { get; set; }
        public string StatusCode { get; set; }
    }


}
