using FluentAssertions;
using PhotoChef.API.Services;
using PhotoChef.Domain.Entities;
using QuestPDF.Infrastructure;
using Xunit;

namespace PhotoChef.Tests.Services
{
    public class PdfGeneratorServiceTests
    {
        private readonly PdfGeneratorService _pdfGeneratorService;

        public PdfGeneratorServiceTests()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _pdfGeneratorService = new PdfGeneratorService();
        }

        [Fact]
        public void GenerateRecipeBook_ShouldReturnPdfBytes_WhenValidDataProvided()
        {
            var testImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "testing.png");
            var recipes = new List<Recipe>
            {
                new Recipe { Name = "Test Recipe 1", Instructions = "Step 1, Step 2", ImageUrl=testImagePath,
                    Description="Description Recipe 1",Allergens= new List<Domain.Enums.Allergen>(){ Domain.Enums.Allergen.Celery},
                    Ingredients =  new List<Ingredient>(){ new Ingredient() {Name= "Ingredient 1", Quantity="Qty 1" }  }
                },
                new Recipe { Name = "Test Recipe 2", Instructions = "Step A, Step B", ImageUrl =testImagePath,
                    Description="Description Recipe 2",Allergens= new List<Domain.Enums.Allergen>(){ Domain.Enums.Allergen.Fish},
                    Ingredients =  new List<Ingredient>(){ new Ingredient() {Name= "Ingredient 2", Quantity="Qty 2" }  }
                }
            };
            
            var pdfBytes = _pdfGeneratorService.GenerateRecipeBook("Test Book", "Test Author", testImagePath, recipes, 1);

            pdfBytes.Should().NotBeNull();
            pdfBytes.Length.Should().BeGreaterThan(0);
        }
    }
}
