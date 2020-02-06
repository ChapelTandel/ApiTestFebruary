namespace ApiTestFebruary
{
    internal class TestData
    {
        public static string BaseUrl => JsonFileReader.GetValueFromFile("TestData.json", "baseUrl");
    }
}