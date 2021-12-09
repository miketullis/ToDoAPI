using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models; //added for access to the DTO's
using ToDoAPI.DATA.EF; //added for access to the EF layer
using System.Web.Http.Cors; // added for access to modify the Cors functionality in this controller


namespace ToDoAPI.API.Controllers
{
    public class ToDoController : ApiController
    {
        //Create an object that will connect to the db
        ToDoEntities db = new ToDoEntities();

        //Read Functionality - api/ToDo - get all toDoItems from the ToDoItems table
        public IHttpActionResult GetToDos()
        {
            //Create a list of ToDoViewModel objects brought back from the db
            //Before we created the object below, we had to install EntityFramework in the Nuget Package Manager.
            List<ToDoViewModel> toDos = db.ToDoItems.Include("Category").Select(t => new ToDoViewModel()
            {
                //Assign the params of the ToDos coming from the db to the Data Transfer Object (ToDoViewModel)
                ToDoId = t.ToDoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).ToList();

            //Check on the results and handle accordingly below
            if (toDos.Count == 0)
            {
                return NotFound();//404 error
            }

            return Ok(toDos);//200 success code and we also pass resources into the Ok response
        }//end GetResources()



        //api/ToDoItems/id - Get request (READ)
        public IHttpActionResult GetToDo(int id)
        {
            //Create a ResourceViewModel object to collect the data from the db
            ToDoViewModel toDos = db.ToDoItems.Include("Category").Where(t => t.ToDoId == id).Select(t => new ToDoViewModel()
            {
                //assign values - copy this from the GetTodos
                ToDoId = t.ToDoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).FirstOrDefault();

            if (toDos == null)
                return NotFound();//scopeless if 

            return Ok(toDos);

        }//end GetResource()

        //Post - Create functionality
        //api/ToDoItems (HttpPost)
        public IHttpActionResult PostToDo(ToDoViewModel toDo)
        {
            if (!ModelState.IsValid)//checking the validation
            {
                return BadRequest("Invalid Data");
            }

            ToDoItem newToDo = new ToDoItem()
            {
                ToDoId = toDo.ToDoId,
                Action = toDo.Action,
                Done = toDo.Done,
                CategoryId = toDo.CategoryId
            };

            db.ToDoItems.Add(newToDo);
            db.SaveChanges();
            return Ok(newToDo);
        }//end PostToDo()

       
        //Put = Edit
        //api/ToDoItems (HttpPut)
        public IHttpActionResult PutToDo(ToDoViewModel toDo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //Capture the corresponding resource to edit from the db
            ToDoItem existingToDo = db.ToDoItems.Where(t => t.ToDoId == toDo.ToDoId).FirstOrDefault();

            if (existingToDo != null)
            {
                //Have the resource to edit
                existingToDo.ToDoId = toDo.ToDoId;
                existingToDo.Action = toDo.Action;
                existingToDo.Done = toDo.Done;
                existingToDo.CategoryId = toDo.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }//end else
        }//end PutResource()


        //api/ToDoItems/id (HttpDelete)
        public IHttpActionResult DeleteToDo(int id)
        {
            //Go and get the resource from the database
            ToDoItem toDo = db.ToDoItems.Where(t => t.ToDoId == id).FirstOrDefault();

            //If there is a resource with that id, delete the resource
            if (toDo != null)
            {
                db.ToDoItems.Remove(toDo);
                db.SaveChanges();
                return Ok();
            }
            //If not return 404
            else
            {
                return NotFound();
            }
        }//end Delete


        //We use the Dispose() below to dispose of any connections to the db after we are done with them. 
        //This is the best practice to handle performance - dispose of the instance of the controller and the db connection when finished.
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
