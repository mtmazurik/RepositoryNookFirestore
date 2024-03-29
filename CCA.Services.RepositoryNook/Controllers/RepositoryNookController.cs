﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CCA.Services.RepositoryNook.HelperClasses;
using CCA.Services.RepositoryNook.Models;
using CCA.Services.RepositoryNook.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CCA.Services.RepositoryNook.Controllers
{
    [Route("/")]
    public class RepositoryNookController : Controller
    {
        //[HttpGet]   // GET all databases
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> GetDatabases([FromServices]IRepositoryService repositoryService)
        //{
        //    try
        //    {
        //        List<string> found = await repositoryService.GetDatabases();
        //        return ResponseFormatter.ResponseOK(found);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Get databases failed.");
        //    }
        //}
        //[HttpGet("{database}")]   // GET all collections
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> GetCollections([FromServices]IRepositoryService repositoryService, string database)
        //{
        //    try
        //    {
        //        List<string> found = await repositoryService.GetCollections(database);
        //        return ResponseFormatter.ResponseOK(found);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Get collections failed.");
        //    }
        //}
        [HttpPost("{collection}")]  // POST (C)reate Repository object - CRUD operation: Create
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, [FromBody]Repository repoObject)
        {
            try
            {
                return ResponseFormatter.ResponseOK(await repositoryService.Create(string.Empty, collection, repoObject), "Created");
            }
            catch(Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Create failed.");
            }

        }
        //[HttpGet("{database}/{collection}/{_id}")]   // GET Repository object-by-id (Query by Id)
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> GetRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string _id)
        //{
        //    try
        //    {
        //        Repository found = await repositoryService.Read(_id, database, collection);

        //        return ResponseFormatter.ResponseOK(found);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Read failed.");
        //    }

        //}
        //[HttpGet("{database}/{collection}/repository")]   // GET All Repository objects (Query by "*" wildcard operation, or default: all records API call)
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> GetAllRepositoryObjects([FromServices]IRepositoryService repositoryService, string database, string collection, string _id)
        //{
        //    try
        //    {
        //        List<Repository> found = repositoryService.ReadAll(database, collection);

        //        return ResponseFormatter.ResponseOK(found);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Read All failed.");
        //    }

        //}
        //[HttpGet("{database}/{collection}/key")]   // query by keyName = keyValue
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> QueryByKeyRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string keyName, string keyValue)
        //{
        //    try
        //    {
        //        List<Repository> found = repositoryService.QueryByKey(database, collection, keyName, keyValue);

        //        return ResponseFormatter.ResponseOK(found);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Query failed.");
        //    }

        //}
        //[HttpGet("{database}/{collection}/tag")]   // query by tagName = tagValue
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> QueryByTagRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string tagName, string tagValue)
        //{
        //    try
        //    {
        //        List<Repository> found = repositoryService.QueryByTag(database, collection, tagName, tagValue);

        //        return ResponseFormatter.ResponseOK(found);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Query failed.");
        //    }

        //}
        //[HttpPut("{database}/{collection}/{_id}")]  // update
        ////[AllowAnonymous]    // allow anonymous as Tier 2, and API manager/gateway handle auth otherwise - we'll omit middleware from the Microservice API methods (for now)
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> UpdateRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string _id, [FromBody]Repository repoObject)
        //{
        //    try
        //    {
        //        await repositoryService.Update(_id, database, collection, repoObject);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Update failed.");
        //    }
        //    try
        //    {
        //        Repository found = await repositoryService.Read(_id, database, collection);

        //        return ResponseFormatter.ResponseOK(found, "Updated");
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Retreiving Update failed. Record may still have been written.");
        //    }

        //}
        //[HttpDelete("{database}/{collection}/{_id}")]    // delete
        ////[AllowAnonymous]
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> DeleteRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string _id, [FromBody]Repository repoObject)
        //{
        //    try
        //    {
        //        await repositoryService.Delete(_id, database, collection);

        //        return ResponseFormatter.ResponseOK($"_id: {_id} deleted.");
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, $"Delete failed for _id: {_id}.");
        //    }

        //}
    }
}
