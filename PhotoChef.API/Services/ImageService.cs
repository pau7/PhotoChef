namespace PhotoChef.API.Services
{
    public class ImageService
    {
        private readonly string _baseUploadsFolder;

        public ImageService(IWebHostEnvironment environment)
        {
            _baseUploadsFolder = Path.Combine(environment.WebRootPath, "images");
            if (!Directory.Exists(_baseUploadsFolder))
            {
                Directory.CreateDirectory(_baseUploadsFolder);
            }
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, int userId)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png" };

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var contentType = imageFile.ContentType.ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
            }

            if (!allowedContentTypes.Contains(contentType))
            {
                throw new InvalidOperationException("Invalid file type. Only JPEG and PNG are allowed.");
            }

            if (imageFile.Length > 5 * 1024 * 1024) // 5MB limit
            {
                throw new InvalidOperationException("File size exceeds the 5MB limit.");
            }

            // Crear la carpeta del usuario si no existe
            var userFolder = Path.Combine(_baseUploadsFolder, userId.ToString());
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(userFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Devuelve la URL relativa
            return $"/images/{userId}/{fileName}";
        }

        public void DeleteImage(string imageUrl)
        {
            var filePath = Path.Combine(_baseUploadsFolder, imageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                throw new FileNotFoundException("Image not found.");
            }
        }
    }


}
