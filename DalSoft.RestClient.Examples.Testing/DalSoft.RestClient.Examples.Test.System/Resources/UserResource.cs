namespace DalSoft.RestClient.Examples.Test.System.Resources
{
    public class UserResource
    {
        public string GetUsers() => "users";

        public string GetUser(int userId) => $"{GetUsers()}/{userId}";
        
        public string GetUserPosts(int userId) => $"{GetUser(userId)}/posts";
    }
}