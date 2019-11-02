namespace DalSoft.RestClient.Examples.Test.System.Resources
{
    public class PostResource
    {
        public string GetPosts() => "posts";

        public string GetPost(int postId) => $"{GetPosts()}/{postId}";

        public string GetPostComments(int postId) => $"{GetPost(postId)}/comments";
    }
}