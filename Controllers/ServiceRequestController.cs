using System;
using System.Web.Http;
using System.IO;
using Cohesion.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Web;

namespace Cohesion.Controllers
{
    public class ServiceRequestController : ApiController
    {
        ServiceRequests serviceRequests = new ServiceRequests();

        //GET :
        //description: Read service request by id.
        //route: api/servicerequest/{id}
        //    Response
        //200: single service request
        //404: not found
        // GET: api/CreateServiceRequest/5
        public HttpResponseMessage Get(string id)
        {
            var data = serviceRequests.GetData(id);
            if (data.Length > 0)    
            {
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

        }

        //GET
        //description: Read all service requests
        //route: api/servicerequest
        //Response
        //200: list of service requests
        //204: empty content
        // GET: api/CreateServiceRequest
        public HttpResponseMessage Get()
        {
            //return new string[] { "value1", "value2" };
            var data = serviceRequests.GetData();
            //data = data.Remove(0);  //for testing no content
            if (data.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }

        }

        //POST :
        //description: Create new service request
        //route: api/servicerequest
        //Response
        //201: created service request with id
        //400: bad request
        public HttpResponseMessage Post([FromBody] string value)
        {
            //          {
            //              "buildingCode": "COD",
            //              "createdBy": "maintenance man",
            //              "description": "Change a bad light in the mens room."
            //          }
            //create new service request getting data from the body of post (raw json formatted - example above)
            string req_txt;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                req_txt = reader.ReadToEnd();
            }
            //get defined fields by putting it into a service request object
            ServiceRequest srr = JsonConvert.DeserializeObject<ServiceRequest>(req_txt);
            try
            {
                //ensure that the json was sent with the required information to  create a ticket
                if (srr.buildingCode == null || srr.description == null || srr.createdBy == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                var data = serviceRequests.AddRequest(srr.buildingCode, srr.description, srr.createdBy);
                if (data.Length > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, data);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }




        //PUT
        //description: update service request based on id
        //route: api/servicerequest/{id}
        //    Response
        //200: updated service request
        //400: bad service request
        //404: not found

        // PUT: api/CreateServiceRequest/5
    public HttpResponseMessage Put(string id, [FromBody] string value)
        {
            //          {
            //              "id": "0e581177-33dc-4de8-8743-4f91a0327d50",
            //              "CurrentStatus": "Complete",
            //              "lastModifiedBy": "maintenance man"
            //          }
            //create new service request getting data from the body of post (raw json formatted - example above)
            string req_txt;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                req_txt = reader.ReadToEnd();
            }
            //get defined fields by putting it into a service request object
            ServiceRequest srr = JsonConvert.DeserializeObject<ServiceRequest>(req_txt);
            try
            {
                var data = serviceRequests.UpdateRequest(id, (int)srr.currentStatus, srr.lastModifiedBy);
                //data = data.Remove(0);  //for testing bad request
                if (data.Length > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        //DELETE
        //description: delete service request based on id
        //route: api/servicerequest/{id}
        //    Response
        //201: successful
        //404: not found
        // DELETE: api/CreateServiceRequest/5
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                bool success = serviceRequests.DeleteRequest(id);
                //data = data.Remove(0);  //for testing not found
                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
