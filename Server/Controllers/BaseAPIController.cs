namespace MDR_FuiPortal.Server.Controllers;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api")]
public class BaseApiController : ControllerBase
{
    /*
    protected ApiResponse<T> ListSuccessResponse<T>(int count, List<T> data)
    {
        return new ApiResponse<T>
        {
            Total = count,
            StatusCode = Ok().StatusCode,
            Messages = null,
            Data = data
        };
    }

    protected ApiResponse<T> SingleSuccessResponse<T>(List<T> data)
    {
        return new ApiResponse<T>
        {
            Total = 1,
            StatusCode = Ok().StatusCode,
            Messages = null,
            Data = data
        };
    }


    protected EmptyApiResponse DeletionSuccessResponse(int count, string attributeType,
                string parentId, string id)
    {
        string message;
        if (attributeType == "DTA" || attributeType == "DUA")
        {
            string parentType = attributeType.Replace('A', 'P');
            message = $"{attributeType} removed for {parentType} {parentId}";
        }
        else if (parentId == "")
        {
            message = (id == "") ? $"{attributeType} removed." : $"{attributeType} {id} removed.";
        }
        else
        {
            message = $"{attributeType} {parentId} :: {id} removed.";
        }
        return new EmptyApiResponse
        {
            Total = count,
            StatusCode = NoContent().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse NoParentResponse(string parentType, string idType, string id)
    {
        string message = $"Parent {parentType} with {idType} {id} was not found";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = NotFound().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse NoAttributesResponse(string attributeTypes)
    {
        var message = $"No {attributeTypes} were found";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = NotFound().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse NoEntityResponse(string entityType, string id)
    {
        var message = $"No {entityType} with id {id} was found";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = NotFound().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse NoParentAttResponse(string attributeType,
                        string parentType, string parentId, string id)
    {
        var message = $"No {attributeType} with id {id} was found for {parentType} {parentId}";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = NotFound().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse ExistingEntityResponse(string attributeType,
            string id)
    {
        var message = $"A {attributeType} with id {id} already exists";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = BadRequest().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse ErrorResponse(string errorContext,
                            string entityType, string parentType, string parentId, string id)
    {
        string message = "";
        switch (errorContext)
        {
            case "c":
                {
                    message = parentId == "" ? $"Error occured during creation of {entityType} {id}"
                        : $"Error occured during creation of {entityType} for {parentType} {parentId}";
                    break;
                }
            case "r":
                {
                    message = parentId == "" ? $"Error occured during fetch of {entityType} {id}"
                        : $"Error occured during fetch of {entityType} for {parentType} {parentId}";
                    break;
                }
            case "u":
                {
                    message = parentId == "" ? $"Error occured during update of {entityType} {id}"
                        : $"Error occured during update of {entityType} {parentId} :: {id}";
                    break;
                }
            case "d":
                {

                    message = parentId == "" ? $"Error occured during deletion of {entityType} {id}"
                    : $"Error occured during deletion of {entityType} {parentId} :: {id}";
                    break;
                }
        }
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = BadRequest().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse NoLupResponse(string typeName)
    {
        var message = $"No lookup values found using type name '{typeName}'.";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = NotFound().StatusCode,
            Messages = new List<string>() { message }
        };
    }

    protected EmptyApiResponse NoLupDecode(string typeName, string codeName)
    {
        var message = $"No matching values for code {codeName} found in look up type '{typeName}'.";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = NotFound().StatusCode,
            Messages = new List<string>() { message }

        };
    }

    protected EmptyApiResponse NoLupCode(string typeName, string decodeName)
    {
        var message = $"No matching values for decode {decodeName} found in look up type '{typeName}'.";
        return new EmptyApiResponse
        {
            Total = 0,
            StatusCode = NotFound().StatusCode,
            Messages = new List<string>() { message }
        };
    }
    */
}
