using ApiCalculator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiCalculator.Tests
{
    [TestCaseOrderer("TestOrderExamples.TestCaseOrdering.PriorityOrderer", "TestOrderExamples")]
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

            // Act
            var value = await response.Content.ReadAsStringAsync();

            // Assert
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

            var loginResponse = (JObject)JsonConvert.DeserializeObject(value);
            _token = loginResponse["token"].Value<string>();
            //Assert.Equal(result, Convert.ToDecimal(value), 10);
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

        [Fact, TestPriority(11)]
        public async Task Test11_PostCalculateDivision()
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

            decimal firstElement = 5;
            string operation = "%2F";
            decimal secondElement = 2;
            decimal result = firstElement / secondElement;

            string queryParameters = $"?firstElement={firstElement}&operation={operation}&secondElement={secondElement}";

            response = await Client.PostAsync("/api/Operations" + queryParameters, null);

            // Act
            value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(result, Convert.ToDecimal(value), 10);

        }

        [Fact, TestPriority(12)]
        public async Task Test12_PostCalculateMultiplication()
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

            decimal firstElement = 4;
            string operation = "%2A";
            decimal secondElement = 7;
            decimal result = firstElement * secondElement;

            string queryParameters = $"?firstElement={firstElement}&operation={operation}&secondElement={secondElement}";

            response = await Client.PostAsync("/api/Operations" + queryParameters, null);

            // Act
            value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(result, Convert.ToDecimal(value), 10);

        }

        [Fact, TestPriority(13)]
        public async Task Test13_PostCalculateAddition()
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

            decimal firstElement = 199;
            string operation = "%2B";
            decimal secondElement = 7;
            decimal result = firstElement + secondElement;

            string queryParameters = $"?firstElement={firstElement}&operation={operation}&secondElement={secondElement}";

            response = await Client.PostAsync("/api/Operations" + queryParameters, null);

            // Act
            value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(result, Convert.ToDecimal(value), 10);

        }

        [Fact, TestPriority(14)]
        public async Task Test14_PostCalculateSubtraction()
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

            decimal firstElement = 199;
            string operation = "-";
            decimal secondElement = 7;
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
