using ApiCalculator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiCalculator.Tests
{
    [TestCaseOrderer("ApiCalculator.Tests.PriorityOrderer", "ApiCalculator.Tests")]
    public class CalculatorTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;
        private string _token;
        private string _username = "test_user";
        private string _password = "123456";
        private string _email = "test@email.com";


        public CalculatorTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact, TestPriority(0)]
        public async Task Test0_RegisterUser()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            RegisterModel newUser = new RegisterModel()
            {
                Username = _username,
                Password = _password,
                Email = _email
            };

            string jsonInString = JsonConvert.SerializeObject(newUser);

            var response = await Client.PostAsync("/api/Authenticate/register", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            // Assert
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                Client = new HttpClient();
                Client.BaseAddress = new Uri("http://localhost:3688");
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

                LoginModel login = new LoginModel()
                {
                    Username = _username,
                    Password = _password
                };

                jsonInString = JsonConvert.SerializeObject(login);

                response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

                // Assert
                response.EnsureSuccessStatusCode();
            }
            else
                response.EnsureSuccessStatusCode();
        }

        [Fact, TestPriority(1)]
        public async Task Test1_Login()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            LoginModel login = new LoginModel()
            {
                Username = _username,
                Password = _password
            };

            string jsonInString = JsonConvert.SerializeObject(login);

            var response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            // Act
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact, TestPriority(10)]
        public async Task Test10_DeleteCalculationHistory()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            LoginModel login = new LoginModel()
            {
                Username = _username,
                Password = _password
            };

            string jsonInString = JsonConvert.SerializeObject(login);

            var response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            var value = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var loginResponse = (JObject)JsonConvert.DeserializeObject(value);
            _token = loginResponse["token"].Value<string>();

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer { _token }");

            // Act
            var deleteResponse = await Client.DeleteAsync(string.Format("/api/Operations"));

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Theory, TestPriority(11)]
        [InlineData(1, 2)]
        [InlineData(-4, -6)]
        [InlineData(-2, 2)]
        [InlineData(196, 45)]
        public async Task Test11_PostCalculateDivision(decimal firstElement, decimal secondElement)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            LoginModel login = new LoginModel()
            {
                Username = _username,
                Password = _password
            };

            string jsonInString = JsonConvert.SerializeObject(login);

            var response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            var value = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var loginResponse = (JObject)JsonConvert.DeserializeObject(value);
            _token = loginResponse["token"].Value<string>();

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer { _token }");

            string operation = "%2F";
            decimal result = firstElement / secondElement;

            string queryParameters = $"?firstElement={firstElement}&operation={operation}&secondElement={secondElement}";

            response = await Client.PostAsync("/api/Operations" + queryParameters, null);

            // Act
            value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(result, Convert.ToDecimal(value), 10);

        }

        [Theory, TestPriority(12)]
        [InlineData(1, 2)]
        [InlineData(-4, -6)]
        [InlineData(-2, 2)]
        [InlineData(196, 45)]
        public async Task Test12_PostCalculateMultiplication(decimal firstElement, decimal secondElement)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            LoginModel login = new LoginModel()
            {
                Username = _username,
                Password = _password
            };

            string jsonInString = JsonConvert.SerializeObject(login);

            var response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            var value = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var loginResponse = (JObject)JsonConvert.DeserializeObject(value);

            _token = loginResponse["token"].Value<string>();
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer { _token }");

            string operation = "%2A";
            decimal result = firstElement * secondElement;

            string queryParameters = $"?firstElement={firstElement}&operation={operation}&secondElement={secondElement}";

            response = await Client.PostAsync("/api/Operations" + queryParameters, null);

            // Act
            value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(result, Convert.ToDecimal(value), 10);

        }

        [Theory, TestPriority(13)]
        [InlineData(1, 2)]
        [InlineData(-4, -6)]
        [InlineData(-2, 2)]
        [InlineData(196, 45)]
        public async Task Test13_PostCalculateAddition(decimal firstElement, decimal secondElement)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            LoginModel login = new LoginModel()
            {
                Username = _username,
                Password = _password
            };

            string jsonInString = JsonConvert.SerializeObject(login);

            var response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            var value = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var loginResponse = (JObject)JsonConvert.DeserializeObject(value);
            _token = loginResponse["token"].Value<string>();

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer { _token }");

            string operation = "%2B";
            decimal result = firstElement + secondElement;

            string queryParameters = $"?firstElement={firstElement}&operation={operation}&secondElement={secondElement}";

            response = await Client.PostAsync("/api/Operations" + queryParameters, null);

            // Act
            value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(result, Convert.ToDecimal(value), 10);

        }

        [Theory, TestPriority(14)]
        [InlineData(1, 2)]
        [InlineData(-4, -6)]
        [InlineData(-2, 2)]
        [InlineData(196, 45)]
        public async Task Test14_PostCalculateSubtraction(decimal firstElement, decimal secondElement)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            LoginModel login = new LoginModel()
            {
                Username = _username,
                Password = _password
            };

            string jsonInString = JsonConvert.SerializeObject(login);

            var response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            var value = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var loginResponse = (JObject)JsonConvert.DeserializeObject(value);
            _token = loginResponse["token"].Value<string>();

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer { _token }");

            string operation = "-";
            decimal result = firstElement - secondElement;

            string queryParameters = $"?firstElement={firstElement}&operation={operation}&secondElement={secondElement}";

            response = await Client.PostAsync("/api/Operations" + queryParameters, null);

            // Act
            value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(result, Convert.ToDecimal(value), 10);

        }


        [Fact, TestPriority(15)]
        public async Task Test15_GetUserHistory()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer");

            LoginModel login = new LoginModel()
            {
                Username = _username,
                Password = _password
            };

            string jsonInString = JsonConvert.SerializeObject(login);

            var response = await Client.PostAsync("/api/Authenticate/login", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            var value = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var loginResponse = (JObject)JsonConvert.DeserializeObject(value);
            _token = loginResponse["token"].Value<string>();

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:3688");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer { _token }");

            // Arrange
            var request = "/api/Operations/user-history";
            // Act
            response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

    }
}
