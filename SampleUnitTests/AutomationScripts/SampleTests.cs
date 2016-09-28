using System;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace SampleTests
{
    // "logicalRow": 1, "logicalCol": 0, "_eventId": "55f4ba853451c8bc227664ff", "userKey": "bd813ec4-5abb-4cc3-ad9d-51ca46eb509b", "_id": "56ca53758f5e6f900a5ed19b", "userSeat": { "level": "floor", "row": "1", "seat": "1", "section": "101" }}

    [DataContract]
    public class ULResult
    {
        [DataMember]
        public int logicalRow;

        [DataMember]
        public int logicalCol;

        [DataMember]
        public string _eventId;

        [DataMember]
        public string userKey;

        [DataMember]
        public string _id;
    }

    [DataContract]
    public class EJ
    {
        [DataMember]
        public string mobileTime;
    }

    [DataContract]
    public class UL
    {
        //   "{ \"userKey\": \"9F2A87E8-9458-494B-9637-EdB54B15519CfTT\", \"userSeat\": { \"level\": \"floor\", \"section\": \"101\", \"row\": \"22\", \"seat\": \"23\" } }
        [DataMember]
        public string userKey;

        [DataMember]
        public userSeat userSeat;
    }

    [DataContract]
    public class userSeat
    {
        [DataMember]
        public string level;

        [DataMember]
        public string section;

        [DataMember]
        public string row;

        [DataMember]
        public string seat;
    }

    [TestClass]
    public class SampleTests
    {
        static public Stack<string> ulStack = new Stack<string>();

        static public bool inShow = false;

        static public int currentRow = 1;

        static public int currentSeat = 1;

        static async Task<string> RunAsync(string url, bool isPost = false, UL userLocationToRegister = null, EJ eventJoin = null)
        {
            using (var client = new HttpClient())
            {
                // New code:
                //client.BaseAddress = new Uri("http://main-1156949061.us-west-2.elb.amazonaws.com/");
                client.BaseAddress = new Uri("http://www.litewaveinc.com/");
                //client.BaseAddress = new Uri("http://127.0.0.1:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = new TimeSpan(0, 0, 0, 0, 20000);

                HttpResponseMessage response = null;
                if (isPost)
                {
                    MemoryStream ms = new MemoryStream();
                    DataContractJsonSerializer jsonSer;
                    if (userLocationToRegister != null)
                    {
                        //Create a Json Serializer for our type
                        jsonSer = new DataContractJsonSerializer(typeof(UL));

                        // use the serializer to write the object to a MemoryStream
                        jsonSer.WriteObject(ms, userLocationToRegister);
                    }
                    else if (eventJoin != null)
                    {
                        //Create a Json Serializer for our type
                        jsonSer = new DataContractJsonSerializer(typeof(EJ));

                        // use the serializer to write the object to a MemoryStream
                        jsonSer.WriteObject(ms, eventJoin);
                    }
                    ms.Position = 0;

                    // use a Stream reader to construct the StringContent (Json)
                    StreamReader sr = new StreamReader(ms);
                    StringContent theContent = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");

                    response = await client.PostAsync(url, theContent);
                }
                else
                {
                    response = await client.GetAsync(url);
                }

                if (response.IsSuccessStatusCode)
                {
                    string s_response = response.ToString();
                    string s_response_content = await response.Content.ReadAsStringAsync();

                    if (isPost && userLocationToRegister != null)
                    {
                        ULResult ulResult = new ULResult();
                        DataContractJsonSerializer jsonSer;
                        jsonSer = new DataContractJsonSerializer(typeof(ULResult));
                        Stream streamContent = await response.Content.ReadAsStreamAsync();
                        ulResult = (ULResult)jsonSer.ReadObject(streamContent);
                        ulStack.Push(ulResult._id);
                    }

                    return response.StatusCode.ToString();
                }
                else
                {
                    if (eventJoin != null)
                    { 
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            inShow = false;
                            return System.Net.HttpStatusCode.OK.ToString();
                        }
                        else
                        {
                            // if EJ returned successful we are in a show. Stop UL from joining.
                            inShow = true;
                        }
                    }

                    //Console.WriteLine(response);
                    //Logger.LogDebugMessages("Failed with status code: " + response.StatusCode.ToString());
                    //Assert.Fail("Failed with status code: " + response.StatusCode.ToString());
                    return response.StatusCode.ToString();
                }
            }
        }

        [ClassInitialize]
        public static void SampleTestsClassInit(TestContext ctx)
        {
        }

        [ClassCleanup]
        public static void SampleTestsClassCleanup()
        {
        }

        [TestInitialize]
        public void SampleTestInit()
        {
        }

        [TestCleanup]
        public void SampleTestCleanup()
        {
        }
       

        [TestMethod]
        public void CallGetClient()
        {
            //Task <string> result = RunAsync("api/stadiums/client/5260316cbf80240000000001");
            RunAsync("api/stadiums/client/5260316cbf80240000000001").Wait(1120000);
            /*result.Wait(9120000);
            string s_result = result.Result;
            if (s_result != "OK")
                Assert.Fail("Test failed with status code " + s_result);*/
        }

        [TestMethod]
        public void CallGetLevelOne()
        {
            RunAsync("api/stadiums/55de78afa1d569ec11646bc9/levels/one").Wait(1120000);
        }

        [TestMethod]
        public void CallGetLevelTwo()
        {
            RunAsync("api/stadiums/55de78afa1d569ec11646bc9/levels/two").Wait(1120000);
        }

        [TestMethod, Timeout(9120000)]
        public void CallGetLevelThree()
        {
            //Task<string> result = RunAsync("api/stadiums/55de78afa1d569ec11646bc9/levels/three").Wait(1120000);
            RunAsync("api/stadiums/55de78afa1d569ec11646bc9/levels/three").Wait(1120000);
            //result.Wait(1);
            //string s_result = result.Result;
            //if (s_result != "OK")
            //    Assert.Fail("Test failed with status code " + s_result);
            
        }

        [TestMethod]
        public void CallGetEventsAPI()
        {
            RunAsync("api/clients/5260316cbf80240000000001/events/").Wait(1120000);
        }

        [TestMethod]
        public void CallPostUL()
        {
            if (inShow)
            {
                // we are in a show, users can't join anymore.
                return;
            }

            Guid g = Guid.NewGuid();
            UL userLocation = new UL();
            
            userLocation.userSeat = new userSeat();
            // Generate unique id and save result for EJ.
            userLocation.userKey = g.ToString();
            userLocation.userSeat.level = "floor1";
            userLocation.userSeat.section = "101";
            userLocation.userSeat.row = currentRow.ToString();
            userLocation.userSeat.seat = "1";   // currentSeat.ToString();
            currentRow++;
            currentSeat++;

            // PROD 
            RunAsync("api/events/5704a152182753c925df18f0/user_locations", true, userLocation).Wait(1120000);

            // LOCAL  55f4ba853451c8bc227664ff  
            //RunAsync("api/events/55dd5fe01c1fd0cc1c7ffeab/user_locations", true, userLocation).Wait(1120000);
        }

        [TestMethod]
        public void CallPostEJ()
        {
            if (ulStack.Count < 100)
            {
                return;
            }

            EJ eventJoin = new EJ();
            eventJoin.mobileTime = new DateTime(DateTime.Now.Ticks).ToString();

            Logger.LogDebugMessages("eventJoin.mobileTime: " + eventJoin.mobileTime);

            // Get next UL from stack and join show.
            string api = "api/user_locations/" + ulStack.Pop() + "/event_joins";
            RunAsync(api, true, null, eventJoin).Wait(1120000);
        }
    }
}
