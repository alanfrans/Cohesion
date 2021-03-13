using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Cohesion.Models;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace Cohesion.Models
{
    public class ServiceRequests
    {

        List<ServiceRequest> srList = new List<ServiceRequest>();
        public delegate void notifyUser(string value);

        class EventPublisher
        {
            private string theVal;
            public event notifyUser requestUpdated;

            public string Val
            {
                set
                {
                    this.theVal = value;
                    this.requestUpdated(theVal);
                }
            }
        }

        //given more time - the email and texting functionality should be split out into classes or even services of their own
        static void request_statusChanged(string value)
        {
            //get user to notify
            String username = value.Substring(0, value.IndexOf(':'));
            //get message to give to user
            String message = value.Substring(username.Length + 2);
            //could use data annotations here to verify, 
            //but since we are overloading username here in a sense 
            //we wont stick these in any particular subclass for this singular use case
            //check if user is a email address
            if (IsValidEmail(username))
            {
                //send user an email
                //email really should be send using smtp over iis, with smtp enabled for the web api
                //in this case for simplicity - will just use the gmail smtp server with the code provided
                //Note: send email functionality copied from Net-informations.com
                try
                {
                    //this connection will need a secure connection in order to work
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                    mail.From = new MailAddress("cohesionservicerequest@gmail.com");
                    mail.To.Add(username);
                    mail.Subject = "Request Status Change";
                    mail.Body = message;
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("cohesionservicerequest@gmail.com", "password");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch (Exception ex)
                {
                    //logger not implemented

                }
            }
            else if (Regex.IsMatch(username, @"^\(?\d{3}\)?[\s\-]?\d{3}\-?\d{4}$"))       //check if user is a phone number
            {
                //sms service and account needs to be implemented
                //send user a text
                //account sid
                //auth token
            }
            //otherwise - user cannot be notified.  

        }


        //Note: method IsValidEmail from stackoverflow
        static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public ServiceRequests()
        {
            //srList.Add(new ServiceRequest("COH", "Please turn up the AC in suite 1200D. It is too hot here.", "Nik Patel"));
            //srList.Add(new ServiceRequest("COH", "Please turn off the AC in suite 1200D. It is too cold here.", "Mary Johnson"));
            //srList.Add(new ServiceRequest("COH", "Please move me to an office away from Mary.", "Nik Patel"));
            GetData();
        }

        public void removeRequests()
        {
            srList.Clear();
        }

        public string GetData()
        {

            String dataToReturn = String.Empty;
            String dataFile = String.Empty;
            string appDataFolder = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            dataFile = Path.Combine(appDataFolder, "data.json");
            dataToReturn = File.ReadAllText(dataFile);
            srList = JsonConvert.DeserializeObject<List<ServiceRequest>>(dataToReturn);
            string retVal = String.Empty;
            foreach (ServiceRequest sr in srList)
            {
                retVal += sr.getRequest();
            }

            return retVal;
        }
        public string GetData(String id)
        {
            string retVal = String.Empty;
            ServiceRequest sr = srList.Find(x => x.id.ToString().ToLower().Contains(id.ToLower()));
            if (sr != null)
            {
                retVal = sr.getRequest();
            }

            return retVal;
        }
        public string AddRequest(String buildingCode, String desc, String user)
        {
            //public ServiceRequest(String bc, String desc, string user)
            EventPublisher thisReq = new EventPublisher();
            thisReq.requestUpdated += request_statusChanged;
            ServiceRequest newReq = new ServiceRequest(buildingCode, desc, user);
            //add to list
            srList.Add(newReq);
            //save the list
            SaveData();
            string retVal = GetData(newReq.id.ToString());
            thisReq.Val = ($"{newReq.createdBy}: A new request was created with an id of: {newReq.id}");
            return retVal;
        }

        public string UpdateRequest(String id, int status, String user)
        {

            //find request in the list
            //update the request in the list
            EventPublisher thisReq = new EventPublisher();
            thisReq.requestUpdated += request_statusChanged;

            ServiceRequest sr = srList.Find(x => x.id.ToString().ToLower().Contains(id.ToLower()));
            if (sr != null)
            {
                sr.currentStatus = (CurrentStatus)status;
                sr.lastModifiedBy = user;
                sr.lastModifiedDate = DateTime.Now;
            }


            //save the list
            SaveData();
            string retVal = GetData(id);
            thisReq.Val = ($"{sr.createdBy}: the request {id} was updated with a status of {status}");
            return retVal;
        }

        public bool DeleteRequest(String id)
        {
            bool success = false;
            //find request in the list
            //update the request in the list
            ServiceRequest sr = srList.Find(x => x.id.ToString().ToLower().Contains(id.ToLower()));
            if (sr != null)
            {
                srList.Remove(sr);
                success = true;
            }
            //save the list
            SaveData();
            return success;
        }

        public void SaveData()
        {
            string appDataFolder = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            String dataFile = Path.Combine(appDataFolder, "data.json");

            String dataToSave = "[";
            foreach (ServiceRequest sr in srList)
            {
                dataToSave += sr.getRequest() + ",";
            }
            //remove last comma
            dataToSave = dataToSave.Remove(dataToSave.Length - 1) + "]";
            File.WriteAllText(dataFile, dataToSave);

        }

        public ServiceRequest GetRequest(String id)
        {

            ServiceRequest retRequest = null;
            retRequest = srList.Find(sr => sr.id.ToString().ToLower().Contains(id.ToLower()));

            return retRequest;
        }


    }
}
