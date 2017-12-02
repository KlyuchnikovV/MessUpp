﻿using System;
using System.Web.Http;
using Messenger.Model;
using Messenger.DataLayer;
using Messenger.DataLayer.SQL;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net;

namespace Messenger.Api.Controllers
{
    public class AttachmentController : ApiController
    {
        private readonly IAttachmentRepository attachRepository;
        
        public AttachmentController()
        { 
            attachRepository = new AttachmentRepository(Constants.Constants.ConnectionString);
        }

        [HttpPost]
        [Route("api/attach")]
        public Attachment CreateAttachment([FromBody]Attachment attach)
        {
            try
            {
                return attachRepository.LoadAttachment(attach);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        [Route("api/attach/{id}")]
        public Attachment GetAttachment(Guid id)
        {
            try
            {
                return attachRepository.GetAttachment(id);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
            catch (Exception exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpDelete]
        [Route("api/attach/{id}")]
        public void DeleteChat(Guid id)
        {
            try
            {
                attachRepository.DeleteAttachment(id);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }
    }
}