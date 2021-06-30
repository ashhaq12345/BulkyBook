﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id != null)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Id", id);
                coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);

                if (coverType == null)
                    return NotFound();
            }
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Name", coverType.Name);
                if (coverType.Id == 0)
                {
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameters);

                }
                else
                {
                    parameters.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameters);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = allObj });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var objFromDb = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete successful" });
            }


        }


        #endregion
    }
}
