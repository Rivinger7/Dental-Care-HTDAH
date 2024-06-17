﻿//=======================================QUẢN LÝ CHUYÊN KHOA=======================================

using Dental_Clinic_System.Areas.Admin.Models;
using Dental_Clinic_System.Areas.Admin.ViewModels;
using Dental_Clinic_System.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;

namespace Dental_Clinic_System.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/[controller]")]
	public class ManagerSpecialtyController : Controller
	{
		private readonly DentalClinicDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly HiddenSpecialtyService _hiddenSpecialty;

		public ManagerSpecialtyController(DentalClinicDbContext context, HiddenSpecialtyService hiddenSpecialty, IWebHostEnvironment iwebhostenvironment)
		{
			_context = context;
			_hiddenSpecialty = hiddenSpecialty;
			_webHostEnvironment = iwebhostenvironment;
		}

		//===================LIST CHUYÊN KHOA===================
		[Route("ListSpecialty")]
		public async Task<IActionResult> ListSpecialty()
		{
			var hiddenSpecialties = _hiddenSpecialty.GetHiddenSpecialty();

			var specialty = await _context.Specialties
				.Where(s => !hiddenSpecialties.Contains(s.ID))
				.ToListAsync();

			var specialtyList = specialty.Select(s => new ManagerSpecialtyVM
			{
				Id = s.ID,
				Name = s.Name,
				Image = s.Image,
				Description = s.Description,
				Deposit = s.Deposit
			}).ToList();

			return View("ListSpecialty", specialtyList);
		}

		//===================TÌM KIẾM===================
		[Route("SeachSpecialty")]
		public async Task<IActionResult> SeachSpecialty(string search)
		{
			var hiddenSpecialties = _hiddenSpecialty.GetHiddenSpecialty();

			var specialty = await _context.Specialties
				.Where(s => !hiddenSpecialties.Contains(s.ID) && (string.IsNullOrEmpty(search) || s.Name.Contains(search)))
				.ToListAsync();

			var specialtyList = specialty.Select(s => new ManagerSpecialtyVM
			{
				Id = s.ID,
				Name = s.Name,
				Image = s.Image,
				Description = s.Description,
				Deposit = s.Deposit
			}).ToList();

			return View(nameof(ListSpecialty), specialtyList);
		}

		//===================CHỈNH SỬA===================
		[HttpGet]
		[Route("EditSpecialty/{id}")]
		public async Task<IActionResult> EditSpecialty(int id)
		{
			var specialty = await _context.Specialties.FindAsync(id);
			if (specialty == null)
			{
				return NotFound();
			}

			var viewModel = new ManagerSpecialtyVM
			{
				Id = specialty.ID,
				Name = specialty.Name,
				Image = specialty.Image,
				Description = specialty.Description,
				Deposit = specialty.Deposit
			};

			return View(viewModel);
		}

		[HttpPost]
		[Route("EditSpecialty")]
		public async Task<IActionResult> EditSpecialty(ManagerSpecialtyVM model)
		{
			if (ModelState.IsValid)
			{
				var specialty = await _context.Specialties.FindAsync(model.Id);

				if (specialty == null)
				{
					return NotFound();
				}

				// Kiểm tra tên chuyên khoa có bị trùng không
				var isDuplicateName = await _context.Specialties
					.FirstOrDefaultAsync(s => s.Name == model.Name);
				if(isDuplicateName != null)
				{
                    ModelState.AddModelError("Name", "Tên chuyên khoa đã tồn tại.");
					return View("EditSpecialty", model);
				}

                specialty.Name = model.Name;
				specialty.Description = model.Description;
				specialty.Deposit = model.Deposit;
				specialty.Image = model.Image;

				_context.Specialties.Update(specialty);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(ListSpecialty));
			}

			return View("EditSpecialty", model);
		}

		//===================XÓA CHUYÊN KHOA TẠM THỜI===================
		[Route("DeleteSpecialty/{id}")]
		public IActionResult DeleteSpecialty(int id)
		{
			_hiddenSpecialty.HiddenSpecialty(id);
			return RedirectToAction(nameof(ListSpecialty));
		}

		//===================THÊM CHUYÊN KHOA===================
		[HttpPost]
		[Route("AddSpecialty")]
		public async Task<IActionResult> AddSpecialty(string name, string description, string imageUrl, decimal deposit)
		{
			//Check Tên chuyên khoa có bị trùng không
			var existingName = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == name);

			if (existingName != null)
			{
				//Thấy tên bị trùng, thông báo lỗi
				ModelState.AddModelError("Name", "Tên chuyên khoa đã tồn tại. Vui lòng chọn tên khác.");

				//Lấy lại list specialty để hiển thị
				var specialties = await _context.Specialties.ToListAsync();

				var listSpecialty = specialties.Select(s => new ManagerSpecialtyVM
				{
					Id = s.ID,
					Name = s.Name,
					Image = s.Image,
					Description = s.Description,
					Deposit = s.Deposit
				}).ToList();

				return View(nameof(ListSpecialty), listSpecialty);
			}

			//Check Deposis không được âm
			if (deposit <= 0)
			{
				//Thấy Tiền cọc âm, thông báo lỗi
				ModelState.AddModelError("Deposit", "Tiền cọc phải lớn hơn 0!!!");

				//Lấy lại list specialty để hiển thị
				var specialties = await _context.Specialties.ToListAsync();
				var specialtyList = specialties.Select(s => new ManagerSpecialtyVM
				{
					Id = s.ID,
					Name = s.Name,
					Image = s.Image,
					Description = s.Description,
					Deposit = s.Deposit
				}).ToList();

				return View(nameof(ListSpecialty), specialtyList);
			}

			var specialty = new Specialty
			{
				Name = name,
				Description = description,
				Image = imageUrl,
				Deposit = deposit
			};

			_context.Specialties.Add(specialty);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(ListSpecialty));
		}
	}
}


//KIẾM TRA LỖI
//List<string> errors = new List<string>();
//foreach (var value in ModelState.Values)
//{
//	foreach (var error in value.Errors)
//	{
//		errors.Add(error.ErrorMessage);
//	}
//}
//string errorMessage = string.Join("\n", errors);
//return BadRequest(errorMessage);