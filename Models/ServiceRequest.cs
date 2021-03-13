using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Cohesion.Models
{
    public enum CurrentStatus
    {
        NotApplicable,
        Created,
        InProgress,
        Complete,
        Canceled
    }
    [DataContractAttribute]
    public class ServiceRequest
    {
        [DataMemberAttribute]
        public Guid id { get; set; }
        [DataMemberAttribute]
        public String buildingCode { get; set; }
        [DataMemberAttribute]
        public String description { get; set; }
        [DataMemberAttribute]
        public CurrentStatus currentStatus { get; set; }
        [DataMemberAttribute]
        public String createdBy { get; set; }
        [DataMemberAttribute]
        public DateTime createdDate { get; set; }
        [DataMemberAttribute]
        public String lastModifiedBy { get; set; }
        [DataMemberAttribute]
        public DateTime lastModifiedDate { get; set; }

        [JsonConstructorAttribute]
        public ServiceRequest()
        {

        }
        public ServiceRequest(String bc, String desc, string user)
        {
            this.id = Guid.NewGuid();
            this.buildingCode = bc;
            this.description = desc;
            this.currentStatus = CurrentStatus.Created;
            this.createdBy = user;
            this.createdDate = DateTime.Now;
            this.lastModifiedBy = user;
            this.lastModifiedDate = DateTime.Now;
        }

        public ServiceRequest(String id, String bc, String desc, int status, string user, DateTime created, string modUser, DateTime lastModified)
        {
            this.id = Guid.NewGuid();
            this.buildingCode = bc;
            this.description = desc;
            this.currentStatus = (CurrentStatus)status;
            this.createdBy = user;
            this.createdDate = created;
            this.lastModifiedBy = modUser;
            this.lastModifiedDate = lastModified;
        }

        //returns this object in json format
        public string getRequest()
        {
            string requestsJSON = string.Empty;

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(ServiceRequest));
            System.IO.MemoryStream msObject = new System.IO.MemoryStream();
            js.WriteObject(msObject, this);
            msObject.Position = 0;
            System.IO.StreamReader sr = new StreamReader(msObject);
            requestsJSON = sr.ReadToEnd();
            sr.Close();
            msObject.Close();
            return requestsJSON;

        }

        //updates request with json passed in
        public void setRequest(string requestJSON)
        {
            using (var ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(requestJSON)))
            {
                // Deserialization from JSON  
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(ServiceRequest));
                ServiceRequest newRequest = (ServiceRequest)deserializer.ReadObject(ms);
                this.id = newRequest.id;
                this.buildingCode = newRequest.buildingCode;
                this.description = newRequest.description;
                this.currentStatus = newRequest.currentStatus;
                this.createdBy = newRequest.createdBy;
                this.createdDate = newRequest.createdDate;
                this.lastModifiedBy = newRequest.lastModifiedBy;
                this.lastModifiedDate = newRequest.lastModifiedDate;
            }
        }
    }

}