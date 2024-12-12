using QuestPDF.Fluent;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PhotoChef.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace PhotoChef.API.Services
{
    [Authorize]
    public class PdfGeneratorService
    {
        public byte[] GenerateRecipeBook(string title, string author, string coverImageUrl, List<Recipe> recipes, int userId)
        {
            return Document.Create(container =>
            {
                // Cover Page
                container.Page(page =>
                {
                    page.Margin(56.69f); // 2 cm margin
                    page.Content().Column(column =>
                    {
                        column.Item().Text(title).FontSize(40).Bold().AlignCenter();
                        column.Item().PaddingTop(56.69f); // 2 cm padding
                        var coverImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", userId.ToString(), Path.GetFileName(coverImageUrl));
                        if (!string.IsNullOrEmpty(coverImagePath) && File.Exists(coverImagePath))
                        {
                            column.Item().Image(coverImagePath).FitWidth();
                        }
                        column.Item().PaddingTop(113.38f); // 4 cm padding

                        column.Item().Text(author).FontSize(20).AlignCenter();
                    });

                });

                // Table of Contents
                container.Page(page =>
                {
                    page.Margin(56.69f); // 2 cm margin
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Table of Contents").FontSize(30).Bold().AlignCenter();
                        column.Item().PaddingTop(56.69f); // 2 cm padding
                        foreach (var (recipe, index) in recipes.Select((recipe, index) => (recipe, index)))
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text(recipe.Name).FontSize(16);
                                row.ConstantItem(50).AlignRight().Text($"{index + 3}").FontSize(16); // Adjust page number based on content
                            });
                        }
                    });
                });

                // Recipes
                foreach (var recipe in recipes)
                {
                    container.Page(page =>
                    {
                        page.Margin(56.69f); // 2 cm margin
                        page.Content().Column(column =>
                        {
                            column.Item().Text(recipe.Name).FontSize(24).Bold().AlignCenter();
                            column.Item().PaddingTop(56.69f); // 2 cm padding

                            column.Item().Row(row =>
                            {
                                // Image in the first column
                                if (!string.IsNullOrEmpty(recipe.ImageUrl))
                                {
                                    var imagePath = Path.Combine(
                                        Directory.GetCurrentDirectory(),
                                        "wwwroot",
                                        "images",
                                        userId.ToString(),
                                        Path.GetFileName(recipe.ImageUrl)
                                    );
                                    if (File.Exists(imagePath))
                                    {
                                        row.RelativeItem(1).Image(imagePath).FitWidth();
                                    }
                                }

                                // Ingredients and allergens in the second column
                                row.RelativeItem(1).Column(col =>
                                {
                                    if (recipe.Ingredients.Any())
                                    {
                                        col.Item().Text("Ingredients:").FontSize(16).Bold().AlignCenter();
                                        foreach (var ingredient in recipe.Ingredients)
                                        {
                                            col.Item().Text($"{ingredient.Name}: {ingredient.Quantity}").FontSize(14).AlignCenter();
                                        }
                                    }

                                    if (recipe.Allergens.Any())
                                    {
                                        col.Item().PaddingTop(20).Text("Allergens:").FontSize(16).Bold().AlignCenter();
                                        col.Item().Text(string.Join(", ", recipe.Allergens)).FontSize(14).AlignCenter();
                                    }
                                });
                            });

                            column.Item().PaddingTop(56.69f).Text("Instructions:").FontSize(16).Bold(); // 2 cm padding
                            column.Item().Text(recipe.Instructions).FontSize(14);
                        });
                    });
                }
            }).GeneratePdf();

        }

    }
}
