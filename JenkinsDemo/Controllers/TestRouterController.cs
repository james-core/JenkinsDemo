using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JenkinsDemo.Controllers
{
    [ApiController]
    [Route("api/TestRouter")]
    public class TestRouterController : ControllerBase
    {
        /// <summary>
        /// Get方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("funGet")]
        public List<string> Get()
        {
            return new List<string>() { "this is get" };
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("funPost")]
        public List<string> Post()
        {
            return new List<string>() { "this is post" };
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updateInfo")]
        public string UpdateInfo()
        {
            return "1";
        }

        /// <summary>
        /// Put方法
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("funPut")]
        public List<string> Put()
        {
            return new List<string>() { "this is put" };
        }

        /// <summary>
        /// Delete方法
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("funPut")]
        public List<string> Delete()
        {
            return new List<string>() { "this is delete" };
        }

    }
}
