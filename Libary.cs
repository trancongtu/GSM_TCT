using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrawFB.DTO;
using DevExpress.Data.Helpers;
using DevExpress.Data.NetCompatibility.Extensions;
using DevExpress.Skins;
using DevExpress.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V130.Page;
using OpenQA.Selenium.Interactions;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace CrawFB
{
    public class Libary
    {
        private static Libary instance;
        public Libary() { }

        public static Libary Instance
        {
            get { if (instance == null) instance = new Libary(); return Libary.instance; }
            private set { Libary.instance = value; }
        }

        public ChromeOptions Options()
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("user-data-dir=C:/User/trant/PycharmProject/PythonProject/ProfileTCT29.3.2025");
           //option.AddArgument("--headless"); //chay ngam hay k
            option.AddArgument("--disable-infobars");
            option.AddArgument("start-maximized");
            option.AddArgument("--disable-extensions");
            option.AddArgument("--disable-blink-features=AutomationControlled");
            // Pass the argument 1 to allow and 2 to block
            return option;
        }
        public ChromeDriver khoitao(string profile)
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArguments("user-data-dir=" + profile);
            option.AddArgument("--disable-infobars");
            option.AddArgument("start-maximized");
            option.AddArgument("--disable-extensions");

            //option.AddArgument("--headless"); //chạy ngầm
            ChromeDriver driver = new ChromeDriver(option);
            return driver;
        }
        public ChromeOptions Options2(string profile)
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArguments("user-data-dir=" + profile);
            option.AddArgument("--disable-infobars");
            option.AddArgument("start-maximized");
            option.AddArgument("--disable-extensions");
            //option.AddArgument("--headless"); //chạy ngầm
            return option;
        }
        public List<IWebElement> CheckAcoount(ChromeDriver driver)
        {
            List<IWebElement> element = null;
            List<IWebElement> infor = new List<IWebElement>(driver.FindElements(By.XPath("//div[@class = 'x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k xsyo7zv x16hj40l x10b6aqq x1yrsyyn']//span"))); 
            if (infor != null)
            {
                element = infor;
            }
            return element;
        }

        public string SumShare(ChromeDriver Driver, string linkbaiviet)
        {
            string sumshare = "";
            Driver.Url = linkbaiviet;
            Libary.Instance.randomtime(6000, 10000);
            Driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            List<IWebElement> temp = new List<IWebElement>(Driver.FindElements(By.CssSelector("span[class = 'html-span xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x1hl2dhg x16tdsg8 x1vvkbs x1sur9pj xkrqix3']")));
            sumshare = temp[temp.Count - 1].Text;
            if(sumshare != "")
            {
                int index = sumshare.IndexOf(" ");
                sumshare = sumshare.Substring(0, index);
            }
            return sumshare;
        }
        public List<string> thongtincanhan(ChromeDriver Driver, string link)
        {
            //string thongtinsongtai, thongtindentu = "";
            Driver.Url = link;
            randomtime(5000, 10000);
            Random random = new Random();
            int ran = random.Next(1, 4);
            randomAction(Driver, ran);
            List<string> thongtin = new List<string>();
            List<IWebElement> element = new List<IWebElement>(Driver.FindElements(By.CssSelector("div[class = 'xzsf02u x6prxxf xvq8zen x126k92a']>span")));
            randomtime(5000, 10000);
            if (element != null)
            {
                foreach (IWebElement element2 in element)
                {
                    string temp1 = element2.Text;                                 
                   if(temp1.Contains("Sống") == true)
                    {
                        thongtin.Add("songtai");
                        int index = temp1.IndexOf("\n");
                        temp1 = temp1.Substring(index, temp1.Length - index);
                        thongtin.Add(temp1);                      
                    }    
                    if(temp1.Contains("Đến") == true)
                    {                       
                        thongtin.Add("dentu");
                        int index = temp1.IndexOf("Đến từ");
                        temp1 = temp1.Substring(index + 6, temp1.Length - index-6);
                        thongtin.Add(temp1);
                    }  
                  
                }               
            }
            else { thongtin.Add("");}
            return thongtin;
        }
        public List<Person> ThongtinPerson(ChromeDriver Driver, string link)
        {
            List<Person> person = new List<Person>();
            Driver.Url = link;
            randomtime(5000, 10000);
            RandomActiconNew(Driver);
            string songtai = "";
            string dentu = "";
            string hocvan = "";
            string tenfb = "";
            List<IWebElement> element = new List<IWebElement>(Driver.FindElements(By.CssSelector("div[class = 'xzsf02u x6prxxf xvq8zen x126k92a']>span")));
            randomtime(5000, 10000);
            if (element != null)
            {
                foreach (IWebElement element2 in element)
                {
                    string temp1 = element2.Text;
                    if (temp1.Contains("Sống") == true)
                    {                    
                        int index = temp1.IndexOf("\n");
                        temp1 = temp1.Substring(index, temp1.Length - index);
                        songtai = temp1;
                    }
                    if (temp1.Contains("Đến") == true)
                    {
                       
                        int index = temp1.IndexOf("Đến từ");
                        temp1 = temp1.Substring(index + 6, temp1.Length - index - 6);
                        dentu = temp1;
                    }
                }
               
            }
            List<IWebElement> thongtinkhac = new List<IWebElement>(Driver.FindElements(By.CssSelector("div[class = 'xzsf02u x6prxxf xvq8zen x126k92a x12nagc']")));
            foreach (IWebElement tt in thongtinkhac)
            {
                hocvan += tt.Text.ToString();
            }
            List<IWebElement> tenfacebook = new List<IWebElement>(Driver.FindElements(By.CssSelector("h1[class = 'html-h1 xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x1vvkbs x1heor9g x1qlqyl8 x1pd3egz x1a2a7pz']")));
            foreach (IWebElement ten in tenfacebook)
            {
               tenfb += ten.Text.ToString();
            }
            person.Add(new Person(-1, link, tenfb, dentu, songtai, hocvan, ""));
            return person;
        }
        
            public string xulylinkperson(string link)
            {
            string ketqua = "";
            int i = link.IndexOf("id=");           
            int j = link.IndexOf("&__");
            if (i != -1 && j != -1)
            {
                link = link.Substring(i + 3, j - i - 3);
                ketqua = "https://Fb.com/" + link;

            }
            else
            {
                int index2 = link.IndexOf("?__");
                if (index2 != -1)
                {
                    link = link.Substring(0, index2);
                    ketqua = link.Replace("https://www.facebook.com/", "https://Fb.com/");
                }
            }
            return ketqua;
        }
        public string rutgonlinkshare(string link)
        {
            string ketqua = "";

            int k = link.IndexOf("?__");
            int t = link.IndexOf("/posts/");
            int i = link.IndexOf("&id=");
            int j = link.IndexOf("&__");
            if ((i != -1) && (j != -1))
            {
                link = link.Substring(0, j);
                ketqua = link.Replace("https://www.facebook.com/", "https://Fb.com/");
            }
            if ((k != -1) && (t != -1))
            {
                ketqua = link.Substring(0, k);
                ketqua = link.Replace("https://www.facebook.com/", "https://Fb.com/");
            }    
                return ketqua;

        }
        public string HrefToLinkFb(string link)
        {
           
            string idfb = "";
            string linkfb = "";
            int i = link.IndexOf("&id=");
            int j = link.IndexOf("&__");
            int k = link.IndexOf("?__");
            int t = link.IndexOf("/posts/");       
            if ((i != -1) && (j != -1))
            {      
                idfb = link.Substring(i + 4, j - i - 4);
                linkfb = "https://Fb.com/" + idfb;
            }
            if ((k != -1) && (t != -1))
            {
                
                linkfb = link.Substring(0, t);
            }           
            return linkfb;
        }
        public string HrefToIdFb(string link)
        {
            string linkshare = "";
            string idfb = "";   
            int i = link.IndexOf("&id=");
            int j = link.IndexOf("&__");
            if ((i != -1) && (j != -1))
            {
                linkshare = link.Substring(0, j);
                idfb = link.Substring(i + 4, j - i - 4);
            }         
            return idfb;
        }
        public string HrefShareGroupsToIdFb(string link)
        {
           
            string idfb = "";
           
            int i = link.IndexOf("/groups/");
            int k = link.IndexOf("?__");
            int t = link.IndexOf("/user/");
            if ((i != -1) && (k != -1) && (t != -1))
            {
                idfb = link.Substring(t + 6, k - t - 7);
            }        
            return idfb;
        }
        public void randomtime (int time1,  int time2)
        {
            Random random = new Random();
            int time = random.Next(time1, time2);         
            Thread.Sleep(time);
        }
        bool CheckPerson (ChromeDriver driver)
        {
            List<IWebElement> list = new List<IWebElement>(driver.FindElements(By.CssSelector("a[class = 'x1i10hfl xjbqb8w x1ejq31n xd10rxx x1sy0etr x17r0tee x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x1ypdohk xt0psk2 xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x16tdsg8 x1hl2dhg xggy1nq x1a2a7pz x1sur9pj xkrqix3 xi81zsa x1s688f']")));
            foreach (IWebElement element in list)
            {
                if(element.Text.ToString().Contains("người theo dõi") == true) return false;
            }

            return true;
        }
        public void randomAction(ChromeDriver Driver, int t)
        {
            bool test = CheckPerson(Driver);
            if (test == true)
            {
                if (t == 1)
                {
                    IWebElement element = Driver.FindElement(By.LinkText("Bạn bè"));
                    if (element != null)
                    {
                        element.Click();
                    }
                    Libary.Instance.randomtime(5000, 10000);
                }
                if (t == 2)
                {
                    IWebElement element = Driver.FindElement(By.LinkText("Ảnh"));
                    if (element != null)
                    {
                        element.Click();
                    }
                    Libary.Instance.randomtime(5000, 10000);
                }
                if (t == 3)
                {
                    IWebElement element = Driver.FindElement(By.LinkText("Video"));
                    if (element != null)
                    {
                        element.Click();
                    }
                    Libary.Instance.randomtime(5000, 10000);
                }
                if (t == 4)
                {
                    IWebElement element = Driver.FindElement(By.LinkText("Check in"));
                    if (element != null)
                    {
                        element.Click();
                    }
                    Libary.Instance.randomtime(5000, 10000);
                }
            }
           

            IWebElement element2 = Driver.FindElement(By.LinkText("Giới thiệu"));
            if (element2 != null)
            {
                element2.Click();
            }
            Libary.Instance.randomtime(5000, 10000);
        }
        public void RandomActiconNew(ChromeDriver Driver)
        {
            Random ran = new Random();
            int tam = ran.Next(2, 5);
            Console.WriteLine(tam);
            List<IWebElement> fullshare = new List<IWebElement>(Driver.FindElements(By.CssSelector("a[class = 'x1i10hfl xe8uvvx xggy1nq x1o1ewxj x3x9cwd x1e5q0jg x13rtm0m x87ps6o x1lku1pv x1a2a7pz xjyslct xjbqb8w x18o3ruo x13fuv20 xu3j5b3 x1q0q8m5 x26u7qi x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x1heor9g x1ypdohk xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 x16tdsg8 x1hl2dhg x1vjfegm x3nfvp2 xrbpyxo x1itg65n x16dsc37']")));

            if (tam < fullshare.Count)
            {
                fullshare[tam].Click();
                Libary.Instance.randomtime(3000, 5000);
            }
            fullshare[1].Click();
            IWebElement element2 = Driver.FindElement(By.LinkText("Giới thiệu"));
            if (element2 != null)
            {
                element2.Click();
            }
            Libary.Instance.randomtime(5000, 10000);
        }
        public void keoluotshare (ChromeDriver Driver)
        {
            IWebElement element2 = Driver.FindElement(By.XPath("//div[@class = 'xb57i2i x1q594ok x5lxg6s xdt5ytf x6ikm8r x1ja2u2z x1pq812k x1rohswg xfk6m8 x1yqm8si xjx87ck x1l7klhg x1iyjqo2 xs83m0k x2lwn1j xx8ngbg xwo3gff x1oyok0e x1odjw0f x1e4zzel x1n2onr6 xq1qtft x78zum5 x179dxpb']"));
            int last_count = 0;
            int new_count = 0;
            do
            {
                Libary.Instance.randomtime(6000, 10000);
                //last_count = Driver.FindElements(By.XPath("//div[@class = 'xb57i2i x1q594ok x5lxg6s xdt5ytf x6ikm8r x1ja2u2z x1pq812k x1rohswg xfk6m8 x1yqm8si xjx87ck x1l7klhg x1iyjqo2 xs83m0k x2lwn1j xx8ngbg xwo3gff x1oyok0e x1odjw0f x1e4zzel x1n2onr6 xq1qtft x78zum5 x179dxpb']")).Count();
                last_count = Driver.FindElements(By.CssSelector("div[class = 'x1yztbdb']")).Count();
                Console.WriteLine(last_count);

                for (int i = 0; i < 2; i++)
                {
                    element2.SendKeys(Keys.End);
                    element2.SendKeys(Keys.Up);
                    element2.SendKeys(Keys.Up);
                    element2.SendKeys(Keys.Up);
                    element2.SendKeys(Keys.Down);
                    element2.SendKeys(Keys.Down);
                    element2.SendKeys(Keys.Down);
                    Libary.Instance.randomtime(6000, 10000);
                }

                new_count = Driver.FindElements(By.CssSelector("div[class = 'x1yztbdb']")).Count();
                Console.WriteLine(new_count);
            }
            while (last_count != new_count);
        }
        public string xulyKshare(string link)
        {
            string ketqua = "";
            int phay = link.IndexOf(",");
            int nghin = link.IndexOf("K");           
            if (nghin != -1)
            {
                for (int i = 0; i < link.Length; i++)
                {
                    if (link[i] != ',' && link[i] != 'K')
                    {
                        ketqua += link[i];
                    }
                }
                if (phay != -1)
                {
                    ketqua = ketqua + "00";
                }
                else ketqua = ketqua + "000";
            }
            else ketqua = link;
            return ketqua;
        }
        public List<PersonShare> GetShareOnePage(ChromeDriver Driver, string linkpage, int sobai)
        {
            List<PersonShare> personshares = new List<PersonShare>();
            Actions action = new Actions(Driver);
            Driver.Url = linkpage;
            int dem = 1; 
            Libary.Instance.randomtime(6000, 10000);
            while (dem < sobai)
            {

                List<IWebElement> elementshare = new List<IWebElement>(Driver.FindElements(By.CssSelector("div[class = 'x9f619 x1ja2u2z x78zum5 x2lah0s x1n2onr6 x1qughib x1qjc9v5 xozqiw3 x1q0g3np xykv574 xbmpl8g x4cne27 xifccgj']>div:nth-of-type(3)>span>div>div")));
                foreach (IWebElement element in elementshare)
                {
                    IWebElement sumshare = element.FindElement(By.CssSelector("span[class = 'html-span xdj266r x11i5rnm xat24cr x1mh8g0r xexx8yu x4uap5 x18d9i69 xkhd6sd x1hl2dhg x16tdsg8 x1vvkbs xkrqix3 x1sur9pj']"));
                    //Console.WriteLine("tổng share: " + sumshare.Text);
                    int SumShare = Convert.ToInt32(sumshare.Text);
                    IWebElement buttonshare = element.FindElement(By.CssSelector("div[class = 'x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x2lah0s x193iq5w xeuugli xg83lxy x1h0ha7o x10b6aqq x1yrsyyn']"));
                    action.MoveToElement(buttonshare).Build().Perform();
                    action.Click(buttonshare).Build().Perform();
                    //buttonshare.Click();
                    randomtime(3000, 7000);
                    if (SumShare > 10)
                    {
                        IWebElement element2 = Driver.FindElement(By.CssSelector("div[class = 'xb57i2i x1q594ok x5lxg6s xdt5ytf x6ikm8r x1ja2u2z x1pq812k x1rohswg xfk6m8 x1yqm8si xjx87ck x1l7klhg x1iyjqo2 xs83m0k x2lwn1j xx8ngbg xwo3gff x1oyok0e x1odjw0f x1e4zzel x1n2onr6 xq1qtft x78zum5 x179dxpb']"));
                        int last_count = 0;
                        int new_count = 0;
                        do
                        {
                            Libary.Instance.randomtime(6000, 10000);
                            //last_count = Driver.FindElements(By.XPath("//div[@class = 'xb57i2i x1q594ok x5lxg6s xdt5ytf x6ikm8r x1ja2u2z x1pq812k x1rohswg xfk6m8 x1yqm8si xjx87ck x1l7klhg x1iyjqo2 xs83m0k x2lwn1j xx8ngbg xwo3gff x1oyok0e x1odjw0f x1e4zzel x1n2onr6 xq1qtft x78zum5 x179dxpb']")).Count();
                            last_count = Driver.FindElements(By.CssSelector("div[class = 'x1yztbdb']")).Count();
                            //Console.WriteLine("count cuối: " + last_count);

                            for (int i = 0; i < 2; i++)
                            {
                                element2.SendKeys(Keys.End);
                                element2.SendKeys(Keys.Up);
                                element2.SendKeys(Keys.Up);
                                element2.SendKeys(Keys.Up);
                                element2.SendKeys(Keys.Down);
                                element2.SendKeys(Keys.Down);
                                element2.SendKeys(Keys.Down);
                                Libary.Instance.randomtime(6000, 10000);
                            }

                            new_count = Driver.FindElements(By.CssSelector("div[class = 'x1yztbdb']")).Count();
                            //Console.WriteLine("count cu", +new_count);
                        }
                        while (last_count != new_count);
                    }

                    List<IWebElement> fullshare = new List<IWebElement>(Driver.FindElements(By.CssSelector("div[class ^= 'x1yztbdb']>div>div>div>div>div>div>div>div>div>span>div>h3>span>span>a")));
                    foreach (IWebElement element3 in fullshare)
                    {
                        string link = element3.GetAttribute("href");
                        Console.WriteLine(link);
                        string linkshare = "";
                        string idfb = "";
                        string linkfb = "";
                        int i = link.IndexOf("&id=");
                        int j = link.IndexOf("&__");
                        int k = link.IndexOf("?__");
                        int t = link.IndexOf("/posts/");
                        if ((i != -1) && (j != -1))
                        {
                            linkshare = link.Substring(0, j);
                            idfb = link.Substring(i + 4, j - i - 4);
                            linkfb = "https://Fb.com/" + idfb;
                        }
                        if ((k != -1) && (t != -1))
                        {
                            linkshare = link.Substring(0, k);
                            linkfb = link.Substring(0, t);
                        }
                        Console.WriteLine(linkfb);
                        // string diachishare = list[0].ToString();
                        // string linkfb = list[1].ToString();
                        // string idfb = list[2].ToString();

                        //personshares.Add(new PersonShare(linkshare, linkfb));

                    }
                    IWebElement buttonclose = Driver.FindElement(By.CssSelector("div[class = 'x1i10hfl xjqpnuy xa49m3k xqeqjp1 x2hbi6w x13fuv20 xu3j5b3 x1q0q8m5 x26u7qi x1ypdohk xdl72j9 x2lah0s xe8uvvx xdj266r x11i5rnm xat24cr x1mh8g0r x2lwn1j xeuugli x16tdsg8 x1hl2dhg xggy1nq x1ja2u2z x1t137rt x1q0g3np x87ps6o x1lku1pv x1a2a7pz x6s0dn4 xzolkzo x12go9s9 x1rnf11y xprq8jg x972fbf xcfux6l x1qhh985 xm0m39n x9f619 x78zum5 xl56j7k xexx8yu x4uap5 x18d9i69 xkhd6sd x1n2onr6 xc9qbxq x14qfxbe x1qhmfi1']"));
                    buttonclose.Click();
                    dem++;
                    Console.WriteLine(dem);
                }
                Driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                Thread.Sleep(10000);
            }
            return personshares;
        }
    }
}
