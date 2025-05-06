using Microsoft.AspNetCore.Mvc;
using Syncfusion.Presentation;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;

            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        [Route("/")]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            var villaList = await _unitOfWork.Villa.GetAll(includeNavigationProperties: "VillaAmenities");

            HomeViewModel homeVM = new()
            {
                VillaList = villaList,

                NumberOfNights = 1,

                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(homeVM);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetVillasByDate(int nights, DateOnly checkInDate)
        {
            var villaList = await _unitOfWork.Villa
                                             .GetAll(includeNavigationProperties: "VillaAmenities");

            var villaNumbersList = await _unitOfWork.VillaNumber.GetAll();

            var bookedVillas = await _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved || 
            u.Status == SD.StatusCheckedIn);

            foreach (var villa in villaList)
            {
                int roomAvailable = SD.VillaRoomsAvailable_Count(villa.Id,
                    villaNumbersList.ToList(), checkInDate, nights, bookedVillas.ToList());

                villa.IsAvailable = roomAvailable > 0 ? true : false;
            }

            HomeViewModel homeViewModel = new()
            {
                CheckInDate = checkInDate,
                VillaList = villaList,
                NumberOfNights = nights
            };

            return PartialView("_VillaList", homeViewModel);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GeneratePPTExport(int id)
        {
            var villa = (await _unitOfWork.Villa
                                         .GetAll(includeNavigationProperties: "VillaAmenities"))
                                         .FirstOrDefault(x => x.Id == id);
            if (villa is null)
            {
                return RedirectToAction(nameof(Error));
            }

            string basePath = _webHostEnvironment.WebRootPath;
            string filePath = basePath + @"/Exports/ExportVillaDetails.pptx";


            using IPresentation presentation = Presentation.Open(filePath);

            ISlide slide = presentation.Slides[0];


            IShape? shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaName") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = villa.Name;
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaDescription") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = villa.Description;
            }


            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtOccupancy") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Max Occupancy : {0} adults", villa.Occupancy);
            }
            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaSize") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Villa Size: {0} sqft", villa.Sqft);
            }
            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtPricePerNight") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("USD {0}/night", villa.Price.ToString("C"));
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaAmenitiesHeading") as IShape;
            
            if (shape is not null)
            {
                List<string> listItems = villa.VillaAmenities.Select(x => x.Name).ToList();

                shape.TextBody.Text = "";

                foreach (var item in listItems)
                {
                    IParagraph paragraph = shape.TextBody.AddParagraph();
                    ITextPart textPart = paragraph.AddTextPart(item);

                    paragraph.ListFormat.Type = ListType.Bulleted;
                    paragraph.ListFormat.BulletCharacter = '\u2022';
                    textPart.Font.FontName = "system-ui";
                    textPart.Font.FontSize = 18;
                    textPart.Font.Color = ColorObject.FromArgb(144, 148, 152);
                }
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "imgVilla") as IShape;
            
            if (shape is not null)
            {
                byte[] imageData;
                string imageUrl;
                try
                {
                    imageUrl = string.Format("{0}{1}", basePath, villa.ImageUrl);
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                catch (Exception)
                {
                    imageUrl = string.Format("{0}{1}", basePath, "/images/placeholder.png");
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                slide.Shapes.Remove(shape);
                using MemoryStream imageStream = new(imageData);
                IPicture newPicture = slide.Pictures.AddPicture(imageStream, 60, 120, 300, 200);
            }

            MemoryStream memoryStream = new();

            presentation.Save(memoryStream);

            memoryStream.Position = 0;

            return File(memoryStream, "application/pptx", "villa.pptx");
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
