using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace KnockServer
{
    public class EnableCorsBehavior : IEndpointBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            Console.WriteLine("ApplyDispatchBehavior");
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CorsEnablingMessageInspector());
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }

    public class CorsEnablingMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var httpRequest = (HttpRequestMessageProperty) request.Properties[HttpRequestMessageProperty.Name];

            return new
            {
                origin = httpRequest.Headers["Origin"],
                handlePreflight = httpRequest.Method.Equals("OPTIONS", StringComparison.InvariantCultureIgnoreCase)
            };
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var state = (dynamic) correlationState;

            // handle request preflight
            if (state.handlePreflight)
            {
                reply = Message.CreateMessage(MessageVersion.None, "PreflightReturn");

                var httpResponse = new HttpResponseMessageProperty();
                reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);

                httpResponse.SuppressEntityBody = true;
                httpResponse.StatusCode = HttpStatusCode.OK;
            }

            // add allowed origin info
            var response = (HttpResponseMessageProperty) reply.Properties[HttpResponseMessageProperty.Name];
            response.Headers.Add("Access-Control-Allow-Origin", "https://vrknock.app");

            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            response.Headers.Add("Access-Control-Allow-Credentials", "true");
        }
    }
}