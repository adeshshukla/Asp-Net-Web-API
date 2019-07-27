using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace API_Auth_1
{
    public class MyAuthProvider : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            //UserEntities obj = new UserEntities();
            //var userdata = obj.EF_UserLogin(context.UserName, context.Password).FirstOrDefault();

            // Validation Logic
            var userdata = ValidateUser(context.UserName, context.Password);

            if (userdata != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, userdata.UserRole));
                identity.AddClaim(new Claim(ClaimTypes.Name, userdata.UserName));
                //context.Validated(identity);

                AuthenticationProperties properties = CreateProperties(userdata.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);

                context.Validated(ticket);
            }
            else
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                context.Rejected();
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        private Userdata ValidateUser(string UserName, string Password)
        {
            if (UserName == "adesh" && Password == "12345")
            {
                return new Userdata() { UserName = "adesh", UserRole = "admin" };
            }

            return null;
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>();

            data.Add("username", userName);
            data.Add("role", "admin");
            data.Add("mydata", "Bla Bla");

            return new AuthenticationProperties(data);
        }

    }


    internal class Userdata
    {
        public Userdata()
        {
        }

        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}