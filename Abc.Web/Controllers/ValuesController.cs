using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Abc.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private CLogger.ILogger _loggerService;
        private string _source = "Abc.Web";

        public ValuesController(CLogger.ILogger loggerService)
        {
            _loggerService = loggerService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var info = $"record at {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
            var result = "{source} ValuesController.Get(). This is a test log {info} year {year}";
            _loggerService.LogInformation(CLogger.LogLevels.Debug, result, null, new object[] { _source, info, DateTime.Now.Year });


            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
