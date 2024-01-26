using Microsoft.AspNetCore.Mvc;
using Npgsql;
using c_sharp_lab_15_1.Models;

namespace c_sharp_lab_15_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodController : ControllerBase
    {
        [HttpGet]
        [Route("select")]
        public IActionResult Select()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            List<GoodModel> clubs = new List<GoodModel>();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("select * from good", connection);
                var firstReader = command.ExecuteReader();
                while (firstReader.Read())
                {
                    var club = new GoodModel
                    {
                        GoodId = firstReader.GetInt32(0),
                        Name = firstReader.GetString(1),
                        Photo = firstReader.IsDBNull(2) ? null : firstReader.GetFieldValue<byte[]>(2),
                        SupplierName = firstReader.IsDBNull(3) ? null : firstReader.GetString(3),
                    };
                    clubs.Add(club);
                }
                connection.Close();
            }
            return Ok(clubs);
        }

        [HttpPost]
        [Route("insert")]
        public IActionResult Insert(IFormFile photo, string name, string supplier_name)
        {
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                photo.CopyTo(memoryStream);
                imageBytes = memoryStream.ToArray();
            }
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("insert into good (name, supplier_name, photo) values (@name, @supplier_name, @photo)", connection);
                command.Parameters.AddWithValue("name", name);
                command.Parameters.AddWithValue("supplier_name", supplier_name);
                command.Parameters.AddWithValue("photo", imageBytes);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return Ok(true);
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Update(int id, IFormFile photo, string supplier_name, string name, int positionid, int experience)
        {
            try
            {
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    photo.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("update good set name = @name, supplier_name = @supplier_name, photo = @photo where id = @id", connection);
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("name", name);
                    command.Parameters.AddWithValue("supplier_name", supplier_name);
                    command.Parameters.AddWithValue("photo", imageBytes);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("delete from good where id = @id", connection);
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }
    }
}
