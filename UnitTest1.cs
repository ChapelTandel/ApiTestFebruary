using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;

namespace ApiTestFebruary
{
    [Parallelizable]
    [TestFixture]
    public class WhenICallTheCorrectResourcePath
    {
        //Arrange 
        private readonly SocialMediaApi _socialMediaApi = new SocialMediaApi();

        [TestCase("posts", HttpStatusCode.OK)]
        [TestCase("comments", HttpStatusCode.OK)]
        [TestCase("profile", HttpStatusCode.OK)]
        public void ThenTheResponseIsOk(string resourcePath, HttpStatusCode statusCode)
        {
            //Action
            var actualResult = _socialMediaApi.GetResource(resourcePath).Result.StatusCode;

            //Assert
            Assert.AreEqual(statusCode, actualResult);
        }
    }

    [Parallelizable]
    [TestFixture]
    public class WhenICallIncorrectResourcePath
    {
        //Arrange 
        private readonly SocialMediaApi _socialMediaApi = new SocialMediaApi();

        [TestCase("posts*", HttpStatusCode.NotFound)]
        [TestCase("comments or 1=1", HttpStatusCode.NotFound)]
        [TestCase("profiles", HttpStatusCode.NotFound)]
        public void ThenTheResponseIsNotFound(string resourcePath, HttpStatusCode statusCode)
        {
            //Action
            var actualResult = _socialMediaApi.GetResource(resourcePath).Result.StatusCode;

            //Assert
            Assert.AreEqual(statusCode, actualResult);
        }
    }

    [Parallelizable]
    [TestFixture]
    public class WhenISearcPostById
    {
        //Arrange 
        private readonly SocialMediaApi _socialMediaApi = new SocialMediaApi();

        private static object[] _sourceList =
        { 
            new object[] {new Post{ id= 1, title = "json-server", author = "Kartik"}},
            new object[] {new Post{ id= 2, title = "learn json", author = "Chapel"}}
        };

        [TestCaseSource(nameof(_sourceList))]
        public void ThenIGetTheCorrectPost(Post post)
        {
            //Action
            var actualResult = _socialMediaApi.GetPostById(post.id).Result.Content;
            var d = JsonConvert.DeserializeObject<List<Post>>(actualResult);
            var g = d.Find(x=>x.id == post.id);

            //Assert
            Assert.AreEqual(JsonConvert.SerializeObject(post), JsonConvert.SerializeObject(g));
        }
    }

    [Parallelizable]
    [TestFixture]
    public class WhenIAddANewPost
    {
        //Arrange 
        private readonly SocialMediaApi _socialMediaApi = new SocialMediaApi();

        private static object[] _sourceList =
        {
            new object[] {new Post{title = "json-server", author = "Kartik"}},
            new object[] {new Post{title = "learn json", author = "Chapel"}}
        };

        [TestCaseSource(nameof(_sourceList))]
        public void ThenTheNewPostIsSuccssfullyAdded(Post post)
        {
            Random random = new Random();
            post.id = random.Next();

            //Action
            var actualResult = _socialMediaApi.AddPost(post).Result.Content;
            var g = JsonConvert.DeserializeObject<Post>(actualResult);

            //Assert
            Assert.AreEqual(JsonConvert.SerializeObject(post), JsonConvert.SerializeObject(g));
        }
    }

    [Parallelizable]
    [TestFixture]
    public class WhenIAddADuplicatePost
    {
        //Arrange 
        private readonly SocialMediaApi _socialMediaApi = new SocialMediaApi();

        private static object[] _sourceList =
        {
            new object[] {new Post{ id= 28, title = "json-server", author = "Kartik"}},
            new object[] {new Post{ id= 29, title = "learn json", author = "Chapel"}}
        };

        [TestCaseSource(nameof(_sourceList))]
        public void ThenIGetAnErrorMessage(Post post)
        {
            //Action
            var actualResult = _socialMediaApi.AddPost(post).Result.Content;

            //Assert
            Assert.That(actualResult.Contains("Error: Insert failed, duplicate id"));
        }
    }

    [Parallelizable]
    [TestFixture]
    public class WhenIAddUpdateAPost
    {
        //Arrange 
        private readonly SocialMediaApi _socialMediaApi = new SocialMediaApi();

        private static object[] _sourceList =
        {
            new object[] {new Post{ id= 28, title = "json-server", author = "Kartik x"}},
            new object[] {new Post{ id= 29, title = "learn json", author = "Chapel x"}}
        };

        [TestCaseSource(nameof(_sourceList))]
        public void ThenThePostIsSuccessfullyUpdated(Post post)
        {
            //Action
            var actualResult = _socialMediaApi.UpdatePost(post).Result.Content;
            var g = JsonConvert.DeserializeObject<Post>(actualResult);

            //Assert
            Assert.AreEqual(JsonConvert.SerializeObject(post), JsonConvert.SerializeObject(g));
        }
    }



    internal class SocialMediaApi
    {
        public RestClient RestClient = new RestClient(TestData.BaseUrl);
        public async Task<IRestResponse> GetPostById(int id)
        {
            var request = new RestRequest("Posts", Method.GET); 
            request.AddQueryParameter("id", id.ToString());
            var response = RestClient.ExecuteAsync(request);
            return await response;
        }

        public async Task<IRestResponse> GetResource(string resourcePath)
        {
            var request = new RestRequest(resourcePath, Method.GET);
            var response = RestClient.ExecuteAsync(request);
            return await response;
        }

        public async Task<IRestResponse> AddPost(Post post)
        {
                var request = new RestRequest("Posts", Method.POST);
                request.AddJsonBody(JsonConvert.SerializeObject(post));
                var response = RestClient.ExecuteAsync(request);
                return await response;
        }

        public async Task<IRestResponse> UpdatePost(Post post)
        {
            // RestClient.Authenticator = new SimpleAuthenticator("username", "foo", "password", "bar");
            // RestClient.Authenticator = new HttpBasicAuthenticator("username",  "password");


            var request = new RestRequest("Posts/{id}", Method.PATCH);
            request.AddUrlSegment("id", post.id);
            request.AddJsonBody(JsonConvert.SerializeObject(post));
            var response = RestClient.ExecuteAsync(request);
            return await response;
        }
    }
}