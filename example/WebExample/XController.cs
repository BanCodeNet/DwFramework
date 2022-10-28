using DwFramework.Web.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebExample
{
    [ApiController]
    [Route("x")]
    public class XController : Controller
    {
        public XController()
        {
        }

        [HttpGet("t1")]
        public IActionResult T1()
        {
            return Ok(JwtManager.Generate("dwgoing", new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dsfjoihnoisdhf823b4iu834h"))));
        }

        [HttpGet("t2")]
        public IActionResult T2(string token)
        {
            return Ok(JwtManager.Decode(token));
        }
    }
}