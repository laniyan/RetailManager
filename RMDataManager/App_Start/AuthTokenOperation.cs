using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace RMDataManager.App_Start
{
    public class AuthTokenOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            //In docfilter we are saying I want you to add one new route called /token and put in the auth cateogry continues down below in auth cat
            swaggerDoc.paths.Add("/token", new PathItem
            {
                post = new Operation
                {
                    tags = new List<string> { "Auth" },
                    consumes = new List<string>
                    {
                        //the command is a post command and the type of data set in the param is app/x-www-form-urlencoded and the def for the 3 param set below
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter>
                    {
                        new Parameter 
                        {
                          type = "string",
                          name = "grant_type",
                          required = true,
                          @in = "formData",
                          @default = "password"//sets the for this field 
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "Username",
                            required = false,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "Password",
                            required = false,
                            @in = "formData"
                        }
                    }
                }
            });
        }
    }
}