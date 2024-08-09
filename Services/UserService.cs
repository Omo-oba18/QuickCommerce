using MongoDB.Bson;
using MongoDB.Driver;
using QuickCommerce.Models;

namespace QuickCommerce.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User> RegisterUser(User user)
        {
            // Validate email uniqueness
            var existingUser = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new Exception("Email already in use.");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User> AuthenticateUser(LoginRequest request)
        {
            var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User> GetUserById(ObjectId id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateUser(ObjectId id, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            var update = Builders<User>.Update
                .Set(user => user.Username, updatedUser.Username)
                .Set(user => user.Email, updatedUser.Email)
                .Set(user => user.PasswordHash, updatedUser.PasswordHash)
                .Set(user => user.PhoneNumber, updatedUser.PhoneNumber)
                .Set(user => user.ProfilePicture, updatedUser.ProfilePicture)
                .Set(user => user.UpdatedAt, updatedUser.UpdatedAt);

            await _users.UpdateOneAsync(filter, update);
        }

    }
}
