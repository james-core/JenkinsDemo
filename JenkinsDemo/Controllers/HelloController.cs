using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JenkinsDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        /// <summary>
        /// Get方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<string> Get()
        {
            return new List<string>() { "this is get" };
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<string> Post()
        {
            return new List<string>() { "this is post" };
        }

        /// <summary>
        /// Put方法
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public List<string> Put()
        {
            return new List<string>() { "this is put" };
        }

        /// <summary>
        /// Delete方法
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public List<string> Delete(string id)
        {
            return new List<string>() { "this is delete" };
        }

        // this is dev region

        [HttpPost("{id}")]   // GET /api/test2/xyz
        public bool update(string id)
        {
            return true;
        }

        // this is hotfix region


    }
}
