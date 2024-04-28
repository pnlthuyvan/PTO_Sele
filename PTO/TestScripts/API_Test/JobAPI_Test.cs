using PTO.API;
using NUnit.Framework;

namespace PTO.TestScripts.API_Test
{
    public class JobAPI_Test
    {
        [Test]
        [Category("APITest")]
        public async Task GetJobsAPI_Test()
        {
            JobAPI jobAPI = new JobAPI();
            var reasonPhrase = await jobAPI.GetAllJobs();

            Assert.Equals(true, !reasonPhrase.Equals(null));
        }

        [Test]
        [Category("APITest")]
        public async Task UpdateProductAPI_Test()
        {
            ProductAPI api =new ProductAPI();
            var reasonPhrase = await api.UpdateProductBySection();

            Assert.Equals("OK", reasonPhrase);
        }
    }
}
