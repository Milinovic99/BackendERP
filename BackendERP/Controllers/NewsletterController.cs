using AutoMapper;
using BackendERP.Data;
using BackendERP.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendERP.Controllers
{
    [ApiController]
    [Route("api/newsletter")]
    [Produces("application/json")]
    public class NewsletterController : ControllerBase
    {
        private readonly INewsletterRepository assortmentPricelistRepository;
        private readonly LinkGenerator linkGenerator; //Služi za generisanje putanje do neke akcije 
        private readonly IMapper mapper;


        public NewsletterController(INewsletterRepository assortmentPricelistRepository, LinkGenerator linkGenerator, IMapper mapper)
        {
            this.assortmentPricelistRepository = assortmentPricelistRepository;
            this.linkGenerator = linkGenerator;
            this.mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        [Consumes("application/json")]
        public ActionResult<List<Newsletter>> GetNewsletters()
        {
            //   string token = Request.Headers["token"].ToString();
            //  string[] split = token.Split('#');
            // if (split[1] != "administrator")
            //   return Unauthorized();
            // }
            //HttpStatusCode res = fileService.AuthorizeAsync(token).Result;
            //if (res.ToString() != "OK")
            // {
            //    return Unauthorized();
            //}

            var delivery = assortmentPricelistRepository.GetNewsletters();
            if (delivery == null || delivery.Count == 0)
            {
                return NoContent();
            }

            return Ok(mapper.Map<List<Newsletter>>(delivery));
        }

        [HttpGet("{Newsletter_id}")]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<Newsletter> GetNewsletter(int Assortment_id)
        {
            //   string token = Request.Headers["token"].ToString();
            //  string[] split = token.Split('#');
            // if (split[1] != "administrator" )
            //{ 
            //   return Unauthorized();
            //}
            //HttpStatusCode res = fileService.AuthorizeAsync(token).Result;
            //if (res.ToString() != "OK")
            //{
            //   return Unauthorized();
            //}
            var delivery = assortmentPricelistRepository.GetNewsletterById(Assortment_id);
            if (delivery == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<Newsletter>(delivery));
        }


        [HttpPost]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public ActionResult<Newsletter> CreateNewsletter([FromQuery] Newsletter delivery)
        {
            //  string token = Request.Headers["token"].ToString();
            // string[] split = token.Split('#');
            //if (split[1] != "administrator" )
            //{ 
            //   return Unauthorized();
            //}
            //HttpStatusCode res = fileService.AuthorizeAsync(token).Result;
            //if (res.ToString() != "OK")
            //{
            //   return Unauthorized();
            //}
            try
            {
                Newsletter pro = assortmentPricelistRepository.CreateNewsletter(delivery);
                assortmentPricelistRepository.SaveChanges();
                // Dobar API treba da vrati lokator gde se taj resurs nalazi
                // string location = linkGenerator.GetPathByAction("GetDeliveryData", "Delivery_data", new { delivery = pro.Delivery_id });
                return StatusCode(StatusCodes.Status200OK, pro);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Create Error");
            }
        }

        [HttpDelete("{Newsletter_id}")]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult DeleteNewsletter(int Assortment_id)
        {
            //    string token = Request.Headers["token"].ToString();
            //   string[] split = token.Split('#');
            //  if (split[1] != "administrator")
            // { 
            //    return Unauthorized();
            //}
            //HttpStatusCode res = fileService.AuthorizeAsync(token).Result;
            //if (res.ToString() != "OK")
            // {
            //    return Unauthorized();
            //}

            try
            {
                var productModel = assortmentPricelistRepository.GetNewsletterById(Assortment_id);
                if (productModel == null)
                {
                    return NotFound();
                }
                assortmentPricelistRepository.DeleteNewsletter(Assortment_id);
                assortmentPricelistRepository.SaveChanges();
                // Status iz familije 2xx koji se koristi kada se ne vraca nikakav objekat, ali naglasava da je sve u redu

                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Delete Error");
            }
        }

        [HttpPut]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public ActionResult<Newsletter> UpdateNewsletter(Newsletter delivery)
        {
            //   string token = Request.Headers["token"].ToString();
            //  string[] split = token.Split('#');
            // if (split[1] != "administrator" || split[1] != "menadzer" || split[1] != "licitant"
            //    || split[1] != "tehnicki sektetar" || split[1] != "prva komisija" || split[1] != "operator nadmetanja")
            //{
            //   return Unauthorized();
            //}
            //HttpStatusCode res = fileService.AuthorizeAsync(token).Result;
            //if (res.ToString() != "OK")
            //{
            //   return Unauthorized();
            //}
            try
            {
                var complaintCheck = assortmentPricelistRepository.GetNewsletterById(delivery.Newsletter_id);
                //Proveriti da li uopšte postoji prijava koju pokušavamo da ažuriramo.
                if (complaintCheck == null)
                {
                    return NotFound();
                }
                mapper.Map(delivery, complaintCheck);
                assortmentPricelistRepository.SaveChanges();
                return Ok(mapper.Map<Newsletter>(complaintCheck));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Update error");
            }
        }
    }



}
