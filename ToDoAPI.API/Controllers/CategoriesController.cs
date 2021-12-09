using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//Add the following using statements
using ToDoAPI.API.Models;//Access to the DTO's
using ToDoAPI.DATA.EF;//Access to the EF layer
using System.Web.Http.Cors;//Added to access and modify the CORS (Cross Origin Resource Sharing)

namespace ResourceAPI.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CategoriesController : ApiController
    {
        ToDoEntities db = new ToDoEntities();

        //api/Categories (HttpGet)
        public IHttpActionResult GetCategories()
        {
            List<CategoryViewModel> cats = db.Categories.Select(c => new CategoryViewModel()
            {
                //Assign the values from the db to a new CategoryViewModel object for each entry in the db.
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            if (cats.Count == 0)
                return NotFound();

            return Ok(cats);
        }//end GetCategories()

        //api/categories/id (HttpGet)
        public IHttpActionResult GetCategory(int id)
        {
            CategoryViewModel cat = db.Categories.Where(c => c.CategoryId == id).Select(c => new CategoryViewModel() {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).FirstOrDefault();

            if (cat == null)
            {
                return NotFound();
            }

            return Ok(cat);
        }//end GetCategory()

        //api/Categories (HttpPost)
        public IHttpActionResult PostCategory(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            db.Categories.Add(new Category() {
                Name = cat.Name,
                Description = cat.Description
            });

            db.SaveChanges();
            return Ok();

        }//end PostCategory()


        //api/Categories (HttpPut)
        public IHttpActionResult PutCategory(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            Category existingCategory = db.Categories.Where(c => c.CategoryId == cat.CategoryId).FirstOrDefault();

            if (existingCategory != null)
            {
                existingCategory.Name = cat.Name;
                existingCategory.Description = cat.Description;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//End PutCategory()

        //api/categories/id (HttpDelete)
        public IHttpActionResult DeleteCategory(int id)
        {
            Category cat = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();

            if (cat != null)
            {
                db.Categories.Remove(cat);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteCategory()

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }//end class
}//end namespace
