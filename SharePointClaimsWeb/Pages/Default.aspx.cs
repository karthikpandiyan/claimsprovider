using Microsoft.SharePoint.Client;
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

            //
            //https://jcistage.sharepoint.com/"
            //"https://jcistage.sharepoint.com/sites/secondlevel/
            using (var clientContext = this.GetClientContextWithAccessToken("https://jcistage.sharepoint.com/sites/dev"))
            {
                //clientContext.Load(clientContext.Web, web => web.Title);
                //clientContext.ExecuteQuery();
                //Response.Write(clientContext.Web.Title);


                Web web = clientContext.Web;
                var props = web.AllProperties;
                web.Context.Load(props);
                web.Context.ExecuteQuery();

                props["test"] = "sampleProp";
                web.Update();
                web.Context.ExecuteQuery();


                clientContext.Load(web, w => w.AllProperties, w => w.Url);
                clientContext.ExecuteQuery();
                Response.Write(web.AllProperties["test"].ToString());
                Response.Write(web.Url);


            }

            //

            ////////var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            ////////using (var clientContext = spContext.CreateUserClientContextForSPHost())
            ////////{
            ////////    //Web web = clientContext.Web;
            ////////    //clientContext.Load(web);
            ////////    //clientContext.ExecuteQuery();

            ////////    //ListCollection lists = web.Lists;
            ////////    //clientContext.Load<ListCollection>(lists);
            ////////    //clientContext.ExecuteQuery();


            ////////    Web web = clientContext.Web;
            ////////    var props = web.AllProperties;
            ////////    web.Context.Load(props);
            ////////    web.Context.ExecuteQuery();

            ////////    props["test"] = "update";
            ////////    web.Update();
            ////////    web.Context.ExecuteQuery();


            ////////    clientContext.Load(web, w => w.AllProperties, w => w.Url);
            ////////    clientContext.ExecuteQuery();
            ////////    Response.Write(web.AllProperties["test"].ToString());
            ////////    Response.Write(web.Url);
            ////////}
            //
        //    Uri hostWeb =
        //new Uri("https://jcistage.sharepoint.com/");
            

       //     Uri hostWeb =
       //new Uri(Request.QueryString["SPHostUrl"]);

       //     using (var clientContext = TokenHelper.GetS2SClientContextWithWindowsIdentity(hostWeb, Request.LogonUserIdentity))
       //     {
       //         clientContext.Load(
       //           clientContext.Web, web => web.Title);
       //         clientContext.ExecuteQuery();
       //         Response.Write(clientContext.Web.Title);
       //         //clientContext.Web.Title =
       //         //   DateTime.Now.ToLongTimeString();
       //         clientContext.Web.Update();
       //         clientContext.ExecuteQuery();
       //     }
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