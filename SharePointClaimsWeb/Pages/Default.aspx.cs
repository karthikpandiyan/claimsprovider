﻿using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SharePointClaimsWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            //var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            //using (var clientContext = spContext.CreateUserClientContextForSPHost())
            //{
            //    clientContext.Load(clientContext.Web, web => web.Title);
            //    clientContext.ExecuteQuery();
            //    Response.Write(clientContext.Web.Title);
            //}


            using (var clientContext = this.GetClientContextWithAccessToken("https://jcistage.sharepoint.com/sites/secondlevel/"))
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                Response.Write(clientContext.Web.Title);
            }
        }

        /// <summary>
        /// gets access token and returns clientcontext
        /// </summary>
        /// <returns></returns>
        public ClientContext GetClientContextWithAccessToken(string url)
        {
            Uri siteUri = new Uri(url);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);

            //Get the access token for the URL.  
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                siteUri.Authority, realm).AccessToken;

            //Get client context with access token
            ClientContext clientcontext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken);
            return clientcontext;
        }


    }
}