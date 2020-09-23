using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalksRepository _walksRepo;
        private readonly IOwnerRepository _ownerRepo;
        private readonly INeighborhoodRepository _neighborhoodRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkersController(IWalkerRepository walkerRepository, IWalksRepository walksRepository, IOwnerRepository ownerRepository, INeighborhoodRepository neighborhoodRepository)
        {
            _walkerRepo = walkerRepository;
            _walksRepo = walksRepository;
            _ownerRepo = ownerRepository;
            _neighborhoodRepo = neighborhoodRepository;
        }

        // GET: Walkers
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();

            // If no owner logged-in return list of all walkers otherwise return list of walkers in owner's neighborhood
            if (ownerId == 0)
            {
                List<Walker> walker = _walkerRepo.GetAllWalkers();

                return View(walker);
            }

            // Get owner object with owner id to access neighborhoodId
            Owner owner = _ownerRepo.GetOwnerById(ownerId);

            List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);

            return View(walkers);
        }

        // GET: Walkers/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walks> walks = _walksRepo.GetWalksByWalkerId(id);

            if (walker == null)
            {
                return NotFound();
            }

            WalkerProfileViewModel vm = new WalkerProfileViewModel()
            {
                Walker = walker,
                Walks = walks
            };

            return View(vm);
        }

        // GET: Walkers/Create
        public ActionResult Create()
        {
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAllNeighborhoods();

            WalkerFormViewModel vm = new WalkerFormViewModel()
            {
                Walker = new Walker(),
                Neighborhoods = neighborhoods
            };

            return View(vm);
        }

        // POST: Walkers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Walker walker)
        {
            try
            {
                _walkerRepo.AddWalker(walker);

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                WalkerFormViewModel vm = new WalkerFormViewModel()
                {
                    Walker = walker,
                    Neighborhoods = _neighborhoodRepo.GetAllNeighborhoods()
                };

                return View(vm);
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // Method to get logged-in owner's id
        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If id is null return int value 0
            if (id == null)
            {
                return 0;
            }

            return int.Parse(id);
        }
    }
}
