﻿using ColegioTerciario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using ColegioTerciario.Areas.Admin.Models;
using ColegioTerciario.Models.Repositories;
using System.Net;

namespace ColegioTerciario.Areas.Admin.Controllers
{
    public class UsuariosController : AdminController
    {
        private UserRepository _repo;
        private ColegioTerciarioContext context;
        public UsuariosController()
        {
            context = GetContext();
            _repo = new UserRepository();
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/Seguridad
        public ActionResult Index()
        {
            List<UserViewModel> usuarios = new List<UserViewModel>();

            var users = UserManager.Users.ToList();

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);



            foreach (ApplicationUser user in users)
            {
                var roles = userManager.GetRoles(user.Id);
                string persona;
                if (user.USER_PERSONA_ID != null)
                {
                    persona = context.Personas.Find(user.USER_PERSONA_ID).PERSONA_NOMBRE_COMPLETO;
                }
                else
                {
                    persona = "";
                }
                
                usuarios.Add(new UserViewModel()
                {
                    Id = user.Id,
                    RoleName = string.Join(", ", roles),
                    UserName = user.UserName,
                    Email = user.Email,
                    Persona = persona
                });

            }

            ViewBag.error = Session["error"];
            return View(usuarios.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.ALUMNOS = new SelectList(new PersonasRepository().GetAlumnos(), "ID", "PERSONA_NOMBRE_COMPLETO");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewUserViewModel newUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var repo = new UserRepository();
                if (repo.GetUserByPersonaId(newUserViewModel.USER_PERSONA_ID) != null)
                {
                    Session["error"] = "Ya existe un usuario con esa persona asignada";
                    return RedirectToAction("Index");
                }
                var user = repo.CreateUser(newUserViewModel);
                return RedirectToAction("Index");
            }
            ViewBag.ALUMNOS = new SelectList(new PersonasRepository().GetAlumnos(), "ID", "PERSONA_NOMBRE_COMPLETO");
            return View(newUserViewModel); 

        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ALUMNOS = new SelectList(new PersonasRepository().GetAlumnos(), "ID", "PERSONA_NOMBRE_COMPLETO");
            ApplicationUser user = _repo.GetUser(id);
            EditUserViewModel vm = new EditUserViewModel
            {
                Email = user.Email,
                USER_PERSONA_ID = user.USER_PERSONA_ID.Value
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel vm)
        {
            ViewBag.ALUMNOS = new SelectList(new PersonasRepository().GetAlumnos(), "ID", "PERSONA_NOMBRE_COMPLETO");
            if (ModelState.IsValid)
            {
                var db = new ColegioTerciarioContext();
                var usuario = db.Users.Find(vm.ID);
                if (usuario.Email != vm.Email)
                {
                    usuario.Email = vm.Email;
                    usuario.UserName = vm.Email;
                };
                usuario.USER_PERSONA_ID = vm.USER_PERSONA_ID;

                db.SaveChanges();

                if (vm.Password != null)
                {
                    UserManager.RemovePassword(usuario.Id);
                    var result = UserManager.AddPassword(usuario.Id, vm.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    
                    AddErrors(result);
                    return View(vm);
                }
                

                return RedirectToAction("Index");
            }
            else
            {
                
                return View(vm);
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}