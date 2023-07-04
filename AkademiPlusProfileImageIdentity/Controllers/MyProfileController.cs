using AkademiPlusProfileImageIdentity.DAL;
using AkademiPlusProfileImageIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AkademiPlusProfileImageIdentity.Controllers
{
    public class MyProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public MyProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditViewModel userEditViewModel = new UserEditViewModel();
            userEditViewModel.Name= values.Name;
            userEditViewModel.Email= values.Email;
            userEditViewModel.Surname= values.Surname;
            userEditViewModel.PhoneNumber= values.PhoneNumber;
            userEditViewModel.City= values.City;
            userEditViewModel.Username = values.UserName;
            userEditViewModel.ImageUrl = values.ImageUrl;
            return View(userEditViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserEditViewModel p)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(p.ImageFile!=null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(p.ImageFile.FileName);
                var imagename = Guid.NewGuid() + extension;
                var saveLocation = resource + "/wwwroot/userimages/" + imagename;
                var stream=new FileStream(saveLocation, FileMode.Create);
                await p.ImageFile.CopyToAsync(stream);
                user.ImageUrl=imagename;
            }
            user.Name= p.Name;
            user.Email= p.Email;
            user.City= p.City;
            user.Surname = p.Surname;
            user.PhoneNumber= p.PhoneNumber;
            user.PasswordHash=_userManager.PasswordHasher.HashPassword(user,p.Password);
            var result=await _userManager.UpdateAsync(user);
            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
